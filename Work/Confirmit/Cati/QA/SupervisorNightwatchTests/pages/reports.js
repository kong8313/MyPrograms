var SELECTORS = {
    surveyTab: {
        selector: '//*[@id="Content_manuSplitter_tmpl0_mainMenu_mainMenuBar_tree"]/ul/li/*[contains(text(),"Surveys")]',
        locateStrategy: 'xpath'
    },
    surveyTab: {
        selector: '//*[@id="Content_manuSplitter_tmpl0_mainMenu_mainMenuBar_tree"]/ul/li/*[contains(text(),"Reports")]',
        locateStrategy: 'xpath'
    },
    surveyOverviewTab: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Survey overview")]',
        locateStrategy: 'xpath'
    },
    surveyProductivityTab: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Survey productivity")]',
        locateStrategy: 'xpath'
    },
    iterviewerProductivity: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Interviewer productivity")]',
        locateStrategy: 'xpath'
    },
    iterviewerSessions: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Interviewer sessions")]',
        locateStrategy: 'xpath'
    },
    quotaProgress: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Quota progress")]',
        locateStrategy: 'xpath'
    },
    sampleStatusSummary: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Sample status summary")]',
        locateStrategy: 'xpath'
    },
    sampleStatusSummaryByQuestion: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Sample status summary by question")]',
        locateStrategy: 'xpath'
    },
    callAttempts: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Call attempts")]',
        locateStrategy: 'xpath'
    },
    attemptsByDisposition: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Attempts by disposition")]',
        locateStrategy: 'xpath'
    },
    numberOfAttempts: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Number of attempts")]',
        locateStrategy: 'xpath'
    },
    sampleUtilization: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Sample utilization")]',
        locateStrategy: 'xpath'
    },
    interviewerSubmissionDetails: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Interviewer submission details")]',
        locateStrategy: 'xpath'
    },
    aggregatedInterviewerSubmission: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Aggregated interviewer submission")]',
        locateStrategy: 'xpath'
    },
    userFilterSelectButton: '#ctl00_Content_updatePanel1 > table > tbody > tr:nth-child(1) > td:nth-child(4)',
    selectStatusButton: '#Content_updatePanel1 > table > tbody > tr:nth-child(1) > td:nth-child(4)',
    selectSurveyButton: '#ctl00_Content_updatePanel1 > table > tbody > tr:nth-child(2) > td:nth-child(2) > table > tbody > tr > td:nth-child(1)',
    interviewerColumnName: '#dataGrid_columnheader_0 > div.gridHeaderLabel > div',
    buildReportButton: '#ctl00_Content_updatePanel1 > table > tbody > tr:nth-child(1) > td:nth-child(5)', 
    statusText: '#ctl00_Content_lblITS',
    hideZepoStatusesCheckBox: '#ctl00_Content_cbxHideZeroStatuses',
    statusFilterCheckBox: '#ctl00_Content_cbxITS',
    surveyIdColumnName: '#dataGrid_columnheader_2 > div.gridHeaderLabel > div',
    dataRangeSelectField: '#ctl00_Content_dtrsDates_ddlFilter',
    buildReportButtonInNumbers: {
        selector: '//*[@id="aspnetForm"]/table[1]/tbody/tr[1]/td[3]',
        locateStrategy: 'xpath'
    },
    surveyIdColumnNameForInterviewerSubmissionDetails: '#dataGrid_columnheader_1 > div.gridHeaderLabel > div',
    interviewerColumnNameForAgregatedTab: '#dataGrid_columnheader_1 > div.gridHeaderLabel > div',
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