dotnet new install Confirmit.Template.IIS-Application
Push-Location $PSScriptRoot
dotnet new iis-app -n cati -A "Confirmit.Cati.Supervisor" -N Confirmit.CATI.Supervisor.New -B ConfirmitCati_FeatureBranch_4CatiBuildInstallations -S supervisor -H cati-supervisor-legacy-client -D false -o . --force --allow-scripts yes
Pop-Location