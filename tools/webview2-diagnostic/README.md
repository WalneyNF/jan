# WMK WebView2 Diagnóstico

Utilitário independente do Jan para verificar se o Microsoft Edge WebView2 inicializa e renderiza uma página HTML local no Windows 10/11.

## Como testar
1. Baixe o artefato do GitHub Actions **WMK-WebView2-Diagnostico-Windows-x64**.
2. Extraia o ZIP e execute `WMK-WebView2-Diagnostico.exe`.
3. Uma janela deve permanecer aberta mostrando a frase **SUCESSO!**.
4. Caso falhe, o aplicativo tenta mostrar o erro na própria janela.

O relatório fica em `%LOCALAPPDATA%\WMK-WebView2-Diagnostico\diagnostico.log`. Use o botão **Abrir pasta dos logs**.

O programa não utiliza IA, Ollama, Jan, Blender, conexão de rede ou arquivos do Jan. Requer o WebView2 Runtime instalado no computador. O executável de diagnóstico não tem assinatura digital e ainda precisa ser testado em Windows real.

Projeto: Walney Moreira Klein — walneyk@hotmail.com
