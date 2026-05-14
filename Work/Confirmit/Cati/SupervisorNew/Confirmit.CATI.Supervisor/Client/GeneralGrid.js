function GeneralGrid(settings) {
    var oldRow = null;
    var curRow = null;

    this.InitializedEvent = new Y.CustomEvent("InitializedEvent");
    this.BeforeContextMenu = new Y.CustomEvent("BeforeContextMenu");    
    
    var self = this;
    this.InitializeGrid = function (sender, args) {
        Y.all("#" + settings.GridId + " input[type='checkbox'].Selection").on("click", rowChecked);

        var gridController = $find(settings.GridId);

        if (!settings.KeepSelection) {
            Y.one("#" + settings.hHighlightedId).set('value', "");
            gridController.get_behaviors().get_selection().get_selectedRows().clear();
        }

        Y.all(".gridHeaderFilter").on("click", function (event) { event.stopPropagation(); });
        Y.all(".gridHeaderFilter").on("mousedown", function (event) { event.stopPropagation(); });
        Y.all(".gridHeaderFilter").on("selectstart", function (event) { event.stopPropagation(); });

        // Previously there was hack with hiding and showing again the first column. After it was removed we received problem with rendering of CallManagement grid.
        // _onResize method is called inside infragistics hideColumn method, so we do this to keep proper rendering of the whole grid.
        // Without this fix grid is rendered partially.
        gridController._onResize();

        self.InitializedEvent.fire(sender, args);

        synchronizeSelectAllCheckbox();

        fixScrollJumpingOnSecondPostBackBug();

    };

    this.hideContextMenu = function() {
        hideContextMenu(settings.ItemContextMenuId);
    };

    this.clearSearchFields = function () {
        var filterTexts = Y.all("#" + settings.GridId + ' .igg_HeaderCaption input');
        filterTexts.getDOMNodes().forEach(function (filterText) {
            filterText.value = '';
        });

        var filterSelects = Y.all("#" + settings.GridId + ' .igg_HeaderCaption select');
        filterSelects.getDOMNodes().forEach(function (filterSelect) {
            filterSelect.value = '';
        });
    };

    this.setValueToSearchField = function (columnName, value) {
        var filterBox = Y.one("#" + settings.GridId + ' .igg_HeaderCaption[key="' + columnName + '"] input');
        filterBox.set('value', value);
    };

    this.refresh = function () {
        eval(settings.RefreshCommand);
    };

    function synchronizeSelectAllCheckbox() {
        // we uncheck 'select all' checkbox if some checkboxes are unchecked
        var checkboxes = Y.all("#" + settings.GridId + " input[type='checkbox'].Selection");
        var uncheck = checkboxes.size() == 0 || checkboxes.some(isUnchecked);
        Y.all("#" + settings.GridId + " input[type='checkbox'].SelectAll").set("checked", !uncheck);
    };
    
    function isUnchecked(node){
        return !node.get("checked");
    }
    function rowChecked() {
        writeSelected(settings.GridId, settings.RecordsCountLabelId, settings.HiddenSelectedId, settings.PrimaryKeyColumn);
    }

    function writeSelected(gridId, countId, selectedId, primaryKeyColumn) {
        var tbCount = document.getElementById(countId);
        if (tbCount == null) return;
        var count = parseInt(tbCount.innerHTML.substr(tbCount.innerHTML.lastIndexOf(": ") + 2));
        var hSelected = document.getElementById(selectedId);
        var grid = $find(gridId);
        // This "if" is for support multiselection (with shift-key pressed).
        var row;
        if (curRow != null) {
            row = curRow;
        } else {
            row = grid.get_behaviors().get_selection().get_selectedRows().getItem(0);
        }
        if (row == null) return;
        var keyValue = row.get_cellByColumnKey(primaryKeyColumn).get_value();
        var selArr = hSelected.value.split(",");
        var checkboxNode = Y.one(row.get_cellByColumnKey("Selected").get_element()).one("input[type='checkbox']");
        if (checkboxNode.get("checked")) {
            if (hSelected.value.length > 0) {
                var b = false;
                for (var i in selArr) {
                    if (selArr[i] == keyValue) {
                        b = true;
                        break;
                    }
                }
                if (!b) {
                    hSelected.value += "," + keyValue;
                }
            }
            else {
                hSelected.value = keyValue;
            }
            count++;
        }
        else {
            if (hSelected.value.length > 0) {
                for (var i in selArr) {
                    if (selArr[i] == keyValue) {
                        selArr.splice(i, 1);
                        count--;
                    }
                }
                hSelected.value = selArr.join(",");
            }

        }
        tbCount.innerHTML = tbCount.innerHTML.substr(0, tbCount.innerHTML.lastIndexOf(": ") + 2) + count;
    }

    this.GetSelectedRow = function () {
        var rows = $find(settings.GridId).get_behaviors().get_selection().get_selectedRows();
        if (rows.get_length() > 0) {
            return rows.getItem(0);
        }
        return null;
    };

    this.BeforeSubmit = function() {
        var result = { };
        for (var i in settings.DateControlIds) {
            var id = settings.DateControlIds[i];
            var dateCtrl = $IG.WebTextEditor.find(id);
            var value = dateCtrl.get_value();

            if (value) {
                result[id] = value.format("yyyy-MM-dd");
            }
        }

        Y.one("#" + settings.DateValuesHiddenId).set("value", Y.JSON.stringify(result));
    };


    this.onContextMenu = function (sender, args) {
        self.BeforeContextMenu.fire(sender, args);
        var type = args.get_type();
        if (type == "cell") {
            args.set_cancel(true);
            // select clicked row
            var selectedRows = sender.get_behaviors().get_selection().get_selectedRows();
            selectedRows.clear();
            selectedRows.add(args.get_item().get_row());
            Y.one("#" + settings.hHighlightedId).set('value', args.get_item().get_row().get_cellByColumnKey(settings.PrimaryKeyColumn).get_value());

            showContextMenu(args.get_browserEvent(), settings.ItemContextMenuId);
        }
    };

    this.onClick = function (sender, args) {
        var type = args.get_type();
        if (settings.EnableSorting && type == "header") {
            var key = args.get_item().get_column().get_key();

            if (Y.Array.indexOf(settings.SortingDisablecColumnKeys, key) < 0) {
                args.set_cancel(true);

                Y.one("#" + settings.hSortColumnKeyId).set('value', key);

                eval(settings.SortPostBackReference);
            }
        }

        if (type == "cell") {
            var menu = $find(settings.ItemContextMenuId);
            if (menu && menu.get_visible()) {
                menu.hide();
            }
        }
        
        Y.fire('GridRowClicked');
    };
    
    this.onRowSelectionChanging = function(sender, args) {
        oldRow = sender.get_behaviors().get_selection().get_selectedRows().getItem(0);
    };

    this.onRowSelectionChanged = function (sender, args) {
        var newRow = args.getSelectedRows().getItem(0);
        if (newRow) {
            Y.one("#" + settings.hHighlightedId).set('value', newRow.get_cellByColumnKey(settings.PrimaryKeyColumn).get_value());
        }

        curRow = null;
    };

    this.onMouseDown = function (sender, args) {
        var type = args.get_type();
        if (type != "cell")
            return;

        var newRow = sender.get_behaviors().get_selection().get_selectedRows().getItem(0);
        curRow = null;

        var evt = args.get_browserEvent();
        if (!evt.ctrlKey && !evt.shiftKey)
            return;

        if (newRow == null)
            return;

        if (!newRow.get_cellByColumnKey("Selected"))
            return;

        var checkboxNode = Y.one(newRow.get_cellByColumnKey("Selected").get_element()).one("input[type='checkbox']");

        // Ctrl+Click (or navigate with keybord) control's behaviour logic.
        if (evt.ctrlKey) {
            // Invert selection.
            checkboxNode.set("checked", !checkboxNode.get("checked"));
            rowChecked();
            return;
        }
        if (null == oldRow)
            return;
        // Shift+Click (or navigate with keybord) control's behaviour logic.

        var startRowId = (newRow.get_index() > oldRow.get_index()) ? oldRow.get_index() : newRow.get_index();
        var stopRowId = (newRow.get_index() > oldRow.get_index()) ? newRow.get_index() : oldRow.get_index();

        // Select the whole range.
        for (var i = startRowId; i <= stopRowId; i++) {
            curRow = sender.get_rows().get_row(i);

            checkboxNode = Y.one(curRow.get_cellByColumnKey("Selected").get_element()).one("input[type='checkbox']");
            if (!checkboxNode.get("checked")) {
                checkboxNode.set("checked", true);
                rowChecked();
            }
        }
    };

    this.onSelectAllClick = function (checked) {
        var grid = $find(settings.GridId);

        if (grid) {
            for (var rowId = 0; rowId < grid.get_rows().get_length(); rowId++) {
                curRow = grid.get_rows().get_row(rowId);
                var checkboxNode = Y.one(curRow.get_cellByColumnKey("Selected").get_element()).one("input[type='checkbox']");
                if (checked) {
                    if (!checkboxNode.get("checked")) {
                        checkboxNode.set("checked", true);
                        rowChecked();
                    }
                } else {
                    if (checkboxNode.get("checked")) {
                        checkboxNode.set("checked", false);
                        rowChecked();
                    }
                }
            }
        }
    };

    this.onSearchControlKeyDown = function (evt, allowOnlyDigits, allowDecimal, obj) {
        var event = Y.Event.getEvent(evt);

        // Handling special key codes for FF (in other browsers this handler is not called for them)
        if (event.keyCode == 8 /*Backspace*/ || event.keyCode == 9 /*Tab*/ || event.keyCode == 46 /*Delete*/ || (event.keyCode >= 35 && event.keyCode <= 40)  /*home / end / arrows*/)
            return;
        if (event.ctrlKey && (event.keyCode == 99 || event.keyCode == 118 || event.keyCode == 120))  /*Ctrl-C / Ctrl-V / Ctrl-X for FF*/
            return;

        if (event.keyCode == 13) //Enter pressed
        {
            event.halt();
            this.refresh();
        } else if (allowOnlyDigits && (event.keyCode < 48 || event.keyCode > 57)) {
            if (event.keyCode != 45 /* '-' */ || obj == null || obj.value != '') {
                if (!allowDecimal || event.keyCode != settings.DecimalSeparatorKeyCode) {
                    event.preventDefault();
                }
            }
        }
    };

    this.onDoubleClick = function(sender, args) {
        if (args.get_type() == 'cell') {
            eval(settings.DoubleClickCommand);
        }
    };
    
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
            var gridControl = Y.one("#" + settings.GridId).get("control");

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
}
