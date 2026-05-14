function HierarchicalGridControl(settings) {

    var self = this;
    
    var Menus = new Array();

    this.AddMenu = function (bandKey, menuClientId) {
        Menus[bandKey] = menuClientId;
    };

    this.ShowContextMenu = function (sender, args) {

        args.set_cancel(true);

        if (args.get_type() == "cell") {

            var row = args.get_item().get_row();

            self.selectRow(row);

            var menuId = getContextMenuId(sender);

            hideOpenContextMenus();
            showContextMenu(args.get_browserEvent(), menuId);
        }
    };

    this.InitializeGridHandler = function (sender, args) {

        WriteExpandedRowsKeys();

        Y.fire("HierarchicalGridControlInitialized", sender);

        fixScrollJumpingOnSecondPostBackBug();
    };
    
    this.getSelectedRow = function() {

        var selectedRows = getTopGrid().get_behaviors().get_selection().get_selectedRowsResolved();
        if (selectedRows.length > 0)
            return selectedRows[0];

        return null;
    };

    this.getSelectedRowKey = function () {

        var selectedRow = this.getSelectedRow();

        if (selectedRow) {
            return getRowKey(selectedRow);
        }

        return null;
    };
    
    /*Clears all selected rows, and selects the passed row.*/
    this.selectRow = function (row) {

        var selectedRowsCollections = getTopGrid().get_behaviors().get_selection().get_selectedRowsCollections();
        for (var i = 0; i < selectedRowsCollections.length; i++) {
            selectedRowsCollections[i].clear();
        }

        /*activate the row, otherwise more than one row can be selected.*/
        row.get_grid().get_behaviors().get_activation().set_activeCell(row.get_cell(0));

        row.get_grid().get_behaviors().get_selection().get_selectedRows().add(row);

        writeSelectedRow(row);
    };

    this.selectFirstRow = function () {

        var firstRow = getTopGrid().get_rows().get_row(0);
        if (firstRow) {
            this.selectRow(firstRow);
        }
    };

    this.onRowSelectionChanged = function (sender, args) {
        var selectedRow = args.getSelectedRows().getItem(0);
        writeSelectedRow(selectedRow);
    };

    this.writeSelectedRowKey = function (rowKey) {
        Y.one("#" + settings.hHighlightedId).set('value', rowKey);
    }

    function writeSelectedRow(row) {        
        if (row) {
            var rowKey = getRowKey(row);
            Y.one("#" + settings.hHighlightedId).set('value', rowKey);            
        }        
    }

    function getRowKey(currentRow) {
        var key = "";
        var row = currentRow;

        do {
                      
            var currentRowKey = row.get_dataKey().toString();           

            key = (key != "") ? currentRowKey + "_" + key : currentRowKey;
            
            row = row.get_grid().get_parentRow();
        }
        while (row != null)

        return key;
    }
    
    /*Expands all parents of the passed row, so the row will be visible for the user. Works for 3 levels.*/
    this.expandRowParents = function (row) {

        var parentRow = row.get_grid().get_parentRow();
        
        if (parentRow) {

            parentRow.set_expanded(true);
            
            var parentOfParentRow = parentRow.get_grid().get_parentRow();
            if (parentOfParentRow) {
                 parentRow.set_expanded(true);
            }
        }
   };

     /*Finds grid's cell containing value fulfilling the condition specified by regex object. */
   this.findRow = function (regExp, searchType, foundRow) {

       var grid = getTopGrid();

       var activeRow = this.getSelectedRow();
       if (!activeRow) {
           activeRow = grid.get_rows().get_row(0);
       }

       if (activeRow == null)
           return null;


       var nextRow = activeRow;

       do {
           
           if (nextRow != foundRow) {
               if (isRowMatch(nextRow, regExp, searchType)) {
                   return nextRow;
               }
           }

           nextRow = getNextRow(nextRow);

       } while (nextRow != null)

       return null;
   };

   this.onClick = function (sender, args) {
       hideOpenContextMenus();
   };

    this.onDoubleClick = function (sender, args) {
       if (args.get_type() == 'cell') {
           eval(settings.DoubleClickCommand);
       }
   };

    this.ExpandAll = function () {
       setAllExpanded(getTopGrid().get_rows(), true);
       onExpandCollapse();
   };

    this.CollapseAll = function () {
        setAllExpanded(getTopGrid().get_rows(), false);
        onExpandCollapse();
    };

    this.RowExpandedHandler = function (sender, args) {
        onExpandCollapse();
    };

    this.RowCollapsedHandler = function (sender, args) {
        onExpandCollapse();
    };

    function onExpandCollapse() {
        hideOpenContextMenus();
        WriteExpandedRowsKeys();
    }

    function WriteExpandedRowsKeys() {
        
        var keys = [];
        fillExpandedRows(getTopGrid().get_rows(), keys);
        Y.one("#" + settings.hExpandedRowsId).set('value', keys.join(","));
    } 

    function fillExpandedRows(rows, keys) {
        
        for (var i = 0; i < rows.get_length(); i++) {

            var row = rows.get_row(i);
            /*Method get_RowIsland is undefined  for row that has been just added on the client side. 
             Function get_rowIslands is not available for such rows.*/
            if (row._rowIslands != undefined && row.get_rowIslands().length > 0) {

                var childrenRows = row.get_rowIslands()[0].get_rows();

                if (childrenRows.get_length() > 0) {

                    if(row.get_expanded()) {
                        keys.push(getRowKey(row));
                        fillExpandedRows(childrenRows, keys);
                    }
                }
            }
        }
    }
    
    function setAllExpanded(rows, expanded) {

        for (var i = 0; i < rows.get_length(); i++) {

            var row = rows.get_row(i);            

            if (row.get_rowIslands().length > 0) {

                var childrenRows = row.get_rowIslands()[0].get_rows();

                if (childrenRows.get_length() > 0) {
                                        
                    row.set_expanded(expanded);                    
                    setAllExpanded(childrenRows, expanded);
                }
            }
        }
    }

    /*Gets the next sibling for the passed row.*/
    function getNextRow(row) {
                
        /*At first go through all children. */
        if (row.get_rowIslands().length > 0) {

            var childrenRows = row.get_rowIslands()[0].get_rows();

            if (childrenRows.get_length() > 0) {

                return childrenRows.get_row(0);
            }
        }

        var index = row.get_index();
        var rows = row.get_grid().get_rows();

        if (index + 1 < rows.get_length()) {
            return rows.get_row(index + 1);
        }

        /*At second go down through all siblings of parent of passed row. */
        
        var parentRow = row.get_grid().get_parentRow();

        if (parentRow) {
            var parentRowIndex = parentRow.get_index();
            var parentRows = parentRow.get_grid().get_rows();

            if (parentRowIndex + 1 < parentRows.get_length()) {
                return parentRows.get_row(parentRowIndex + 1);
            }
            else {
                /*go on one parent level above */
                parentRow = parentRow.get_grid().get_parentRow();

                if (parentRow) {

                    parentRowIndex = parentRow.get_index();
                    parentRows = parentRow.get_grid().get_rows();

                    if (parentRowIndex + 1 < parentRows.get_length()) {
                        return parentRows.get_row(parentRowIndex + 1);
                    }
                }
            }
        }
        
        return null;
    }
      
    function isRowMatch(row, regEx, searchType) {
        
        for (var i = 0; i < row.get_cellCount(); i++) {
            var cell = row.get_cell(i);
            if (regEx.test(cell.get_value())) {
                if (searchType == "" ||
                    cell.get_column().get_key() == searchType) {
                    return true;
                }                
            }
                
        }

        return false;            
    }

    function getTopGrid() {

        return $find(settings.GridId).get_gridView();
    }

    /* IG bug fix description
         1. On second postback scroll jumps to the top: 
             a) IG maintains clientState on client
             b) Before submit this client state is serialized and sent to server, see method _onSubmitOtherHandler 
             c) The record on client side is added only when the scroll position has been changed
             d) The the client state comes from server the correct scroll position is setup but client record is not added
             e) On before submit stage of second postback there is no record about scroll postion and no information is sent to server
             g) Server doesn't sent information about scroll position
         2. Keep in mind that clientState has following feature
             a) If position has been changed the record is added
             b) If position has been returned back to initial state the record is removed
             c) There is default value, if position is changed to default value the record is not added
         3. The fix works in following way
             a) When client state comes from server, we artificially add client state record calling _onVScrollHandler             
             b) The scroll position should differ from default value, before make call to _onVScrollHandler we reset client state default value
    */
    function fixScrollJumpingOnSecondPostBackBug() {        
        try {

            var gridControl = getTopGrid();

            var scrollTopIndex = $IG.WebDataGridProps.ScrollTop;
            gridControl._clientStateManager._items[0][scrollTopIndex[0]] = scrollTopIndex[1];

            var scrollLeftIndex = $IG.WebDataGridProps.ScrollLeft;
            gridControl._clientStateManager._items[0][scrollLeftIndex[0]] = scrollLeftIndex[1];

            gridControl._onVScrollHandler(null);
            gridControl._onHScrollHandler(null);

        } catch (e) {            
            Y.log(e.message);
        }
    }

    function getContextMenuId(sender) {

        var bandKey = sender._get_band().get_key();
        var menuId = Menus[bandKey];

        if (menuId == null)
            menuId = Menus["default"];

        return menuId;
    }

    function hideOpenContextMenus() {

        Object.keys(Menus).forEach(function (objectKey) {
            var menuId = Menus[objectKey];
            var menu = $find(menuId);
            if (menu != null && menu.get_visible() ) {
                menu.hide();
            }
        });

    }
}