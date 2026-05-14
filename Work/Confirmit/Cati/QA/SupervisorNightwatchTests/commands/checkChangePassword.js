var Command = function () {
};

Command.prototype.command = function (callback) {
    this.api.execute(function () {
        var element = document.querySelector('div.password-changed-page')
        return element != null;
    }, function (result) {
        callback(result.value)
    });

    return this;
};

module.exports = Command;