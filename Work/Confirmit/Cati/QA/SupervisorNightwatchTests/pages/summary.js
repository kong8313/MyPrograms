var SELECTORS = {
    table: '#MainForm > table',
    firstRowId: '#Content_Summary_gridSummary_dataGrid > table > tbody > tr:nth-child(1) > td:nth-child(1) > table > tbody:nth-child(2) > tr > td > div:nth-child(2) > table > tbody > tr:nth-child(1) > td:nth-child(1)',
    summaryTab: {
        selector: '//*[@id="Content_dialog_tabs"]/span/span/span[span="Summary"]',
        locateStrategy: 'xpath'
    },
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