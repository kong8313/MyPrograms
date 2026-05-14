var events = require("events");
var util = require("util");

function Command() {
    events.EventEmitter.call(this);
}

util.inherits(Command, events.EventEmitter);

Command.prototype.command = function (userData, callback) {
    var url = this.api.globals.login.url;

    this.api.url(url)
        .waitForElementVisible('body', 30000);

    this.api.url(result => {
        if (result.value.includes("/confirm/authoring/") || result.value.includes("/supervisor/")){
            if (callback) {
                callback();
            }
            this.emit("complete");
            return this;
        }

        const useIdentity = result.value.includes("identity/login");
        const usernameSelector = useIdentity ? "#username" : "#__l_username";
        const passwordSelector = useIdentity ? "#password" : "#__l_password";
        const buttonSelector = useIdentity ? "#btnlogin" : "#__l_LoginButton";

        this.api
            .waitForElementVisible(buttonSelector, 10000)
            .setValue(usernameSelector, userData.name)
            .setValue(passwordSelector, userData.password)
            .click(buttonSelector)
            .waitForElementNotPresent(buttonSelector, 10000, false, () => {
                if (callback) {
                    callback();
                }
                this.emit("complete");
            });
    });

    return this;
};

module.exports = Command;
