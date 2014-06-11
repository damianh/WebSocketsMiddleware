param(
	[int]$buildNumber = 0
	)
$packageConfigs = Get-ChildItem . -Recurse | where{$_.Name -eq "packages.config"}
foreach($packageConfig in $packageConfigs){
	Write-Host "Restoring" $packageConfig.FullName 
	src\.nuget\nuget.exe i $packageConfig.FullName -o src\packages
}
Import-Module .\src\packages\psake.4.3.2\tools\psake.psm1
Import-Module .\BuildFunctions.psm1
Invoke-Psake .\default.ps1 default -framework "4.0x64" -properties @{ buildNumber=$buildNumber }
Remove-Module BuildFunctions
Remove-Module psake