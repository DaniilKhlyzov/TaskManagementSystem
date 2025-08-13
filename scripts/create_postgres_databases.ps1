Param(
    [string]$DbHost = "localhost",
    [int]$Port = 5432,
    [string]$Username = "postgres",
    [string]$Password = "your_password",
    [string]$Owner = "postgres",
    [string[]]$Databases = @("TaskManagement_Auth", "TaskManagement_Tasks", "TaskManagement_Notifications")
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Ensure-Tool([string]$name) {
    $cmd = Get-Command $name -ErrorAction SilentlyContinue
    if (-not $cmd) {
        throw "Требуется утилита '$name' в PATH. Установите PostgreSQL (psql/createdb) или добавьте путь к ним в PATH."
    }
}

function Test-DatabaseExists([string]$db) {
    $env:PGPASSWORD = $Password
    $query = "SELECT 1 FROM pg_database WHERE datname = '$db'"
    $output = & psql -h $DbHost -p $Port -U $Username -d postgres -Atc $query 2>$null
    return $output -eq '1'
}

function New-DatabaseIfMissing([string]$db) {
    if (Test-DatabaseExists -db $db) {
        Write-Host "БД '$db' уже существует — пропускаю" -ForegroundColor Yellow
        return
    }

    $env:PGPASSWORD = $Password
    # Попытка через createdb (проще и без транзакций)
    try {
        & createdb -h $DbHost -p $Port -U $Username --owner=$Owner --encoding=UTF8 $db 2>$null
        Write-Host "Создана БД '$db' (createdb)" -ForegroundColor Green
        return
    } catch {
        Write-Host "createdb недоступен или возникла ошибка, пробую через psql" -ForegroundColor DarkYellow
    }

    # Резервный путь: через psql и команду CREATE DATABASE
    try {
        $create = "CREATE DATABASE `"$db`" OWNER `"$Owner`" ENCODING 'UTF8' TEMPLATE template0;"
        & psql -h $DbHost -p $Port -U $Username -d postgres -c $create | Out-Null
        Write-Host "Создана БД '$db' (psql)" -ForegroundColor Green
    } catch {
        throw "Не удалось создать БД '$db': $($_.Exception.Message)"
    }
}

# Проверки утилит
Ensure-Tool -name psql
try { Ensure-Tool -name createdb } catch { Write-Host $_ -ForegroundColor DarkYellow }

Write-Host "Подключение к PostgreSQL: $Username@${DbHost}:$Port (owner: $Owner)" -ForegroundColor Cyan

foreach ($db in $Databases) {
    New-DatabaseIfMissing -db $db
}

Write-Host "Готово." -ForegroundColor Cyan


