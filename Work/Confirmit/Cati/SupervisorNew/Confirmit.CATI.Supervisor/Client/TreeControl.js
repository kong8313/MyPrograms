
function BaseTreeControl(settings) {

    var selfChanged = false;
    var lastEvent;

    Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(pageLoadedHandler);
    
    var mouseDownHandler = function (event) {
        lastEvent = event._event;
    };
    
    function pageLoadedHandler() {
        Y.all(".igdt_Node").detach("mousedown", mouseDownHandler);
        Y.all(".igdt_Node").on("mousedown", mouseDownHandler);
    }

    this.NodePopulated = function (sender, e) {
        pageLoadedHandler();
    };

    //
    // Overrided IG methods
    
    // This method is called during tree initialization.
    // Original method iterates through all nodes even if client rendering is disabled (our case).
    // We fixed it and skip this heavy oparation in this case.

    /*
    _initNodesClientside: function(nodesAddress)
    {
        for (var i = 0; i < nodesAddress.length; i++)
		{
			var node = this._itemCollection._getObjectByAdr(nodesAddress[i]);
            
            // Checks the nodes on the client side when clientside rendering is enabled
            if(this._get_enableClientRendering() && node && !node.get_checkState())
            {
                this._toggleNodeCheckState(node);
            }
		}
    }
    */

    $IG.WebDataTree.prototype._initNodesClientside = function(nodesAddress) {
        if (this._get_enableClientRendering()) {
            for (var i = 0; i < nodesAddress.length; i++) {
                var node = this._itemCollection._getObjectByAdr(nodesAddress[i]);

                // Checks the nodes on the client side when clientside rendering is enabled
                if (node && !node.get_checkState()) {
                    this._toggleNodeCheckState(node);
                }
            }
        }
    };


    // This method is called on node checkbox click. 
    // We simplified its logic and do not iterate through all nodes if we found unchecked node.
    // As a result - clicking on a ckeckbox now do not result in iterating through all nodes.

    /*
    _applyCheckStateToParent: function(node)
    {
    var parentNode = node.get_parentNode();
    if(parentNode == null)
    return;
			
    var childrenCount = parentNode.get_childrenCount(); 
    var checkedCount = 0;
    var partialCount = 0;
    for(var i = 0; i < childrenCount; i++)
    {
    var childState = parentNode.get_childNode(i).get_checkState();
    if(childState == $IG.CheckBoxState.Checked)
    checkedCount++;
    else if(childState == $IG.CheckBoxState.Partial)
    partialCount++;
    }
		
    if(partialCount > 0)
    {
    parentNode.set_checkState($IG.CheckBoxState.Partial);
    }
    else if(checkedCount == 0)
    {
    parentNode.set_checkState($IG.CheckBoxState.Unchecked);
    }
    else if(checkedCount == childrenCount)
    {
    parentNode.set_checkState($IG.CheckBoxState.Checked);
    }
    else if(this._checkBoxMode == $IG.CheckBoxMode.TriState)
    {
    parentNode.set_checkState($IG.CheckBoxState.Partial);
    }
    else
    {
    parentNode.set_checkState($IG.CheckBoxState.Unchecked);
    }
		
    this._applyCheckStateToParent(parentNode);
    }
    */

    $IG.WebDataTree.prototype._applyCheckStateToParent = function(node) {
        var parentNode = node.get_parentNode();
        if (parentNode == null)
            return;

        var childrenCount = parentNode.get_childrenCount();
        var unchecked = false;
        for (var i = 0; i < childrenCount; i++) {
            var childState = parentNode.get_childNode(i).get_checkState();
            if (childState == $IG.CheckBoxState.Unchecked) {
                unchecked = true;
                parentNode.set_checkState($IG.CheckBoxState.Unchecked);
                break;
            }
        }

        if (!unchecked) {
            parentNode.set_checkState($IG.CheckBoxState.Checked);
        }

        this._applyCheckStateToParent(parentNode);
    };


    // Previously here we called a separate method $util.isNullOrUndefined to determine if "previous" node is null or undefined
    // replacing it with while (previous) inproved the performance of the method by 20%

    /*
    indexOfDomElement: function(domNode)
    {
    var previous = domNode;
    var i = -1;
    while(!$util.isNullOrUndefined(previous))
    {
    if(previous.nodeName == "LI")
    {
    i++;
    }
    previous = previous.previousSibling;
    }
    return i;
    }
    */

    $IG._AddressUtility.prototype.indexOfDomElement = function(domNode) {
        var previous = domNode;
        var i = -1;
        while (previous) {
            if (previous.nodeName == "LI") {
                i++;
            }
            previous = previous.previousSibling;
        }
        return i;
    };




    /* Special handler for situations when ctrl+click doesn't activate a new node (ctrl+click at node which is already active)*/
    this.NodeClick = function (sender, e) {
        var event = e.get_browserEvent();
        if (event.button != 2) {
            /*If user's action changed node selection, it means that BeforeNodeSelectionChange handler passed through.
            So function just drops selfChanged flag and halts its execution.*/
            if (selfChanged) {
                selfChanged = false;
                return;
            }

            if (!event.ctrlKey) return;

            var node = e.getNode();
            toggleNodeCheckState(node);
        }
    };

    this.NodeDropping = function (sender, e) {

        e.set_cancel(true);

        if (e.get_sourceTreeId() == sender._id) { return; }

        if (e.get_sourceNodes().length == 0) { return; }

        var targetNode = e.get_destNode();
        var sourceNode = e.get_sourceNodes()[0];
        
        /*set check state as 'checked' for source node*/
        sourceNode.set_checkState(1);

        var result = {
            NodeKey: targetNode.get_key(),
            NodePath: targetNode.get_dataPath()
        };
        
        Y.one("#" + settings.ClientDataContainerFieldId).set("value", Y.JSON.stringify(result));
        Y.one("#" + settings.NodeDroppedClickEventSenderButtonId)._node.click();
    };

    this.NodeEditingEntering = function (sender, e) {

        e.set_cancel(true);
        
        var node = e.getNode();

        var result = {
            NodeKey: node.get_key(),
            NodePath: node.get_dataPath()
        };

        Y.one("#" + settings.ClientDataContainerFieldId).set("value", Y.JSON.stringify(result));
        Y.one("#" + settings.NodeDoubleClickEventSenderButtonId)._node.click();
    };

    /*Scrolls tree to currently selected node 
     Note that there is native tree's function _scrollToNode but it doesn't take into account height of horizontal grid.*/
    this.ScrollToSelected = function (treeControl) {
        var nodes = treeControl.get_selectedNodes();

        if (nodes && nodes.length > 0) {
            
            var node = nodes[0];
            var scrollElement = treeControl.get_element();

            var styleEl = node.get_styleElement();
            if (styleEl == null) return;
            /* active node bounds */
            var bounds = Sys.UI.DomElement.getBounds(styleEl);
            var scrollElementBounds = Sys.UI.DomElement.getBounds(scrollElement);
            var el_x1 = bounds.x;
            var el_y1 = bounds.y;
            var el_x2 = el_x1 + bounds.width;
            var el_y2 = el_y1 + bounds.height;
            /* visible area */
            var va_x1 = scrollElementBounds.x; /*scrollElement.scrollLeft;*/
            var va_y1 = scrollElementBounds.y; /*scrollElement.scrollTop;*/
            var va_x2 = va_x1 + scrollElementBounds.width - 1; /*scrollElement.offsetWidth - 1;*/
            if (scrollElement.scrollWidth > scrollElement.offsetWidth)
                va_x2 = va_x2 - treeControl.__get_scrollbarWidth();
            var va_y2 = va_y1 + scrollElementBounds.height - 1; /*scrollElement.offsetHeight - 1;*/
            if (scrollElement.scrollHeight > scrollElementBounds.height) /*scrollElement.offsetHeight)*/
                va_y2 = va_y2 - treeControl.__get_scrollbarWidth();
            /* delta X */
            var dx = 0;
            if (el_x1 < va_x1) dx = el_x1 - va_x1;
            else if (el_x2 > va_x2) dx = el_x2 - va_x2;
            /* delta Y */
            var dy = 0;
            if (el_y1 < va_y1) dy = el_y1 - va_y1;
            else if (el_y2 > va_y2) dy = el_y2 - va_y2 + 100;

            scrollElement.scrollLeft += dx;
            scrollElement.scrollTop += dy;
        }
    };

    this.SelectionChanged = function (sender, e) {
        var event = lastEvent;
        if (!event.shiftKey && !event.ctrlKey) return;

        var oldNode = null;
        var newNode = null;

        if (e.getOldSelectedNodes().length > 0)
            oldNode = e.getOldSelectedNodes()[0];

        if (e.getNewSelectedNodes().length > 0)
            newNode = e.getNewSelectedNodes()[0];

        if (newNode == null || oldNode == null) return;

        selfChanged = true;

        /*Ctrl+click behaviour*/
        if (event.ctrlKey) {
            toggleNodeCheckState(newNode);
            return;
        }

        var goUp = false;
        var goDown = false;
        var tmp1 = oldNode._address.split('.');
        var tmp2 = newNode._address.split('.');
        var count = tmp1.length;
        if (tmp2.length > count) count = tmp2.length;

        for (var i = 1; i < count; i++) {

            if (parseInt(tmp1[i], 0) > parseInt(tmp2[i], 0)) {
                goUp = true;
                break;
            }
            else if (parseInt(tmp1[i], 0) < parseInt(tmp2[i], 0)) {
                goDown = true;
                break;
            }
        }

        if (!goDown && !goUp)
            if (tmp1.length < tmp2.length)
                goDown = true;
            else
                goUp = true;

        var nextNode = oldNode;

        if (goUp) {

            while (true) {

                if (nextNode == null) break;

                checkState = getNodeRevertedCheckState(nextNode);

                if (!checkChildren(nextNode, null, newNode, false)) break;

                if (nextNode.get_enabled())
                    nextNode.set_checkState(checkState);

                if (nextNode._address == newNode._address) break;

                if (nextNode.get_previousNode() != null)
                    nextNode = nextNode.get_previousNode();
                else {
                    nextNode = nextNode.get_parentNode();

                    nextNode.set_checkState(getNodeRevertedCheckState(nextNode));

                    if (nextNode._address == newNode._address) break;

                    nextNode = nextNode.get_previousNode();
                }
            }
        }
        if (goDown) {

            while (true) {
                if (nextNode == null) break;

                var checkState = getNodeRevertedCheckState(nextNode);

                if (nextNode.get_enabled())
                    nextNode.set_checkState(checkState);

                if (!checkChildren(nextNode, checkState, newNode, true)) break;

                if (nextNode._address == newNode._address) break;

                if (nextNode.get_nextNode() != null)
                    nextNode = nextNode.get_nextNode();
                else {
                    nextNode = nextNode.get_parentNode().get_nextNode();
                }
            }
        }
    };

    /* in case checkState is null the node check state will be reverted*/
    function checkChildren(node, checkState, exit_node, direct) {

        var count = node.get_childrenCount();

        if (direct) {
            for (var i = 0; i < count; i++) {

                var childNode = node.get_childNode(i);

                if (childNode.get_enabled()) /* some nodes may be disable, so we shouldn't select them*/
                {
                    if (checkState != null)
                        childNode.set_checkState(checkState);
                    else
                        toggleNodeCheckState(childNode);

                    if (checkState != null) {
                        if (!checkChildren(childNode, checkState, exit_node, direct)) return false;
                    }
                    else {
                        if (!checkChildren(childNode, getNodeRevertedCheckState(childNode), exit_node, direct)) return false;
                    }
                }

                if (childNode == exit_node) return false;
            }
        }
        else {
            for (var i = count - 1; i > -1; i--) {

                var childNode = node.get_childNode(i);

                if (childNode.get_enabled()) /* some nodes may be disable, so we shouldn't select them*/
                {
                    if (checkState != null)
                        childNode.set_checkState(checkState);
                    else
                        toggleNodeCheckState(childNode);

                    if (childNode.hasChildren()) {
                        if (checkState != null) {
                            if (!checkChildren(childNode, checkState, direct)) return false;
                        }
                        else {
                            if (!checkChildren(childNode, getNodeRevertedCheckState(childNode), direct)) return false;
                        }
                    }
                }
                if (childNode == exit_node) return false;
            }
        }
        return true;
    }

    function toggleNodeCheckState(node) {
        node.set_checkState(getNodeRevertedCheckState(node));
    }

    function getNodeRevertedCheckState(node) {
        if (node.get_checkState() == 0) {
            return 1;
        }
        else {
            return 0;
        }
    }

};
