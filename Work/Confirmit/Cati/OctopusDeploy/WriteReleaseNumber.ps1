function UpdateSystemSettingsValue($connectionString, $settingName, $settingValue)
{
    $query = "UPDATE [dbo].[BvSystemSettings] SET [Value] = @Value WHERE [SystemName] = @Name"

    $conn = New-Object System.Data.SqlClient.SQLConnection
    $conn.ConnectionString=$connectionString
    $conn.Open()
    $cmd = New-Object system.Data.SqlClient.SqlCommand($query, $conn)
    $cmd.Parameters.Add("@Value", $settingValue) | Out-Null
    $cmd.Parameters.Add("@Name", $settingName) | Out-Null
    $cmd.ExecuteNonQuery() | Out-Null
    $conn.Close()
}

$connectionString = "Server={0};Database=ConfirmitCATIV15;UID={1};PWD={2}" -f $CatiDatabaseServerName, $ConfirmitDatabaseUserSystemAdminName, $ConfirmitDatabaseUserSystemAdminPassword

Write-Host "Set release number in $OctopusReleaseNumber"
UpdateSystemSettingsValue $connectionString 'Setup.ReleaseNumber' $OctopusReleaseNumber

$backendVersion = $OctopusActionPackageNuGetPackageVersion + ".0"
Write-Host "Set backend version in $backendVersion"
UpdateSystemSettingsValue $connectionString 'Setup.BackendVersion' $backendVersion

$releaseDate = Get-Date -Format "dd.MM.yyyy"
Write-Host "Set release date in $releaseDate"
UpdateSystemSettingsValue $connectionString 'Setup.ReleaseDate' $releaseDate