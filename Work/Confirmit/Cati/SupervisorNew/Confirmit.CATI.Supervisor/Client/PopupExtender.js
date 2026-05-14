var visibleSlave = null;

// it is called when some dropdown behaviour is initialized
function initBehaviour(masterId, slaveId, popupDirection, onPopupScript, autoHide) {
    Y.on('load', function () {
        var master = document.getElementById(masterId);
        var slave = document.getElementById(slaveId);
        if (master == null || slave == null) return;

        master.setAttribute('slaveId', slaveId);
        master.setAttribute('popupDirection', popupDirection);

        //text representation of javascript code that should run after popup is shown
        master.onPopupScript = onPopupScript;
        Y.on("click", showPopup, master);

        if (autoHide.toLowerCase() == 'true') {

            Y.on("mousedown", documentMouseDown, document);            
            Y.on("GridRowClicked", function() { hidePopup(); });            
        }
    });
}

// adds global listener for 'mouseup' event of the 'document' object
function showPopup(evnt) {
    var master = evnt.currentTarget;
    // if slave attribute is not set - search parent objects for it.
    while (master != null && master.getAttribute('slaveId') == null) {
        master = master.parentNode;
    }
    if (master == null) return;
    var slave = Y.one('#' + master.getAttribute('slaveId'));
    slave.setAttribute('masterId', master.get('id'));
    
    showSlave(master, slave, true);
}

//
function hidePopup() {
    var slave = visibleSlave;
    if (slave == null) return;
    var master = Y.one('#' + slave.getAttribute('masterId'));
    if (master == null) return;
    showSlave(master, slave, false);
}

// shows/hides slave control
function showSlave(master, slave, fShow) {
    if (master == null || slave == null) return;
    
    slave.setStyle('display', fShow ? 'block' : 'none');
    slave.setStyle('visibility',fShow ? 'visible' : 'hidden');
    visibleSlave = fShow ? slave : null;
    
    if (fShow) {
        slave.setStyle('zIndex', '1000');
        positionContainer(master, slave);
        if (master.onPopupScript != null)
            eval(master.onPopupScript);
    }
}

// sets position of slave below/above master
function positionContainer(master, slave) {
    if (master == null || slave == null || visibleSlave == null) return;

    // left and top position of slave (ie left & bottom position of master)
    var x = 0;
    var y;
    
    switch (master.getAttribute('popupDirection')) {
        case "Down":
            y = master.get('offsetHeight');
            break;
        case "Up":
            y = -slave.get('offsetHeight');
            break;
        default:
            y = 0;
    }
    // width & heigth of window client area.
    var w = document.body.clientWidth; //offsetWidth;

    var masterPosition = master.getXY();
    var slaveParentPosition = slave.get('offsetParent').getXY();

    // slave control can have its own offsetParent controls, so we need to take into account the top and left values of the slave parent control
    x += (masterPosition[0] - slaveParentPosition[0]);
    y += (masterPosition[1] - slaveParentPosition[1]);
    
    if (x + slave.get('offsetWidth') > w)
        x = w - slave.get('offsetWidth') > 0 ? w - slave.get('offsetWidth') - 6 : 0;

    // it is applied relatively to slave.offsetParent control only.
    slave.setStyle('left', x + 'px');
    slave.setStyle('top', y + 'px');
}

// processes mouse click events for document: close drop-down
function documentMouseDown(evnt) {
    
    var slave = visibleSlave;
    if (slave == null) return;
    var master = slave.getAttribute('masterId');
    if (master == null) return;
    var elem = evnt.target;
    while (elem != null) {
        // ignores events that belong to slave
        if (elem == slave) 
        return;
        elem = elem.get('offsetParent');
    }
    
    hidePopup();
}
