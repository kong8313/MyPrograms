var SELECTORS = {
    surveyTab: {
        selector: '//*[@id="Content_manuSplitter_tmpl0_mainMenu_mainMenuBar_tree"]/ul/li/*[contains(text(),"Surveys")]',
        locateStrategy: 'xpath'
    },
    resourcesTab: {
        selector: '//*[@id="Content_manuSplitter_tmpl0_mainMenu_mainMenuBar_tree"]/ul/li/*[contains(text(),"Resources")]',
        locateStrategy: 'xpath'
    },
    callGroups: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Call Groups")]',
        locateStrategy: 'xpath'
    },
    breakTab: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Break Classifications")]',
        locateStrategy: 'xpath'
    },
    dialerTab: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Dialer")]',
        locateStrategy: 'xpath'
    },
    telephoneBlackListTab: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Telephone Blacklist")]',
        locateStrategy: 'xpath'
    },
    ddiNumbersTab: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"DDI Numbers")]',
        locateStrategy: 'xpath'
    },
    masterTimezoneListTab: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Master Timezone List")]',
        locateStrategy: 'xpath'
    },
    activeTimezoneListTab: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Active Timezone List")]',
        locateStrategy: 'xpath'
    },
    tasksTab: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Tasks")]',
        locateStrategy: 'xpath'
    },
    aboutCatiSupervisorTab: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"About CATI Supervisor")]',
        locateStrategy: 'xpath'
    },
    inboundSettingsTab: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Inbound Settings")]',
        locateStrategy:'xpath'
    },
    ivrSettingsTab: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"IVR Settings")]',
        locateStrategy:'xpath'
    },
    inboundSettingatelephoneNumberColumnName: {
        selector: '//*[@id="dataGrid_columnheader_1"]/div[1]/div[contains(text(),"Telephone Number (Direct Dial-In)")]',
        locateStrategy: 'xpath'
    },
    ivrSettingatelephoneNumberColumnName: {
        selector: '//*[@id="dataGrid_columnheader_1"]/div[1]/div[contains(text(),"Language ID")]',
        locateStrategy: 'xpath'
    },
    callGroupsIdColumn: '#dataGrid_columnheader_0 > div.gridHeaderLabel > div',
    breakTabIdColumnName: '#dataGrid_columnheader_1 > div.gridHeaderLabel > div',
    dialerTabIdColumnName: '#dataGrid_columnheader_1 > div.gridHeaderLabel > div',
    telephoneBlackListTabTelNumberColumnName: '#dataGrid_columnheader_2 > div.gridHeaderLabel > div',
    ddiNumbersTabTelNumberColumnName: '#dataGrid_columnheader_2 > div.gridHeaderLabel > div',
    masterTimezoneIdColumnName: '#dataGrid_columnheader_0 > div > div',
    activeTimezoneNameColumnName: '#dataGrid_columnheader_1 > div > div',
    tasksTabIdColumnName: {
        selector: '//*[@id="dataGrid_columnheader_2"]/div[1]/div[contains(text(),"Task ID")]',
        locateStrategy: 'xpath'
    },
    aboutCatiSupervisorTabTitleName: '#Content_lbTitle',
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