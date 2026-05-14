GO
PRINT N'Add performance metrcis system settings';

GO

DECLARE @DbName nvarchar(128) = (SELECT DB_NAME());

IF (@DbName = 'ConfirmitCATIV15' OR @DbName like 'ConfirmitCATIV15TEST%' )
BEGIN
  ;WITH data( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] ) AS
            (
			SELECT 'Console.Metrics.EnableCallAttemptsPerHourAboveAverageComparison', 'When enabled, Above average tag will be shown for interviewer', 'Interviewing', 'When enabled, the interviewer will be able to see the Above Average tag, indicating that they are making more call attempts per hour than the company average', 3, 0, 'False'
                UNION ALL
            SELECT 'Console.Metrics.EnableInterviewsCompletedPerHourAboveAverageComparison', 'When enabled, Above average tag will be shown for interviewer', 'Interviewing', 'When enabled, the interviewer will be able to see the Above Average tag, indicating that they are making more interview completes per hour than the company average', 3, 0, 'False'
                UNION ALL
            SELECT 'Console.Metrics.EnableCallAttemptsPerHourBelowAverageComparison', 'When enabled, Below average tag will be shown for interviewer', 'Interviewing', 'When enabled, the interviewer will be able to see the Below Average tag, indicating that they are making fewer call attempts per hour than the company average', 3, 0, 'False'
                UNION ALL
            SELECT 'Console.Metrics.EnableInterviewsCompletedPerHourBelowAverageComparison', 'When enabled, Below average tag will be shown for interviewer', 'Interviewing', 'When enabled, the interviewer will be able to see the Below Average tag, indicating that they are making less interview completes per hour than the company average', 3, 0, 'False'
                UNION ALL
            SELECT 'Console.Metrics.EnableCallAttemptsPerCompleteAboveAverageComparison', 'When enabled, Above average tag will be shown for interviewer', 'Interviewing', 'When enabled, the interviewer will be able to see the Above Average tag, indicating that they are making more call attempts per completed interview than the company average', 3, 0, 'False'
                UNION ALL
            SELECT 'Console.Metrics.EnableCallAttemptsPerCompleteBelowAverageComparison', 'When enabled, Below average tag will be shown for interviewer', 'Interviewing', 'When enabled, the interviewer will be able to see the Below Average tag, indicating that they are making fewer call attempts per completed interview than the company average', 3, 0, 'False'
                UNION ALL
            SELECT 'Console.Metrics.EnableTotalInterviewingTime', 'When enabled, the interviewer will be able to see the total interviewing time in the performance metrics', 'Interviewing', 'When enabled, the interviewer will be able to see the total interviewing time in the performance metrics', 3, 0, 'True'
            )
   INSERT INTO BvSystemSettings( [SystemName], [DisplayName], [Group], [Description], [Type], [Hidden], [Value] )
SELECT * FROM Data

END