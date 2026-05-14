
function SetUpClientSecretForCustomGrantClient()
{   
    param(
        [String] $clientId = $(throw 'Error: No clientId was given.'),
        [String] $configFilePath = $(throw 'Error: No configFile path was given.')
    )
    Write-Host 'Setting up Custom Grant client.'
    
    $clientIdSettingKey = 'CustomGrant.ClientId'
    $clientSecretSettingKey = 'CustomGrant.ClientSecret'

    InsertClientSecretIntoConfigFile $configFilePath $clientId $clientIdSettingKey $clientSecretSettingKey $ConfirmitSiteIdentityClientKeyGeneratorSecret #$Confirmit.Site.Identity.ClientKeyGeneratorSecret      
}

function SetUpClientSecretForServiceClient($clientId, $pathToConfigFile)
{	
	Write-Host 'Setting up Service Client.'

	$clientIdSettingKey = 'ServiceClient.ClientId'
	$clientSecretSettingKey = 'ServiceClient.ClientSecret'

	InsertClientSecretIntoConfigFile $pathToConfigFile $clientId $clientIdSettingKey $clientSecretSettingKey $ConfirmitSiteIdentityClientKeyGeneratorSecret #$Confirmit.Site.Identity.ClientKeyGeneratorSecret      
}

function SetUpClientSecretForNodeJSClient($clientId, $pathToConfigFile)
{	
	Write-Host 'Setting up NodeJS Client.'

	$clientIdSettingKey = 'ClientId'
	$clientSecretSettingKey = 'ClientSecret'

	InsertClientSecretInJsonConfigFile $pathToConfigFile $clientId $clientIdSettingKey $clientSecretSettingKey $ConfirmitSiteIdentityClientKeyGeneratorSecret #$Confirmit.Site.Identity.ClientKeyGeneratorSecret      
}

function SetUpClientSecretForIntrospection($clientId, $pathToWebConfig)
{ 
    Write-Host 'Info: Setting up introspection client'
    
    $clientIdSettingKey = 'Introspection.ClientId'
    $clientSecretSettingKey = 'Introspection.ClientSecret'

    InsertClientSecretIntoConfigFile $pathToWebConfig $clientId $clientIdSettingKey $clientSecretSettingKey $ConfirmitSiteIdentityClientKeyGeneratorSecret #$Confirmit.Site.Identity.ClientKeyGeneratorSecret      
}

function SetUpClientSecretForIntrospectionInAppsettings($clientId, $pathToAppsettings)
{ 
    Write-Host 'Info: Setting up introspection client'    
    $settingKey = "Introspection"
    InsertClientSecretInAppsettingsFile $pathToAppsettings $clientId $settingKey $ConfirmitSiteIdentityClientKeyGeneratorSecret #$Confirmit.Site.Identity.ClientKeyGeneratorSecret      
}

function SetUpClientSecretForServiceClientInAppsettings($clientId, $pathToAppsettings)
{   
    Write-Host "Info: Setting up Service Client."
    $settingKey = "ServiceClient"
    InsertClientSecretInAppsettingsFile $pathToAppsettings $clientId $settingKey $ConfirmitSiteIdentityClientKeyGeneratorSecret #$Confirmit.Site.Identity.ClientKeyGeneratorSecret      
}

function SetUpClientSecretForCustomGrantClientInAppsettings($clientId, $pathToAppsettings)
{   
    Write-Host 'Info: Setting up Custom Grant client.'    
    $settingKey = 'CustomGrant'   
    InsertClientSecretInAppsettingsFile $pathToAppsettings $clientId $settingKey $ConfirmitSiteIdentityClientKeyGeneratorSecret #$Confirmit.Site.Identity.ClientKeyGeneratorSecret      
}

function EncryptWebConfigSection(){
    param(
      [String] $filePath = $(throw "Application web.config file path is mandatory"),
      [String] $sectionName = $(throw "Configuration section name is mandatory"),
      [String] $dataProtectionProvider = "RSAProtectedConfigurationProvider"
    )
    Write-Host "Loading config from:" $filePath
    $configuration = OpenWebConfiguration $filePath
    Write-Host "Loaded config:" $configuration

    EncryptConfigSection $configuration $sectionName $dataProtectionProvider  
}

function EncryptAppConfigSection(){
    param(
      [String] $filePath = $(throw "Application file path is mandatory"),
      [String] $sectionName = $(throw "Configuration section name is mandatory"),
      [String] $dataProtectionProvider = "RSAProtectedConfigurationProvider"
    )
    Write-Host "Loading config from:" $filePath
    $configuration = OpenAppConfiguration $filePath
    Write-Host "Loaded config:" $configuration

    EncryptConfigSection $configuration $sectionName $dataProtectionProvider   
}

function OpenWebConfiguration($configPath)
{
    #The System.Web assembly must be loaded
    $configurationAssembly = "System.Web, Version=4.0.0.0, Culture=Neutral, PublicKeyToken=b03f5f7f11d50a3a"
    [void] [Reflection.Assembly]::Load($configurationAssembly)
    Write-Host "Loaded assembly:" $configurationAssembly
    
    $configFile = New-Object System.IO.FileInfo($configPath)
    $vdm = New-Object System.Web.Configuration.VirtualDirectoryMapping($configFile.DirectoryName, $true, $configFile.Name)
    $wcfm = New-Object System.Web.Configuration.WebConfigurationFileMap
    $wcfm.VirtualDirectories.Add("/", $vdm)
    return [System.Web.Configuration.WebConfigurationManager]::OpenMappedWebConfiguration($wcfm, "/")
}

function OpenAppConfiguration($configPath)
{
    #The System.Configuration assembly must be loaded
    $configurationAssembly = "System.Configuration, Version=4.0.0.0, Culture=Neutral, PublicKeyToken=b03f5f7f11d50a3a"
    [void] [Reflection.Assembly]::Load($configurationAssembly)
    Write-Host "Loaded assembly:" $configurationAssembly
    
    $configFileMap = New-Object System.Configuration.ExeConfigurationFileMap
    $configFileMap.ExeConfigFilename = $configPath
    return [System.Configuration.ConfigurationManager]::OpenMappedExeConfiguration($configFileMap, [System.Configuration.ConfigurationUserLevel]::None)
}

function EncryptConfigSection(){
     param(
          $configuration = $(throw "Configuration object is mandatory"),
          [String] $sectionName = $(throw "Configuration section name is mandatory"),
          [String] $dataProtectionProvider = "RSAProtectedConfigurationProvider"
        )

    $section = $configuration.GetSection($sectionName)
    if(!$section)
        { throw "Could not find configuration section: $sectionName" }
    Write-Host "Loaded section:" $sectionName

    if (-not $section.SectionInformation.IsProtected)
    {
        Write-Host "Encrypting configuration section..."
        $section.SectionInformation.ProtectSection($dataProtectionProvider);
        $section.SectionInformation.ForceSave = [System.Boolean]::True;
        $configuration.Save([System.Configuration.ConfigurationSaveMode]::Modified); 
        Write-Host "Encryption complete."
    }
    else
    {
        Write-Host "Configuration section already encrypted"
    }
}

function CreateSecret($masterKey, $clientKey)
{
	 $combined = "$($masterKey)$($clientKey)"
	 $secret = Hash $combined
	 return $secret
}

function Hash($textToHash)
{
	$hasher = new-object System.Security.Cryptography.SHA256Managed
	$toHash = [System.Text.Encoding]::UTF8.GetBytes($textToHash)
	$hashByteArray = $hasher.ComputeHash($toHash)
	$hashedString = [Convert]::ToBase64String($hashByteArray)
	return $hashedString
}

function InsertClientSecretInJsonConfigFile() {
	param(
        [string] $pathToConfigFile = $(throw 'Error: No config file was found.'),
        [string] $clientId = $(throw 'Error: No clientId was given.'),
        [string] $clientIdSettingKey = $(throw 'Error: No clientIdSettingKey was given.'),
        [string] $clientSecretSettingKey = $(throw 'Error: No clientSecretSettingKey was given.'),
        [string] $generatorKey = $(throw 'Error: Variable Confirmit.Site.Identity.ClientKeyGeneratorSecret is not set.')
    )
	
	Write-Host 'Setting clientSecret'
	$clientSecret = CreateSecret $generatorKey $clientId
	$configurationFile = Get-Content $pathToConfigFile -raw | ConvertFrom-Json

	$configurationFile.Confirmit.Authentication.OpenIdConnect.PSObject.Properties[$clientSecretSettingKey].Value = $clientSecret
	$configurationFile.Confirmit.Authentication.OpenIdConnect.PSObject.Properties[$clientIdSettingKey].Value = $clientId


	Write-Host "Saving to: $($pathToConfigFile)"
	$configurationFile | ConvertTo-Json -Depth 10 | set-content $pathToConfigFile
}

function InsertClientSecretIntoConfigFile()
{
    param(
        [string] $configFile = $(throw 'Error: No config file was found.'),
        [string] $clientId = $(throw 'Error: No clientId was given.'),
        [string] $clientIdSettingKey = $(throw 'Error: No clientIdSettingKey was given.'),
        [string] $clientSecretSettingKey = $(throw 'Error: No clientSecretSettingKey was given.'),
        [string] $generatorKey = $(throw 'Error: Variable Confirmit.Site.Identity.ClientKeyGeneratorSecret is not set.')
    )
	
	$doc = (Get-Content $configFile) -as [Xml]
	
	$secureSectionHandler = $doc.SelectSingleNode("/configuration/configSections/section[@name='secureAppSettings']")
   
	if($secureSectionHandler -eq $null)
	{
		Write-Host 'Appending secure section handler'
		$secureSectionHandlerXml = '<section name="secureAppSettings" type="System.Configuration.NameValueSectionHandler,system, Version=4.0.0.0, Culture=neutral,PublicKeyToken=b77a5c561934e089" />'    
		$tempXmlDoc = new-object System.Xml.XmlDocument
		$tempXmlDoc.LoadXml($secureSectionHandlerXml)
		$secureSectionHandlerNode = $doc.ImportNode($tempXmlDoc.DocumentElement, $true)
		$doc.SelectSingleNode("/configuration/configSections").AppendChild($secureSectionHandlerNode) | Out-Null
	}
	   
	$secureAppSettingsNode = $doc.SelectSingleNode('/configuration/secureAppSettings')
   
	if($secureAppSettingsNode -eq $null)
	{
		Write-Host 'Appending secureAppSettings element'
		$secureAppSettingsNode = $doc.CreateElement('secureAppSettings');
		$doc.configuration.AppendChild($secureAppSettingsNode) | Out-Null
	}

	$clientIdSettings = $doc.configuration.secureAppSettings.add | where {$_.key -eq $clientIdSettingKey}
	foreach($node in $clientIdSettings)
	{
		$node.ParentNode.RemoveChild($node) | Out-Null
	}
	#add new element
	Write-Host 'Setting clientId'
	$addClientId = $doc.CreateElement('add')
	$addClientId.SetAttribute("key", $clientIdSettingKey) 
	$addClientId.SetAttribute("value", $clientId) 
	$secureAppSettingsNode.AppendChild($addClientId) | Out-Null

	$clientSecretSettings = $doc.configuration.secureAppSettings.add | where {$_.key -eq $clientSecretSettingKey}
	foreach($node in $clientSecretSettings)
	{
		$node.ParentNode.RemoveChild($node) | Out-Null
	}
	#add new element
	$clientSecret = CreateSecret $generatorKey $clientId
	Write-Host 'Setting clientSecret'
	$addClientSecret = $doc.CreateElement('add')
	$addClientSecret.SetAttribute("key", $clientSecretSettingKey) 
	$addClientSecret.SetAttribute("value", $clientSecret) 
	$secureAppSettingsNode.AppendChild($addClientSecret) | Out-Null
	
	$path = Resolve-Path $configFile
	Write-Host "Saving to: $($path)"
	$doc.Save($path)
	
}

function InsertClientSecretInAppsettingsFile()
{
    param(
        [string] $appsettingsFile = $(throw 'Error: No appsettings.json file was found.'),
        [string] $clientId = $(throw 'Error: No clientId was given.'),
        [string] $settingKey = $(throw 'Error: No settingKey was given.'),        
        [string] $generatorKey = $(throw 'Error: Variable Confirmit.Site.Identity.ClientKeyGeneratorSecret is not set.')
    )
    
    $appSettings = Get-Content -Raw -Path $appsettingsFile | ConvertFrom-Json
    if ( -Not ($appSettings.PSobject.Properties.Name -Contains "Confirmit" )) {
       Write-Host "Info: Adding Confirmit setting"
       $appSettings | Add-Member -MemberType NoteProperty -Name Confirmit -Value (New-Object -TypeName PSCustomObject)
    }

    if ( -Not ($appSettings.Confirmit.PSobject.Properties.Name -Contains "Authentication" ) ) {
       Write-Host "Info: Adding Authentication setting"
       $appSettings.Confirmit | Add-Member -MemberType NoteProperty -Name Authentication -Value (New-Object -TypeName PSCustomObject)
    }
    
    if ( -Not ($appSettings.Confirmit.Authentication.PSobject.Properties.Name -Contains $settingKey ) ) {
       Write-Host "Info: Adding $($settingKey) settings"
       $appSettings.Confirmit.Authentication | Add-Member -MemberType NoteProperty -Name $settingKey -Value (New-Object -TypeName PSCustomObject)
    }    
    
    Write-Host "Info: Setting clientId $($clientId)"
    if ( -Not ($appSettings.Confirmit.Authentication.($settingKey).PSobject.Properties.Name -Contains "ClientId" ) ) {
       $appSettings.Confirmit.Authentication.($settingKey) | Add-Member -MemberType NoteProperty -Name "ClientId" -Value (New-Object -TypeName PSCustomObject)
    }
    $appSettings.Confirmit.Authentication.($settingKey).ClientId = $clientId
        
    $clientSecret = CreateSecret $generatorKey $clientId
    Write-Host "Info: Setting clientSecret $($settingKey)"
    if ( -Not ($appSettings.Confirmit.Authentication.($settingKey).PSobject.Properties.Name -Contains "ClientSecret" ) ) {
       $appSettings.Confirmit.Authentication.($settingKey) | Add-Member -MemberType NoteProperty -Name "ClientSecret" -Value (New-Object -TypeName PSCustomObject)
    }
    $appSettings.Confirmit.Authentication.($settingKey).ClientSecret = $clientSecret
    
    $path = Resolve-Path $appsettingsFile
    Write-Host "Info: Saving to: $($path)"
    $appSettings | ConvertTo-Json -Depth 100 | Format-Json | Set-Content -Path $appsettingsFile    
}

# Formats JSON in a nicer format than the built-in ConvertTo-Json does.
function Format-Json([Parameter(Mandatory, ValueFromPipeline)][String] $json) {
  $indent = 0;
  ($json -Split '\n' |
    % {
      if ($_ -match '[\}\]]') {
        # This line contains  ] or }, decrement the indentation level
        $indent--
      }
      $line = (' ' * $indent * 2) + $_.TrimStart().Replace(':  ', ': ')
      if ($_ -match '[\{\[]') {
        # This line contains [ or {, increment the indentation level
        $indent++
      }
      $line
  }) -Join "`n"
}

function CreateClientSecret
{
    param(
        [string] $clientId = $(throw 'Error: No clientId was given.'),
        [string] $masterKey = $(throw 'Error: No masterKey was given.')
    )

  $clientSecret = CreateSecret $masterKey $clientId

  $encoding = [System.Text.Encoding]::UTF8
  return [Convert]::ToBase64String($encoding.GetBytes("${clientId}:${clientSecret}"))
}

function SetUpEncryptedConfiguration($serviceClient, $introspectionClient, $customGrantClient)
{  

  $pathToAppsettings = Join-Path $OctopusOriginalPackageDirectoryPath appsettings.json
  $pathToWebConfig = Join-Path $OctopusOriginalPackageDirectoryPath web.config

  if(Test-Path($pathToAppsettings)) {
    Write-Host "Info: Updating appsettings.json with client secrets"
    if($serviceClient -ne $null) {
        SetUpClientSecretForServiceClientInAppsettings $serviceClient $pathToAppsettings
    }    
    if($introspectionClient -ne $null) {
      SetUpClientSecretForIntrospectionInAppsettings $introspectionClient $pathToAppsettings
    }
    if($customGrantClient -ne $null) {
      SetUpClientSecretForCustomGrantClientInAppsettings $customGrantClient $pathToAppsettings
    }    
  }
  else {
    Write-Host "Info: Updating web.config with client secrets"
    if($serviceClient -ne $null) {
      SetUpClientSecretForServiceClient $serviceClient $pathToWebConfig
    }    
    if($introspectionClient -ne $null) {
      SetUpClientSecretForIntrospection $introspectionClient $pathToWebConfig
    }
    if($customGrantClient -ne $null) {
      SetUpClientSecretForCustomGrantClient $customGrantClient $pathToWebConfig
    }

    EncryptWebConfigSection $pathToWebConfig "secureAppSettings"
  }  
}