function _stateChecker(settings) {
    var self = this;

    this.IsChanged = false;

    this.MarkAsChanged = function () {
        if (settings.disabled)
            return;

        $get(settings.pageStateId).value = "True";
        self.IsChanged = true;

        top.setChanged(true, settings.pageStateId);

        for (var i = 0; i < settings.saveButtonIds.length; i++) {
            var id = settings.saveButtonIds[i];
            replaceImgSrc(id, true);
        }
    };

    this.MarkAsUnchanged = function () {
        if (settings.disabled)
            return;

        $get(settings.pageStateId).value = "False";
        self.IsChanged = false;

        top.setChanged(false, settings.pageStateId);

        for (var i = 0; i < settings.saveButtonIds.length; i++) {
            var id = settings.saveButtonIds[i];
            replaceImgSrc(id, false);
        }
    };

    this.BeforeSubmit = function () {
        self.IsChanged = false;
        top.skipBeforeUnload = true;
    };

    if (!settings.disabled) {
        Y.on("load", function () {

            self.IsChanged = $get(settings.pageStateId).value === "True";
            top.setChanged(self.IsChanged, settings.pageStateId);
            top.skipBeforeUnload = false;

            if (self.IsChanged) {
                self.MarkAsChanged();
            }
            else {
                self.MarkAsUnchanged();
            }

            if (settings.automaticallySubscribeOnChangeEvents) {
                Y.all("select, input[type='file'], input[type='range']").on("change", self.MarkAsChanged);
                Y.all("input[type='radio'], input[type='checkbox']").on("click", self.MarkAsChanged);
                Y.all("textarea, input[type='password'], input[type='search'], input[type='text']").on("valueChange", self.MarkAsChanged);
            }
        });

        var beforeUnloadEventHandler = function (e) {
            if (top.isDataChanged() && !top.skipBeforeUnload) {
                e.returnValue = "You will lose all your changes made since last save";
                e.preventDefault();
            }
        };

        if (settings.showBeforeUnloadWarning) {
            if (window.parent.frameElement && window.parent.frameElement.id === "infoFrame") {
                if (!window.parent.catiBeforeUnloadHandler) {
                    window.parent.catiBeforeUnloadHandler = Y.on("beforeunload", beforeUnloadEventHandler);
                }
            }
            else if (!window.catiBeforeUnloadHandler) {
                window.catiBeforeUnloadHandler = Y.on("beforeunload", beforeUnloadEventHandler);
            }
        }
    }

    function replaceImgSrc(parentNodeId, isChanged) {
        var node = Y.one('#' + parentNodeId);
        if (!node) {
            return;
        }

        if (isChanged) {
            node.addClass("save-icon-blinking");
        } else {
            node.removeClass("save-icon-blinking");
        }
    }
}

//---------------------------------------------------------------------------
function GetWM() {
    return getTopCPWindow().wm;
}

//---------------------------------------------------------------------------
function prepareParamsForDialog(sPath, sParams) {
    var res = new Object();
    res.Path = sPath;
    res.Params = new Array();
    var arr = sParams.split("&");
    for (var i = 0; i < arr.length; i++) {
        var prmInfo = arr[i].split("=");
        var nLength = res.Params.length;
        res.Params[nLength] = new Object();
        res.Params[nLength].name = prmInfo[0];
        res.Params[nLength].value = prmInfo[1];
    }
    return (res);
}

//---------------------------------------------------------------------------
function DisablePopupMenu() {
    document.oncontextmenu = function () { return false; };
}

function getTopCPWindow() {
    var wnd = top.window;
    do {
        if (wnd.top.topCatiWindow) {
            return wnd.top;
        }
        wnd = wnd.opener;
    }
    while (wnd);

    return undefined;
}

function goToTemplates() {
    getTopCPWindow().catiGoTo.jumpToReportTemplatesCustomization();
}

/// Executes on each page load to keep Confirmit session alive.
/// Works only if time from last call is more then 1 minute.
function stayAlive(confirmitKeepSessionAspxUrl) {
    try {
        var topWindow = getTopCPWindow();
        var event = new Event('custom_pageLoad');
        document.documentElement.dispatchEvent(event);
        setTimeout(function () {
            if (topWindow &&
                (topWindow.lastRefreshTime === undefined || (new Date().getTime() - topWindow.lastRefreshTime) > 60 * 1000)) {
                if (topWindow.setSessionFrameUrl) {
                    topWindow.setSessionFrameUrl(confirmitKeepSessionAspxUrl);
                    topWindow.lastRefreshTime = new Date().getTime(); // total milliseconds.
                }
            }
        },
            2000); // wait 2 seconds before loading KeepSession pages, because synchonous execution may slow down page loading.
    }
    catch (e) { }
}

// Used for resizing pop-up windows
function resizeWindow(width, height) {
    window.dialogHeight = height + 'px';
    window.dialogWidth = width + 'px';
}

function Common() {
}

// Updates Microsoft UpdatePanel
Common.updatePanel = function (updatePanelId) {
    __doPostBack(updatePanelId, "");
};

Common.refreshListFrame = function () {
    if (window.refreshListFrame) {
        window.refreshListFrame();
        return;
    }
    if (parent.window.refreshListFrame) {
        parent.window.refreshListFrame();
        return;
    }
    if (parent.parent.window.refreshListFrame) {
        parent.parent.window.refreshListFrame();
        return;
    }
};

Common.refreshInfoFrame = function () {
    if (window.refreshInfoFrame) {
        window.refreshInfoFrame();
        return;
    }
    if (parent.window.refreshInfoFrame) {
        parent.window.refreshInfoFrame();
        return;
    }
};

Common.setTitle = function (title) {
    try {
        var topWindow = getTopCPWindow();
        if (topWindow && topWindow.$get("listFrame") && topWindow.$get("listFrame").contentWindow.location === window.location) {
            topWindow.setTitle(title);
        }
    }
    catch (e) { }
};

Common.fireGlobalEvent = function (eventName, params) {
    var topWindow = getTopCPWindow();
    topWindow.Y.fire(eventName, params);
};

Common.onGlobalEvent = function (eventName, eventHandler) {
    var topWindow = getTopCPWindow();
    if (topWindow == undefined) {
        return;
    }

    topWindow.Y.on(eventName, eventHandler);

    // because event subscribers can be in different frames that may be unloaded / reloaded separately from top window - we have to detach subrcribers in unloading frames.
    Y.on('unload', function () {
        topWindow.Y.detach(eventName, eventHandler);
    });
};

Common.validateRequiredValue = function (controlId, errorMessage) {
    var control = document.getElementById(controlId);
    var value = control.value;
    if (value.trim() == "") {
        alert(errorMessage);
        control.focus();
        return false;
    }
    return true;
};

Common._setProcessingState = function (isProcessing) {
    if (window.Sys) {
        var manager = Sys.WebForms.PageRequestManager.getInstance();
        if (manager && manager._postBackSettings && manager._postBackSettings.async)
            return;
    }

    if (isProcessing && typeof Page_Validators != "undefined" && Y.Array.some(Page_Validators, function (v) { return !v.isvalid; }))
        return;

    if (window.changeProcessingState)
        window.changeProcessingState(isProcessing);

    if (parent.changeProcessingState)
        parent.changeProcessingState(isProcessing);
};

Common._setFocus = function () {
    var nodes = Y.all("input[type='text'], textarea");
    for (var j = 0; j < nodes.size(); j++) {
        var node = nodes.item(j);

        if (node.get("readOnly") || node.get("disabled"))
            continue;

        node.focus();
        if (document.activeElement &&
            document.activeElement.id == node.get("id")) {
            return;
        }
    }

    nodes = Y.all("input, select");
    for (j = 0; j < nodes.size(); j++) {
        node = nodes.item(j);
        node.focus();
        if (document.activeElement &&
            document.activeElement.id == node.get("id")) {
            return;
        }
    }

    Y.one("form").focus();
};

Common._disablePopupMenu = function () {
    document.oncontextmenu = function () { return false; };
};

Common._reportViewerPrintClicked = false;

Common._removeFormTargetAttribute = function () {
    //debugger;
    if (Common._reportViewerPrintClicked == false) {
        //Y.one("form").removeAttribute("target");
    }

    Common._reportViewerPrintClicked = false;
};

Common._disableAutoFocus = false;

Y.on('load', function () {
    // Hack - If a page has IG controls - it renders additional text nodes on the page.
    // In Quirk mode these text nodes becomes visible after 2 async postbacks.
    // We cannot use YUI here because its CSS celectors ignore text nodes
    if (typeof theForm != "undefined") {
        var nodes = theForm.childNodes;

        for (var i = nodes.length - 1; i >= 0; i--) {
            var node = nodes[i];
            if (node && node.nodeType && node.nodeType == 3 /*text node*/) {
                if (node.nodeValue == "" || node.nodeValue == " ") {
                    theForm.removeChild(node);
                }
            }
        }
    }

    Common._setProcessingState(false);
    if (!Common._disableAutoFocus)
        Common._setFocus();
});

function showContextMenu(oEvent, menuId) {
    var menu = $find(menuId);
    // show context menu
    if (menu != null) {
        // clear previously selected item.
        if (menu.get_selectedItem()) {
            menu.get_selectedItem().set_selected(false);
        }

        // first open menu hidden to add ability to calculate menu height
        menu.set_visible(false);
        menu.showAt(1, 1);

        var yPosition;
        if (document.body.clientHeight - oEvent.clientY >= menu.get_element().clientHeight) {
            yPosition = oEvent.clientY;
        }
        else {
            yPosition = oEvent.clientY - menu.get_element().clientHeight;
        }

        if (yPosition < 0) {
            yPosition = document.body.clientHeight - menu.get_element().clientHeight - 5; // decrease yPosition on 5px to make it not cling bottom border
        }

        var xPosition;
        if (document.body.clientWidth - oEvent.clientX >= menu.get_element().clientWidth) {
            xPosition = oEvent.clientX;
        }
        else {
            xPosition = oEvent.clientX - menu.get_element().clientWidth;
        }

        // now show menu visible in proper place
        menu.showAt(xPosition, yPosition);
        menu.set_visible(true);
    }
}

function hideContextMenu(menuId) {
    /*For some reason context menu is not automatically closed 
      after click on a context item. Only IE is afflicted.*/
    var menu = $find(menuId);

    if (menu != null) {
        menu.hide();
    }
}

var ContextMenus = new Array();

AddContextMenu = function (key, menuClientId) {
    ContextMenus[key] = menuClientId;
};


Common._startupScript = function () {
    try {
        // might be an IE8+ compatibility mode.
        if ($util.IsIE7) {
            $util.IsIE7 = false;
            $util.IsIE8 = true;
            $util.IsIEStandards = true;
        }
    } catch (e) {
    }

    if (window.$IG && window.$IG.WebDataMenu) {
        // Override IG menu function to avoid menu closing on a parent menu item click;
        $IG.WebDataMenu.prototype.__closeMenuOnClick = function (itemClicked) {
            if ($util.isNullOrUndefined(itemClicked))
                return;

            if (itemClicked.get_level() > 0) // applys only to non root items.
            {
                if (this.__closeOnClick && !itemClicked.hasChildren()) {
                    if (this.get_isContextMenu()) {
                        this.hide();
                    } else {
                        this.__hideAll();
                    }
                }
            } else {
                if (this.get_isContextMenu() && !itemClicked.hasChildren()) {
                    // we clicked on a root item of a context menu that do not have children -> close the menu.
                    this.hide();
                }
            }
        };
    }
};

/*
  This function is used in Activity views and Reports.
  SourceList specifies source page where this function has been called from.
  PanelId specifies identifier of update panel. Used to make asynchronous update in ActivityView.
*/
Common.selectSurveys = function (panelId, sourceList, selectedSurveyId, postbackReference, url) {

    var settings = { height: "620px", width: "980px", top: "80px", calledWindow: window, OpenSource: 1 };

    var params = { SourceList: sourceList, SelectedId: selectedSurveyId };

    var overlay = top.overlay;

    overlay.overlayClosedEvent.on(function (args) {
        if (args.result !== true)
            return;

        if (postbackReference)
            eval(postbackReference);
        else if (panelId)
            Common.updatePanel(panelId);

    });

    overlay.show("Select Surveys", url, params, settings, null);

    return overlay;
};

Common.selectGroupsInterviewers = function (panelId) {

    var settings = { height: "520px", width: "780px", top: "80px", calledWindow: window };

    top.overlay.overlayClosedEvent.on(function (args) {
        if (args.result !== true)
            return;

        if (panelId)
            Common.updatePanel(panelId);
    });

    top.overlay.show("Select Groups And Interviewers", "SurveysInterviewersSelection/GroupsInterviewersSelectionPage.aspx", null, settings, null);

    return top.overlay;
};

Common.selectInterviewers = function (panelId, sourceList, selectedInerviewerId, postbackReference, url) {

    var settings = { height: "620px", width: "880px", top: "80px", calledWindow: window, OpenSource: 1 };

    var params = { SourceList: sourceList, SelectedId: selectedInerviewerId };

    var overlay = top.overlay;

    overlay.overlayClosedEvent.on(function (args) {
        if (args.result !== true)
            return;

        if (postbackReference)
            eval(postbackReference);
        else if (panelId)
            Common.updatePanel(panelId);
    });

    overlay.show("Select Interviewers/Groups", url, params, settings, null);

    return overlay;
};

Common.selectShiftForReport = function (panelId, sourceList) {

    var settings = { height: "320px", width: "420px", top: "80px", calledWindow: window };
    var params = { SourceList: sourceList };

    top.overlay.overlayClosedEvent.on(function (args) {
        if (args.result !== true)
            return;

        if (panelId)
            Common.updatePanel(panelId);
    });

    top.overlay.show("Select Shift", "Reports/SelectShiftForReport.aspx", params, settings, null);

    return top.overlay;
};

String.prototype.format = function () {
    var result = this;

    for (var i = 0; i < arguments.length; i++) {
        result = result.replace('{' + i + '}', arguments[i]);
    }

    return result;
};

/*
 This is an override of print function for telerik reports to fix bug with printing in Chrome.
 The core of this code was copied from offitial workaround from https://docs.telerik.com/reporting/knowledge-base/print-error-chrome page
 See "Workaround for the obsolete ASP.NET Web Forms Report Viewer" section
 It can be removed after upgrade to Telerik 13.2 or higher
 */
Y.on("load", function () {
    if (typeof ReportViewer !== "undefined") {
        ReportViewer.prototype.PrintReport = function () {
            switch (this.defaultPrintFormat) {
                case "Default":
                    this.DefaultPrint();
                    break;
                case "PDF":
                    this.PrintAs("PDF");
                    previewFrame = document.getElementById(this.previewFrameID);
                    previewFrame.onload = function () { previewFrame.contentDocument.execCommand("print", true, null); };
                    break;
            }
        };
    }
});