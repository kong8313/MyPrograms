const OAuth = require('oauth');
var ServiceBase = require("./service-base");

class AccessTokenService extends ServiceBase {
    constructor(apiUrl) {
        super(null);
        this.apiUrl = apiUrl;
    }

    getIdentityServiceUrl() {
        var options = {
            url: this.apiUrl,
            method: "GET"
        };

        return this
            .promise(options)
            .then(response => {
                let identityServiceUrl = undefined

                response.result.items.forEach(function(item) {
                    if (item.id === 'authentication') {
                        identityServiceUrl = item.links.self
                    }
                })

                return identityServiceUrl
            })
            .catch(error => {
                if (error.status === 401) {
                    let identityServiceUrl = undefined
                    const wwwAuthenticate = error.response.headers['www-authenticate']

                    if (wwwAuthenticate) {
                        let parts = wwwAuthenticate.split(/\s+/)

                        parts.forEach((item) => {
                            let kvp = item.split(/=/)
                            if (kvp.length === 2 && kvp[0] === 'authorization_uri') {
                                identityServiceUrl = kvp[1].replace(/"/g, '')
                            }
                        })

                        return identityServiceUrl
                    }
                }

                throw error
            })
            .then(identityServiceUrl => {
                if (!identityServiceUrl) {
                    throw new Error('Could not find the identity server!')
                }

                console.log('identityServiceUrl = ' + identityServiceUrl)

                return identityServiceUrl
            })
    }

    getAccessToken(username, password) {
        return new Promise((resolve, reject) => {
            this.getIdentityServiceUrl()
                .then(identityServiceUrl => {

                    const consumerKey = "ro-client"
                    const consumerSecret = "F4C0E4F7-019B-45E6-ABCD-2B48E41AC6FF"

                    const oauth2 = new OAuth.OAuth2(
                        consumerKey,
                        consumerSecret,
                        identityServiceUrl,
                        '',
                        '/connect/token',
                        null)

                    oauth2.getOAuthAccessToken(
                        '',
                        {
                            grant_type: 'password',
                            username: username,
                            password: password,
                            scope: 'openid profile users',
                            acr_values: "tenant:cf"
                        },
                        function(e, access_token) {
                            if (e) {
                                let ex = new Error("Token authorization error")
                                ex.statusCode = e.statusCode
                                ex.body = e.data

                                reject(ex)
                            }
                            else {
                                resolve(`Bearer ${access_token}`)
                            }
                        })
                })
                .catch(error => {
                    reject(error)
                })
        })
    }
}

module.exports = AccessTokenService;
