function SchedulingShiftsTypesController(gridId) {

    var self = this;

    var isRowAdding = false;
    var rowInEditMode = null;    

    // Determins if all actions should be disabled.
    this.IsActionsDisabled = function() {
        // if there is row in edit mode no other actions should be allowed.
        return isInEditMode();
    };
    
    this.editRow = function () {
        
        if (self.IsActionsDisabled()) return;
        
        var row = getSelectedRow();
        enterEditMode(row);
    };

    this.addNewRow = function (isExclusive) {

        if (self.IsActionsDisabled()) return;

        var row = new Array(-1, "", isExclusive);

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
        if (self.IsActionsDisabled()) return;
        if (args.get_type() == "cell") {
            self.editRow();
        }
    };
    /*Handles grid's 'TemplateClosing' event. 
    Uses to prevent template closing if focus goes away from template. */
    this.templateClosingHandler = function (sender, args) {        
        args.set_cancel(true);
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
        /*activate the row, otherwise more than one row can be selected.*/
        row.get_grid().get_behaviors().get_activation().set_activeCell(row.get_cell(0));
        /*select the row*/
        row.get_grid().get_behaviors().get_selection().get_selectedRows().add(row);        
    }
}

var shiftTypesTemplateBinder = new function () {

    var templateControlId;
    this.Init = function (templateControlClientId) {

        templateControlId = templateControlClientId;
    };

    this.getValue = function (columnKey) {
        
        if (columnKey == "Name") {
            return $get(templateControlId + '_tbShiftTypeName').value;
        }
        else if (columnKey == "ColorName") {
            return $get(templateControlId + '_ddlColor').value;
        }        
        return "";
    };
    
    this.setValue = function (columnKey, value) {
        
        if (columnKey == "Name") {
            $get(templateControlId + '_tbShiftTypeName').value = value;
        }
        else if (columnKey == "ColorName") {

            var ddlColor = $get(templateControlId + '_ddlColor');

            (value == null || value == "") ? ddlColor.selectedIndex = 0 : ddlColor.value = value;
            
            eval(ddlColor.attributes["onchange"].value);
        }                
    };
};

