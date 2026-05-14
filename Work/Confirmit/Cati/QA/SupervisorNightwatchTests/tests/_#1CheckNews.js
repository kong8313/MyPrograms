module.exports = {
    'Check open news': function (browser) {
        browser
            .login({ name: browser.globals.login.user_name, password: browser.globals.login.password })
            .checkNewsInConfirmit()
            .url(browser.launchUrl)
            .waitForElementVisible('body', 30000)
            .checkNews();

        browser.end();
    },
}