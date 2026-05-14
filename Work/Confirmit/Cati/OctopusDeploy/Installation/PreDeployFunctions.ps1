$global:parametersAreIncorrect = $false

function VerifyParameterAssignment($name, $value)
{
    if($value -eq $null)
    {
        $global:parametersAreIncorrect = $true
        Write-Host "Parameter ""$name"" is missing. Add it to Octopus variables"
    }
}

function VerifyStringParameter($name, $value)
{
    if($value -eq $null)
    {
        return
    }

    if(!$value)
    {
        $global:parametersAreIncorrect = $true
        Write-Host "Parameter ""$name"" is empty. It must have a value"
    }
}

function VerifyBoolParameter($name, $value)
{
    if($value -eq $null)
    {
        return
    }

    $out = $null
    if (![bool]::TryParse($value, [ref]$out)) 
    {
        $global:parametersAreIncorrect = $true
        Write-Host "Parameter ""$name"" has wrong value. It must be 'True' or 'False'"
    } 
}

function VerifyIntParameter($name, $value)
{
    if($value -eq $null)
    {
        return
    }

    $out = $null
    if (![int]::TryParse($value, [ref]$out)) 
    {
        $global:parametersAreIncorrect = $true
        Write-Host "Parameter ""$name"" has wrong value. It must be a number"
    } 
}

function PrintAllParameter($setting)
{
    foreach ($setting in $settings.GetEnumerator())
    {
        Write-Host "$($setting.Name) = $($setting.Value)"
    }
}

function VerifyAllParameterAssignment($setting)
{
    foreach ($setting in $settings.GetEnumerator())
    {
        VerifyParameterAssignment $($setting.Name) $($setting.Value)
    }
}

function FinishVerification
{
    if($global:parametersAreIncorrect -eq $true)
    {
        throw [System.Exception] "Stop execution because some parameters are incorrect"
    }
    else
    {
        Write-Host "All parameters are correct"
    }
}