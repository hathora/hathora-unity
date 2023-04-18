$PathToExe = "../Build/Hathora-Unity.exe"
$NumClients = Read-Host -Prompt 'Num Clients to Create'

Write-Host ""
Write-Host "Creating 1 server @ '$PathToExe' ..."
& $PathToExe -mode server -logfile log-server.txt -memo "Server"
Write-Host "Done."

Write-Host ""
Write-Host "Preparing to create $NumClients client(s):"
for ($i = 0; $i -le $NumClients)
{
	Write-Host "Creating client$i ..."
	& $PathToExe -mode client -logfile log-client$i.txt -memo "Client$i"
	$i++
} 
Write-Host "Done."

Write-Host ""
Write-Host "Ready!"
