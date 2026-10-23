using System.Reflection;
using System.Security.Cryptography;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace Eletroguia;

static class Program
{
    // Preenchido por tools/encrypt.mjs a cada build. Não edite à mão.
    //KEY-BEGIN
    static readonly byte[] KA = { 185, 168, 55, 37, 39, 162, 148, 125, 199, 100, 69, 150, 188, 253, 118, 1, 40, 63, 211, 126, 192, 220, 28, 145, 137, 83, 166, 220, 143, 203, 28, 145 };
    static readonly byte[] KM = { 9, 161, 176, 85, 34, 224, 26, 191, 164, 219, 84, 79, 147, 156, 100, 218, 68, 217, 73, 165, 54, 236, 165, 88, 230, 224, 189, 252, 91, 187, 214, 113 };
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
                    e.Cancel = true;
            };
            c.NewWindowRequested += (_, e) => e.Handled = true;
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
}
