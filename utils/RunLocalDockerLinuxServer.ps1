#########################################################
# RunLocalDockerLinuxServer.ps1
# Instead of deploying, we'll run it in a local container
#
# REQUIREMENTS:
# 1. Docker-Desktop
#########################################################

$PathToBuild = "../src/.hathora"
$BuildNameNoExt = "Hathora-Unity_LinuxServer"
$BuildName = "$BuildNameNoExt.x86_64.tar.gz"
$NewContainerDir = "$PathToBuild\LocalContainer"
$Port = 7777

#########################################################

# Init
cls
Write-Host "Starting..."

# Check if directory exists
if(!(Test-Path -Path $NewContainerDir )) {
  # Create directory if it doesn't exist
  New-Item -ItemType directory -Path $NewContainerDir
}

Write-Host "Preparing files in LocalContainer..."

# Use Robocopy to sync directories
robocopy $PathToBuild $NewContainerDir /mir

# Change to the directory
Set-Location -Path $NewContainerDir

# Extract the tar file to the directory
tar -xf "$PathToBuild\$BuildName"

Write-Host "Files prepared. Building Docker image..."

# Build Docker image
docker build -t my-unity-server .

Write-Host "Image built. Running Docker container..."

# Run Docker container
docker run -p $Port:$Port my-unity-server

Write-Host "Docker container is running at localhost:$Port"
