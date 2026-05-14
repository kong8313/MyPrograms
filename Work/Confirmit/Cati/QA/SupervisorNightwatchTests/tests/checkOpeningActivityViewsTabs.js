module.exports = {
    'Check opening tabs in activity views': function (browser) {
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

    'open activity view': function openActivityView(browser) {
        var supervisor = browser.page.supervisor();
        browser.frame(null)
        supervisor.click('@activityViewTab')
            .waitForElementVisible('@frame', 10000)
        browser.frame('listFrame')
        supervisor.waitForElementVisible('@activityViewNameColimn', 10000)
    },

    'open tabs in activity view': function openActivityViewTabs(browser) {
        var activityView = browser.page.activityView();
        browser.frame(null)
        activityView.click('@surveyListTab')
        browser.pause(1000)
            .windowHandles(function (result) {
                var handle = result.value[1];
                browser.switchWindow(handle)
                    .waitForElementVisible('.activityViewHeader', 10000)
                    .closeWindow()
            });

        browser.pause(1000)
            .windowHandles(function (result) {
                var handle = result.value[0];
                browser.switchWindow(handle)
                    .waitForElementVisible('#headerContainer', 10000)
            });

        browser.frame(null)
        activityView
            .click('@interviewerListTab')
        browser.pause(1000)
            .windowHandles(function (result) {
                var handle = result.value[1];
                browser.switchWindow(handle)
                    .waitForElementVisible('.activityViewHeader', 10000)
                    .closeWindow()
            });

        browser.pause(1000)
            .windowHandles(function (result) {
                var handle = result.value[0];
                browser.switchWindow(handle)
                    .waitForElementVisible('#headerContainer', 10000)
            });

        browser.frame(null)
        activityView
            .click('@appointmentListTab')
        browser.pause(1000)
            .windowHandles(function (result) {
                var handle = result.value[1];
                browser.switchWindow(handle)
                    .waitForElementVisible('.activityViewHeader', 10000)
                    .closeWindow()
            });

        browser.pause(1000)
            .windowHandles(function (result) {
                var handle = result.value[0];
                browser.switchWindow(handle)
                    .waitForElementVisible('#headerContainer', 10000)
            });

        browser.frame(null)
        activityView
            .click('@perfomanceListTab')
        browser.pause(1000)
            .windowHandles(function (result) {
                var handle = result.value[1];
                browser.switchWindow(handle)
                    .waitForElementVisible('.activityViewHeader', 10000)
                    .closeWindow()
            });
        browser.end();
    }
};