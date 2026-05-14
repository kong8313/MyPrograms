var SELECTORS = {
    surveyTab: {
        selector: '//*[@id="Content_manuSplitter_tmpl0_mainMenu_mainMenuBar_tree"]/ul/li/*[contains(text(),"Surveys")]',
        locateStrategy: 'xpath'
    },
    callCentersTab: {
        selector: '//*[@id="Content_manuSplitter_tmpl0_mainMenu_mainMenuBar_tree"]/ul/li/*[contains(text(),"Call Centers")]',
        locateStrategy: 'xpath'
    },
    surveysTab: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Surveys")]',
        locateStrategy: 'xpath'
    },
    supervisorTab: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Supervisors")]',
        locateStrategy: 'xpath'
    },
    surveyIdColumnName: '#dataGrid_columnheader_1 > div.gridHeaderLabel > div',
    userIdColumnName: '#dataGrid_columnheader_1 > div.gridHeaderLabel > div',
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