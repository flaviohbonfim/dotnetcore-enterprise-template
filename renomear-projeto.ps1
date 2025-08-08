# Define os nomes antigos e novos
$oldName = "Ambev.DeveloperEvaluation"
$newName = "DotNetCore.EnterpriseTemplate"

# Caminho para o diretório raiz do projeto (onde o script será executado)
$projectRoot = Get-Location

Write-Host "Iniciando a renomeação do projeto de '$oldName' para '$newName'..."

# 1. Renomear o arquivo de solução (.sln)
$slnFile = Get-ChildItem "$projectRoot\*.sln" | Where-Object { $_.Name -like "*$oldName*" }
if ($slnFile) {
    $newSlnFileName = $slnFile.Name -replace $oldName, $newName
    Rename-Item -Path $slnFile.FullName -NewName $newSlnFileName -Force
    Write-Host "Renomeado: $($slnFile.Name) -> $($newSlnFileName)"
} else {
    Write-Warning "Arquivo .sln contendo '$oldName' não encontrado. Verifique manualmente."
}

# 2. Renomear pastas de projetos dentro de 'src' e 'tests'
$projectDirs = Get-ChildItem -Path "$projectRoot\src", "$projectRoot\tests" -Directory -Recurse -ErrorAction SilentlyContinue | Where-Object { $_.Name -like "*$oldName*" }

foreach ($dir in $projectDirs) {
    $newDirName = $dir.Name -replace $oldName, $newName
    $newDirPath = Join-Path $dir.Parent.FullName $newDirName
    
    # Verifica se o novo caminho já existe para evitar erros
    if (-not (Test-Path $newDirPath)) {
        Rename-Item -Path $dir.FullName -NewName $newDirName -Force
        Write-Host "Renomeado diretório: $($dir.Name) -> $($newDirName)"
    } else {
        Write-Warning "O diretório '$newDirPath' já existe. Pulando renomeação para '$($dir.Name)'."
    }
}

# 3. Renomear arquivos .csproj dentro das pastas renomeadas
# (Precisa ser feito após a renomeação das pastas para que os caminhos estejam corretos)
$csprojFiles = Get-ChildItem -Path "$projectRoot\src", "$projectRoot\tests" -File -Filter "*.csproj" -Recurse -ErrorAction SilentlyContinue | Where-Object { $_.Name -like "*$oldName*" }

foreach ($file in $csprojFiles) {
    $newCsprojFileName = $file.Name -replace $oldName, $newName
    $newCsprojPath = Join-Path $file.Directory.FullName $newCsprojFileName

    # Verifica se o novo caminho já existe para evitar erros
    if (-not (Test-Path $newCsprojPath)) {
        Rename-Item -Path $file.FullName -NewName $newCsprojFileName -Force
        Write-Host "Renomeado arquivo .csproj: $($file.Name) -> $($newCsprojFileName)"
    } else {
        Write-Warning "O arquivo '$newCsprojPath' já existe. Pulando renomeação para '$($file.Name)'."
    }
}

# 4. Substituição global de texto dentro dos arquivos
Write-Host "Realizando substituição de texto global nos arquivos..."

# Define os tipos de arquivo para procurar
$fileTypes = "*.cs", "*.csproj", "*.sln", "*.json", "*.yml", "*.md", "Dockerfile"

Get-ChildItem -Path $projectRoot -Include $fileTypes -Recurse -ErrorAction SilentlyContinue | ForEach-Object {
    $filePath = $_.FullName
    $content = Get-Content $filePath -Raw
    
    # Substitui o nome antigo pelo novo, ignorando maiúsculas/minúsculas
    $newContent = $content -replace $oldName, $newName
    
    if ($newContent -ne $content) {
        Set-Content -Path $filePath -Value $newContent -Force
        Write-Host "Conteúdo atualizado em: $($filePath)"
    }
}

Write-Host "Processo de renomeação concluído. Por favor, verifique o projeto manualmente."