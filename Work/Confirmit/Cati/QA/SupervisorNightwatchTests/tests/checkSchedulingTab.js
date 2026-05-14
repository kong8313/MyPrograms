module.exports = {
    'Check scheduling tab': function (browser) {
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
    'open sheduling tab': function openShedulingTab(browser) {
        var supervisor = browser.page.supervisor();
        browser.frame(null)
        supervisor.click('@schedulingTab')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        supervisor.waitForElementVisible('@schedulingNameColumn', 10000)
    },
    'select first sheduling and open properties': function selectFirstScheduling(browser) {
        var scheduling = browser.page.scheduling();
        scheduling.waitForElementVisible('@firstRowScheduling', 100000)
            .click('@firstRowScheduling')
            .waitForElementVisible('@viewButton', 100000)
            .click('@viewButton')
    },
    'check rules tab': function checkRulesTab(browser) {
        var scheduling = browser.page.scheduling();
        browser.frame('infoFrame')
        browser.waitForElementPresent('.ig_AjaxIndicator', 10000)
        browser.frame(0)
        scheduling.waitForElementVisible('@rulesColumnName', 10000)
    },
    'check shifts tab': function checkShiftsTab(browser) {
        var scheduling = browser.page.scheduling();
        browser.frame(null)
        browser.frame('listFrame').frame('infoFrame')
        scheduling.waitForElementVisible('@shiftsTab', 10000)
            .click('@shiftsTab')
        browser.frame(1)
        scheduling.waitForElementVisible('@displayCheckBoxInShifts', 10000)
    },
    'check shift type tab': function checkSchiftTypeTab(browser) {
        var scheduling = browser.page.scheduling();
        browser.frame(null)
        browser.frame('listFrame').frame('infoFrame')
        scheduling.waitForElementVisible('@shiftTypeTab', 10000)
            .click('@shiftTypeTab')
        browser.frame(2)
        scheduling.waitForElementVisible('@shiftTypeNameColumn', 10000)
    },
    'check parameters tab': function checkParametersTab(browser) {
        var scheduling = browser.page.scheduling();
        browser.frame(null)
        browser.frame('listFrame').frame('infoFrame')
        scheduling.waitForElementVisible('@parametersTab', 10000)
            .click('@parametersTab')
        browser.frame(3)
        scheduling.waitForElementVisible('@idColumnInParametersTab', 10000)
    },
    'check custom script tab': function checkCustomScriptTab(browser) {
        var scheduling = browser.page.scheduling();
        browser.frame(null)
        browser.frame('listFrame').frame('infoFrame')
        scheduling.waitForElementVisible('@customScriptTab', 10000)
            .click('@customScriptTab')
        browser.frame(4)
        scheduling.waitForElementVisible('@saveButtonInParametersTab', 10000)
        browser.end()
    }
}