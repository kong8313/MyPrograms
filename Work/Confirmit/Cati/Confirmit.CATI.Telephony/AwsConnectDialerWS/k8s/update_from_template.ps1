dotnet new install Confirmit.Template.IIS-Application
Push-Location $PSScriptRoot
dotnet new iis-app -n cati -A "Confirmit.CATI.Dialer.AWSConnect" -N Confirmit.CATI.AWSConnectDialer -B ConfirmitCatiNew_Compile -S awsconnectdialer -H aws-connect-dialer-proxy -D false -o . --force --allow-scripts yes
Pop-Location