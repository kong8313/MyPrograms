var Q = require("q");
var ServiceBase = require("./service-base");

class PermissionUserService extends ServiceBase {
    constructor(apiUrl, accessToken, userId) {
        super(accessToken);
        this.url = apiUrl + '/users/' + userId + '/permissions';
        this.accessToken = accessToken;
    }

    addUserPermission(permission) {
        var options = {
            url: this.url,
            body: JSON.stringify(permission),
            headers: {
                "content-type": "application/json"
            },
            method: "POST"
        };

        return this.promise(options);
    }

    addUserPermissions(permissions) {
        var self = this;
        return Q.all(permissions.map(function(permission) {
            return self.addUserPermission(permission)
        }));
    }
}

module.exports = PermissionUserService;