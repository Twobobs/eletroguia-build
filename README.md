# Eletroguia — Cálculo de Motores

**Calcule, organize e consulte dados de motores de indução em um aplicativo Windows de tela cheia.** O Eletroguia reúne ferramentas de cálculo, referências e histórico em uma interface feita para o dia a dia de quem trabalha com motores.

[⬇️ Baixar para Windows 64 bits](https://github.com/Twobobs/eletroguia-build/releases/latest) · [🐞 Reportar um problema](https://github.com/Twobobs/eletroguia-build/issues/new?template=bug_report.yml) · [💡 Sugerir uma melhoria](https://github.com/Twobobs/eletroguia-build/issues/new?template=feature_request.yml) · [💚 Apoiar o projeto](https://livepix.gg/twoboby)

> **Versão de testes:** este aplicativo está em evolução. Experimente, envie comentários e reporte resultados inesperados. Confira as notas e os requisitos na página de cada lançamento.

## O que é o Eletroguia?

O Eletroguia é um sistema de apoio para cálculos e consulta de dados de motores de indução. O aplicativo foi escrito em **.NET 8 / C# para Windows**, com a interface e as telas de cálculo em HTML, executadas dentro de uma janela nativa com WebView2. Assim, ele mantém a interface original e oferece instalação e inicialização como aplicativo de desktop.

## O que você pode fazer

- **Calcular motores trifásicos e monofásicos** com os dados disponíveis na interface.
- **Consultar fórmulas, referências e parâmetros** na aba Ajustes.
- **Guardar e consultar cálculos e clientes** no histórico do aplicativo.
- **Usar exemplos de verificação** para conferir entradas e resultados.
- **Fazer backup e restaurar dados** pelas opções disponíveis no aplicativo.
- **Alternar o idioma entre português e inglês.**
- **Abrir em tela cheia**, com a experiência de aplicativo para Windows.
- **Apoiar o desenvolvimento** pela área de doações integrada.

Os campos e resultados variam conforme o tipo de cálculo escolhido. Use os dados informados na interface e revise as entradas antes de salvar ou usar um resultado.

## Como instalar e começar

1. Baixe **Eletroguia-Setup.exe** na seção [Releases](https://github.com/Twobobs/eletroguia-build/releases/latest).
2. Execute o instalador e siga as instruções do Windows. O atalho na área de trabalho é **Eletroguia Cálculo de Motores**.
3. Abra o aplicativo. A janela principal e o conteúdo interno iniciam em tela cheia.
4. Escolha o tipo de motor e preencha os dados solicitados.
5. Consulte fórmulas e parâmetros em **Ajustes**, alterne o idioma quando quiser e use o histórico para encontrar trabalhos anteriores.

O instalador é destinado a **Windows 10/11 de 64 bits**. O aplicativo é publicado como executável .NET autocontido; o instalador pode obter o Microsoft Edge WebView2 Runtime caso ele ainda não esteja disponível no computador.

## Privacidade e limites

O Eletroguia é um aplicativo local. Os dados de clientes e cálculos são mantidos no dispositivo conforme os recursos de armazenamento e backup do app. A área de doação abre um serviço externo quando você escolhe contribuir.

O app é uma ferramenta de apoio. Confira fórmulas, parâmetros, unidades e resultados com fontes técnicas confiáveis e com um profissional qualificado antes de tomar decisões de projeto, manutenção ou segurança. Os resultados não substituem inspeção, ensaio, normas aplicáveis ou responsabilidade técnica.

## Feedback e comunidade

Seu retorno ajuda a melhorar o Eletroguia:

- [Reportar um bug](https://github.com/Twobobs/eletroguia-build/issues/new?template=bug_report.yml) — inclua o que você fez, o que esperava e o que aconteceu.
- [Sugerir uma função](https://github.com/Twobobs/eletroguia-build/issues/new?template=feature_request.yml) — descreva a necessidade e como a ideia ajudaria.
- Consulte [problemas já registrados](https://github.com/Twobobs/eletroguia-build/issues) antes de abrir um novo.
- Ao relatar um problema, não publique dados pessoais, dados de clientes ou informações confidenciais.

## Apoie o projeto

Se o Eletroguia for útil para você, considere apoiar seu desenvolvimento:

- **Brasil — LivePix:** [livepix.gg/twoboby](https://livepix.gg/twoboby)
- A opção internacional via Stripe ainda não está disponível no aplicativo.

## Para quem deseja contribuir com o código

O projeto usa **.NET 8, Windows Forms, WebView2, HTML, CSS e JavaScript**. A interface principal fica em `html/Eletroguia.html`; o hospedeiro Windows está em `Program.cs`. O script `build.bat` publica para `win-x64` e gera o instalador com Inno Setup (necessário no ambiente de compilação).

Para compilar no Windows, instale o .NET 8 SDK, o Node.js e o Inno Setup; depois execute `build.bat`. Consulte também [LEIA-ME.txt](LEIA-ME.txt) para notas do pacote e proteção do conteúdo.

## English

**Eletroguia is a full-screen Windows desktop tool for induction motor calculations and reference data.** It combines calculation screens, formulas, saved records, and language options in one place.

The desktop host is built with **.NET 8 / C#**. Its calculation interface is HTML, displayed inside a native Windows window with WebView2. Available tools include single-phase and three-phase motor calculations, formulas and parameters in Settings, saved calculations and customer records, verification examples, backup and restore, Portuguese and English languages, and an integrated donation area.

### Download and use

Download **Eletroguia-Setup.exe** from [Releases](https://github.com/Twobobs/eletroguia-build/releases/latest), run the installer on **64-bit Windows 10/11**, and launch **Eletroguia Cálculo de Motores** from the desktop. Choose a motor type, fill in the requested data, review the result, and use Settings to consult formulas or change the language. The app opens full screen. WebView2 Runtime may be installed by the setup if it is missing.

### Feedback and support

- [Report a bug](https://github.com/Twobobs/eletroguia-build/issues/new?template=bug_report.yml)
- [Suggest a feature](https://github.com/Twobobs/eletroguia-build/issues/new?template=feature_request.yml)
- [Browse existing issues](https://github.com/Twobobs/eletroguia-build/issues)
- [Support the project via LivePix](https://livepix.gg/twoboby) (Brazil). The international Stripe option is not available yet.

This is a work in progress. Please verify formulas, units, parameters, and results with reliable technical references and a qualified professional before using them for engineering, maintenance, or safety decisions. The app is a support tool and does not replace inspection, testing, applicable standards, or professional responsibility.

## Licença

Consulte os arquivos deste repositório ou entre em contato com o mantenedor para esclarecer os termos de uso e redistribuição.
