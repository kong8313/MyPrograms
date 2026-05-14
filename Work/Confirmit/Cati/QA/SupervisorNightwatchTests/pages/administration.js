var SELECTORS = {
    surveyTab: {
        selector: '//*[@id="Content_manuSplitter_tmpl0_mainMenu_mainMenuBar_tree"]/ul/li/*[contains(text(),"Surveys")]',
        locateStrategy: 'xpath'
    },
    administrationTab: {
        selector: '//*[@id="Content_manuSplitter_tmpl0_mainMenu_mainMenuBar_tree"]/ul/li/*[contains(text(),"Administration")]',
        locateStrategy: 'xpath'
    },
    settingsTab: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(), "Settings") and not(contains(text(), "Inbound")) and not(contains(text(), "System Settings"))]',
        locateStrategy: 'xpath'
    },
    systemSettingsTab: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"System Settings")]',
        locateStrategy: 'xpath'
    },
    databaseTab: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Database Update Logs")]',
        locateStrategy: 'xpath'
    },
    managementTab: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Management")]',
        locateStrategy: 'xpath'
    },

    settingsTabGeneralName: {
        selector: '//*[@id="Content_tabs"]/span/span/span[1]/span/span/span[contains(text(),"General")]',
        locateStrategy: 'xpath'
    },
    systemSettingsFirstColumnName: {
        selector: '//*[@id="dataGrid_columnheader_0"]//div[contains(text(),"System Name")]',
        locateStrategy: 'xpath'
    },
    databaseTabScriptVersionColumnName: '#dataGrid_columnheader_1 > div.gridHeaderLabel > div',
    managementTabFlushLogButton: '#Content__flushLog',
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