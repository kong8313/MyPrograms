function RunQuery($query, $serverName, $databaseName, $login, $pass)
{
	$connectionString = "Server=$serverName;Database=$databaseName;User Id=$login;Password=$pass" # ;Encrypt=False;

	$connection = New-Object System.Data.SqlClient.SqlConnection $connectionString
	$command = $connection.CreateCommand()
	$command.CommandText = $query

	$connection.Open()
	$command.ExecuteNonQuery()
	$connection.Close()
}

function GetParameterQuery($query, $parameterName, $serverName, $databaseName, $login, $pass)
{
	$connectionString = "Server=$serverName;Database=$databaseName;User Id=$login;Password=$pass" #;Encrypt=False;

	$connection = New-Object System.Data.SqlClient.SqlConnection $connectionString
	$command = $connection.CreateCommand()
	$command.CommandText = $query

	$connection.Open()
	$reader = $command.ExecuteReader()

	$result = ""
	if ($reader.Read()) {
		$result = "" + $reader[$parameterName]
	}
	$connection.Close()
	
	return $result
}