if (top._allWindows == undefined) {
    top._allWindows = new Object();
}

function WindowManager() {
    this.openSingleWindow = WindowManager_OpenSingleWindow;
    this.openWindow = WindowManager_openWindow;
    this.closeAllWindows = WindowManager_closeAllWindows;
    this.focusWindow = WindowManager_focusWindow;

    WindowManager.prototype.getWndByKey = WindowManager_getWndByKey;

    top.Y.on("beforeunload", WindowManager_closeAllWindows);

    return (this);

    function WindowManager_getWndByKey(key) {
        var info = top._allWindows[key];
        return (info == null ? null : info.wnd);
    }

    function WindowManager_closeAllWindows() {
        for (var key in top._allWindows) {
            try {
                var wnd = WindowManager.prototype.getWndByKey(key);
                if (wnd && !wnd.closed) {
                    wnd.close();
                }
            } catch(e) {
            }
        }
    }

    /*This function opens modeless window.
      If there is already open window with the same targetName, 
      the page will be reloaded into that window.*/
    function WindowManager_OpenSingleWindow(url, params, targetName, width, height, leftPos, topPos, scrolling, menubar, toolbar, status, resizable) {
        
        if (params != '') { url += "?" + params; }
       
        width = width || 500;
        if (width > screen.availWidth) width = screen.availWidth;
        
        height = height || 500;
        if (height > screen.availHeight) height = screen.availHeight;
        
        leftPos = leftPos || ((screen.availWidth - width) / 2);
        if (leftPos < 0) leftPos = 0;
        
        topPos = topPos || (((screen.availHeight - height) / 2) - 40);
        if (topPos < 0) topPos = 0;
        
        if (scrolling == null) scrolling = true;
        if (resizable == null) resizable = true;
        
        var wndProperties = "width=" + width + ", height=" + height +
		        ", toolbar=" + (toolbar ? "yes" : "no") +
		        ", menubar=" + (menubar ? "yes" : "no") +
		        ", scrollbars=" + (scrolling ? "yes" : "no") +
		        ", status=" + (status ? "yes" : "no") +
		        ", location=no" +
		        ", resizable=" + (resizable ? "yes" : "no") +
		        ", titlebar=0, top=" + topPos + ", left=" + leftPos;
        
        var win = top.window.open(url, targetName, wndProperties);
        
        if (win) {
            if (top._allWindows) {
                var i = 0, orig = targetName;
                while (top._allWindows[targetName]) {
                    targetName = orig + (i++);
                }
                top._allWindows[targetName] = new Object();
                top._allWindows[targetName].wnd = win;
            }

            win.focus();
        }
    }       

    function WindowManager_openWindow(baseUrl, title, wndProperties, reload)
    {        
        var url = baseUrl;

        var wnd = WindowManager.prototype.getWndByKey(url);

        if (wnd == undefined || wnd == null || wnd.closed)
        {
            if (wndProperties == undefined)
            {
                wndProperties = "width=1024px, height=768px,location=no,toolbar=no, menubar=no,status=no,resizable=yes,scrollbars=yes";
            }

            wnd = top.window.open(url, "", wndProperties);
            top._allWindows[url] = new Object();
            top._allWindows[url].wnd = wnd;

            /* Use setTimeout to workaround bug in FF, 
               for some reason the focus is not set correctly without such timeout */
            setTimeout(function() {
                wnd.focus();
            }, 50);
        }
        else
        {
            if (reload === true) {
                wnd.location.reload(true);
            }

            wnd.focus();
        }
    }

    function WindowManager_focusWindow(baseUrl) {
        var wnd = WindowManager.prototype.getWndByKey(baseUrl);

        if (typeof wnd !== "undefined" && wnd!=null && !wnd.closed) {
            wnd.focus();
        }
    }
}
