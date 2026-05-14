

$surveys = $Api.Databases.Cati.ExecuteRowList('SELECT SID as SurveyId, Name as ProjectId FROM BvSurvey WHERE IsQuotaInCatiDb IS NULL')

foreach( $survey in $surveys )
{
    $surveyId = $survey.Item('SurveyId');
    $projectId = $survey.Item('ProjectId');

    Write-Host "Processing Survey(SurveyId=$surveyId, ProjectId=$projectId)...";

    try{
        $surveyDb = $Api.Databases.Survey($projectId);
        if( -not $surveyDb.IsExist()){
            $surveyDb.Attach();
            Write-Host "   Survey database were attached successfully.";
        }

        $quotasColumns = $surveyDb.ExecuteRow("SELECT 
	        CASE WHEN EXISTS( SELECT * FROM INFORMATION_SCHEMA.COLUMNS c WHERE c.COLUMN_NAME = 'iscati' AND c.TABLE_NAME = 'quotas' ) THEN 1 ELSE 0 END AS iscati,
	        CASE WHEN EXISTS( SELECT * FROM INFORMATION_SCHEMA.COLUMNS c WHERE c.COLUMN_NAME = 'is_optimistic' AND c.TABLE_NAME = 'quotas' ) THEN 1 ELSE 0 END AS is_optimistic")
    
        if($quotasColumns.Item('iscati')  -eq 0){ 
            $Api.Databases.Cati.ExecuteNonQuery("UPDATE BvSurvey SET IsQuotaInCatiDb = 0 WHERE SID = $surveyId")
            Write-Host "   Skiped, because quotas table doesn't contains iscati column!";
            continue;
        }

        $Api.Databases.Cati.ExecuteNonQuery("DELETE FROM BvSurveyQuotaCell WHERE SurveyId = $surveyId
                                             DELETE FROM BvSurveyQuota WHERE SurveyId = $surveyId")

        $quotas = $surveyDb.ExecuteRowList("SELECT quotaid, quotaname, tablename, iscati FROM quotas where iscati > 0");

        foreach( $quota in $quotas )
        {
            $quotaid = $quota.Item('quotaid');
            $quotaName = $quota.Item('quotaname');
            $tablename = $quota.Item('tablename');
            $iscati = $quota.Item('iscati');
            $fields = $surveyDb.ExecuteScalarList("SELECT fieldname FROM quota_field WHERE quotaid = $quotaid");

            $quotaColumns = $surveyDb.ExecuteRow("SELECT 
	            CASE WHEN EXISTS( SELECT * FROM INFORMATION_SCHEMA.COLUMNS c WHERE c.COLUMN_NAME = 'live_counter' AND c.TABLE_NAME = '$tablename' ) THEN 1 ELSE 0 END AS live_counter,
                CASE WHEN EXISTS( SELECT * FROM INFORMATION_SCHEMA.COLUMNS c WHERE c.COLUMN_NAME = 'live_limit' AND c.TABLE_NAME = '$tablename' ) THEN 1 ELSE 0 END AS live_limit,
	            CASE WHEN EXISTS( SELECT * FROM INFORMATION_SCHEMA.COLUMNS c WHERE c.COLUMN_NAME = 'disabled' AND c.TABLE_NAME = '$tablename' ) THEN 1 ELSE 0 END AS disabled")

            $live_counter = if($quotaColumns.Item('live_counter') -eq 0){ "0" }else { "live_counter" }
            $live_limit = if($quotaColumns.Item('live_limit') -eq 0){ "0" }else { "live_limit" }
            $disabled = if($quotaColumns.Item('disabled') -eq 0){ "0" }else { "disabled" }


            $selectCellQuery = "select 
	                $surveyId as SurveyID, 
	                $quotaid as QuotaID, 
	                quotaid as CellID, 
	                counter as Counter, 
	                limit as Limit, 
	                $live_counter as LiveCounter,  
	                $live_limit as LiveLimit, 
	                $disabled as IsDisabled,
	                CAST( '<QuotaCellData xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><FieldValues>" +
		                ( $fields | %{ "<QuotaCellFieldValue><Field>$_</Field><Value>' + $_ + '</Value></QuotaCellFieldValue>" } ) + 
                        "</FieldValues></QuotaCellData>'
	                AS XML) as XmlData
	                FROM $tablename"

            $migratedCells = $Api.Databases.Cati.CopyDataToExistTable($surveyDb, 'BvSurveyQuotaCell', $selectCellQuery );

	        Write-Host "   QuotaName='$quotaName', fields='$($fields -join ',')', iscati=$iscati, migratedCells=$migratedCells"
        }

        $isOptimisticColumnName = if($quotasColumns.Item('is_optimistic') -eq 0){'0'}else{'is_optimistic'}

        $selectQuotaQuery = "
            select 
	            $surveyId as SurveyID,
	            quotaid as QuotaID, 
	            quotaname as Name, 
	            tablename as TableName, 
	            email as Email, 
	            CASE WHEN iscati = 1 THEN 1 ELSE 0 END as IsFCD,
	            $isOptimisticColumnName AS IsOptimistic,
	            ( '<QuotaData xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""><FieldNames>' +
	            (SELECT qf.fieldname as [string] from quota_field qf WHERE qf.quotaid = q.quotaid FOR XML PATH('')) +
	            '</FieldNames></QuotaData>') as XmlData
	        from quotas q 
            where iscati > 0"

        $migratedQuotas = $Api.Databases.Cati.CopyDataToExistTable($surveyDb, 'BvSurveyQuota', $selectQuotaQuery );
	    
        $Api.Databases.Cati.ExecuteNonQuery("UPDATE BvSurvey SET IsQuotaInCatiDb = CASE WHEN $migratedQuotas = 0 THEN 0 ELSE 1 END WHERE SID = $surveyId")

        Write-Host "   Survey migration was completed successful. Total migrated quotas = $migratedQuotas"
        
    }
    catch {
        Write-Host ('   Error: ' + $_.Exception.Message)
    }
    
}

Write-Host 'Update complete.'