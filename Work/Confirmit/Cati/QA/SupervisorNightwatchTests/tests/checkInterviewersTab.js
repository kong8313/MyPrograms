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
    },
    'open interviewers': function openInterviewers(browser) {
        var supervisor = browser.page.supervisor();
        browser.frame(null)
        supervisor.click('@interviewersTab')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        supervisor.waitForElementVisible('@interviewersIdColumn', 10000)
    },
    'sort interviewers by id': function sortInterviewers(browser) {
        var interviewers = browser.page.interviewers();
        browser.frame('listFrame')
        interviewers.waitForElementVisible('@idColumnName', 100000)
            .click('@idColumnName')
            .waitForElementVisible('@idColumnName', 100000)
            browser.pause(2000)
    },
    'select first interviewers and open properties': function selectFirstInterviewers(browser) {
        var interviewers = browser.page.interviewers();
        browser.frame('listFrame')
        interviewers.waitForElementVisible('@firstRowInterviewersCheckbox', 100000)
            .click('@firstRowInterviewersCheckbox')
            .waitForElementVisible('@propertiesButton', 100000)
            .click('@propertiesButton')
    },

    'check properties tab': function checkPropertiesTab(browser) {
        var interviewers = browser.page.interviewers();
        browser.frame('infoFrame').frame(0)
        interviewers.waitForElementVisible('@idRowInProperties', 10000)
    },
    'check membership tab': function checkMembershipTab(browser) {
        var interviewers = browser.page.interviewers();
        browser.frame(null)
        browser.frame('listFrame').frame('infoFrame')
        interviewers.waitForElementVisible('@membershipTab', 10000)
            .click('@membershipTab')
        browser.frame(1)
        interviewers.waitForElementVisible('@memberOfNameColumns', 10000)
    },
    'open assignments': function openAssignments(browser) {
        var interviewers = browser.page.interviewers();
        browser.frame(null)
        browser.frame('listFrame').frame('infoFrame')
        interviewers.waitForElementVisible('@assignmentsTab', 10000)
            .click('@assignmentsTab')
        browser.frame(2)
        interviewers.waitForElementVisible('@surveyIdColumnNameInAssignments', 10000)
        browser.end()
    }
}