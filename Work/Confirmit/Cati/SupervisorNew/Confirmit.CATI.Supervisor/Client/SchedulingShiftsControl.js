function SchedulingShiftsController(gridId) {

    var self = this;
    
    var isRowAdding = false;
    var rowInEditMode = null;

    Y.on("HierarchicalGridControlInitialized", function (sender) {
        self.InitializeHandler(sender);
    });

    // Determins if all actions should be disabled.
    this.isActionsDisabled = function() {
        // if there is row in edit mode no other actions should be allowed.
        return isInEditMode();
    };

    this.editRow = function () {
        
        if (self.isActionsDisabled()) return;
        
        var row = getSelectedRow();
        enterEditMode(row);
    };

    this.addNewRow = function () {
        
        if (self.isActionsDisabled()) return;

        var row = new Array(null, "");

        getTopGrid().get_rows().add(row);
    };

    /*Occurs when new row is added to the grid.*/
    this.newRowAddedEventHandler = function (sender, args) {

        isRowAdding = true;
        var addedRow = args.get_row();
        selectRow(addedRow);
        enterEditMode(addedRow);
    };

    this.closeTemplate = function (saveChanges) {
        
        var editTemplate = getEditTemplate(rowInEditMode);
        editTemplate.exitEditMode(saveChanges);
        if (saveChanges) {
            rowInEditMode.get_grid().get_behaviors().get_editingCore().commit();
        }
        else {
            
            if (isRowAdding) {
                
                rowInEditMode.get_grid().get_rows().remove(rowInEditMode);
                /*there is no way to remove row without postback!*/
                rowInEditMode.get_grid().get_behaviors().get_editingCore().commit();
            }
        }
        isRowAdding = false;
        rowInEditMode = null;
    };

    this.doubleClickHandler = function (sender, args) {
        if (self.isActionsDisabled()) return;
        if (args.get_type() == "cell") {
            self.editRow();
        }
    };
    /*Handles grid's 'TemplateClosing' event. 
    Uses to prevent template closing if focus goes away from template. */
    this.templateClosingHandler = function (sender, args) {        
        args.set_cancel(true);
    };

    /*Occurs when grid is initialized. Acctually occurs as for parent grid and as for each sub-grids.
    Here we go through all rows and hide collapse/expand indicator for rows that have no children.*/
    this.InitializeHandler = function (sender, args) {        
        rowInEditMode = null;        
    };

    this.setDefault = function () {
        
        if (self.isActionsDisabled()) return false;

        var row = getSelectedRow();

        if (row != null) {

            var hasRespondentTimeZone = row.get_cellByColumnKey('HasRespondentTimeZone').get_value();
            if (hasRespondentTimeZone == false) {
                alert('Selected shift has not respondent timezone data and can not be set to default');
                return false;
            }
            else {
                return confirm('Do you want to set default value for this shift in the current timezone?');
            }
        }
        return true;
    };
   
    function isInEditMode() {
       
        return (rowInEditMode != null);
    }
    
    function getTopGrid() {

        return $find(gridId).get_gridView();
    }

    function getEditTemplate(row) {

        return row._owner.get_behaviors().get_editingCore().get_behaviors().get_rowEditingTemplate();
    }

    function enterEditMode(row) {
        
        var editTemplate = row._owner.get_behaviors().get_editingCore().get_behaviors().get_rowEditingTemplate();

        if (editTemplate != undefined) {
            
            rowInEditMode = getSelectedRow();
            editTemplate.enterEditMode(row);            
        }
    }

    function getSelectedRow() {

        var selectedRows = getTopGrid().get_behaviors().get_selection().get_selectedRowsResolved();
        if (selectedRows.length > 0)
            return getTopGrid().get_behaviors().get_selection().get_selectedRowsResolved()[0];
        
        return null;
    }

   
    /*Clears all selected rows, and selects the passed row.*/
    function selectRow(row) {
        
        var selectedRowsCollections = row.get_grid().get_behaviors().get_selection().get_selectedRowsCollections();
        for (var i = 0; i < selectedRowsCollections.length; i++) {
            selectedRowsCollections[i].clear();
        }
        //activate the row, otherwise more than one row can be selected.
        row.get_grid().get_behaviors().get_activation().set_activeCell(row.get_cell(0));
        //select the row
        row.get_grid().get_behaviors().get_selection().get_selectedRows().add(row);        
    }
}

var shiftsTemplateBinder = new function () {

    var TemplateType = { Shifts: 0, Exclusions: 1 };

    var templateControlId;
    var templateType = TemplateType.Shifts;

    this.Init = function (templateControlClientId) {

        templateControlId = templateControlClientId;
        templateType = $get(templateControlId + '_hfTemplateType').value;
    };

    this.getValue = function (columnKey) {
        if (columnKey == "Id") {
            return $get(templateControlId + '_hfRowId').value;
        }
        else if (columnKey == "ShiftTypeId") {
            return $get(templateControlId + '_ddlShiftType').value;
        }
        else if (columnKey == "StartDayName") {
            if (templateType == TemplateType.Exclusions) {

                var chooser = $IG.WebTextEditor.find(templateControlId + '_wdteStartDate');
                return chooser.get_text();
            }
            else {
                return $get(templateControlId + '_ddlStartDay').value;
            }
        }
        else if (columnKey == "StartTimeToString") {
            return $get(templateControlId + '_tbStartTime').value;
        }
        else if (columnKey == "EndDayName") {
            if (templateType == TemplateType.Exclusions) {

                var chooser = $IG.WebTextEditor.find(templateControlId + '_wdteEndDate');

                return chooser.get_text();

            }
            else {
                return $get(templateControlId + '_ddlEndDay').value;
            }

        }
        else if (columnKey == "EndTimeToString") {
            return $get(templateControlId + '_tbEndTime').value;
        }
        else if (columnKey == "HasRespondentTimeZone") {
            return $get(templateControlId + '_hfHasRespondentTimeZone').value;
        }
        return "";
    };
    this.setValue = function (columnKey, value) {
        if (columnKey == "Id") {
            $get(templateControlId + '_hfRowId').value = value;
        }
        else if (columnKey == "ShiftTypeId") {
            var control = $get(templateControlId + '_ddlShiftType');
            (value == "") ? control.selectedIndex = 0 : control.value = value;
        }
        else if (columnKey == "StartDayName") {
            
            if (templateType == TemplateType.Exclusions) {

                var chooser = $IG.WebTextEditor.find(templateControlId + '_wdteStartDate');

                if (chooser && value != "") {
                    chooser.set_text(value);
                }
            }
            else {

                var control = $get(templateControlId + '_ddlStartDay');
                (value == "") ? control.selectedIndex = 0 : control.value = value;
            }
        }
        else if (columnKey == "StartTimeToString") {
            $get(templateControlId + '_tbStartTime').value = value;
        }
        else if (columnKey == "EndDayName") {

            if (templateType == TemplateType.Exclusions) {

                var chooser = $IG.WebTextEditor.find(templateControlId + '_wdteEndDate');

                if (chooser && value != "") {
                    chooser.set_text(value);
                }
            }
            else {
                var control = $get(templateControlId + '_ddlEndDay');
                (value == "") ? control.selectedIndex = 0 : control.value = value;
            }
        }
        else if (columnKey == "EndTimeToString") {
            $get(templateControlId + '_tbEndTime').value = value;
        }
        else if (columnKey == "HasRespondentTimeZone") {
            $get(templateControlId + '_hfHasRespondentTimeZone').value = value;
        }
    };
};

