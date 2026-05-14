module.exports.command = function(callback) {
    this
        .url(this.globals.login.url)
        .deleteCookies()
        .refresh()

    if (callback) callback();
    return this;
};
