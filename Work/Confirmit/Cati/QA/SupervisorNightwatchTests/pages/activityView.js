var SELECTORS = {
    surveyTab: {
        selector: '//*[@id="Content_manuSplitter_tmpl0_mainMenu_mainMenuBar_tree"]/ul/li/*[contains(text(),"Surveys")]',
        locateStrategy: 'xpath'
    },
    activityViewTab: {
        selector: '//*[@id="Content_manuSplitter_tmpl0_mainMenu_mainMenuBar_tree"]/ul/li/*[contains(text(),"Activity Views")]',
        locateStrategy: 'xpath'
    },
    surveyListTab: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Survey List")]',
        locateStrategy: 'xpath'
    },
    interviewerListTab: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Interviewer List")]',
        locateStrategy: 'xpath'
    },
    appointmentListTab: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Appointment List")]',
        locateStrategy: 'xpath'
    },
    perfomanceListTab: {
        selector: '//*[@class="mainMenuItemInner"][contains(text(),"Performance List")]',
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