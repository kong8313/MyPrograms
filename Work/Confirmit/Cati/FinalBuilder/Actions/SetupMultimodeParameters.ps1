function Update-CatiParameterIfNeeded ([string] $parameterName, [string] $value, [string] $defaultValue) {

    $confirmlogDatabaseName = "confirmlog"

    $sqlQquery = "SELECT [$parameterName] FROM [company] WHERE companyid = 1"

    $currentParameterValue = GetParameterQuery $sqlQquery $parameterName $ConfirmitDatabaseServerSystemServerName $confirmlogDatabaseName $ConfirmitDatabaseUserDeployCName $ConfirmitDatabaseUserDeployCPassword

    if($currentParameterValue -eq $defaultValue){
        Write-Host "Update $parameterName parameter"
        $sqlQquery = "
            UPDATE [company] 
            SET [$parameterName] = $value
            WHERE companyid =  1"
        RunQuery $sqlQquery $ConfirmitDatabaseServerSystemServerName $confirmlogDatabaseName $ConfirmitDatabaseUserDeployCName $ConfirmitDatabaseUserDeployCPassword
    }
}

$ErrorActionPreference = 'Stop'

. ".\LoadParameters.ps1"

Write-Host "Stop Agent service"
Get-Service 'Confirmit Agent Controller' | Stop-Service

Write-Host "Write default CATI parameters to default company"
Update-CatiParameterIfNeeded "CatiCompanyIdentifier" "'Confirmit'" "CompanyAlias1"
Update-CatiParameterIfNeeded "MaxConcurrentInterviewers" "100" "0"
Update-CatiParameterIfNeeded "TelephonyEnabled" "1" "False"
Update-CatiParameterIfNeeded "PredictiveDiallingEnabled" "1" "False"
Update-CatiParameterIfNeeded "MaxIvrAgents" "100" "0"

Write-Host "Set developemnt environment variables"
[System.Environment]::SetEnvironmentVariable('ASPNETCORE_ENVIRONMENT', 'Development', 'Machine')

Write-Host "Start Agent service"
Get-Service 'Confirmit Agent Controller' | Start-Service
