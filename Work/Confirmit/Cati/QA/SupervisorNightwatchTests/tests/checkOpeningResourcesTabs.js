module.exports = {
    tags: ['checkOpeningResourcesTabs'],

    'Check opening tabs in resources': function (browser) {
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

    'open resources': function openResources(browser) {
        var supervisor = browser.page.supervisor();
        browser.frame(null)
        supervisor.click('@resourcesTab')
            .waitForElementVisible('@frame', 10000)
        browser.frame('listFrame')
        supervisor.waitForElementVisible('@resourcesIdColumn', 10000)
    },

    'open all tabs in resources': function openAllTabsResources(browser) {
        var resources = browser.page.resources();
        browser.frame(null)
        browser
            .element('xpath', '//*[@class="mainMenuItemInner"][contains(text(),"Call Groups")]', function (visible) {
                if (visible.status !== -1) {
                    console.log('You have call groups tab');
                    resources.click('@callGroups')
                        .waitForElementVisible('@frame', 100000)
                    browser.frame('listFrame')
                    resources.waitForElementVisible('@callGroupsIdColumn', 10000)
                } else {
                    console.log("You don't have call groups tab")
                }
            })

        browser.frame(null)
        resources.click('@breakTab')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        resources.waitForElementVisible('@breakTabIdColumnName', 10000)

        browser.frame(null)
        resources.click('@dialerTab')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        resources.waitForElementVisible('@dialerTabIdColumnName', 10000)

        browser.frame(null)
        browser
            .element('xpath', '//*[@class="mainMenuItemInner"][contains(text(),"Inbound Settings")]', function (visible) {
                if (visible.status !== -1) {
                    console.log('You have Inbound Settings tab');
                    resources.click('@inboundSettingsTab')
                        .waitForElementVisible('@frame', 100000)
                    browser.frame('listFrame')
                    resources.waitForElementVisible('@inboundSettingatelephoneNumberColumnName', 10000)
                } else {
                    console.log("You don't have Inbound Settings tab")
                }
            })

        browser.frame(null)
        browser
            .element('xpath', '//*[@class="mainMenuItemInner"][contains(text(),"IVR Settings")]', function (visible) {
                if (visible.status !== -1) {
                    console.log('You have IVR Settings tab');
                    resources.click('@ivrSettingsTab')
                        .waitForElementVisible('@frame', 100000)
                    browser.frame('listFrame')
                    resources.waitForElementVisible('@ivrSettingatelephoneNumberColumnName', 10000)
                } else {
                    console.log("You don't have IVR Settings tab")
                }
            })

        browser.frame(null)
        resources.click('@telephoneBlackListTab')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        resources.waitForElementVisible('@telephoneBlackListTabTelNumberColumnName', 10000)

        browser.frame(null)
        browser
            .element('xpath', '//*[@class="mainMenuItemInner"][contains(text(),"DDI Numbers")]', function (visible) {
                if (visible.status !== -1) {
                    console.log('You have DDI Numbers tab');
                    resources.click('@ddiNumbersTab')
                        .waitForElementVisible('@frame', 100000)
                    browser.frame('listFrame')
                    resources.waitForElementVisible('@ddiNumbersTabTelNumberColumnName', 10000)
                } else {
                    console.log("You don't have DDI Numbers tab")
                }
            })

        browser.frame(null)
        resources.click('@masterTimezoneListTab')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        resources.waitForElementVisible('@masterTimezoneIdColumnName', 10000)

        browser.frame(null)
        resources.click('@activeTimezoneListTab')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        resources.waitForElementVisible('@activeTimezoneNameColumnName', 10000)

        browser.frame(null)
        resources.click('@tasksTab')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        resources.waitForElementVisible('@tasksTabIdColumnName', 10000)

        browser.frame(null)
        resources.click('@aboutCatiSupervisorTab')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        resources.waitForElementVisible('@aboutCatiSupervisorTabTitleName', 10000)

        browser.end();
    }
};