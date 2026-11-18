# Ставит подмену dwgdxfExp.rtw: штатный экспорт остаётся, в DXF пишется $INSUNITS = 4.
# Запуск от администратора, КОМПАС должен быть закрыт.
param(
    [switch]$Uninstall
)

$ErrorActionPreference = 'Stop'
$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Write-Error "Нужны права администратора. Запустите PowerShell от имени администратора и повторите."
}

$kompas = (Get-ItemProperty 'HKLM:\SOFTWARE\ASCON\KOMPAS-3D').CurrentExe
$root = Split-Path (Split-Path $kompas -Parent) -Parent
$imp = Join-Path $root 'Libs\ImpExp'
$target = Join-Path $imp 'dwgdxfExp.rtw'
$backup = Join-Path $imp 'dwgdxfExp_ascon.rtw'
$built = Join-Path $PSScriptRoot 'bin\dwgdxfExp.rtw'

if (Get-Process KOMPAS -ErrorAction SilentlyContinue) {
    Write-Error "Закройте КОМПАС и запустите установку снова."
}

if ($Uninstall) {
    if (-not (Test-Path $backup)) {
        Write-Error "Резервная копия не найдена: $backup"
    }
    Copy-Item $backup $target -Force
    Write-Output "Штатная библиотека восстановлена: $target"
    return
}

if (-not (Test-Path $built)) {
    Write-Error "Сначала соберите патч. Нет файла $built"
}

$installed = $false
if (Test-Path $target) {
    $text = [Text.Encoding]::ASCII.GetString([IO.File]::ReadAllBytes($target))
    $installed = $text.Contains('DxfUnitsPatchInstalled')
}

if (-not $installed) {
    Copy-Item $target $backup -Force
}
elseif (-not (Test-Path $backup)) {
    Write-Error "Патч уже стоит, но резервная копия $backup отсутствует. Восстановите dwgdxfExp.rtw из дистрибутива КОМПАС."
}

Copy-Item $built $target -Force
Write-Output "Патч установлен: $target"
Write-Output "Штатная библиотека сохранена: $backup"
