IF NOT EXISTS (SELECT NULL FROM msdb..sysjobs WHERE name = 'Set compatibility level for databases')
BEGIN
	EXEC msdb..sp_add_job @job_name = 'Set compatibility level for databases', @enabled = '1', @description = 'Set compatibility level for databases', @start_step_id = '1', @notify_level_eventlog = '2', @delete_level = '0'
	
	/***Steps***/
	EXEC msdb..sp_add_jobstep @job_name = 'Set compatibility level for databases', @step_id = '1', @step_name = 'Set compatibility level for databases', @subsystem = 'TSQL', @command = 'exec usp_SetCompLevelForConfirmitDatabases', @flags = '4', @cmdexec_success_code = '0',  @on_success_action = '1', @on_success_step_id = '0', @on_fail_action = '2', @on_fail_step_id = '0', @database_name = 'master', @retry_attempts = '0', @retry_interval = '0'
	
	/***Schedule***/
	EXEC msdb..sp_add_jobschedule @job_name = 'Set compatibility level for databases', @name = 'Set compatibility level for databases', @enabled = '1', @freq_type = '4', @freq_interval = '1', @freq_subday_type=8, @freq_subday_interval=12, @active_start_time=0, @active_end_time=235959 
	
	/***Apply***/
	EXEC msdb..sp_apply_job_to_targets @job_name = 'Set compatibility level for databases',  @target_servers = '(local)', @operation = 'APPLY'
END