param(
    [switch]$ResetDb
)

$ErrorActionPreference = 'Stop'

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path

Write-Host "== Sprint 4 Sandbox: Postgres ==" -ForegroundColor Cyan
Set-Location $PSScriptRoot

if ($ResetDb) {
    Write-Host "Resetting local DB volume (.pgdata)..." -ForegroundColor Yellow
    docker compose down
    if (Test-Path .\.pgdata) { Remove-Item -Recurse -Force .\.pgdata }
}

docker compose up -d

docker compose ps

$postgresContainer = (docker compose ps -q postgres).Trim()
if ([string]::IsNullOrWhiteSpace($postgresContainer)) {
    throw "Could not resolve Postgres container id (service 'postgres')."
}

Write-Host "Waiting for Postgres to become ready..." -ForegroundColor Cyan
$timeoutSeconds = 90
$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
$isReady = $false
while ($stopwatch.Elapsed.TotalSeconds -lt $timeoutSeconds) {
    docker exec $postgresContainer pg_isready -U unsa -d unsa *> $null
    if ($LASTEXITCODE -eq 0) {
        $isReady = $true
        break
    }
    Start-Sleep -Seconds 2
}

if (-not $isReady) {
    throw "Postgres did not become ready within $timeoutSeconds seconds."
}

# Create dedicated test database (idempotent)
$dbName = 'unsa_test_sprint4'
Write-Host "Ensuring database '$dbName' exists..." -ForegroundColor Cyan
$createDbSql = "SELECT 1 FROM pg_database WHERE datname = '$dbName';"
$exists = docker exec $postgresContainer psql -U unsa -d postgres -tAc $createDbSql
if ([string]::IsNullOrWhiteSpace($exists)) {
    docker exec $postgresContainer psql -U unsa -d postgres -c "CREATE DATABASE $dbName;"
}

# Set connection string for this PowerShell session
$env:ConnectionStrings__Database = "Host=localhost;Port=5432;Database=$dbName;Username=unsa;Password=unsa"
Write-Host "ConnectionStrings__Database set for this session:" -ForegroundColor Green
Write-Host $env:ConnectionStrings__Database

Write-Host "== Applying migrations (EF) ==" -ForegroundColor Cyan
Set-Location $repoRoot

dotnet build

# Standard modules

dotnet ef database update --project Modules/Identity/Identity.Infrastructure/Identity.Infrastructure.csproj --startup-project Application/Application.csproj --context AuthDbContext

dotnet ef database update --project Modules/University/University.Infrastructure/University.Infrastructure.csproj --startup-project Application/Application.csproj --context UniversityDbContext

dotnet ef database update --project Modules/Support/Support.Infrastructure/Support.Infrastructure.csproj --startup-project Application/Application.csproj --context SupportDbContext

# Faculty module (manual)

dotnet ef database update --project Modules/Faculty/Faculty.Infrastructure/Faculty.Infrastructure.csproj --startup-project Application/Application.csproj --context FacultyDbContext

Write-Host "== Seeding tenants + users ==" -ForegroundColor Cyan

dotnet run --project Tools/SandboxSeeder/SandboxSeeder.csproj

Write-Host "== Verification commands ==" -ForegroundColor Cyan
Write-Host "Backend:  cd Application; dotnet run" -ForegroundColor Gray
Write-Host "Frontend: cd Frontend; npm run dev" -ForegroundColor Gray
Write-Host "Trust HTTPS dev cert (one-time): dotnet dev-certs https --trust" -ForegroundColor Gray
