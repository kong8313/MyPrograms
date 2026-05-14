var SELECTORS = {
    surveyTab: {
        selector: '//*[@id="Content_manuSplitter_tmpl0_mainMenu_mainMenuBar_tree"]/ul/li/*[contains(text(),"Surveys")]',
        locateStrategy: 'xpath'
    },
    interviewersTab: {
        selector: '//*[@id="Content_manuSplitter_tmpl0_mainMenu_mainMenuBar_tree"]/ul/li/*[contains(text(),"Interviewers")]',
        locateStrategy: 'xpath'
    },
    schedulingTab: {
        selector: '//*[@id="Content_manuSplitter_tmpl0_mainMenu_mainMenuBar_tree"]/ul/li/*[contains(text(),"Scheduling")]',
        locateStrategy: 'xpath'
    },
    reportsTab: {
        selector: '//*[@id="Content_manuSplitter_tmpl0_mainMenu_mainMenuBar_tree"]/ul/li/*[contains(text(),"Reports")]',
        locateStrategy: 'xpath'
    },
    activityViewTab: {
        selector: '//*[@id="Content_manuSplitter_tmpl0_mainMenu_mainMenuBar_tree"]/ul/li/*[contains(text(),"Activity Views")]',
        locateStrategy: 'xpath'
    },
    recordedTab: {
        selector: '//*[@id="Content_manuSplitter_tmpl0_mainMenu_mainMenuBar_tree"]/ul/li/*[contains(text(),"Recorded Interviews")]',
        locateStrategy: 'xpath'
    },
    callCentersTab: {
        selector: '//*[@id="Content_manuSplitter_tmpl0_mainMenu_mainMenuBar_tree"]/ul/li/*[contains(text(),"Call Centers")]',
        locateStrategy: 'xpath'
    },
    resourcesTab: {
        selector: '//*[@id="Content_manuSplitter_tmpl0_mainMenu_mainMenuBar_tree"]/ul/li/*[contains(text(),"Resources")]',
        locateStrategy: 'xpath'
    },
    administrationTab: {
        selector: '//*[@id="Content_manuSplitter_tmpl0_mainMenu_mainMenuBar_tree"]/ul/li/*[contains(text(),"Administration")]',
        locateStrategy: 'xpath'
    },
    provideFeedbackTab: {
        selector: '//*[@id="Content_manuSplitter_tmpl0_mainMenu_mainMenuBar_tree"]/ul/li/*[contains(text(),"Provide Feedback")]',
        locateStrategy: 'xpath'
    },
    surveyIdName: '#dataGrid_columnheader_2 > div.gridHeaderLabel > div',
    firstRowSurveyId: {
        selector: '//*[@id="Content_listSplitter_tmpl0_RightFrameContent_SrvList_m_grid_dataGrid"]/table/tbody/tr[1]/td[1]/table/tbody[2]/tr/td/div[2]/table/tbody/tr[1]/td[3]',
        locateStrategy: 'xpath'
    },
    interviewersIdColumn: '#dataGrid_columnheader_1 > div.gridHeaderLabel > div',
    schedulingNameColumn: '#dataGrid_columnheader_2 > div.gridHeaderLabel > div',
    reportsNameColumn: '#dataGrid_columnheader_0 > div > div',
    activityViewNameColimn: '#dataGrid_columnheader_1 > div > div',
    recordedSurveyIdColumn: '#dataGrid_columnheader_1 > div.gridHeaderLabel > div',
    callCentersIdColumn: '#dataGrid_columnheader_0 > div.gridHeaderLabel > div',
    resourcesIdColumn: '#dataGrid_columnheader_0 > div.gridHeaderLabel > div',
    settingsTabGeneralName: {
        selector: '//*[@id="Content_tabs"]/span/span/span[1]/span/span/span[contains(text(),"General")]',
        locateStrategy: 'xpath'
    },
    sortByIdImg: '#dataGrid_columnheader_2 > div.gridHeaderLabel > div > img',
    selectedRow: 'td[class = " ig_Selected igg_SelectedCell"]',
    callManagementTab: {
        selector: '//*[@id="Content_listSplitter_tmpl0_RightFrameContent_SrvList_m_grid_topToolbar_rightMenu_ctl06"]',
        locateStrategy: 'xpath'
    },
    frame: '#listFrame'
};

var commands = {

    checkNameTabs: function () {
        return this.assert.containsText('@surveyTab', 'Surveys')
            .assert.containsText('@interviewersTab', 'Interviewers')
            .assert.containsText('@schedulingTab', 'Scheduling')
            .assert.containsText('@reportsTab', 'Reports')
            .assert.containsText('@activityViewTab', 'Activity Views')
            .assert.containsText('@recordedTab', 'Recorded Interviews')
            .assert.containsText('@callCentersTab', 'Call Centers')
            .assert.containsText('@resourcesTab', 'Resources')
            //.assert.containsText('@provideFeedbackTab', 'Provide Feedback')
    },
};

module.exports = {
    url: function () {
        return this.api.launchUrl;
    },
    commands: [commands],
    elements: SELECTORS
};