var ServiceBase = require("./service-base");

class CreateUserService extends ServiceBase {
    constructor(apiUrl, accessToken) {
        super(accessToken);
        this.url = apiUrl + '/users';
        this.accessToken = accessToken;
    }

    getByCompanyId(companyId) {
        var options = {
            url: this.url + '?companyid=' + companyId,
            method: "GET"
        };

        return this.promise(options);
    }

    createNormal(data) {
        var options = {
            url: this.url + '/normal',
            body: JSON.stringify(data),
            headers: {
                "content-type": "application/json"
            },
            method: "POST"
        };

        return this.promise(options);
    }

    createUser(data) {
        return this.getByCompanyId(data.companyId)
            .catch(console.log)
            .then(response => {
                const users = response.result.Items;
                return users.find(user => user.UserName == data.name);
            })
            .then(user => {
                if (!user) {
                    console.log('Creating user');
                    return this.createNormal(data)
                        .catch(console.log)
                        .then(response => {
                            return { id: response.result.Id, name: response.result.Username, companyId: response.result.Company.Id, isNew: true };
                        })
                }
                else {
                    return { id: user.UserId, name: user.UserName, companyId: user.CompanyId, isNew: false };
                }
            });
    }
}

module.exports = CreateUserService;