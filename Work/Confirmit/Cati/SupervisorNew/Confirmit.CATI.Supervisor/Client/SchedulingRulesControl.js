function SchedulingRulesController(settings, gridController) {

    var self = this;
    var foundRow = null;

    Y.on("HierarchicalGridControlInitialized", function (sender) {
        self.InitializeHandler(sender);
    });

    function getSchedulingScriptId() {
        var urlParams = new URLSearchParams(window.location.search);
        return urlParams.get('ID');
    }

    this.editRule = function () {

        var bandKey = gridController.getSelectedRow().get_grid().get_band().get_key().toLowerCase();

        if (bandKey == "") {
            eval(settings.EditRuleFunction);
        } else if (bandKey == "subrules") {
            eval(settings.EditSubRuleFunction);
        } else if (bandKey == "actions") {
            eval(settings.EditActionFunction);
        }
    };

    this.deleteRow = function () {
        var rowKey = gridController.getSelectedRowKey();
        var scriptId = getSchedulingScriptId();
        PageMethods.DeleteRow(rowKey, scriptId, onPageMethodSuccess, onPageMethodFailure);
    };

    this.enableAction = function (enabled) {

        var actionKey = gridController.getSelectedRowKey();
        var scriptId = getSchedulingScriptId();
        PageMethods.EnableAction(actionKey, enabled, scriptId, onPageMethodSuccess, onPageMethodFailure);
    };

    this.moveRow = function (moveUp) {

        var rowKey = gridController.getSelectedRowKey();
        var scriptId = getSchedulingScriptId();
        PageMethods.MoveRow(rowKey, moveUp, scriptId, onPageMethodSuccess, onPageMethodFailure);
    };

    this.copyRow = function () {
        var key = gridController.getSelectedRowKey();
        if (key) {
            Y.one("#" + settings.CopiedRowKey).set('value', key);
        }
    };

    this.pasteRow = function () {

        var copiedRowkey = Y.one("#" + settings.CopiedRowKey).get('value');
        var pasteRowKey = gridController.getSelectedRowKey();
        var scriptId = getSchedulingScriptId();

        if (copiedRowkey && pasteRowKey) {
            PageMethods.PasteRow(copiedRowkey, pasteRowKey, scriptId, onPageMethodSuccess, onPageMethodFailure);
        }

        return false;
    };

    function onPageMethodSuccess(result) {

        if (result.Success === true) {

            if (result.HighlightRowKey) {
                gridController.writeSelectedRowKey(result.HighlightRowKey);
            }

            eval(settings.UpdateAndMarkAsChangedFunction);

        } else {
            alert(result.ErrorMessage);
        }
    }

    function onPageMethodFailure(result) {
        alert('Operation failed');
        Y.log(result);
    }

    this.getParentId = function () {

        var bandKey = gridController.getSelectedRow().get_grid().get_band().get_key().toLowerCase();
        var parentRow = gridController.getSelectedRow().get_grid().get_parentRow();

        if (bandKey == "subrules") {
            return parentRow.get_cellByColumnKey("Id").get_value();
        } else if (bandKey == "actions") {
            return parentRow.get_cellByColumnKey("Id").get_value();
        }

        return null;
    };

    this.getGrandParentIdForNewRow = function (rowType) {

        var bandKey = gridController.getSelectedRow().get_grid().get_band().get_key().toLowerCase();
        var parentRow = gridController.getSelectedRow().get_grid().get_parentRow();

        if (rowType == "action") {
            if (bandKey == "actions") {
                return parentRow.get_grid().get_parentRow().get_cellByColumnKey("Id").get_value();
            } else if (bandKey == "subrules") {
                return parentRow.get_cellByColumnKey("Id").get_value();
            }
        }

        return null;
    };

    this.getGrandParentId = function () {
        var bandKey = gridController.getSelectedRow().get_grid().get_band().get_key().toLowerCase();
        var parentRow = gridController.getSelectedRow().get_grid().get_parentRow();

        if (bandKey == "actions") {
            return parentRow.get_grid().get_parentRow().get_cellByColumnKey("Id").get_value();
        }

        return null;
    };

    this.getIdForNewRow = function (rowType) {
        var bandKey = gridController.getSelectedRow().get_grid().get_band().get_key().toLowerCase();
        var parentRow = gridController.getSelectedRow().get_grid().get_parentRow();
        var row = gridController.getSelectedRow();

        if (bandKey == "") {
            return row.get_cellByColumnKey("Id").get_value();
        } else if (bandKey == "subrules") {

            if (rowType == "action") {
                return row.get_cellByColumnKey("Id").get_value();
            } else if (rowType == "subrule") {
                return parentRow.get_cellByColumnKey("Id").get_value();
            }

        } else if (bandKey == "actions") {
            return parentRow.get_cellByColumnKey("Id").get_value();
        }

        return null;
    };

    /*Occurs when grid is initialized. Acctually occurs as for parent grid and as for each sub-grids.
    Here we go through all rows and hide collapse/expand indicator for rows that have no children.*/
    this.InitializeHandler = function (sender, args) {

        foundRow = null;

        var grid = sender;
        var rows = grid.get_rows();

        if (rows.get_length() == 1) {
            this.ExpandAll();
        }

        SetExpandIndicatorForRowsWithChildren(rows);
    };

    this.ExportClick = function () {

        var btnExport = $get(settings.ExportButtonClientId);
        if (btnExport) {
            btnExport.click();
        }
    };

    this.ExpandAll = function () {
        gridController.ExpandAll();
    };

    this.CollapseAll = function () {
        gridController.CollapseAll();
    };

    this.validateAndSaveSubRule = function (ddlShiftTypeID) {

        var ddlShiftType = document.getElementById(ddlShiftTypeID);
        if (ddlShiftType.selectedIndex < 0) {
            alert('<%=GetResString("Shift type should be selected")%>');
            return;
        }
        this.closeTemplate(true);
    };

    function SetExpandIndicatorForRowsWithChildren(rows) {
        for (var i = 0; i < rows.get_length(); i++) {
            var row = rows.get_row(i);

            if (row._expColBtn) {
                var element = row._expColBtn.get_element();
                if (element)
                    element.style.visibility = "visible";
            }
            if (row.get_rowIslands().length > 0) {
                var childrenRows = row.get_rowIslands()[0].get_rows();
                if (childrenRows.get_length() > 0) {

                    if (row._expColBtn) {
                        var element = row._expColBtn.get_element();
                        if (element)
                            element.style.visibility = "visible";
                    }

                    SetExpandIndicatorForRowsWithChildren(childrenRows);
                }
            }
        }
    }

    /* Function allow find specified text into grid
       tbSearch.value is specified text into this column
       ddlSearch.value is specified column into grid */
    this.findNext = function () {

        var ddlSearch = document.getElementById(settings.SeachDropDownClientId);
        var tbSearch = document.getElementById(settings.SearchTextBoxClientId);

        try {
            var regExp = new RegExp(tbSearch.value, "i");
        }
        catch (error) {
            alert("Search text is incorrect");
            return;
        }

        var row = gridController.findRow(regExp, ddlSearch.value, foundRow);

        if (row == null) {

            alert("Value not found");

            foundRow = null;
            gridController.selectFirstRow();

            return;
        }

        foundRow = row;
        gridController.selectRow(row);
        gridController.expandRowParents(row);
    };
}