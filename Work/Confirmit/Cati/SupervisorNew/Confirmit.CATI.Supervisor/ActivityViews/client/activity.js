function ActivityViews() {
}

ActivityViews.showContextMenuForActivityView = function (evnt, viewName) {
    var menuId = ContextMenus[viewName];
    if (menuId != null)
        showContextMenu(evnt, menuId);
}

ActivityViews.exportView = function (btnExportId) {
    var btnExport = document.getElementById(btnExportId);
    if (btnExport != null) {
        btnExport.click();
    }
};

ActivityViews.showHelp = function (helpPageUrl) {
    GetWM().openSingleWindow(helpPageUrl, '', 640, 580);
};

ActivityViews.subscribeForContextMenu = function(viewName) {
    var unselectedRows = Y.all('.hierarchical-grid > tbody > tr');
    if (unselectedRows) {
        unselectedRows.on('contextmenu',
            function (evt) {
                evt.preventDefault();
                return false;
            });
    }
    var selectedRow = Y.all('.hierarchical-grid > tbody > tr.tableRowSelectedCell');
    if (selectedRow) {
        selectedRow.detach('contextmenu').on('contextmenu',
            function (evt) { ActivityViews.showContextMenuForActivityView(evt, viewName) });
    }
};

function MessageSender(dialogTitle, dialogUrl) {
    this.title = dialogTitle;
    this.dialogUrl = dialogUrl;        
}

MessageSender.prototype.sendMessage = function(argParams) {
    var settings = { height: "375px", width: "600px", top: "100px" };
    top.overlay.show(this.title, this.dialogUrl + "?" + argParams, null, settings, null);
    return top.overlay;
};

// Progress indicator
Y.on("domready", function () {
    if (window.Sys && Sys.WebForms.PageRequestManager.getInstance()) {
        var prm = Sys.WebForms.PageRequestManager.getInstance();

        prm.add_initializeRequest(onInitializeRequest);
        prm.add_endRequest(onEndRequest);
        prm.add_pageLoaded(onPageLoaded);

        setTimeout(getIndicator, 10);
    }

    var progressImage =
        {
            src: "../svgimages/color_spinner.svg",
            title: "Refreshing data"
        };

    var okImage =
        {
            src: "../svgimages/checked_green.svg",
            title: "Done"
        };

    var errorImage =
        {
            src: "../svgimages/error_red.svg",
            title: "Communication error"
        };

    function getIndicator() {
        var indicator = Y.one("#activityProgressIndicator");
        if (!indicator) {
            indicator = Y.Node.create("<img id='activityProgressIndicator' src='" + okImage.src + "' title='" + okImage.title + "' border='0'></img>");
            Y.one("#activity-progress-placeholder").insert (indicator, "replace");
        }
        return indicator;
    }

    function onEndRequest(sender, args) {
        if (args.get_error()) {
            Y.log(args.get_error().message, "warn");
            getIndicator().setAttribute("src", errorImage.src).setAttribute("title", errorImage.title);
            args.set_errorHandled(true);
        }
    }

    function onPageLoaded(sender, args) {
        getIndicator().setAttribute("src", okImage.src).setAttribute("title", okImage.title);
    }

    function onInitializeRequest(sender, args) {
        document.getElementById("activity-progress-placeholder").innerHTML = '<svg class="comd-icon" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 66 66" width="24" height="24"><circle class="comd-icon-spinner" fill="none" stroke-width="6" stroke-linecap="round" cx="33" cy="33" r="30"></circle></svg>';
    }
});
