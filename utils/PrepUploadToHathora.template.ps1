########################################################
# Copy this template (Eg: `PrepUploadToHathora.ps1`) 
# and fill in secret keys
#######################################################

# Unity Build config
$sourceDir = "../src/"
$serverBuildDir = "$sourceDir/Build-Server/"
$serverExecutable = "Hathora-Unity-LinuxServer.x86_64""
$dockerfilePath = "$sourceDir/Dockerfile"
$archiveName = "Build-Server.tar"
$compressedArchiveName = "Build-Server.tar.gz"
$outputPath = "./uploadToHathora/"
$tempPath = "./_uploadToHathora/"

# Hathora Deploy Config
$appId = "TODO"
$roomsPerProcess = 1
$planName = "tiny"
$transportType = "udp"
$containerPort = 7777
$envJsonArr = "[]"
$hathoraConsoleAppBaseUrl = "https://console.hathora.dev/application/"

function GenerateDockerfile {
    $dockerfileContent = @"
FROM ubuntu

COPY ./Build-Server .

CMD ./$serverExecutable -mode server -batchmode -nographics
#-single-instance
"@
    Set-Content -Path $dockerfilePath -Value $dockerfileContent
}

function UploadToHathora {
	# Install/update prereqs
	Write-Host "Updating npm @hathora/cli ..."
	npm i -g @hathora/cli
	
	Write-Host ""
	Write-Host "Logging in ..."
	hathora-cloud login
	
	# Use the Hathora CLI to upload the build
	Write-Host "Uploading to Hathora..."
    $tarballPath = "$outputPath$compressedArchiveName"
    hathora-cloud deploy --file $tarballPath --appId $appId --roomsPerProcess $roomsPerProcess --planName $planName --transportType $transportType --containerPort $containerPort --env $envJsonArr
	
	# Prompt the user to view the app in the Hathora console
	$viewConsoleChoice = Read-Host "View the app in the Hathora console? (y/n)"
	if ($viewConsoleChoice -eq "y") {
		# Open the Hathora console in the default browser
		$consoleUrl = $hathoraConsoleAppBaseUrl + $appId
		Start-Process $consoleUrl
	}
}

# Ensure the output path and temporary path exist
Write-Host "Checking and creating output and temporary directories..."
if (!(Test-Path -Path $outputPath)) {
    New-Item -ItemType Directory -Path $outputPath
}
if (!(Test-Path -Path $tempPath)) {
    New-Item -ItemType Directory -Path $tempPath
}

# Check if the Linux build exists
if (!(Test-Path -Path "$serverBuildDir$serverExecutable")) {
    Write-Host "Error: Linux build not found. We currently only support Linux builds."
    exit
}

# Generate the Dockerfile
Write-Host "Generating Dockerfile..."
GenerateDockerfile

# Wipe the old, if any - just to be safe
Write-Host "Deleting old archives, if any..."
del ./uploadToHathora/*

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
Write-Host ""

# Prompt the user to upload to Hathora
$uploadChoice = Read-Host "Deploy new build to Hathora? (y/n)"
if ($uploadChoice -eq "y") {
    UploadToHathora
}