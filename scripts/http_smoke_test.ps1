$urls = @(
    "/",
    "/Account/Login",
    "/Account/Register",
    "/Lesson/Detail/1001",
    "/Lesson/Detail/1002",
    "/Quiz?lessonId=1001",
    "/Quiz?lessonId=1002"
)

$baseUrl = "http://localhost:5070"
$passCount = 0
$failCount = 0

Write-Host "Starting HTTP Smoke Test..."
Write-Host "--------------------------------------------------"

foreach ($url in $urls) {
    $fullUrl = "$baseUrl$url"
    try {
        $response = Invoke-WebRequest -Uri $fullUrl -UseBasicParsing -MaximumRedirection 0 -ErrorAction SilentlyContinue
        if ($response.StatusCode -eq 200 -or $response.StatusCode -eq 301 -or $response.StatusCode -eq 302) {
            Write-Host "[PASS] $fullUrl - Status: $($response.StatusCode)" -ForegroundColor Green
            $passCount++
        } else {
            Write-Host "[FAIL] $fullUrl - Status: $($response.StatusCode)" -ForegroundColor Red
            $failCount++
        }
    } catch {
        $exception = $_.Exception.Response
        if ($exception) {
            $statusCode = $exception.StatusCode.value__
            if ($statusCode -eq 302 -or $statusCode -eq 301) {
                Write-Host "[PASS] $fullUrl - Status: $statusCode (Redirect)" -ForegroundColor Green
                $passCount++
            } else {
                Write-Host "[FAIL] $fullUrl - Status: $statusCode" -ForegroundColor Red
                $failCount++
            }
        } else {
            Write-Host "[FAIL] $fullUrl - Error: $($_.Exception.Message)" -ForegroundColor Red
            $failCount++
        }
    }
}

Write-Host "--------------------------------------------------"
Write-Host "Total Tests: $($urls.Count)"
Write-Host "PASS: $passCount" -ForegroundColor Green
Write-Host "FAIL: $failCount" -ForegroundColor Red
