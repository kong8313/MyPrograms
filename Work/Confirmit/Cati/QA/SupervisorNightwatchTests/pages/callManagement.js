var SELECTORS = {
    customViewTab: {
        selector: '//*[@id="Content_m_grid_topToolbar_leftMenu_CustomViewActions"]',
        locateStrategy: 'xpath'
    },
    customViewTabDisabled: '#Content_m_grid_topToolbar_leftMenu_CustomViewActions[disabled="true"]',
    customViewAddTab: '#Content_viewStateContextMenu > div > ul > li > a',
    customViewNameField: {
        selector: '//*[@id="Content_dialog_tbxCusomViewName"]',
        locateStrategy: 'xpath'
    },
    uncheckAllCastomView: '#Content_dialog_columnNamesGrid_dataGrid_Header0_CheckAllControl',
    customViewTelephoneNumberCheckBox: {
        selector: '//*[@id="Content_dialog_columnNamesGrid_dataGrid"]/table/tbody/tr[1]/td[1]/table/tbody[2]/tr/td/div[2]/table/tbody/tr/td[contains(text(),"TelephoneNumber")]/preceding-sibling::td/input',
        locateStrategy: 'xpath'
    },
    customViewResrondentNameCheckBox: {
        selector: '//*[@id="Content_dialog_columnNamesGrid_dataGrid"]/table/tbody/tr[1]/td[1]/table/tbody[2]/tr/td/div[2]/table/tbody/tr/td[contains(text(),"RespondentName")]/preceding-sibling::td/input',
        locateStrategy: 'xpath'
    },
    customViewTimeToCallCheckBox: {
        selector: '//*[@id="Content_dialog_columnNamesGrid_dataGrid"]/table/tbody/tr[1]/td[1]/table/tbody[2]/tr/td/div[2]/table/tbody/tr/td[contains(text(),"TimeText")]/preceding-sibling::td/input',
        locateStrategy: 'xpath'
    },
    customViewCallPriorityCheckBox: {
        selector: '//*[@id="Content_dialog_columnNamesGrid_dataGrid"]/table/tbody/tr[1]/td[1]/table/tbody[2]/tr/td/div[2]/table/tbody/tr/td[contains(text(),"Priority")]/preceding-sibling::td/input',
        locateStrategy: 'xpath'
    },
    customViewExpirationTimeCheckBox: {
        selector: '//*[@id="Content_dialog_columnNamesGrid_dataGrid"]/table/tbody/tr[1]/td[1]/table/tbody[2]/tr/td/div[2]/table/tbody/tr/td[contains(text(),"ExpireTimeText")]/preceding-sibling::td/input',
        locateStrategy: 'xpath'
    },
    customViewSaveButton: {
        selector: '//*[@id="Content_dialog_btnOK"]',
        locateStrategy: 'xpath'
    },
    moveUpButton: '#Content_dialog_columnNamesGrid_topToolbar_rightMenu_ctl01 > img',
    interviewIdColumnName: 'th[key="InterviewID"]',
    telephoneNumberColumnName: 'th[key="TelephoneNumber"]',
    respondentNameColumnName: 'th[key="RespondentName"]',
    dialModeColumnName: 'th[key="DialingMode"]',
    dialTypeColumnName: 'th[key="DialTypeId"]',
    timeToCallColumnName: 'th[key="TimeText"]',
    callPriorityColumnName: 'th[key="Priority"]',
    extendedStatusColumnName: 'th[key="StateName"]',
    callAttemptsColumnName: 'th[key="AttemptNumber"]',
    shiftTypeColumnName: 'th[key="ShiftType"]',
    expireTimeTextColumnName: 'th[key="ExpireTimeText"]',
    appointmentExpirationColumnName: 'th[key="ExpTimeText"] > div > div',
    stateColumnName: 'th[key="CallState"]',
    viewToolBar: '#Content_m_grid_topToolbar_leftMenu_ctl00_ddlState',
    selectedNameInToolbar: '#Content_m_grid_topToolbar_leftMenu_ctl00_ddlState > option[selected="selected"]',
    editCustomViewButton: '#Content_viewStateContextMenu > div > ul > li:nth-child(2) > a',
    deleteCustomViewButton: '#Content_viewStateContextMenu > div > ul > li:nth-child(3) > a',
    overlayElement: '.overlayLayer',
    customViewNameInViewScheduled: {
        selector: '//*[@id="Content_m_grid_topToolbar_leftMenu_ctl00_ddlState"]/option[contains(text(),"Scheduled")]',
        locateStrategy: 'xpath'
    },
    customViewNameInViewAll: {
        selector: '//*[@id="Content_m_grid_topToolbar_leftMenu_ctl00_ddlState"]/option[contains(text(),"All")]',
        locateStrategy: 'xpath'
    },
    frame: '#listFrame'
};

var commands = {
};

module.exports = {
    url: function () {
        return this.api.launchUrl;
    },
    commands: [commands],
    elements: SELECTORS
};