function OverlayLightBox(rootPath) {

    this.rootPath = rootPath;
    this.overlayProxyPageUlr = rootPath + "/OverlayProxy.aspx";
    this.overlayCloseImageUrl = rootPath + "/images/overlayClose.gif";
    this.loader = null;
    this.layer = null;
    /*Needed to workaround bug in IE8-10 that it is impossible to set focus on any text element on the parent page
      after child overlay window has been closed and focus remained in this child window. 
      On overlay open, the temporary input element is created as a first element on the page, 
      on overlay closing the element is removed*/
    this.inputElement = null;
    this.overlayWindow = null;
    this.frame = null;
    this.overlayClosingFunction = null;
    this.isOpen = false;
    this.isLoading = false;
    this.targetPageUrl = "";
    this.settings = {};

    this.layerZindex = this.zIndex();
    this.loaderZindex = this.zIndex();
    this.windowZindex = this.zIndex();

    this.overlayClosedEvent = new Y.CustomEvent("onOverlayClosedEvent");
}

OverlayLightBox.zIndex = 9000;

OverlayLightBox.prototype = {
    _overlays: [], // a private shared variable to contain the currently opened overlays
    zIndex: function () {
        OverlayLightBox.zIndex = OverlayLightBox.zIndex + 1;
        return OverlayLightBox.zIndex;
    },
    generateId: function () {
        return Y.guid("yui-gen");
    },
    initLoadingImage: function () {

        if (!this.layer) {
            var id = this.generateId();
            var html = "<div class=\"overlayLayer\" id=\"" + id + "\"></div>";

            document.body.insertAdjacentHTML("afterBegin", html);
            this.layer = document.getElementById(id);
            this.layer.style.zIndex = this.layerZindex;

            var inputElementId = this.generateId();
            var inputElementHtml = "<input type='text' style='position: absolute; height: 1px; width: 1px; top: -10px; ' id=\"" + inputElementId + "\"></input>";
            document.body.insertAdjacentHTML("afterBegin", inputElementHtml);
            this.inputElement = document.getElementById(inputElementId);
        }
        if (!this.loader) {

            id = this.generateId();
            html = '<div id="' + id + '" class="overlayLoader"><div class="comd-busy-dots comd-busy-dots--extra-large"><div class="comd-busy-dots__dot"></div><div class="comd-busy-dots__dot"></div><div class="comd-busy-dots__dot"></div></div></div>';

            document.body.insertAdjacentHTML("afterBegin", html);
            this.loader = Y.one("#" + id);
            this.loader.setStyle("zIndex", this.loaderZindex);
        }
    },
    showLoadingImage: function () {

        this.initLoadingImage();

        this.layer.style.left = "0px";
        this.layer.style.top = "0px";
        //workaround the problem wiht correct layer size if document has scrolls
        Y.one("body").setStyle("overflow", "hidden");

        this.loader.setStyle("left", ((Y.DOM.winWidth() / 2) + Y.DOM.docScrollX() - 50) + "px");
        this.loader.setStyle("top", (150 + Y.DOM.docScrollY()) + "px");
    },
    init: function (settings) {
        this.settings = settings || {};

        if (this.settings.height) {
            this.settings.height = parseInt(this.settings.height, 10);
        }
        if (this.settings.width) {
            this.settings.width = parseInt(this.settings.width, 10);
        }
        if (this.settings.top) {
            this.settings.top = parseInt(this.settings.top, 10);
        }

        var noClose = !!this.settings.noclose;

        if (!noClose) {
            this.layer.style.opacity = .12;
            this.layer.style.filter = "progid:DXImageTransform.Microsoft.Alpha(opacity=12)";
        }

        if (!this.overlayWindow) {

            var inlineStyle = [],
                titleid = this.generateId(),
                windowid = this.generateId(),
                closeid = this.generateId(),
                frameid = this.generateId(),
                hideHeader = !!this.settings.hideheader;

            document.body.insertAdjacentHTML("afterBegin", ""
                + "<div" + (inlineStyle.length ? " style=\"" + inlineStyle.join(";") + "\"" : "")
                + " id=\"" + windowid + "\" class=\"overlayWindow c_wrapper\">\n"
                + (hideHeader ? "" : "<div class='modal-dialog__heading'>\n"
                    + "<div class='modal-dialog__title'><h3 id=\"" + titleid + "\"></h3></div>\n"
                    + (noClose ? "" :
                        "<button id='" + closeid + "'class='modal-dialog__close comd-button comd-button--icon'><svg class=\"overlayClose\"  viewBox=\"0 0 24 24\" aria-labelledby=\"CloseIcon\" width=\"19\" height=\"19\"><title id=\"CloseIcon\"></title><polygon points=\"24 2.42 21.58 0 12 9.58 2.42 0 0 2.42 9.58 12 0 21.58 2.42 24 12 14.42 21.58 24 24 21.58 14.42 12 24 2.42\"></polygon></svg></button>\n")
                    + "</div>\n")
                + "<div style=\"width:100%;padding:0px;margin:0px;background-color:#fff;\">\n"
                + "<iframe" + (this.settings.noscroll ? " scrolling=\"no\"" : "") + " src=\"about:blank\""
                + " style=\"width:100%;padding:0;margin:0;display: block\" id=\"" + frameid
                + "\" frameborder=\"0\"></iframe>\n"
                + "</div>\n"
                + "</div>");

            this.title = Y.one("#" + titleid).getDOMNode();
            this.overlayWindow = Y.one("#" + windowid).getDOMNode();
            this.frame = Y.one("#" + frameid).getDOMNode();

            this.overlayWindow.style.zIndex = this.windowZindex;

            if (hideHeader == false) {
                Y.one("#" + closeid).on("click", this.close, this);
            }
        }
    },
    onResize: function () {

        if (this._timer) {
            this._timer.cancel();
            this._timer = null;
        }
        this._timer = Y.later(100, this, function () {
            this.setPosition();
        });
    },
    resize: function (width, height) {

        if (this.isOpen) {
            this.frame.height = height + "px";
            this.settings.height = parseInt(height, 10);
            this.settings.width = parseInt(width, 10);
            this.updateSettings();
            this.onResize();
        }

    },
    show: function (title, url, pageArguments, settings, onOverlayClosingFunction) {

        var self = this;

        if (self.isLoading || self.isOpen) {
            return;
        }

        self.isLoading = true;

        self.targetPageUrl = url;
        self.overlayClosingFunction = onOverlayClosingFunction;

        self.initLoadingImage();
        self.init(settings);
        self.showLoadingImage();

        window.overlayArguments = {
            url: url,
            data: pageArguments
        };

        Y.later(0, self, function () {
            if (url) {
                self.frame.src = self.overlayProxyPageUlr;
                if (Y.UA.ie) {
                    self.checkTimer = Y.later(30, self, self.check, null, true);
                } else {
                    Y.one(self.frame).on("load", self.loaded, self);
                }
            }

            self.setTitle(title);
        });
    },
    check: function () {
        if (this.frame.readyState == "complete") {
            this.loaded();
            this.checkTimer.cancel();
        }
    },
    loaded: function () {
        var self = this;

        self._overlays.push(self);
        Y.one(self.frame).detach("load", self.loaded);

        self.loader.setStyle("left", "-1000px");

        self.showWhenLoaded();

        self.isLoading = false;
    },
    getMaxHeight: function (header) {
        return Y.DOM.docHeight() - (header ? header.getDOMNode().clientHeight : 0) - 60;
    },
    getWinTop: function (settingsHeight) {
        var header = Y.one(".modal-dialog__heading"),
            maxHeight = this.getMaxHeight(header);

        if (settingsHeight + this.settings.top > maxHeight) {
            return 2;
        }

        return this.settings.top;
    },
    getWinHeight: function () {

        var height = 500,
            header = Y.one(".modal-dialog__heading"),
            maxHeight = this.getMaxHeight(header),
            contentWin,
            contentHeight,
            wrapper;

        if (this.settings.height) {
            if (this.settings.height > maxHeight) {
                return maxHeight;
            }

            return this.settings.height;
        }

        maxHeight -= 100;

        try {
            contentWin = this.frame.contentWindow;
            wrapper = contentWin.document.getElementById("c_wrapper");
            contentHeight = wrapper ? wrapper.clientHeight : height;
            if (contentHeight > height)
                height = contentHeight;
        } catch (e) { }
        return Math.min(height, maxHeight);
    },
    updateSettings: function () {
        var winHeight = this.getWinHeight();
        this.frame.height = winHeight + 50 + "px";

        if (this.settings.height) {
            this.settings.height = winHeight;
            this.settings.top = this.getWinTop(this.settings.height);
        }

        if (this.settings.width) {
            this.overlayWindow.style.width = Math.min(this.settings.width, Y.DOM.docWidth() - 20) + "px";
        }
    },
    showWhenLoaded: function () {
        if (this.overlayWindow) {

            this.isOpen = true;

            this.updateSettings();

            this.setPosition();

            if (!this.settings.noclose) {
                Y.one(this.layer).on("click", this.close, this);
            }

            Y.one(window).on("resize", this.onResize, this);

        }
    },
    getTop: function (height) {
        var top = this.settings.top;
        if (top == null) {
            top = Math.floor((((Y.DOM.docHeight() / 2) + Y.DOM.docScrollY()) - (height / 2)));
        }
        return top;
    },
    setPosition: function () {
        if (this.overlayWindow && this.overlayWindow.clientWidth) {
            var overlayLeft = (((Y.DOM.docWidth() / 2) + Y.DOM.docScrollX()) - (this.overlayWindow.clientWidth / 2) - 5);
            this.overlayWindow.style.left = (overlayLeft < 0 ? "0" : overlayLeft) + "px";
            this.overlayWindow.style.top = this.getTop(this.overlayWindow.clientHeight) + "px";
        }
    },
    close: function (executeClosingFunction, data) {
        //probalby it's needed for FX browser when esc key is used
        //cb.cancelEvent(e);        

        // revert overflow that was previously set to "hidden" in showLoadingImage function
        Y.one("body").setStyle("overflow", "visible");

        this.hide(executeClosingFunction, data);
    },
    _execute: function (functionForExcecution, parameters) {
        if (Y.Lang.isString(functionForExcecution)) {
            return overlayWindow["eval"](functionForExcecution);
        }
        else {
            functionForExcecution(parameters);
        }
    },
    _remove: function (elem) {
        try {
            elem.parentElement.removeChild(elem);
        } catch (e) { }
    },
    hide: function (executeClosingFunction, data) {

        if (!this.layer)
            return;

        try {
            if (this.settings.calledWindow) {
                /*workaround for IE browser: 
                For some reasons if focus stayes on an element inside overlay's frame while overlay is closing,
                it is impossible to set focus on any input elements of the called window after overlay has been closed.             
                */

                var el = this.settings.calledWindow.Y.all('input[type=text],textarea').filter(function (node) {
                    return node.getAttribute('disabled') == "";
                });
                if (el.size() > 0)
                    el.item(0).focus();
            }
        }
        catch (ex) {
            //for example calledWindow can be not accessible
            Y.log(ex.message);
        }

        this._remove(this.layer);
        this.layer = null;

        this._remove(this.inputElement);
        this.inputElement = null;

        this._remove(this.frame);
        this.frame = null;

        this._remove(this.overlayWindow);
        this.overlayWindow = null;

        if (executeClosingFunction && this.overlayClosingFunction) {
            this._execute(this.overlayClosingFunction);
        }

        this.isOpen = false;
        Y.one(window).detach("resize", this.onResize);
        this._overlays.pop();

        this.overlayClosedEvent.fire({ result: executeClosingFunction, data: data });
        this.overlayClosedEvent.detachAll();
    },
    setTitle: function (title) {
        if (title && this.title) {
            this.title.innerHTML = title;
        }
    },
    closeLast: function (executeClosingFunction, data) {

        var overlays = this._overlays;

        if (!overlays.length) {
            if (this.checkLightBox(parent)) {
                overlays = parent.OverlayLightBox.prototype._overlays;
            } else if (this.checkLightBox(top)) {
                overlays = top.OverlayLightBox.prototype._overlays;
            }
        }

        if (overlays.length) {
            var o = overlays[overlays.length - 1];
            if (o && o.close) {
                o.close(executeClosingFunction, data);
            }
        }
    },
    setOverlayTitle: function (title) {

        if (window.parent && window.parent.overlay) {
            window.parent.overlay.setTitle(title);
        }
    },
    checkLightBox: function (obj) {
        return obj && obj.OverlayLightBox && obj.OverlayLightBox.prototype._overlays.length;
    }
};
