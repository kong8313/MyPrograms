function CatiReport(settings) {

    var self = this;
    this.reportViewer = null;

    Y.on("domready", function () {
        prepareBuildButtonToWork();
        prepareReportViewerToWork();

        Y.one(window).on("resize", setupHeight);

        if (typeof settings.ReportViewerClientId == "undefined" || !document.getElementById(settings.ReportViewerClientId)) return;

        self.reportViewer = eval(settings.ReportViewerClientId);

        if (self.reportViewer) {
            setupHeight();
            self.reportViewer.AdjustReportAreaHeight();
        }
    });

    function prepareBuildButtonToWork() {
        Y.one("#" + settings.BuildButtonClinetId).on("click", function () { enableBuildButton(false); });
    }

    function prepareReportViewerToWork() {
        var reportViewer = window[settings.ReportViewerClientId];
        
        if (!reportViewer) {
            if (settings.IsBuildButtonPressed) enableBuildButton(true);
            return;
        }

        var navigateReportFrameFunction = reportViewer.NavigateReportFrame;
        reportViewer.NavigateReportFrame = function () {
            enableBuildButton(false);
            return navigateReportFrameFunction.apply(reportViewer, arguments);
        };

        var navigateParametersFrameFunction = reportViewer.NavigateParametersFrame;
        reportViewer.NavigateParametersFrame = function () {
            enableBuildButton(false);
            return navigateParametersFrameFunction.apply(reportViewer, arguments);
        };

        var onReportLoadedFunction = reportViewer.OnReportLoaded;
        reportViewer.OnReportLoaded = function () {
            var result = onReportLoadedFunction.apply(reportViewer, arguments);
            enableBuildButton(true);
            return result;
        };

        if (Y.one("#" + reportViewer.waitControlID).get("offsetHeight") <= 0 &&
            Y.one("#" + reportViewer.waitControlID).get("offsetWidth") <= 0 &&
            settings.IsBuildButtonPressed
            ) {
            enableBuildButton(true);
        }
    }

    function enableBuildButton(enabled) {
        var buildReportButton = Y.one("#" + settings.BuildButtonClinetId);
        if (buildReportButton) buildReportButton.set("disabled", !enabled);
    }

    function setupHeight() {

        var reportViewerElement = Y.one("#" + settings.ReportViewerClientId);

        if (reportViewerElement) {

            var height = Y.one("body").get("winHeight") - Y.one("#" + settings.ReportPanelClientId).get("offsetTop") - 30;

            reportViewerElement.setStyle("height", height + "px");
        }
    }
}
