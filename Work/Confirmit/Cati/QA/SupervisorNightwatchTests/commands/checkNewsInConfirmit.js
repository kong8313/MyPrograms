var Command = function () {
};

Command.prototype.command = function (callback) {
    this.api.element('css selector', '#MarkAsRead', (result) => {
        if (result.status != -1){
            this.api.url()
                .click('label[for=MarkAsRead]')
                .click('input[name=loginButton]');
            return;
        }
    });

    return this;
};

module.exports = Command;
