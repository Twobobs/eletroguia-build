using System.Reflection;
using System.Diagnostics;
using System.Security.Cryptography;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace Eletroguia;

static class Program
{
    // Preenchido por tools/encrypt.mjs a cada build. Não edite à mão.
    //KEY-BEGIN
    static readonly byte[] KA = { 213, 32, 154, 110, 37, 148, 179, 87, 37, 101, 237, 28, 148, 228, 98, 123, 176, 171, 198, 54, 194, 48, 139, 19, 104, 200, 51, 92, 40, 199, 113, 191 };
    static readonly byte[] KM = { 144, 100, 168, 68, 172, 83, 250, 136, 72, 164, 145, 84, 220, 247, 229, 138, 224, 53, 115, 31, 160, 184, 250, 232, 228, 249, 108, 105, 211, 125, 6, 10 };
    //KEY-END

    [STAThread]
    static void Main()
    {
        using var mtx = new Mutex(true, "Eletroguia.SingleInstance.v1", out bool created);
        if (!created) return;

        ApplicationConfiguration.Initialize();

        byte[] html;
        try { html = CarregarHtml(); }
        catch
        {
            MessageBox.Show("O programa está corrompido. Reinstale o Eletroguia.", "Eletroguia",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        Application.Run(new MainForm(html));
    }

    // Resource embutido: nonce(12) | tag(16) | HTML cifrado (AES-256-GCM).
    // A chave fica dividida em duas metades (KA, KM) só para não aparecer
    // como bytes contíguos no binário; isso não é proteção forte, apenas
    // evita o caso mais ingênuo de alguém copiar a chave direto do .exe.
    static byte[] CarregarHtml()
    {
        var key = new byte[32];
        for (int i = 0; i < 32; i++) key[i] = (byte)(KA[i] ^ KM[i]);

        using var s = Assembly.GetExecutingAssembly().GetManifestResourceStream("app.bin")
            ?? throw new InvalidOperationException("recurso ausente");
        using var ms = new MemoryStream();
        s.CopyTo(ms);
        var all = ms.ToArray();

        var nonce = all.AsSpan(0, 12).ToArray();
        var tag = all.AsSpan(12, 16).ToArray();
        var ct = all.AsSpan(28).ToArray();
        var pt = new byte[ct.Length];

        using var aes = new AesGcm(key, 16);
        aes.Decrypt(nonce, ct, tag, pt);
        Array.Clear(key);
        return pt;
    }

}

sealed class MainForm : Form
{
    const string Host = "app.eletroguia.local";
    readonly WebView2 wv = new() { Dock = DockStyle.Fill };
    readonly byte[] html;

    public MainForm(byte[] htmlBytes)
    {
        html = htmlBytes;
        Text = "Eletroguia® Sistema de Cálculo";
        Width = 1280;
        Height = 800;
        MinimumSize = new System.Drawing.Size(800, 560);
        StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Maximized;
        Controls.Add(wv);
        Load += async (_, _) => await IniciarAsync();
    }

    async Task IniciarAsync()
    {
        try
        {
            var perfilDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Eletroguia");
            var env = await CoreWebView2Environment.CreateAsync(null, perfilDir);
            await wv.EnsureCoreWebView2Async(env);

            var c = wv.CoreWebView2;
            c.DocumentTitleChanged += (_, _) =>
            {
                var pageTitle = c.DocumentTitle;
                if (InvokeRequired) BeginInvoke(new Action(() => Text = pageTitle));
                else Text = pageTitle;
            };
            var s = c.Settings;
            // Modo "kiosk": sem barra de endereço, sem menu de contexto do
            // navegador, sem DevTools — a janela mostra só o app.
            s.AreDevToolsEnabled = false;
            s.AreDefaultContextMenusEnabled = false;
            s.AreBrowserAcceleratorKeysEnabled = false;
            s.IsStatusBarEnabled = false;
            s.IsZoomControlEnabled = false;
            s.IsGeneralAutofillEnabled = false;
            s.IsPasswordAutosaveEnabled = false;
            s.AreHostObjectsAllowed = false;

            // O HTML é servido de um domínio virtual local a partir da
            // memória (nunca grava o HTML em disco em texto claro).
            c.AddWebResourceRequestedFilter("https://" + Host + "/*", CoreWebView2WebResourceContext.All);
            c.WebResourceRequested += (_, e) =>
            {
                var uri = new Uri(e.Request.Uri);
                if (uri.Host != Host) return;
                if (uri.AbsolutePath is "/" or "/index.html")
                    e.Response = env.CreateWebResourceResponse(new MemoryStream(html), 200, "OK",
                        "Content-Type: text/html; charset=utf-8\r\nCache-Control: no-store");
                else
                    e.Response = env.CreateWebResourceResponse(null, 404, "Not Found", "");
            };

            // Impede navegar para fora do app (ex.: um link clicado por engano).
            c.NavigationStarting += (_, e) =>
            {
                if (!e.Uri.StartsWith("https://" + Host + "/") &&
                    !e.Uri.StartsWith("blob:") && !e.Uri.StartsWith("data:"))
                {
                    e.Cancel = true;
                    if (e.IsUserInitiated) OpenDonationLink(e.Uri);
                }
            };
            c.NewWindowRequested += (_, e) =>
            {
                e.Handled = true;
                OpenDonationLink(e.Uri);
            };
            c.PermissionRequested += (_, e) => e.State = CoreWebView2PermissionState.Deny;

            c.Navigate("https://" + Host + "/");
        }
        catch
        {
            MessageBox.Show(
                "Não foi possível iniciar o componente WebView2. Instale o \"Microsoft Edge WebView2 Runtime\" e abra o programa de novo.",
                "Eletroguia", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
        }
    }
    static void OpenDonationLink(string address)
    {
        if (Uri.TryCreate(address, UriKind.Absolute, out var uri) &&
            uri.Scheme == Uri.UriSchemeHttps &&
            uri.Host.Equals("livepix.gg", StringComparison.OrdinalIgnoreCase) &&
            (uri.AbsolutePath.Equals("/twoboby", StringComparison.OrdinalIgnoreCase) ||
             uri.AbsolutePath.StartsWith("/twoboby/", StringComparison.OrdinalIgnoreCase)))
        {
            Process.Start(new ProcessStartInfo(uri.ToString()) { UseShellExecute = true });
        }
    }
}
