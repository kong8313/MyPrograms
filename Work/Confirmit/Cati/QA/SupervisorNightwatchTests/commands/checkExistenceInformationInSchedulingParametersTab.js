var Command = function () {
};

Command.prototype.command = function (callback) {
    this.api.execute(function () {
        var element = document.querySelector('#Content_SchedulingParams_m_grid_dataGrid table[role="grid"] div')
        return element != null;
    }, function (result) {
        callback(result.value)
    });

    return this;
};

module.exports = Command;
