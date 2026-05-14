var request = require("request");
var q = require("q");
var errors = require("./defaultErrors");

class ServiceBase {
    constructor(accessToken) {
        this.accessToken = accessToken;
        this.errorStatusCodes = [400, 401, 403, 404, 405, 500];
    }

    promise(options) {
        options.strictSSL = false;
        var defered = q.defer();
        this._addAuthorizationHeader.call(this, options);
        request(options, (error, response, body) => {
            response = response || {};
            var statusCode = response.statusCode ? parseInt(response.statusCode, 10) : 500;
            if (body && typeof body === "string") {
                try {
                    body = JSON.parse(body);
                }
                catch (err) {
                }
            }
            if (body == null) {
                body = {};
            }
            if (error || this.errorStatusCodes.indexOf(statusCode) > -1) {
                if (body == "")
                    body = {};
                body.message = body.message || errors[statusCode] || statusCode.toString();

                defered.reject({
                    success: false,
                    status: statusCode,
                    message: body.message
                });
            }
            else {
                defered.resolve({
                    success: true,
                    status: statusCode,
                    result: body
                });
            }
        });
        return defered.promise;
    }

    getURIParametersString(parametersObject) {
        var result = "";
        var first = true;
        for (var key in parametersObject) {
            if (!first) {
                result += "&";
            }
            result += key;
            var value = parametersObject[key];
            if (typeof value == "undefined") {
                value = "";
            }
            else {
                value = encodeURIComponent(parametersObject[key]);
            }
            result += "=" + value;
            first = false;
        }

        return result;
    }

    _addAuthorizationHeader(options) {
        if (this.accessToken) {
            if (!options.headers)
                options.headers = {};

            Object.assign(options.headers, { "Authorization": this.accessToken });
        }
    }
}

module.exports = ServiceBase;
