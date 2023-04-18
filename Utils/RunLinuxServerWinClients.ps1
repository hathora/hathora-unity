##################################################################################
# ABOUT: Creates 1 LinuxServer (via WSL2) + x WinClients.
##################################################################################
# ARG LIST: https://docs.unity3d.com/Manual/PlayerCommandLineArguments.html 
# -batchmode is useful, but it's hard to know when to -quit (or need to end task)
# -nographics (combined with -batchmode) may improve startup time?
##################################################################################
$PathToWinClient = "../src/Build-Client/Hathora-Unity.exe"
$PathToLinuxServer = "../src/Build-Server/HathoraServer.x86_64"
$NumClients = Read-Host -Prompt 'Num Clients to Create'

Write-Host ""
Write-Host "Preparing to create $NumClients WinClient(s):"
for ($i = 0; $i -le $NumClients)
{
	Write-Host "Creating client$i ..."
	& $PathToWinClient -mode client -logfile logs/log-client$i.txt -memo "Client$i"
	$i++
} 
Write-Host "Done."

Write-Host ""
Write-Host "Creating 1 LinuxServer @ '$PathToLinuxServer' ..."
Write-Host ""

wsl.exe -e $PathToLinuxServer -mode server -single-instance -logfile logs/log-server.txt -memo "Server" 
Write-Host "Done."

Write-Host ""
Write-Host "Ready!"
