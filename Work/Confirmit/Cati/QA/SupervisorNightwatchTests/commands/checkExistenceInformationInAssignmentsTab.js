var Command = function () {
};

Command.prototype.command = function (callback) {
    this.api.execute(function () {
        var element = document.querySelector('#Content_Assignment_m_grid_dataGrid > table > tbody > tr:nth-child(1) > td:nth-child(1) > table > tbody:nth-child(2) > tr > td > div:nth-child(2) > table > tbody > tr > td > table > tbody > tr > td > div')
        return element != null;
    }, function (result) {
        callback(result.value)
    });

    return this;
};

module.exports = Command;
