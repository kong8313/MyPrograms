var Command = function() {
};

Command.prototype.command = function(callback) {
    this.api.element('css selector', '.cati-menu-old-supervisor a', (result) => {
        if (result.status != -1) {
            this.api.click('.cati-menu-old-supervisor a');
        }
    });

    this.api.element('css selector', 'div[class="overlayWindow c_wrapper"]', (result) => {
        if (result.status != -1) {
            if (callback) {
                callback(true);
                return;
            }
            this.api.url().frame(0)
                .waitForElementVisible("#Content_dialog_cbMarkAllAsRead", 10000)
                .click('#Content_dialog_cbMarkAllAsRead')
                .click('#Content_dialog_btnOK')
                .waitForElementNotPresent("#Content_dialog_cbMarkAllAsRead", 10000)
        }
        else {
            if (callback) {
                callback(false);
                return;
            }
        }
    });

    return this;
};

module.exports = Command;
