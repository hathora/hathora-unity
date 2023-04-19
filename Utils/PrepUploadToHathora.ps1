# Define paths
$sourceDir = "../src/"
$serverBuildDir = "$sourceDir/Build-Server/"
$dockerfilePath = "$sourceDir/Dockerfile"
$archiveName = "Build-Server.tar"
$compressedArchiveName = "Build-Server.tar.gz"
$outputPath = "./uploadToHathora/"
$tempPath = "./_uploadToHathora/"

# Ensure the output path and temporary path exist
Write-Host "Checking and creating output and temporary directories..."
if (!(Test-Path -Path $outputPath)) {
    New-Item -ItemType Directory -Path $outputPath
}
if (!(Test-Path -Path $tempPath)) {
    New-Item -ItemType Directory -Path $tempPath
}

# Create a tar archive in the temporary path
Write-Host "Creating tar archive..."
7z.exe a -ttar "$tempPath$archiveName" $serverBuildDir $dockerfilePath

# Compress the tar archive in the temporary path
Write-Host "Compressing tar archive..."
7z.exe a -tgzip "$tempPath$compressedArchiveName" "$tempPath$archiveName"

# Remove the intermediate tar file in the temporary path
Write-Host "Removing intermediate tar file..."
Remove-Item "$tempPath$archiveName"

# Move the final tarball from the temporary path to the output path
Write-Host "Moving final tarball to the output path..."
Move-Item -Path "$tempPath$compressedArchiveName" -Destination $outputPath

# Clean up the temporary directory
Write-Host "Cleaning up the temporary directory..."
Remove-Item -Recurse -Force $tempPath

Write-Host "Process complete."