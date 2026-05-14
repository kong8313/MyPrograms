var surveyID;
var surveySID;
var statuses;

module.exports = {
    tags: ['survey-tabs-info'],

    'Check general tab': function checkGeneral(browser) {
        statuses = new Array({
            'Disabled': 0,
            'Enabled': 1
        });
        browser
            .login({name: browser.globals.login.user_name, password: browser.globals.login.password});
        browser
            .url(browser.launchUrl)

        var supervisor = browser.page.supervisor();
        supervisor
            .navigate()
            .waitForElementVisible('@surveyTab', 30000)

        var surveys = browser.page.surveys();
        browser.frame('listFrame')
        surveys.waitForElementVisible('@surveyNameSort', 30000)
            .click('@surveyNameSort')
            .setValue('@surveyNameSort', 'Olympic')
            .waitForElementVisible('@refreshButton', 30000)
            .click('@refreshButton')
        browser.pause(2000)
        surveys.waitForElementVisible('@firstRowSurvey', 30000)
            .moveToElement('@firstRowSurveyCheckBox', 10, 10)
            .click('@firstRowSurveyCheckBox')
            .waitForElementVisible('@firstRowSurveyID', 30000)
            .getText("@firstRowSurveyID", function (resultId) {
                surveyID = resultId.value;
                browser.getGeneralTabData(surveyID, function (result) {
                    browser.assert.ok(result && result.recordset.length != 0, 'SQL data received')

                    surveySID = result.recordset[0].SID;
                    var surveys = browser.page.surveys();
                    var general = browser.page.surveysGeneral();
                    surveys.waitForElementVisible('@viewButton', 10000)
                    surveys.click('@viewButton')
                    browser.pause(2000)
                    browser.frame('infoFrame').frame(0)
                    general.waitForElementVisible('@table', 30000)
                    general.assert.containsText('@surveyID', result.recordset[0].SurveyName, 'Survey Id OK')
                    general.assert.containsText('@surveyName', result.recordset[0].Description, 'Survey Name OK')
                    general.assert.attributeContains('@groupId', 'value', result.recordset[0].StateGroupID, 'State Group ID OK')
                    general.assert.containsText('@groupId', result.recordset[0].StateGroupName, 'State Group Name OK')
                    general.getAttribute('@target', 'value', function (target) {
                        if (target.value == '') {
                            general.assert.ok(result.recordset[0].Target == null, 'Target null')
                        } else {
                            general.assert.equal(target.value, result.recordset[0].Target, 'Target Ok')
                        }
                    })
                    general.assert.containsText('@size', result.recordset[0].Size, 'Size OK')
                    general.assert.attributeContains('@scheduling', 'value', result.recordset[0].SchedulindID, 'Schedulind ID OK')
                    general.assert.containsText('@scheduling', result.recordset[0].ScheduleName, 'Schedule Name OK')
                    general.getText('@callDeliveryMode', function (callDelivery) {
                        if (callDelivery.value == 'Order by ID (lowest first)') {
                            general.assert.equal(false, result.recordset[0].IsRandomCallDeliveryEnabled, 'call Delivery Order by ID')
                        } else {
                            general.assert.equal(true, result.recordset[0].IsRandomCallDeliveryEnabled, 'call Delivery random')
                        }
                    })
                    general.getText('@openendReview', function (openendReview) {
                        if (openendReview.value == 'Disabled') {
                            general.assert.equal(statuses[0].Disabled, parseInt(result.recordset[0].ForceOpnRev), 'openend Review Disabled')
                        } else {
                            general.assert.equal(statuses[0].Enabled, parseInt(result.recordset[0].ForceOpnRev), 'openend Review Enabled')
                        }
                    })
                    general.getText('@telephoneBlacklist', function (telephoneBlacklist) {
                        if (telephoneBlacklist.value == 'Disabled') {
                            general.assert.equal(statuses[0].Disabled, result.recordset[0].IsTelephoneBlacklistSupported, 'Telephone Blacklist Disabled')
                        } else {
                            general.assert.equal(statuses[0].Enabled, result.recordset[0].IsTelephoneBlacklistSupported, 'Telephone  Blacklist Enabled')
                        }
                    })
                    general.getText('@screenRec', function (screenRecording) {
                        if (screenRecording.value == 'Disabled') {
                            general.assert.equal(statuses[0].Disabled, result.recordset[0].InterviewScreenRecording, 'Screen Recording Disabled')
                        } else {
                            general.assert.equal(statuses[0].Enabled, result.recordset[0].InterviewScreenRecording, 'Screen Recording Enabled')
                        }
                    })
                    general.getText('@voiceRec', function (voiceRecording) {
                        if (voiceRecording.value == 'Disabled') {
                            general.assert.equal(statuses[0].Disabled, result.recordset[0].RecWholeInt, 'Voice Recording Disabled')
                        } else {
                            general.assert.equal(statuses[0].Enabled, result.recordset[0].RecWholeInt, 'Voice Recording Enabled')
                        }
                    })
                });
            })
    },

    'Check summary tab': function checkSummary(browser) {
        var summary = browser.page.summary();
        var summaryElements = null;
        var summaryData = null;
        browser.frame(null)
        browser.frame('listFrame').frame('infoFrame')
        summary.click('@summaryTab')
        browser.frame(1)
        summary.waitForElementVisible('@firstRowId', 10000)
        browser.getSummaryTabData(surveySID, function (resultSummary) {
            summaryData = resultSummary;
            browser.elements('css selector', '#Content_Summary_gridSummary_dataGrid > table > tbody > tr:nth-child(1) > td:nth-child(1) > table > tbody:nth-child(2) > tr > td > div:nth-child(2) > table > tbody > tr', function (elemResult) {
                summaryElements = elemResult;
            })
        })

        browser.perform(function () {
            let i = 0;

            summaryElements.value.forEach(function (elementResult) {
                browser.elementIdText(elementResult.ELEMENT, function (text) {
                    var text = text.value;
                    var splitted = text.split(' ');
                    var id = splitted[0];
                    var name = splitted.slice(1, splitted.length - 12).join(' ');
                    var totalCount = splitted.slice(splitted.length - 12, splitted.length - 9).join(' ');
                    var enabled = splitted.slice(splitted.length - 9, splitted.length - 6).join(' ');
                    var disabledByQuota = splitted.slice(splitted.length - 6, splitted.length - 3).join(' ');
                    var disabled = splitted.slice(splitted.length - 3, splitted.length).join(' ');
                    var percentInDB = eval(summaryData.recordset[i].count * 100 / summaryData.recordset[i].sample_size).toFixed(2)
                    var totalCountInDB = summaryData.recordset[i].count + ' (' + percentInDB + ' %)';
                    var enabledPercentInDB = eval(summaryData.recordset[i].enabled_call * 100 / summaryData.recordset[i].sample_size).toFixed(2);
                    var enabledInDB = summaryData.recordset[i].enabled_call + ' (' + enabledPercentInDB + ' %)';
                    var disabledByQuotaPercentInDB = eval(summaryData.recordset[i].fcd_disabled_call * 100 / summaryData.recordset[i].sample_size).toFixed(2);
                    var disabledByQuotaInDB = summaryData.recordset[i].fcd_disabled_call + ' (' + disabledByQuotaPercentInDB + ' %)';
                    var disabledPercentInDB = eval(summaryData.recordset[i].user_disabled_call * 100 / summaryData.recordset[i].sample_size).toFixed(2);
                    var disabledInDB = summaryData.recordset[i].user_disabled_call + ' (' + disabledPercentInDB + ' %)';
                    browser.assert.equal(id, summaryData.recordset[i].id, 'ID OK')
                    browser.assert.equal(name, summaryData.recordset[i].name, 'Name OK')
                    browser.assert.equal(totalCount, totalCountInDB, 'total Count OK')
                    browser.assert.equal(enabled, enabledInDB, 'enabled OK')
                    browser.assert.equal(disabledByQuota, disabledByQuotaInDB, 'disabled By Quota OK')
                    browser.assert.equal(disabled, disabledInDB, 'disabled OK')
                    i++;
                })
            })
        });
    },

    'Check assignments tab': function checkAssignments(browser) {
        var typeCount = new Array({
            'Person': 0,
            'Group': 1
        });
        var assignmentsElements = null;
        var assignmentsData = null;
        var assignments = browser.page.assignments();
        browser.frame(null)
        browser.frame('listFrame').frame('infoFrame')
        assignments.click('@assignmentsTab')
        browser.frame(2)
        assignments.waitForElementVisible('@firstColumnName', 10000)
        browser.getAssigmentsTabData(surveySID, function (resultAssignments) {
            assignmentsData = resultAssignments;
            browser.elements('css selector', '#Content_Assignment_m_grid_dataGrid > table > tbody > tr:nth-child(1) > td:nth-child(1) > table > tbody:nth-child(2) > tr > td > div:nth-child(2) > table > tbody > tr', function (elemResult) {
                assignmentsElements = elemResult;
            })
        })

        browser.perform(function () {
            let i = 0;

            browser.checkExistenceInformationInAssignmentsTab((res) => {
                if (res) {
                    assignments.assert.containsText('@assignmentsExistenceInformation', 'No items available', 'No items available')
                } else {
                    assignmentsElements.value.forEach(function (elementResult) {
                        browser.elementIdText(elementResult.ELEMENT, function (text) {
                            var text = text.value;
                            var splitted = text.split(' ');
                            var id = splitted[0];
                            var name = splitted.slice(1, splitted.length - 3).join(' ');;
                            var type = splitted[splitted.length - 3];
                            var count = splitted.slice(splitted.length - 2, splitted.length).join(' ');
                            browser.assert.equal(id, assignmentsData.recordset[i].PersonSID, 'ID OK')
                            browser.assert.equal(name, assignmentsData.recordset[i].Name, 'Name OK')
                            if (assignmentsData.recordset[i].IsPersonGroup == typeCount[0].Person) {
                                browser.assert.equal(typeCount[0].Person, assignmentsData.recordset[i].IsPersonGroup, 'Type Person')
                            } else {
                                browser.assert.equal(typeCount[0].Group, assignmentsData.recordset[i].IsPersonGroup, 'Type Group')
                            }
                            if (assignmentsData.recordset[i].Counts == 0) {
                                browser.assert.equal(count, "Any (" + assignmentsData.recordset[i].Counts + ")", 'Count = 0, OK')
                            } else {
                                browser.assert.equal(count, assignmentsData.recordset[i].IsPersonGroup, 'Count != 0, OK')
                            }
                            i++;
                        })

                    })
                }
            })
        })
    },

    'Check scheduling parameters tab': function checkSchedulingParameters(browser) {
        var scheduling = browser.page.scheduling();
        browser.frame(null)
        browser.frame('listFrame').frame('infoFrame')
        scheduling.click('@schedulingTab')
        browser.frame(5)
        scheduling.waitForElementVisible('@idColumnName', 10000)
        browser.getSchedulingParametersTabData(surveySID, function (resultSchedulingParameters) {
            schedulingParametersData = resultSchedulingParameters;
            browser.elements('css selector', '#Content_SchedulingParams_m_grid_dataGrid tr[role="row"] td', function (elemResult) {
                schedulingParametersElements = elemResult;
            })
        })

        var parameterRowCellsData = [];
        browser.perform(function () {
            browser.checkExistenceInformationInSchedulingParametersTab((res) => {
                if (res) {
                    scheduling.assert.containsText('@schedulingExistenceInformation', 'No scheduling parameters are associated with this scheduling script', 'No scheduling parameters are associated with this scheduling script')
                } else {
                    schedulingParametersElements.value.forEach((elementResult, index) => {
                        if (index == 3) {return;} // element with index = 3 does not present on the page
                        browser.elementIdText(elementResult.ELEMENT, (text) => {
                            parameterRowCellsData.push(text.value)
                        })
                    })
                    browser.perform(function () {
                        browser.assert.equal(parameterRowCellsData[0], schedulingParametersData[0].Id, 'ID OK')
                        browser.assert.equal(parameterRowCellsData[1], schedulingParametersData[0].Name, 'Name OK')
                        if (parameterRowCellsData[2] == "Numeric") {
                            browser.assert.equal('Integer', schedulingParametersData[0].Type, 'Type OK')
                        } else {
                            browser.assert.equal(parameterRowCellsData[2], schedulingParametersData[0].Type, 'Type OK')
                        }
                        browser.assert.equal(parameterRowCellsData[3], schedulingParametersData[0].Value, 'Value OK')
                        browser.assert.equal(parameterRowCellsData[4].trim(), schedulingParametersData[0].Description, 'Description OK')
                    });
                }
            })
        })
    },

    // 'Check interviewer search tab': function checkInterviewerSearch(browser) {
    //     var interviewer = browser.page.interviewers();
    //     browser.frame(null)
    //     browser.frame('listFrame').frame('infoFrame')
    //     interviewer.click('@interviewerSearchTab')
    //     browser.frame(4)
    //     interviewer.waitForElementVisible('@nameColumnName', 10000)
    //     browser.elements('css selector', '#Content_AvailableFieldsInConsole_m_grid_dataGrid tr[role="row"] td', function (elemResult) {
    //         interviewerSearchElements = elemResult;
    //         interviewerSearchElements.value.forEach((elementResult, index) => {
    //             browser.elementIdText(elementResult.ELEMENT, (text) => {
    //             })
    //         })
    //     })
    // },

    'Check filters tab': function checkFilters(browser) {
        let textData = [];
        var filters = browser.page.filters();
        browser.frame(null)
        browser.frame('listFrame').frame('infoFrame')
        filters.click('@filtersTab')
        browser.frame(6)
        filters.waitForElementVisible('@idColumnName', 10000)
        browser.getFiltersTabData(surveySID, function (resultFilters) {
            filtersData = resultFilters;
            browser.elements('css selector', '#Content_Filters_filtersList_filtersGrid_dataGrid tr[role="row"] td', function (elemResult) {
                filtersElements = elemResult;
            })
        })

        browser.perform(function () {
            filtersElements.value.forEach((elementResult, index) => {
                if (index % 5 == 0) {return;}
                browser.elementIdText(elementResult.ELEMENT, (text) => {
                    textData.push(text.value)
                })
            })
        })
        browser.perform(function () {
            if (filtersData.recordset[0].SurveySID == 0) {
                browser.assert.equal(filtersData.recordset[0].SID, textData[0], 'SID ok')
                browser.assert.equal(filtersData.recordset[0].Name, textData[1], 'Name ok')
                browser.assert.equal(filtersData.recordset[0].Description, textData[2], 'Description ok')
                browser.assert.equal(filtersData.recordset[0].SurveySID, 0, 'Type ok')
                browser.assert.equal(filtersData.recordset[1].SID, textData[4], 'SID ok')
                browser.assert.equal(filtersData.recordset[1].Name, textData[5], 'Name ok')
                browser.assert.equal(filtersData.recordset[1].Description, textData[6], 'Description ok')
                browser.assert.equal(filtersData.recordset[1].SurveySID, 0, 'Type ok')
            }
            //needs to check if type not be "Site specific"
        })

        browser.end();
    }
}
