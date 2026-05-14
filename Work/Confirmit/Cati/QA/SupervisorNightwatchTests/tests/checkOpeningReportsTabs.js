module.exports = {
    'Check open supervisor and name tabs': function (browser) {
        browser
            .login({ name: browser.globals.login.user_name, password: browser.globals.login.password });
        browser
            .url(browser.launchUrl)

        var supervisor = browser.page.supervisor();
        supervisor
            .navigate()
            .waitForElementVisible('@surveyTab', 10000)
            .checkNameTabs()
        browser.frame('listFrame')
        supervisor.waitForElementVisible('@surveyIdName', 10000)
    },

    'open reports': function openReports(browser) {
        var supervisor = browser.page.supervisor();
        browser.frame(null)
        supervisor.click('@reportsTab')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        supervisor.waitForElementVisible('@reportsNameColumn', 10000)
    },

    'open all tabs in reports': function openAllTabsReports(browser) {
        var reports = browser.page.reports();
        browser.frame(null)
        reports.click('@surveyOverviewTab')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        reports.waitForElementVisible('@userFilterSelectButton', 10000)

        browser.frame(null)
        reports.click('@surveyProductivityTab')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        reports.waitForElementVisible('@selectStatusButton', 10000)

        browser.frame(null)
        reports.click('@iterviewerProductivity')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        reports.waitForElementVisible('@selectSurveyButton', 10000)

        browser.frame(null)
        reports.click('@iterviewerSessions')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        reports.waitForElementVisible('@interviewerColumnName', 10000)

        browser.frame(null)
        reports.click('@quotaProgress')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        reports.waitForElementVisible('@buildReportButton', 10000)

        browser.frame(null)
        reports.click('@sampleStatusSummary')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        reports.waitForElementVisible('@hideZepoStatusesCheckBox', 10000)

        browser.frame(null)
        reports.click('@sampleStatusSummaryByQuestion')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        reports.waitForElementVisible('@statusFilterCheckBox', 10000)

        browser.frame(null)
        reports.click('@callAttempts')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        reports.waitForElementVisible('@surveyIdColumnName', 10000)

        browser.frame(null)
        reports.click('@attemptsByDisposition')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        reports.waitForElementVisible('@dataRangeSelectField', 10000)

        browser.frame(null)
        reports.click('@numberOfAttempts')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        reports.waitForElementVisible('@buildReportButtonInNumbers', 10000)

        browser.frame(null)
        reports.click('@sampleUtilization')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        reports.waitForElementVisible('@statusText', 10000)

        browser.frame(null)
        reports.click('@interviewerSubmissionDetails')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        reports.waitForElementVisible('@surveyIdColumnNameForInterviewerSubmissionDetails', 10000)

        browser.frame(null)
        reports.click('@aggregatedInterviewerSubmission')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        reports.waitForElementVisible('@interviewerColumnNameForAgregatedTab', 10000)
        browser.end();
    }
};