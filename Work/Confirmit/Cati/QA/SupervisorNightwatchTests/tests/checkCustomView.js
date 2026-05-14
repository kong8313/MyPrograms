module.exports = {
    'Check custom view in call management': function (browser) {
        var customViewName = new Date().getTime();
        var supervisor = browser.page.supervisor();
        var callManagement = browser.page.callManagement();
        browser
            .login({ name: browser.globals.login.user_name, password: browser.globals.login.password });
        browser
            .url(browser.launchUrl)

        supervisor
            .navigate()
            .waitForElementVisible('@surveyTab', 10000)

        browser.frame('listFrame')

        supervisor.waitForElementVisible('@surveyIdName', 10000)
            .click('@surveyIdName')
        browser.pause(2000)
        supervisor
            .waitForElementVisible('@firstRowSurveyId', 10000)
            .click('@firstRowSurveyId')
            .waitForElementVisible('@selectedRow', 10000)
            .waitForElementVisible('@sortByIdImg', 10000)
            .waitForElementVisible('@callManagementTab', 10000)
            .click('@callManagementTab')
        browser
            .windowHandles(function (result) {
                var handle = result.value[1];
                browser.switchWindow(handle)
                    .maximizeWindow();
            });
        callManagement.waitForElementVisible('@customViewTab', 10000)
            .click('@customViewTab')
            .waitForElementVisible('@customViewAddTab', 10000)
            .click('@customViewAddTab')

        browser.frame(null)
        browser.frame(0)

        callManagement.waitForElementVisible('@customViewNameField', 10000)
            .setValue('@customViewNameField', customViewName)
            .waitForElementVisible('@uncheckAllCastomView', 10000)
            .click('@uncheckAllCastomView')
            .waitForElementVisible('@customViewTelephoneNumberCheckBox', 10000)
            .click('@customViewTelephoneNumberCheckBox')
            .waitForElementVisible('@customViewResrondentNameCheckBox', 10000)
            .click('@customViewResrondentNameCheckBox')
            .waitForElementVisible('@customViewTimeToCallCheckBox', 10000)
            .click('@customViewTimeToCallCheckBox')
            .waitForElementVisible('@customViewSaveButton', 10000)
            .click('@customViewSaveButton')

        browser.frame(null)

        callManagement.waitForElementNotPresent('@overlayElement', 10000)
            .assert.containsText('@interviewIdColumnName', 'Interview ID')
            .assert.containsText('@telephoneNumberColumnName', 'Telephone Number')
            .assert.containsText('@respondentNameColumnName', 'Respondent Name')
            .assert.containsText('@timeToCallColumnName', 'Time to Call')
            .click('@viewToolBar')
            .waitForElementVisible('@selectedNameInToolbar', 10000)
            .assert.containsText('@selectedNameInToolbar', customViewName)
            .click('@customViewTab')
            .click('@editCustomViewButton')

        browser.frame(0)

        callManagement.waitForElementVisible('@customViewCallPriorityCheckBox', 10000)
            .click('@customViewCallPriorityCheckBox')
            .click('@moveUpButton')
            .waitForElementVisible('@customViewCallPriorityCheckBox', 10000)
            .waitForElementVisible('@customViewExpirationTimeCheckBox', 10000)
        browser.pause(2000)
        callManagement.click('@customViewExpirationTimeCheckBox')
        browser.element('xpath', '//*[@id="Content_dialog_columnNamesGrid_dataGrid"]/table//td[contains(text(),"ExpireTimeText")]/preceding-sibling::td/input', function (response) {
            browser.elementIdSelected(response.value.ELEMENT, function (result) {
                browser.verify.ok(result.value, 'Checkbox is selected');
            });
        })
        callManagement.waitForElementVisible('@customViewSaveButton', 10000)
            .click('@customViewSaveButton')

        browser.frame(null)

        callManagement.waitForElementNotPresent('@overlayElement', 10000)
            .assert.containsText('@interviewIdColumnName', 'Interview ID')
            .assert.containsText('@telephoneNumberColumnName', 'Telephone Number')
            .assert.containsText('@interviewIdColumnName', 'Interview ID')
            .assert.containsText('@respondentNameColumnName', 'Respondent Name')
            .assert.containsText('@callPriorityColumnName', 'Call Priority')
            .assert.containsText('@timeToCallColumnName', 'Time to Call')
            .assert.containsText('@expireTimeTextColumnName', 'Expiration Time')

        browser.frame(null)

        callManagement.click('@customViewTab')
            .click('@deleteCustomViewButton')

        browser.acceptAlert()

        callManagement.click('@viewToolBar')
            .waitForElementVisible('@customViewNameInViewScheduled', 10000)
            .assert.containsText('@selectedNameInToolbar', "Scheduled")
            .waitForElementVisible('@customViewTab', 10000)
            .click('@customViewTab')
            .waitForElementVisible('@customViewAddTab', 10000)
            .waitForElementNotPresent('@editCustomViewButton', 10000)
            .waitForElementNotPresent('@deleteCustomViewButton', 10000)
            .click('@viewToolBar')
            .click('@customViewNameInViewAll')
            .waitForElementVisible('@customViewTabDisabled', 10000)
        browser.useXpath()
            .waitForElementNotPresent('//*[@id="Content_m_grid_topToolbar_leftMenu_ctl00_ddlState"]/option/*[contains(text(), "' + customViewName + '")]', 10000)

        browser.end();
    }

}