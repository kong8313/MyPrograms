dotnet new install Confirmit.Template.IIS-Application
Push-Location $PSScriptRoot
dotnet new iis-app -n cati -A "Confirmit.CATI.Dialer.Simulator" -N Confirmit.CATI.GenericDialerSimulator -B ConfirmitCatiNew_Compile -S catidialersimulator -H cati-dialer-simulator -D false -o . --force --allow-scripts yes
Pop-Location