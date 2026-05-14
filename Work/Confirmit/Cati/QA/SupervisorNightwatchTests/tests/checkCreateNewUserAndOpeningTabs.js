const userData = {name: 'e2e_supervisor', password: '123456'};
const userCompanyId = '1';
const permissionTypes = require('../services/permissionTypes');

module.exports = {
    'Check open news and close': function(browser) {
        browser
            .login({name: browser.globals.login.user_name, password: browser.globals.login.password});
        browser
            .url(browser.launchUrl)
        browser
            .pause(5000)
            .checkNews((res) => {
                if (res) {
                    browser
                        .waitForElementVisible('div[class="overlayWindow c_wrapper"]', 10000)
                        .click('div[class="overlayWindow c_wrapper"] img[alt="Close"]')
                }
            })
    },

    'Check open supervisor and name tabs': function(browser) {
        var supervisor = browser.page.supervisor();
        supervisor
            .navigate()
            .checkNews((res) => {
                if (res) {
                    browser
                        .waitForElementVisible('div[class="overlayWindow c_wrapper"]', 10000)
                        .click('div[class="overlayWindow c_wrapper"] img[alt="Close"]')
                }
            })
            .waitForElementVisible('@surveyTab', 100000)
            .checkNameTabs()
        browser.frame('listFrame')
        supervisor.waitForElementVisible('@surveyIdName', 100000)
    },

    'open interviewers': function openInterviewers(browser) {
        var supervisor = browser.page.supervisor();
        browser.frame(null)
        supervisor.click('@interviewersTab')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        supervisor.waitForElementVisible('@interviewersIdColumn', 10000)
    },

    'open scheduling': function openScheduling(browser) {
        var supervisor = browser.page.supervisor();
        browser.frame(null)
        supervisor.click('@schedulingTab')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        supervisor.waitForElementVisible('@schedulingNameColumn', 10000)
    },

    'open reports': function openReports(browser) {
        var supervisor = browser.page.supervisor();
        browser.frame(null)
        supervisor.click('@reportsTab')
            .waitForElementVisible('@frame', 100000)
        browser.frame('listFrame')
        supervisor.waitForElementVisible('@reportsNameColumn', 10000)
    },

    'open activity view': function openActivityView(browser) {
        var supervisor = browser.page.supervisor();
        browser.frame(null)
        supervisor.click('@activityViewTab')
            .waitForElementVisible('@frame', 10000)
        browser.frame('listFrame')
        supervisor.waitForElementVisible('@activityViewNameColimn', 10000)
    },

    'open recorded': function openRecorded(browser) {
        var supervisor = browser.page.supervisor();
        browser.frame(null)
        supervisor.click('@recordedTab')
            .waitForElementVisible('@frame', 10000)
        browser.frame('listFrame')
        supervisor.waitForElementVisible('@recordedSurveyIdColumn', 10000)
    },

    'open call centers': function openCallCenters(browser) {
        var supervisor = browser.page.supervisor();

        browser.frame(null)
        supervisor.click('@callCentersTab')
            .waitForElementVisible('@frame', 10000)
        browser.frame('listFrame')
        supervisor.waitForElementVisible('@callCentersIdColumn', 10000)
    },

    'open resources': function openResources(browser) {
        var supervisor = browser.page.supervisor();
        browser.frame(null)
        supervisor.click('@resourcesTab')
            .waitForElementVisible('@frame', 10000)
        browser.frame('listFrame')
        supervisor.waitForElementVisible('@resourcesIdColumn', 10000)
    },

    'open provide feedback': function openProvideFeedback(browser) {
        var supervisor = browser.page.supervisor();
        browser.frame(null)
        supervisor.click('@provideFeedbackTab')
        browser.pause(1000)
            .windowHandles(function(result) {
                var handle = result.value[1];
                browser.switchWindow(handle);
            });
        browser.waitForElementVisible('#Content_FeedbackHeader', 10000)
            .end()
    }
};


