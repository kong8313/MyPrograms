const { exec } = require('child_process');

module.exports = {
    abortOnAssertionFailure: true,

    waitForConditionPollInterval: 300,

    waitForConditionTimeout: 5000,

    throwOnMultipleElementsReturned: false,

    asyncHookTimeout: 10000,

    before: function (done) {
        process.on('unhandledRejection', error => {
            throw error;
        });
        done();
    },

    afterEach: function (browser, done) {

        browser.end(function () {
            done()
        })
    },

    after: function (done) {
        child = exec("taskkill /F /IM chromedriver.exe /T", function (error, stdout, stderr) {
            console.info(stdout);
            console.error(stderr);
            if (error !== null) {
              console.log('exec error: ' + error);
            }
            done();
          });
    }
};