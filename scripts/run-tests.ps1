# run-tests.ps1
$ErrorActionPreference = "Stop"

$solutionDir = "$PSScriptRoot\.."

Write-Host "Cleaning and restoring solution..." -ForegroundColor Cyan
dotnet clean "$solutionDir\LingoToneMVC.sln"
dotnet restore "$solutionDir\LingoToneMVC.sln"

Write-Host "`nBuilding solution..." -ForegroundColor Cyan
dotnet build "$solutionDir\LingoToneMVC.sln" --no-restore

Write-Host "`nRunning Unit & Integration Tests (xUnit)..." -ForegroundColor Cyan
dotnet test "$solutionDir\..\LingoToneMVC.Tests\LingoToneMVC.Tests.csproj" --no-build --logger "console;verbosity=detailed"

Write-Host "`nRunning E2E Playwright Tests..." -ForegroundColor Cyan
try {
    dotnet test "$solutionDir\..\LingoToneMVC.E2ETests\LingoToneMVC.E2ETests.csproj" --no-build --logger "console;verbosity=detailed"
} catch {
    Write-Host "`nWarning: E2E tests failed or could not run. Check if Playwright browsers are properly installed." -ForegroundColor Yellow
}

Write-Host "`nAll test commands executed." -ForegroundColor Green
