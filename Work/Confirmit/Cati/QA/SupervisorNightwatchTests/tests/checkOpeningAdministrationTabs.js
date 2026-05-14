module.exports = {
    tags: ['checkOpeningAdministrationTabs'],

    'Check opening tabs in administration': function (browser) {
        browser
            .login({name: browser.globals.login.user_name, password: browser.globals.login.password});
        browser
            .url(browser.launchUrl)

        var supervisor = browser.page.supervisor();
        supervisor
            .navigate()
            .waitForElementVisible('@surveyTab', 10000)
        browser.frame('listFrame')
        supervisor.waitForElementVisible('@surveyIdName', 10000)
    },

    'open administration tab': function openAdministrationTab(browser) {
        var supervisor = browser.page.supervisor();
        browser.frame(null)
        supervisor.click('@administrationTab')
            .waitForElementVisible('@frame', 10000)
        browser.frame('listFrame')
        supervisor.waitForElementVisible('@settingsTabGeneralName', 10000)
    },

    'open all tabs in administration': function openAllTabsResources(browser) {
        var administrationTab = browser.page.administration();
        administrationTab.waitForElementVisible('@settingsTabGeneralName', 10000)

        browser.frame(null)
        browser
            .element('xpath', '//*[@class="mainMenuItemInner"][contains(text(),"System Settings")]', function (visible) {
                if (visible.status !== -1) {
                    console.log('You have System Settings tab');
                    administrationTab.click('@systemSettingsTab')
                        .waitForElementVisible('@frame', 100000)
                    browser.frame('listFrame')
                    administrationTab.waitForElementVisible('@systemSettingsFirstColumnName', 10000)
                } else {
                    console.log("You don't have System Settings tab")
                }
            })

        browser.frame(null)
        administrationTab.click('@databaseTab')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        administrationTab.waitForElementVisible('@databaseTabScriptVersionColumnName', 10000)

        browser.frame(null)
        administrationTab.click('@managementTab')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        administrationTab.waitForElementVisible('@managementTabFlushLogButton', 10000)

        browser.end();
    }
};