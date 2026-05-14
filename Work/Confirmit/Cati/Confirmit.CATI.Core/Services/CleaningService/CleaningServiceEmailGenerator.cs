using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Confirmit.CATI.Core.Misc;
using Confirmit.CATI.Core.Services.Interfaces;
using Confirmit.CATI.Core.SystemSettings;

namespace Confirmit.CATI.Core.Services.CleaningService
{
    public class CleaningServiceEmailGenerator : ICleaningServiceEmailGenerator
    {
        public const string CleanupSubject = "Forsta CATI Clean-up Notification";

        public const string WarningSubject = "Forsta CATI Clean-up Notification Warning";

        private const string BodyTemplateOfCleanup = @"

<body style='font-family: ""Segoe UI"",sans-serif;font-size:10pt'>

    <div style='border:none;border-bottom:solid #FE7813 2.25pt;padding:0cm 0cm 1.0pt 0cm'>
        <b>Forsta CATI Clean-up Notification Warning for Company: {CompanyName}</b><br>
    </div>

    <br>

    <div>
        All CATI calls (call history information and interviewer assignments) for the CATI survey(s) listed below have now been deleted.
    </div>

    <br>

{SurveyList}

    <br>
    <br>
    
    <div>
        Copyright© {CurrentYear} Forsta AS. All Rights Reserved. <a href=""https://www.forsta.com/legal/privacy-notice/"">Forsta Privacy Notice.</a>
    </div>
    <div style='font-size:10px;margin-top:3.75pt'>
        Please do not reply to this message, as it has been sent from an unmonitored e-mail address. 
    </div>
</body>
";

        private const string BodyTemplateOfNotificationWarning = @"

<body style='font-family: ""Segoe UI"",sans-serif;font-size:10pt'>

    <div style='border:none;border-bottom:solid #FE7813 2.25pt;padding:0cm 0cm 1.0pt 0cm'>
        <b>Forsta CATI Clean-up Notification Warning for Company: {CompanyName}</b><br>
    </div>

    <br>

    <div>
        The CATI system has identified that the surveys listed below have had no activity for a substantial amount of time and so will be cleaned up in {Days} days.<br>
        <br>
        The survey(s) and the collected interview data will not be deleted but the automatic clean-up procedure will apply the following actions:<br>
        <ul>
            <li>Calls will no longer be listed in the ‘Scheduled’ state in Call Management, instead they will be listed as ‘Not Scheduled’</li>
            <li>Call properties such as appointments and interviewer assignments for the survey will be cancelled</li>
        </ul>
        These actions are irreversible, however calls can be re-created and assignments may be re-added.<br>
        <br>
        A survey is considered inactive, and an automatic survey clean-up is executed when all of the following conditions are met:<br>
        <ul>
            <li>The specified *inactivity time period has elapsed for the survey</li>
            <li>The following activities WERE NOT performed for the survey during the specified period of inactivity:
                <ul>
                    <li>Opening/closing the survey</li>
                    <li>Loading sample contacts for the survey</li>
                    <li>Invoking the Call Management dialog for the survey</li>
                    <li>Starting an interview for the survey</li>
                </ul>
            </li>    
        </ul>
        *The inactivity period is set to {InactivityPeriod} days and is specified by the system administrator.
    </div>

    <br>

{SurveyList}

    <br>
    <br>
    
    <div>
        Copyright© {CurrentYear} Forsta AS. All Rights Reserved. <a href=""https://www.forsta.com/legal/privacy-notice/"">Forsta Privacy Notice.</a>
    </div>
    <div style='font-size:10px;margin-top:3.75pt'>
        Please do not reply to this message, as it has been sent from an unmonitored e-mail address. 
    </div>
</body>
";

        private readonly ISystemSettings _settings;
        private readonly ICompanyInfo _companyInfo;
        private readonly ITimezoneService _timezoneService;

        public CleaningServiceEmailGenerator(ISystemSettings settings, ICompanyInfo companyInfo, ITimezoneService timezoneService)
        {
            _settings = settings;
            _companyInfo = companyInfo;
            _timezoneService = timezoneService;
        }

        public string GetWarningBody(List<CleaningServiceEmailInfo> surveys)
        {
            var surveyList = GenerateHtmlTableWithSurveys(surveys);

            return BodyTemplateOfNotificationWarning
                .Replace("{CompanyName}", _companyInfo.CompanyName)
                .Replace("{InactivityPeriod}", ((int)_settings.RoutineMaintenance.Actions.SurveyCleanup.NotificationTimeout.TotalDays).ToString())
                .Replace("{Days}", ((int)_settings.RoutineMaintenance.Actions.SurveyCleanup.CleanupTimeout.TotalDays).ToString())
                .Replace("{SurveyList}", surveyList)
                .Replace("{CurrentYear}", DateTime.Now.Year.ToString());
        }

        public string GetCleanupBody(List<CleaningServiceEmailInfo> surveys)
        {
            var surveyList = GenerateHtmlTableWithSurveys(surveys);

            return BodyTemplateOfCleanup
                .Replace("{CompanyName}", _companyInfo.CompanyName)
                .Replace("{SurveyList}", surveyList)
                .Replace("{CurrentYear}", DateTime.Now.Year.ToString());
        }

        private string GenerateHtmlTableWithSurveys(List<CleaningServiceEmailInfo> surveys)
        {
            var res = new StringBuilder();

            res.AppendLine(@"
    <div style='font-weight:bold; text-align:left; margin-bottom: 3.75pt'>
        Survey list
    </div>
    
    <table cellpadding='0' cellspacing='0' border='0' style='font-family:""Segoe UI"",sans-serif; font-size:10pt; width:100%; border-collapse:collapse'>
        <tr style='background:#F2F2F2'>
            <th width='15%' align='left' style='border:solid #CCCCCC 1.0pt; padding:3.75pt 3.75pt 3.75pt 3.75pt'>
                Survey ID
            </th>
            <th width='40%' align='left' style='border:solid #CCCCCC 1.0pt; padding:3.75pt 3.75pt 3.75pt 3.75pt'>
                Name
            </th>
            <th width='15%' align='left' style='border:solid #CCCCCC 1.0pt; padding:3.75pt 3.75pt 3.75pt 3.75pt'>
                Sample size
            </th>
            <th width='15%' align='left' style='border:solid #CCCCCC 1.0pt; padding:3.75pt 3.75pt 3.75pt 3.75pt'>
                Creator
            </th>
            <th width='15%' align='left' style='border:solid #CCCCCC 1.0pt; padding:3.75pt 3.75pt 3.75pt 3.75pt'>
                Last activity
            </th>
        </tr>");

            string trStyle = string.Empty;
            var defaultTimezoneId = _timezoneService.GetDefaultCallCenterTimezoneId();
            foreach (var survey in surveys)
            {
                string lastActivity = survey.LastTouchTime.HasValue
                    ? _timezoneService.ConvertTimeFromUtc(defaultTimezoneId, survey.LastTouchTime.Value).ToString(CultureInfo.CurrentCulture)
                    : string.Empty;
                res.AppendLine($@"
        <tr {trStyle}>
            <td style='border:solid #CCCCCC 1.0pt; padding:3.75pt 3.75pt 3.75pt 3.75pt'>
                {survey.Name}
            </td>
            <td style='border:solid #CCCCCC 1.0pt; padding:3.75pt 3.75pt 3.75pt 3.75pt'>
                {survey.Description}
            </td>
            <td style='border:solid #CCCCCC 1.0pt; padding:3.75pt 3.75pt 3.75pt 3.75pt'>
                {survey.SampleSize}
            </td>
            <td style='border:solid #CCCCCC 1.0pt; padding:3.75pt 3.75pt 3.75pt 3.75pt'>
                {survey.Creator}
            </td>
            <td style='border:solid #CCCCCC 1.0pt; padding:3.75pt 3.75pt 3.75pt 3.75pt'>
                {lastActivity}
            </td>
        </tr>");

                trStyle = trStyle == string.Empty ? "style='background:#F2F2F2'" : string.Empty;
            }

            res.AppendLine("    </table>");

            return res.ToString();
        }
    }
}