var SELECTORS = {
    table: '#MainForm > table',
    firstColumnName: '#dataGrid_columnheader_1 > div.gridHeaderLabel > div',
    assignmentsTab: {
        selector: '//*[@id="Content_dialog_tabs"]/span/span/span[span="Assignments"]',
        locateStrategy: 'xpath'
    },
    assignmentsExistenceInformation:{
        selector: '//div[contains(text(), "No items available")]',
        locateStrategy: 'xpath'
    },
    firstRowAssignmentsID: '#Content_Assignment_m_grid_dataGrid > table > tbody > tr:nth-child(1) > td:nth-child(1) > table > tbody:nth-child(2) > tr > td > div:nth-child(2) > table > tbody > tr > td:nth-child(2)'
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