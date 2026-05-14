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

        browser.frame('listFrame')
        supervisor.waitForElementVisible('@surveyIdName', 10000)

        var surveys = browser.page.surveys();
        browser.frame('listFrame')
        surveys.waitForElementVisible('@firstRowSurvey', 10000)
            .moveToElement('@firstRowSurvey', 10, 10)
            .click('@firstRowSurvey')
            .click('@viewButton')
    },
    'open general': function openGeneral(browser) {
        var surveys = browser.page.surveys();
        browser.frame('infoFrame').frame(0)
        surveys.waitForElementVisible('@surveyIdInGeneralName', 10000)
    },
    'open general': function openGeneral(browser) {
        var surveys = browser.page.surveys();
        browser.frame(null)
        browser.frame('listFrame').frame('infoFrame')
        surveys.waitForElementVisible('@summaryTab', 10000)
        surveys.click('@summaryTab')
        browser.frame(1)
        surveys.waitForElementVisible('@idColumnNameInSummary', 10000)
    },
    'open assignments': function openAssignments(browser) {
        var surveys = browser.page.surveys();
        browser.frame(null)
        browser.frame('listFrame').frame('infoFrame')
        surveys.waitForElementVisible('@assignmentsTab', 10000)
        surveys.click('@assignmentsTab')
        browser.frame(2)
        surveys.waitForElementVisible('@assignmentsIdColumnName', 10000)
    },
    'open quotas': function openQuotas(browser) {
        var surveys = browser.page.surveys();
        browser.frame(null)
        browser.frame('listFrame').frame('infoFrame')
        browser.isVisible('xpath', '//*[@id="Content_dialog_tabs"]/span/span/span[span="Quotas"]', function (visible) {
            if (visible.value == true) {
                console.log("You have quotas tab");
                browser.frame(null)
                browser.frame('listFrame').frame('infoFrame')
                surveys.waitForElementVisible('@quotasTab', 10000)
                surveys.click('@quotasTab')
                browser.frame(3)
                surveys.waitForElementVisible('@quotasFirstColumnName', 10000)
            } else {
                console.log("You don't have quotas tab")
            }
        })
    },
    'open interviewer search tab': function openInterviewerSearch(browser) {
        var surveys = browser.page.surveys();
        browser.frame(null)
        browser.frame('listFrame').frame('infoFrame')
        surveys.waitForElementVisible('@interviewerSearchTab', 10000)
        surveys.click('@interviewerSearchTab')
        browser.frame(4)
        surveys.waitForElementVisible('@interviewerSearchNameColumn', 10000)
    },
    'open sheduling tab': function openShedulingTab(browser) {
        var surveys = browser.page.surveys();
        browser.frame(null)
        browser.frame('listFrame').frame('infoFrame')
        surveys.waitForElementVisible('@shedulingTab', 10000)
        surveys.click('@shedulingTab')
        browser.frame(5)
        surveys.waitForElementVisible('@shedulingIdColumnName', 10000)
    },
    'open filters': function openFilters(browser) {
        var surveys = browser.page.surveys();
        browser.frame(null)
        browser.frame('listFrame').frame('infoFrame')
        surveys.waitForElementVisible('@filtersTab', 10000)
        surveys.click('@filtersTab')
        browser.frame(6)
        surveys.waitForElementVisible('@filtersIdColumnName', 10000)
    },
    'open dialer': function openDialer(browser) {
        var surveys = browser.page.surveys();
        browser.frame(null)
        browser.frame('listFrame').frame('infoFrame')
        surveys.waitForElementVisible('@dialerTab', 10000)
        surveys.click('@dialerTab')
        browser.frame(7)
        surveys.waitForElementVisible('@dialerAbandonColumnName', 10000)
        browser.end()
    }
}