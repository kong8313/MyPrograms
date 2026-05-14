const util = require('util');
const request = require("request");
const events = require('events')

const AccessTokenService = require('../services/get-access-token-service');
const CreateUserService = require('../services/create-user-service');
const PermissionUserService = require('../services/permission-user-service');

function Command() {
    events.EventEmitter.call(this);
}

util.inherits(Command, events.EventEmitter)

Command.prototype.command = function (userName, userPass, companyId, permissions) {
    var self = this;
    const userCreateData = require('../lib/userData')(userName, userPass, companyId);
    const adminUserName = this.api.globals.login.user_name;
    const adminPassword = this.api.globals.login.password;
    const apiUrl = this.api.globals.apiWebService;
    var accessTokenService = new AccessTokenService(apiUrl);

    accessTokenService.getAccessToken(adminUserName, adminPassword).then(access_token => {
        new CreateUserService(apiUrl, access_token)
            .createUser(userCreateData)
            .then((result) => {
                if (result.isNew && permissions) {
                    new PermissionUserService(apiUrl, access_token, result.id).addUserPermissions(permissions).done(function (values) {
                        self.emit('complete');
                    })
                }
                else
                    self.emit('complete');
            })
            .catch(err => {
                console.log(`Error occured during createUser:` + err)
            });
    });

    return this;
}

module.exports = Command;