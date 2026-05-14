var SELECTORS = {
    table: '#MainForm > table',
    surveyTab: '#Content_manuSplitter_tmpl0_mainMenu_mainMenuBar_tree > ul > li:nth-child(1) > div',
    surveyID: '#Content_General_SrvName',
    surveyName: '#Content_General_SrvDescription',
    groupId: '#Content_General_lbITSDefGroup > option[selected]',
    target: '#Content_General_txtTarget',
    size: '#Content_General_txtSrvSize',
    state: '#Content_General_txtSrvState',
    scheduling: '#Content_General_ddlSchedulingScript > option[selected="selected"]',
    callDeliveryMode: '#Content_General_ddlCallDeliveryMode > option[selected="selected"]',
    openendReview: '#Content_General_txtOpenendReview',
    telephoneBlacklist: '#Content_General_txtSupportTelBlacklist',
    screenRec: '#Content_General_txtInterviewScreenRecording',
    voiceRec: '#Content_General_txtInterviewVoiceRecording',
    clusteredQuota: '#Content_General_txtQuotaForClustering',
    callGroups: '#Content_General_ddlCallGroupsMode > option[selected="selected"]',
    quotaBalansing: '#Content_General_txtQuotaForBalancing'

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