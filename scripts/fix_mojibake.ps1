$text = Get-Content -Raw -Path 'd:\LingoToneMVC\wwwroot\js\site.js' -Encoding UTF8
$bytes = [System.Text.Encoding]::GetEncoding(1252).GetBytes($text)
$fixedText = [System.Text.Encoding]::UTF8.GetString($bytes)
Set-Content -Path 'd:\LingoToneMVC\wwwroot\js\site.js' -Value $fixedText -Encoding UTF8
