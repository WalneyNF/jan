using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Drawing;
using System.Windows.Forms;

namespace WmkWebView2Test;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new DiagnosticForm());
    }
}

internal sealed class DiagnosticForm : Form
{
    private readonly WebView2 _webView = new() { Dock = DockStyle.Fill };
    private readonly Label _status = new() { Dock = DockStyle.Top, Height = 70, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(12) };
    private readonly Button _logsButton = new() { Text = "Abrir pasta dos logs", Dock = DockStyle.Bottom, Height = 38 };
    private readonly string _dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WMK-WebView2-Diagnostico");
    private string LogPath => Path.Combine(_dataDir, "diagnostico.log");

    public DiagnosticForm()
    {
        Text = "WMK - Diagnostico WebView2 (independente do Jan)";
        StartPosition = FormStartPosition.CenterScreen;
        Size = new Size(1000, 720);
        MinimumSize = new Size(640, 420);

        Directory.CreateDirectory(_dataDir);
        Controls.Add(_webView);
        Controls.Add(_status);
        Controls.Add(_logsButton);
        _logsButton.Click += (_, _) => System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = _dataDir, UseShellExecute = true });
        Shown += async (_, _) => await StartTestAsync();
    }

    private void Log(string text)
    {
        var entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {text}";
        try { File.AppendAllText(LogPath, entry + Environment.NewLine); } catch { /* Keep diagnostic window responsive. */ }
        _status.Text = text + Environment.NewLine + "Log: " + LogPath;
    }

    private async Task StartTestAsync()
    {
        try
        {
            Log("Jan nao participa deste teste. Verificando ambiente...");
            var installed = CoreWebView2Environment.GetAvailableBrowserVersionString();
            Log("WebView2 instalado: " + installed);
            var env = await CoreWebView2Environment.CreateAsync(
                userDataFolder: Path.Combine(_dataDir, "WebView2Profile"));
            Log("Ambiente WebView2 criado; iniciando controle...");
            _webView.CoreWebView2InitializationCompleted += (_, e) =>
            {
                if (!e.IsSuccess)
                    Log("FALHA ao iniciar WebView2: " + e.InitializationException);
                else
                    Log("WebView2 inicializado com sucesso.");
            };
            await _webView.EnsureCoreWebView2Async(env);
            _webView.CoreWebView2.ProcessFailed += (_, e) =>
                Log($"Falha do processo WebView2: {e.ProcessFailedKind} / {e.Reason}");
            _webView.CoreWebView2.NavigationCompleted += (_, e) =>
                Log(e.IsSuccess
                    ? "SUCESSO: HTML local carregado. WebView2 funciona neste teste."
                    : $"FALHA na navegacao: {e.WebErrorStatus}");
            _webView.NavigateToString("""
                <!doctype html><html lang="pt-BR"><head><meta charset="utf-8">
                <style>body{font:20px Segoe UI;background:#101923;color:#eaf5ff;padding:36px}strong{color:#52e1b5}code{color:#a6c8ff}</style>
                </head><body><h1>WMK — Diagnóstico WebView2</h1>
                <p><strong>SUCESSO!</strong> Se consegue ler esta página, o WebView2 conseguiu renderizar HTML local.</p>
                <p>Isso não garante que o Jan funcione, mas ajuda a isolar o problema.</p>
                <p>Log em <code>%LOCALAPPDATA%\WMK-WebView2-Diagnostico\diagnostico.log</code></p>
                </body></html>
                """);
        }
        catch (Exception ex)
        {
            Log("ERRO: " + ex);
            MessageBox.Show(this, ex.ToString(), "WMK WebView2 - erro detectado", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
