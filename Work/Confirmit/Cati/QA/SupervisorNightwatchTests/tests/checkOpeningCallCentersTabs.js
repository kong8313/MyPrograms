// module.exports = {
//     'Check opening tabs in call centers': function (browser) {
//         browser
//             .login({name: browser.globals.login.user_name, password: browser.globals.login.password});
//         browser
//             .url(browser.launchUrl)

//         var supervisor = browser.page.supervisor();
//         supervisor
//             .navigate()
//             .waitForElementVisible('@surveyTab', 10000)
//         browser.frame('listFrame')
//         supervisor.waitForElementVisible('@surveyIdName', 10000)
//     },

//     'open call centers': function openCallCenters(browser) {
//         var supervisor = browser.page.supervisor();
//         browser.frame(null)
//         supervisor.click('@callCentersTab')
//             .waitForElementVisible('@frame', 10000)
//         browser.frame('listFrame')
//         supervisor.waitForElementVisible('@callCentersIdColumn', 10000)
//     },

//     'open all tabs in call centers': function openAllTabsCallCenters(browser) {
//         var callCenters = browser.page.callCenters();
//         var supervisor = browser.page.supervisor();
//         browser.frame(null)
//         supervisor.click('@callCentersTab')
//             .waitForElementVisible('@frame', 10000)
//         callCenters.click('@surveysTab')
//             .waitForElementVisible('@frame', 100000)
//         browser.frame('listFrame')
//         callCenters.waitForElementVisible('@surveyIdColumnName', 10000)

//         browser.frame(null)
//         callCenters.click('@supervisorTab')
//             .waitForElementVisible('@frame', 100000)
//         browser.frame('listFrame')
//         callCenters.waitForElementVisible('@userIdColumnName', 10000)

//         browser.end();
//     }
// };