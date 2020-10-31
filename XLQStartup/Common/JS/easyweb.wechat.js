/// <reference path="../vsdoc/jquery.d.ts" />
/// <reference path="../vsdoc/easyweb.d.ts" />
var extend;
(function (extend) {
    //日期格式化输出
    Date.prototype.format = function (formatStr) {
        /// <summary>对Date的扩展，将 Date 转化为指定格式的字符串。</summary>
        /// <param name="formatStr" type="String">yyyy-MM-dd hh:mm:ss.S ==> 2006-07-02 08:09:04.423 或 "yyyy-M-d h:m:s.S" ==> 2006-7-2 8:9:4.18</param>
        /// <returns type="void">返回格式化后的日期字符串。</returns>
        var o = {
            "M+": this.getMonth() + 1,
            "d+": this.getDate(),
            "h+": this.getHours(),
            "m+": this.getMinutes(),
            "s+": this.getSeconds(),
            "q+": Math.floor((this.getMonth() + 3) / 3),
            "S": this.getMilliseconds()
        };
        if (/(y+)/.test(formatStr))
            formatStr = formatStr.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        for (var k in o)
            if (new RegExp("(" + k + ")").test(formatStr))
                formatStr = formatStr.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        return formatStr;
    };

    Date.prototype.addYear = function (num) {
        var tmpDate = new Date(this);
        tmpDate.setFullYear(tmpDate.getFullYear() + num);
        return tmpDate;
    };

    Date.prototype.addMonth = function (num) {
        var tmpDate = new Date(this);
        tmpDate.setMonth(tmpDate.getMonth() + num);
        return tmpDate;
    };

    Date.prototype.addDay = function (num) {
        var tmp = this.valueOf();
        tmp = tmp + (num * 24 * 60 * 60 * 1000);
        return new Date(tmp);
    };

    //替换所有字符串
    String.prototype.replaceAll = function (oldStr, newStr, ignoreCase) {
        /// <summary>替换源字符串中所有匹配的字符串为指定的字符串。（javascript原生的replace只替换第1个匹配项。）</summary>
        /// <param name="oldStr" type="String">所要替换的字符串</param>
        /// <param name="newStr" type="String">替换为新的字符串</param>
        /// <param name="ignoreCase" type="bool">[可选]是否忽略大小写,默认为false。</param>
        /// <returns type="String">替换操作后产生新的字符串。</returns>
        if (!RegExp.prototype.isPrototypeOf(oldStr)) {
            return this.replace(new RegExp(oldStr, (ignoreCase ? "gi" : "g")), newStr);
        } else {
            return this.replace(oldStr, newStr);
        }
    };

    String.prototype.format = function (args) {
        var result = this;
        if (arguments.length > 0) {
            if (arguments.length == 1 && typeof (args) == "object") {
                for (var key in args) {
                    if (args[key] != undefined) {
                        var reg = new RegExp("({" + key + "})", "g");
                        result = result.replace(reg, args[key]);
                    }
                }
            } else {
                for (var i = 0; i < arguments.length; i++) {
                    if (arguments[i] != undefined) {
                        var reg = new RegExp("({)" + i + "(})", "g");
                        result = result.replace(reg, arguments[i]);
                    }
                }
            }
        }
        return result;
    };

    //RGB颜色转换为16进制
    String.prototype.toRgbColor = function () {
        var reg = /^#([0-9a-fA-f]{3}|[0-9a-fA-f]{6})$/, that = this;
        if (/^(rgb|RGB)/.test(that)) {
            var aColor = that.replace(/(?:\(|\)|rgb|RGB)*/g, "").split(",");
            var strHex = "#";
            for (var i = 0; i < aColor.length; i++) {
                var hex = Number(aColor[i]).toString(16);
                if (hex === "0") {
                    hex += hex;
                }
                strHex += hex;
            }
            if (strHex.length !== 7) {
                strHex = that;
            }
            return strHex;
        } else if (reg.test(that)) {
            var aNum = that.replace(/#/, "").split("");
            if (aNum.length === 6) {
                return that;
            } else if (aNum.length === 3) {
                var numHex = "#";
                for (var i = 0; i < aNum.length; i += 1) {
                    numHex += (aNum[i] + aNum[i]);
                }
                return numHex;
            }
        } else {
            return '#000000';
        }
    };

    //16进制颜色转为RGB格式
    String.prototype.toHexColor = function () {
        var reg = /^#([0-9a-fA-f]{3}|[0-9a-fA-f]{6})$/, sColor = this.toLowerCase();
        if (sColor && reg.test(sColor)) {
            if (sColor.length === 4) {
                var sColorNew = "#";
                for (var i = 1; i < 4; i += 1) {
                    sColorNew += sColor.slice(i, i + 1).concat(sColor.slice(i, i + 1));
                }
                sColor = sColorNew;
            }

            //处理六位的颜色值
            var sColorChange = [];
            for (var i = 1; i < 7; i += 2) {
                sColorChange.push(parseInt("0x" + sColor.slice(i, i + 2)));
            }
            return "RGB(" + sColorChange.join(",") + ")";
        } else {
            return sColor;
        }
    };

    //修复早期IE不支持数据的indexOf方法
    if (!Array.prototype.indexOf) {
        Array.prototype.indexOf = function (elt) {
            var len = this.length >>> 0;
            var from = Number(arguments[1]) || 0;
            from = (from < 0) ? Math.ceil(from) : Math.floor(from);
            if (from < 0)
                from += len;
            for (; from < len; from++) {
                if (from in this && this[from] === elt)
                    return from;
            }
            return -1;
        };
    }

    //修复早期IE不支持console控制台方法
    if (!window.console) {
        var names = ["log", "debug", "info", "warn", "error", "assert", "dir", "dirxml", "group", "groupEnd", "time", "timeEnd", "count", "trace", "profile", "profileEnd"];

        window.console = {};
        for (var i = 0; i < names.length; i++)
            window.console[names[i]] = function () {
            };
    }
})(extend || (extend = {}));

var easyweb;
(function (easyweb) {
    var appDomain = (function () {
        function appDomain() {
        }
        appDomain.startMain = function () {
            var mainScriptElement = easyweb.appDomain.getMainScriptElement();
            if (mainScriptElement === undefined)
                return;

            var mainScriptUrl = mainScriptElement.getAttribute('data-main');
            if (mainScriptUrl !== undefined)
                appDomain.loadScript(mainScriptUrl, null);
        };

        appDomain.loadScript = function (fileName, callback) {
            var fileNames = [fileName];
            appDomain.loadScriptFiles(fileNames, callback);
        };

        appDomain.loadCSSFile = function (fileName) {
            var fileNames = [fileName];
            appDomain.loadCSSFiles(fileNames);
        };

        appDomain.loadScriptFiles = function (fileNames, callback) {
            if (fileNames.length == 0) {
                callback();
                return;
            }

            var fileName = fileNames[0], script = document.createElement('script'), runCallBack = function (files, callback) {
                files.shift();
                if (files.length == 0)
                    callback();
                else
                    appDomain.loadScriptFiles(files, callback);
            };

            script.src = fileName;
            document.getElementsByTagName('head')[0].appendChild(script);

            if (callback) {
                if (script.readyState === undefined) {
                    script.onload = function (event) {
                        runCallBack(fileNames, callback);
                    };
                } else {
                    script.onreadystatechange = function () {
                        if (script.readyState === 'loaded' || script.readyState === 'complete') {
                            runCallBack(fileNames, callback);
                        }
                    };
                }
                script.onerror = function (event) {
                    console.error('Can not load the script file:"{0}".'.format(script.src));
                };
            }
        };

        appDomain.loadHtmlViews = function (fileNames, viewCache, callback) {
            if (fileNames.length == 0) {
                callback();
                return;
            }

            var tmpBox = $('<div></div>'), fileName = fileNames[0], viewPath = fileName + ' .app-view', runCallBack = function (files, viewCache, callback) {
                files.shift();
                if (files.length == 0)
                    callback();
                else
                    appDomain.loadHtmlViews(files, viewCache, callback);
            };

            tmpBox.load(viewPath, function (httpRequest, textStatus, errorThrown) {
                if (textStatus == 'success') {
                    var appviews = tmpBox.children();
                    for (var i = 0; i < appviews.length; i++) {
                        var appview = appviews.eq(i), viewName = appview.data('viewname');

                        if (validate.isNotNull(viewName))
                            viewCache[viewName] = appview;
                    }

                    runCallBack(fileNames, viewCache, callback);
                }
            });
        };

        appDomain.loadCSSFiles = function (fileNames) {
            for (var n = 0; n < fileNames.length; n++) {
                var link = document.createElement('link');
                link.href = fileNames[n];
                link.rel = 'stylesheet';
                document.getElementsByTagName('head')[0].appendChild(link);
            }
        };

        appDomain.loadImgFiles = function (fileNames, callback) {
            if (fileNames.length == 0) {
                callback();
                return;
            }

            var fileName = fileNames[0], img = new Image(), runCallBack = function (files, callback) {
                files.shift();
                if (files.length == 0)
                    callback();
                else
                    appDomain.loadImgFiles(files, callback);
            };

            img.src = fileName;
            if (img.complete) {
                runCallBack(fileNames, callback);
            } else {
                img.onload = function () {
                    runCallBack(fileNames, callback);
                };
            }
        };

        appDomain.getMainScriptElement = function () {
            var scriptLink, scriptMain, scriptList = document.getElementsByTagName('script');

            for (var n = 0; n < scriptList.length; n++) {
                var tmpScript = scriptList[n];
                if (tmpScript.getAttribute('data-main') != undefined) {
                    return tmpScript;
                }
            }
        };
        appDomain.language = 'zh-cn';

        appDomain.animateStyle = {
            moveToLeft: { outClass: 'pt-page-moveToLeft', inClass: 'pt-page-moveFromRight' },
            moveToRight: { outClass: 'pt-page-moveToRight', inClass: 'pt-page-moveFromLeft' },
            moveToTop: { outClass: 'pt-page-moveToTop', inClass: 'pt-page-moveFromBottom' },
            moveToBottom: { outClass: 'pt-page-moveToBottom', inClass: 'pt-page-moveFromTop' }
        };
        return appDomain;
    })();
    easyweb.appDomain = appDomain;

    var application = (function () {
        function application() {
            this.moduleViewsCache = {};
            this.moduleClassCache = {};
            this.windowBody = null;
            this.currentContainer = null;
            this.currentLayout = { name: null, controllers: {}, views: {}, viewName: null };
            this.currentModule = { name: null, controllers: {}, views: {}, viewName: null };
            this.needLoginDlgIsOnShow = false;
            this.isOnUpdateHash = false;
            this.disableHistory = true;
            this.context = {};
            this.settings = {
                appName: '',
                debugMode: true,
                language: (navigator.language || navigator.browserLanguage).toLowerCase(),
                currentUserAPI: '/API/Common/CurrentUser',
                defaultModuleName: '',
                loginModuleName: 'Login',
                rootPath: '/',
                animateTime: 200,
                resource: {
                    CSS: [],
                    JS: [],
                    Images: [],
                    Modules: [],
                    Views: []
                }
            };
            this.eventManager = new eventManager();
        }
        application.prototype.start = function () {
            var app = this;
            var startApplication = function () {
                document.title = window.lang != undefined ? window.lang['AppName'] : app.settings.appName;
                app.windowBody = $(window.top.document.body).css('opacity', 0);
                app.currentContainer = app.windowBody.empty();

                app.settings.rootPath = app.settings.rootPath.toString().substr(0, app.settings.rootPath.length - 1);

                easyweb.appDomain.language = app.settings.language;
                easyweb.server.rootPath = app.settings.rootPath;
                easyweb.server.debugMode = app.settings.debugMode;
                easyweb.server.onNotLogin = function () {
                    if (app.context.currentUser != null) {
                        if (!app.needLoginDlgIsOnShow) {
                            app.needLoginDlgIsOnShow = true;
                            easyweb.dialog.alert('您的帐号尚未登录或已经超时，请您重新登录系统。', function () {
                                app.needLoginDlgIsOnShow = false;
                                app.loadModule(app.settings.loginModuleName);
                                return true;
                            });
                        }
                    } else {
                        app.loadModule(app.settings.loginModuleName);
                    }
                };

                $(window).resize(function () {
                    var viewController = app.currentModule.controllers[app.currentModule.viewName];
                    if (viewController != undefined)
                        app.triggerEvent(viewController, 'resize', null);

                    var layoutController = app.currentLayout.controllers[app.currentLayout.viewName];
                    if (layoutController != undefined)
                        app.triggerEvent(layoutController, 'resize', null);
                });

                var moduleInfo = app.getModuleInfoByHash();

                if (moduleInfo === null) {
                    moduleInfo = easyweb.validate.isNotNull(app.context.currentUser) ? app.context.currentUser.DefaultModuleName : app.settings.defaultModuleName;
                }

                app.loadModule(moduleInfo).done(function () {
                    app.windowBody.animate({ 'opacity': 1 }, app.settings.animateTime);
                });

                window.addEventListener("hashchange", function () {
                    app.loadViewByHash();
                }, false);

                document.body.addEventListener('touchstart', function () { }, false);
            };
            this.loadResourceFiles(startApplication);
        };

        application.prototype.regModule = function (moduleName, moduleClass) {
            this.moduleClassCache[moduleName] = moduleClass;
        };

        application.prototype.regDialog = function (dialogName, dialogClass) {
            this[dialogName] = function () {
                easyweb.dialog.alert('test');
            };
        };

        application.prototype.loadLayout = function (layoutInfo) {
            var app = this, dtd = $.Deferred(), nameInfo = validate.isNull(layoutInfo) ? [null, null] : layoutInfo.split('.'), moduleName = nameInfo[0], targetView = nameInfo[1];

            if (app.currentLayout.name === moduleName && app.currentLayout.viewName == targetView)
                return dtd.resolve();

            app.destroyModule(app.currentLayout);

            if (moduleName === null && targetView === null) {
                app.currentContainer = app.windowBody;
                return dtd.resolve();
            }

            if (app.moduleClassCache[moduleName] === undefined) {
                easyweb.dialog.error('找不到名称为“' + moduleName + '”的页面布局。');
                return dtd.resolve();
            }

            var moduleInstance = new app.moduleClassCache[moduleName](null), viewName = app.getViewName(targetView, moduleInstance.controllers), view = app.moduleViewsCache[moduleName + '.' + viewName].clone(), controller = new moduleInstance.controllers[viewName](view);
            //view.find('.button').each(function () { $(this)[0].addEventListener('touchstart', function () { }, false); });
            app.currentContainer = app.windowBody.empty();
            app.currentContainer.append(view);

            var initReturn = app.triggerEvent(controller, 'init', null), dtdInit = validate.isDeferred(initReturn) ? initReturn : $.Deferred().resolve();

            dtdInit.done(function () {
                app.currentLayout.name = moduleName;
                app.currentLayout.viewName = viewName;
                app.currentLayout.controllers[viewName] = controller;
                app.currentLayout.views[viewName] = view;

                app.currentContainer = view.find('.app-module-container');

                app.bindViewEvent(view, controller);
                app.triggerEvent(controller, 'show', true);

                view.css('visibility', 'visible');
                dtd.resolve();
            });

            return dtd;
        };

        application.prototype.loadModule = function (moduleInfo, moduleParm) {
            var app = this, dtd = $.Deferred(), nameInfo = validate.isNull(moduleInfo) ? [null, null] : moduleInfo.split('.'), moduleName = nameInfo[0], targetView = nameInfo[1];

            if (validate.isNull(app.moduleClassCache[moduleName])) {
                console.error('找不到名称为“' + moduleName + '”的功能模块。');
                dtd.resolve();
                return dtd;
            }

            var moduleInstance = new app.moduleClassCache[moduleName](moduleParm), layout = moduleInstance.layoutsName;

            targetView = app.getViewName(targetView, moduleInstance.controllers);

            app.destroyModule(app.currentModule);

            app.loadLayout(layout).done(function () {
                var viewIndex = 0;
                var initViewDeferredList = [];

                app.currentModule.name = moduleName;
                app.eventManager.trigger('moduleChanged', moduleName);

                for (var viewName in moduleInstance.controllers) {
                    if (app.moduleViewsCache[moduleName + '.' + viewName] == undefined) {
                        console.error('找不到名称为“' + moduleName + '.' + viewName + '”的模块视图。');
                        dtd.resolve();
                        return dtd;
                    }

                    var view = app.moduleViewsCache[moduleName + '.' + viewName].clone(), controller = new moduleInstance.controllers[viewName](view);
                    //view.find('.button').each(function () { $(this)[0].addEventListener('touchstart', function () { }, false); });
                    app.currentContainer.append(view);

                    var initReturn = app.triggerEvent(controller, 'init', null), dtdInit = validate.isDeferred(initReturn) ? initReturn : $.Deferred().resolve();

                    initViewDeferredList.push(dtdInit);
                    app.bindViewEvent(view, controller);

                    app.currentModule.views[viewName] = view;
                    app.currentModule.controllers[viewName] = controller;

                    viewIndex++;
                }

                asyncTask.waitTaskList(initViewDeferredList, function () {
                    app.changeView(targetView, true);
                    dtd.resolve();
                });
            });

            return dtd;
        };

        application.prototype.changeView = function (targetViewName, viewOnShowParm, animate) {
            var app = this, animateStyle = appDomain.animateStyle[animate], viewName = targetViewName, prevView = app.currentModule.views[app.currentModule.viewName], prevCont = app.currentModule.controllers[app.currentModule.viewName], nextView = app.currentModule.views[viewName], nextCont = app.currentModule.controllers[viewName];

            app.currentModule.viewName = viewName;

            if (animateStyle === undefined) {
                app.hideCurrentView(function () {
                    app.currentModule.views[viewName].appendTo(app.currentContainer).css('visibility', 'visible');
                    app.triggerEvent(app.currentModule.controllers[viewName], 'show', viewOnShowParm);
                });
            } else {
                prevView.addClass(animateStyle.outClass);
                nextView.appendTo(app.currentContainer).css('visibility', 'visible').addClass(animateStyle.inClass);

                app.triggerEvent(prevCont, 'hide', null);
                app.triggerEvent(nextCont, 'show', viewOnShowParm);

                setTimeout(function () {
                    prevView.css('visibility', 'hidden');
                    prevView.removeClass(animateStyle.outClass);
                    nextView.removeClass(animateStyle.inClass);
                }, 600);
            }

            if (app.currentModule.controllers[viewName].title != undefined) {
                window.top.document.title = app.currentModule.controllers[viewName].title;
                // hack在微信等webview中无法修改document.title的情况
                var tmpFrame = $('<iframe src="/favicon.ico"></iframe>');
                tmpFrame.on('load', function () {
                    setTimeout(function () {
                        tmpFrame.off('load').remove();
                        _sgma("send", "pageview");
                    }, 0);
                });
                tmpFrame.appendTo(app.windowBody);
            }


            app.updateLocationHash();
        };

        application.prototype.goback = function () {
            window.top.history.back();
        };

        application.prototype.bind = function (eventName, handle) {
            this.eventManager.bind(eventName, handle);
        };

        application.prototype.unbind = function (eventName) {
            this.eventManager.unbind(eventName);
        };

        application.prototype.hideCurrentView = function (callback) {
            var app = this;
            if (app.currentModule.viewName === null) {
                callback();
                return;
            }

            // 隐藏当前视图
            var view = app.currentModule.views[app.currentModule.viewName], controller = app.currentModule.controllers[app.currentModule.viewName];

            var hideReturn = app.triggerEvent(controller, 'hide', null), dtdHide = validate.isDeferred(hideReturn) ? hideReturn : $.Deferred().resolve();

            dtdHide.done(function () {
                view.css('visibility', 'hidden');
                callback();
            });
        };

        application.prototype.destroyModule = function (moduleInstance) {
            var app = this;
            for (var o in moduleInstance.controllers) {
                app.triggerEvent(moduleInstance.controllers[o], 'destroy', null);
                moduleInstance.views[o].remove();
            }

            moduleInstance.name = null;
            moduleInstance.viewName = null;
            moduleInstance.views = {};
            moduleInstance.controllers = {};
        };

        application.prototype.triggerEvent = function (controller, eventName, parm) {
            if (controller.events != undefined && $.isFunction(controller.events[eventName])) {
                return controller.events[eventName](parm);
            }
        };

        application.prototype.bindViewEvent = function (view, controller) {
            var controls = view.find('[data-event]');
            controls.each(function () {
                var view = $(this), eventInfo = view.data('event').split(':'), eventName = eventInfo.length === 2 ? eventInfo[0] : 'click', actionName = eventInfo.length === 2 ? eventInfo[1] : eventInfo[0];

                if (controller.actions[actionName] === undefined)
                    console.error("找不到指定的事件处理方法，方法名称：" + actionName);

                if (eventName.indexOf('(') == -1) {
                    view.bind(eventName, controller.actions[actionName]);
                } else {
                    var b = eventName.indexOf('('), e = eventName.indexOf(')'), filter = eventName.substring(b + 1, e);

                    eventName = eventName.substring(0, b);
                    view.on(eventName, filter, controller.actions[actionName]);
                }
            });
        };

        application.prototype.loadResourceFiles = function (callback) {
            var instance = this;
            easyweb.appDomain.loadCSSFiles(instance.settings.resource.CSS);
            easyweb.appDomain.loadScriptFiles(instance.settings.resource.JS, function () {
                easyweb.appDomain.loadScriptFiles(instance.settings.resource.Modules, function () {
                    easyweb.appDomain.loadHtmlViews(instance.settings.resource.Views, instance.moduleViewsCache, function () {
                        easyweb.appDomain.loadImgFiles(instance.settings.resource.Images, function () {
                            callback();
                        });
                    });
                });
            });
        };

        application.prototype.getModuleInfoByHash = function () {
            var hash = location.hash.substr(1);

            if (hash.length === 0)
                return null;
            if (hash === '/')
                return null;

            if (hash.indexOf('?') > -1)
                hash = hash.substr(0, hash.indexOf('?'));
            if (hash[0] === '/')
                hash = hash.substr(1);
            if (hash[hash.length - 1] === '/')
                hash = hash.substr(0, hash.length - 1);

            var hashInfo = hash.split('/');

            return (hashInfo.length === 1) ? hash : hashInfo[0] + '.' + hashInfo[1];
        };

        application.prototype.getParmsByHash = function () {
            var hash = location.hash.substr(1);

            if (hash.length === 0)
                return null;
            if (hash === '/')
                return null;

            if (hash.indexOf('?') > -1)
                hash = hash.substr(0, hash.indexOf('?'));
            if (hash[0] === '/')
                hash = hash.substr(1);
            if (hash[hash.length - 1] === '/')
                hash = hash.substr(0, hash.length - 1);

            var hashInfo = hash.split('/');
            if (hashInfo.length == 3) {
                return JSON.parse(hashInfo[2]);
            }
            return null;
        };

        application.prototype.getViewName = function (viewName, controllers) {
            if (viewName === undefined || controllers[viewName] === undefined) {
                for (var name in controllers) {
                    viewName = name;
                    break;
                }
            }
            return viewName;
        };

        application.prototype.loadViewByHash = function () {
            var app = this;
            if (app.isOnUpdateHash) {
                app.isOnUpdateHash = false;
                return;
            }

            var moduleInfo = app.getModuleInfoByHash(), parm = app.getParmsByHash(), nameInfo = validate.isNull(moduleInfo) ? [null, null] : moduleInfo.split('.'), moduleName = nameInfo[0], targetView = nameInfo[1];

            if (app.currentModule.name == moduleName) {
                app.changeView(targetView, parm);
            } else {
                app.loadModule(moduleInfo, null, parm);
            }
        };

        application.prototype.updateLocationHash = function (parm) {
            var app = this, href = url.getNoAnchorUrl(), hash = '#/' + app.currentModule.name + '/' + app.currentModule.viewName;

            if (parm != undefined)
                hash += '/' + $.toJSON(parm);

            app.isOnUpdateHash = true;
            if (app.disableHistory) {
                //console.info('replace:' + href + hash);
                window.top.location.replace(href + hash);
                app.disableHistory = false;
            } else {
                //console.info('href:' + href + hash);
                window.top.location.href = href + hash;
            }
            setTimeout(function () {
                app.isOnUpdateHash = false;
            }, 100);

        };
        return application;
    })();
    easyweb.application = application;

    var asyncTask = (function () {
        function asyncTask() {
        }
        asyncTask.runTaskList = function (taskList, callback) {
            var arrayOfPromises = [];
            for (var i = 0; i < taskList.length; i++) {
                var task = taskList[i];
                arrayOfPromises.push(task());
            }

            jQuery.when.apply(jQuery, arrayOfPromises).done(function () {
                var resultList = arguments;
                callback(resultList);
            }).fail(function (errorInfo) {
                console.error('asyncTask.runTaskList:', errorInfo);
            });
        };

        asyncTask.runTaskQueue = function (taskList, callback, resultList) {
            if (typeof resultList === "undefined") { resultList = []; }
            if (taskList.length == 0) {
                callback([]);
                return;
            }

            var task = taskList[0], runCallBack = function (taskList, callback, resultList) {
                taskList.shift();
                if (taskList.length == 0)
                    callback(resultList);
                else
                    asyncTask.runTaskQueue(taskList, callback, resultList);
            };

            task().done(function (result) {
                resultList.push(result);
                runCallBack(taskList, callback, resultList);
            }).fail(function (errorInfo) {
                //console.error('asyncTask.runTaskQueue:', errorInfo);
            });
        };

        asyncTask.waitTaskList = function (deferredList, callback) {
            var dtd = $.Deferred();
            jQuery.when.apply(jQuery, deferredList).done(function () {
                var resultList = arguments;
                dtd.resolve(resultList);
                if ($.isFunction(callback))
                    callback(resultList);
            });
            return dtd;
        };

        asyncTask.waitTaskQueue = function (taskList) {
            var dtd = $.Deferred();
            asyncTask.runTaskQueue(taskList, function () {
                dtd.resolve();
            });
            return dtd;
        };
        return asyncTask;
    })();
    easyweb.asyncTask = asyncTask;

    var eventManager = (function () {
        function eventManager() {
            this.handleList = {};
        }
        /**
        * 添加绑定事件处理方法
        * @param eventName 事件名称，如click或click.custom01，当事件名称带有后缀时，
        *                  可以选择只删除匹配的事件绑定，而不会删除其它事件的绑定。
        * @param handle 事件处理方法
        */
        eventManager.prototype.bind = function (eventName, handle) {
            if (this.handleList[eventName] instanceof Array === false) {
                this.handleList[eventName] = new Array();
            }
            this.handleList[eventName].push(handle);
        };

        /**
        * 解除事件绑定处理方法
        * @param eventName 事件名称，如：
        *                  click：删除所有click事件
        *                  .custom01：删除所有后缀为custom01的事件，如mousemove.custom01,mouseleave.custom01。
        *                  click.custom01：仅删除后缀为custom01的click事件。
        */
        eventManager.prototype.unbind = function (eventName) {
            var mode = 'global', pos = eventName.indexOf('.');

            if (pos === 0)
                mode = 'filter';
            if (pos > 0)
                mode = 'single';

            var tmpEventName;

            switch (mode) {
                case 'global':
                    for (tmpEventName in this.handleList) {
                        if (tmpEventName.indexOf(eventName + '.') === 0)
                            delete this.handleList[tmpEventName];
                    }
                    break;

                case 'filter':
                    for (tmpEventName in this.handleList) {
                        if (tmpEventName.indexOf(eventName) > 0)
                            delete this.handleList[tmpEventName];
                    }
                    break;

                case 'single':
                    for (tmpEventName in this.handleList) {
                        if (tmpEventName === eventName)
                            delete this.handleList[tmpEventName];
                    }
                    break;
            }
        };

        /**
        * 触发事件处理方法
        * @param eventName 事件名称，如click,mousemove
        * @param params 事件处理方法所需的自定义参数。
        */
        eventManager.prototype.trigger = function (eventName, params) {
            var tmpEventName;
            for (tmpEventName in this.handleList) {
                if (tmpEventName === eventName || tmpEventName.indexOf(eventName + '.') === 0) {
                    for (var i = 0; i < this.handleList[tmpEventName].length; i++) {
                        this.handleList[tmpEventName][i](params);
                    }
                }
            }
        };
        return eventManager;
    })();
    easyweb.eventManager = eventManager;

    var server = (function () {
        function server() {
        }
        server.load = function (url, parms, callback) {
            var waitmsg = appDomain.language == 'zh-cn' ? '' : 'Now loading...';
            //var waitmsg = appDomain.language == 'zh-cn' ? '正在载入...' : 'Now loading...';
            return server.run(url, parms, waitmsg, function (serviceMethodResult) {
                if (server.isSuccess(serviceMethodResult))
                    if ($.isFunction(callback))
                        callback(serviceMethodResult.ContextData);
            });
        };

        server.save = function (url, parms, callback) {
            var waitmsg = appDomain.language == 'zh-cn' ? '正在保存...' : 'Now saving...';
            return server.run(url, parms, waitmsg, function (serviceMethodResult) {
                if (server.isSuccess(serviceMethodResult))
                    if ($.isFunction(callback))
                        callback(serviceMethodResult.ContextData);
            });
        };

        server.remove = function (url, parms, callback) {
            var waitmsg = appDomain.language == 'zh-cn' ? '正在删除...' : 'Now removing...';
            return server.run(url, parms, waitmsg, function (serviceMethodResult) {
                if (server.isSuccess(serviceMethodResult))
                    if ($.isFunction(callback))
                        callback(serviceMethodResult.ContextData);
            });
        };

        server.run = function (url, parms, waitmsg, callback) {
            var dtd = $.Deferred(), delayShowTime = 500, isNeedTip = waitmsg == null ? false : true, tip = {
                close: function () {
                }
            }, hideTip = function () {
                isNeedTip = false;
                server.isTipOnShow = false;
                setTimeout(function () {
                    tip.close();
                }, delayShowTime + 200);
            };

            setTimeout(function () {
                if (isNeedTip) {
                    if (!server.isTipOnShow) {
                        tip = easyweb.tipbox.waiting(waitmsg);
                        server.isTipOnShow = true;
                    }
                }
            }, delayShowTime);

            $.ajax({
                type: "post",
                async: true,
                dataType: "json",
                url: url,
                data: parms,
                success: function (apiResult) {
                    hideTip();
                    if (apiResult.Status == "SUCCESS") {
                        dtd.resolve(apiResult.ContextData);
                    } else {
                        dtd.reject(apiResult.Message);
                    }
                    if ($.isFunction(callback))
                        callback(apiResult);
                },
                error: function (httpRequest, textStatus, errorThrown) {
                    hideTip();
                    var apiResult = {};
                    apiResult.Status = "ERROR";
                    if (httpRequest.status == "404") {
                        apiResult.Message = appDomain.language == "zh-cn" ? '所要访问的远程服务不存在。远程服务地址：' + url : 'The remote address does not exist.Remote address:' + url;
                    } else {
                        apiResult.Message = appDomain.language == "zh-cn" ? '服务器内部发生错误' : 'Server Internal Error';
                        apiResult.ContextData = httpRequest.responseText;
                    }
                    server.isSuccess(apiResult);
                    dtd.reject(apiResult.Message);
                }
            });
            return dtd;
        };

        server.isSuccess = function (apiResult) {
            var instance = this;

            if (!validate.isJson(apiResult)) {
                easyweb.dialog.alert(appDomain.language == 'zh-cn' ? '服务端返回的结果不是一个有效的JSON对象。' : 'The result of server is not json format.');
                return false;
            }

            switch (apiResult.Status) {
                case 'SUCCESS':
                    return true;
                case 'WARN':
                    easyweb.dialog.alert(apiResult.Message);
                    return false;

                case 'ERROR':
                    //easyweb.dialog.error(apiResult.Message);
                    return false;

                case "NOTLOGIN":
                    if ($.isFunction(instance.onNotLogin))
                        instance.onNotLogin();
                    return false;
            }

            return false;
        };
        server.isTipOnShow = false;

        server.debugMode = false;

        server.onNotLogin = null;
        return server;
    })();
    easyweb.server = server;

    var timer = (function () {
        function timer() {
            this.timerFlag = 0;
            this.interval = 1000;
            this.elapsed = null;
        }
        timer.prototype.start = function () {
            var instance = this;
            instance.timerFlag = setInterval(function () {
                if ($.isFunction(instance.elapsed))
                    instance.elapsed();
            }, instance.interval);
        };

        timer.prototype.stop = function () {
            var instance = this;
            clearInterval(instance.timerFlag);
        };
        return timer;
    })();
    easyweb.timer = timer;

    var url = (function () {
        function url() {
        }
        /**
        * 获取一个url请求所包含的参数信息，如：http://localhost:8089/userInfo.ashx?id=1&ver=1.0 ，将返回{id:"1",ver:"1.0"}。
        * @param url url地址
        */
        url.getUrlParms = function (url) {
            if (url == undefined)
                url = window.location.href;

            var parms = {}, bPos = url.indexOf('?'), ePos = url.indexOf('#');

            if (bPos === -1)
                return parms;
            if (ePos === -1)
                ePos = url.length;

            var parmStr = url.substring(bPos + 1, ePos), parmList = parmStr.split('&');
            for (var i = 0; i < parmList.length; i++) {
                var parmInfo = parmList[i].split('=');
                if (parmInfo.length == 2) {
                    parms[parmInfo[0]] = parmInfo[1];
                }
            }
            return parms;
        };

        url.getUrlPath = function () {
            var location = window.top.location;

            return location.protocol + "//" + location.hostname + (location.port ? ':' + location.port : '') + location.pathname;
        };

        url.getNoAnchorUrl = function () {
            var url = window.location.href;
            var bPos = url.indexOf('#');
            if (bPos > -1) url = url.substring(0, bPos);
            return url;
        };
        return url;
    })();
    easyweb.url = url;

    var utility = (function () {
        function utility() {
        }
        utility.htmlEncode = function (html) {
            return $('<div></div>').text(html).html();
        };

        utility.htmlDecode = function (html) {
            return $('<div></div>').html(html).text();
        };

        utility.isSupportFlash = function () {
            var support;
            var userAgent = navigator.userAgent.toLowerCase();
            var msie = /msie/.test(userAgent) && !/opera/.test(userAgent);
            if (msie) {
                try {
                    support = new ActiveXObject("ShockwaveFlash.ShockwaveFlash");
                } catch (error) {
                }
            } else {
                support = navigator.plugins["Shockwave Flash"];
            }
            return (support) !== undefined;
        };
        return utility;
    })();
    easyweb.utility = utility;

    var validate = (function () {
        function validate() {
        }
        validate.isNotNull = function (str) {
            return validate.isNull(str) ? false : true;
        };

        validate.isNull = function (obj) {
            return obj == undefined || obj == null || obj == '';
        };

        validate.isJson = function (obj) {
            var isjson = typeof (obj) == "object" && Object.prototype.toString.call(obj).toLowerCase() == "[object object]" && !obj.length;
            return isjson;
        };

        validate.isInt = function (str) {
            var exp = /^[-]{0,1}[0-9]*$/;
            return exp.test(str);
        };

        validate.isUint = function (str) {
            var exp = /^\+?[1-9][0-9]*$/;
            return exp.test(str);
        };

        validate.isFloat = function (str) {
            var exp = /^[-]{0,1}[0-9]*[.]{0,1}[0-9]{0,25}$/;
            return exp.test(str);
        };

        validate.isMoney = function (str) {
            if (str == 'free')
                return true;
            var tmp = str.replaceAll(',', ''), exp = /^[-]{0,1}[0-9]*[.]{0,1}[0-9]{0,25}$/;
            return exp.test(tmp);
        };

        validate.isMobile = function (str) {
            var patrn = /^1[3|4|5|7|8][0-9]\d{8}$/;
            return patrn.test(str);
        };

        validate.isBasicChar = function (str) {
            var patrn = /^[A-Za-z\u4e00-\u9fa5]+$/;
            return patrn.test(str);
        };

        validate.isBasicCharAnd = function (str) {
            var patrn = /^[A-Za-z\u4e00-\u9fa5\0-9]+$/;
            return patrn.test(str);
        };

        validate.isDate = function (str, allowNull) {
            if (easyweb.validate.isNull(str) && allowNull != true)
                return true;
            var tmpDate = new Date(str.replace(/-/g, "/"));
            return tmpDate.format('yyyy-MM-dd') !== "NaN-aN-aN";
        };

        validate.isTime = function (str) {
            var result = str.match(/^(\d{1,2})(:)(\d{1,2})\2(\d{1,2})$/);

            if (result == null)
                return false;

            var currentDate = new Date();
            var d = new Date(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDay(), parseInt(result[1]), parseInt(result[3]), parseInt(result[4]));
            return (d.getHours() == parseInt(result[1]) && d.getMinutes() == parseInt(result[3]) && d.getSeconds() == parseInt(result[4]));
        };

        validate.isMaxLen = function (str, lenStr) {
            var maxLen = lenStr.indexOf('(') > -1 ? parseInt(lenStr.substring(1, lenStr.length - 1)) : parseInt(lenStr);
            return str.length <= maxLen;
        };

        validate.isMinLen = function (str, lenStr) {
            var minLen = lenStr.indexOf('(') > -1 ? parseInt(lenStr.substring(1, lenStr.length - 1)) : parseInt(lenStr);
            return str.length >= minLen;
        };

        validate.isLen = function (str, lenStr) {
            var len = lenStr.indexOf('(') > -1 ? parseInt(lenStr.substring(1, lenStr.length - 1)) : parseInt(lenStr);
            return str.length == len;
        };

        validate.isBetween = function (numStr, parmStr) {
            var num = parseFloat(numStr);
            var parm = parmStr.substring(1, parmStr.length - 1);
            var min = parseFloat(parm.split('-')[0]);
            var max = parseFloat(parm.split('-')[1]);
            return num >= min && num <= max;
        };

        validate.isGreaterThan = function (numStr, minStr) {
            if (numStr == "")
                return true;

            var num = parseFloat(numStr);
            var minNum = parseFloat(minStr.substring(1, minStr.length - 1));
            return num >= minNum;
        };

        validate.isEmail = function (str) {
            var reg = /^[A-Za-z0-9d]+([-_.][A-Za-z0-9d]+)*@([A-Za-z0-9d]+[-.])+[A-Za-zd]{2,5}$/;
            return reg.test(str);
        };

        validate.isURL = function (str) {
            if (str === 'http://' || str === 'https://')
                return false;
            return !!str.match(/(((^https?:(?:\/\/)?)(?:[-;:&=\+\$,\w]+@)?[A-Za-z0-9.-:]+|(?:www.|[-;:&=\+\$,\w]+@)[A-Za-z0-9.-]+)((?:\/[\+~%\/.\w-_]*)?\??(?:[-\+=&;%@.\w_]*)#?(?:[\w]*))?)(:[0-9]{1,4})?$/g);
        };

        validate.isInArray = function (str, parmStr) {
            var parm = parmStr.substring(1, parmStr.length - 1);
            var array = parm.split('|');
            return array.indexOf(str) > -1;
        };

        validate.isDeferred = function (dtd) {
            if (validate.isNull(dtd))
                return false;
            return $.isFunction(dtd.done);
        };

        validate.getNotNullMsg = function (memberName) {
            return (easyweb.appDomain.language === "zh-cn") ? '“' + memberName + '”不能为空，请您重新输入。' : '"' + memberName + '" is required, Please input again.';
        };

        validate.getIntMsg = function (memberName) {
            return (easyweb.appDomain.language === "zh-cn") ? '“' + memberName + '”必需为一个整数，请您重新输入。' : '"' + memberName + '" must be a integer, Please input again.';
        };

        validate.getUintMsg = function (memberName) {
            return (easyweb.appDomain.language === "zh-cn") ? '“' + memberName + '”必需为一个正整数，请您重新输入。' : '"' + memberName + '" must be a positive integer, Please input again.';
        };

        validate.getFloatMsg = function (memberName) {
            return (easyweb.appDomain.language === "zh-cn") ? '“' + memberName + '”必需为一个数值，请您重新输入。' : '"' + memberName + '" must be a number, Please input again.';
        };

        validate.getMoneyMsg = function (memberName) {
            return (easyweb.appDomain.language === "zh-cn") ? '“' + memberName + '”不是一个有效的货币格式，请您重新输入。' : '"' + memberName + '" must be a number, Please input again.';
        };

        validate.getBasicCharMsg = function (memberName) {
            return (easyweb.appDomain.language === "zh-cn") ? '“' + memberName + '”只能由中英文字符组成，请您重新输入。' : '"' + memberName + '" must be simple char, Please input again.';
        };

        validate.getMobileMsg = function (memberName) {
            return (easyweb.appDomain.language === "zh-cn") ? '“' + memberName + '”格式错误，请您重新输入。' : '"' + memberName + '" format error, Please input again.';
        };

        validate.getDateMsg = function (memberName) {
            return (easyweb.appDomain.language === "zh-cn") ? '“' + memberName + '”日期格式错误，正确的格式为：2016-10-01，请您重新输入。' : '"' + memberName + '" date format wrong, the corect format is:yyyy-mm-dd, Please input again.';
        };

        validate.getTimeMsg = function (memberName) {
            return (easyweb.appDomain.language === "zh-cn") ? '“' + memberName + '”时间格式错误，正确的格式为：08:30，请您重新输入。' : '"' + memberName + '" date format wrong, the corect format is:yyyy-mm-dd, Please input again.';
        };

        validate.getEmailMsg = function (memberName) {
            return (easyweb.appDomain.language === "zh-cn") ? '“' + memberName + '”不是一个有效的邮件地址，请您重新输入。' : '"' + memberName + '" is not a valid email address, Please input again.';
        };

        validate.getURLMsg = function (memberName) {
            return (easyweb.appDomain.language === "zh-cn") ? '“' + memberName + '”不是一个有效的URL地址，请您重新输入' : '"' + memberName + '" must be a valid url, Please input again.';
        };

        validate.getMaxLenMsg = function (memberName, lenStr) {
            var maxLen = lenStr.indexOf('(') > -1 ? parseInt(lenStr.substring(1, lenStr.length - 1)) : parseInt(lenStr);
            return (easyweb.appDomain.language === "zh-cn") ? '“' + memberName + '”长度不能大于' + maxLen + '字符，请您重新输入。' : '"' + memberName + '" can\'t more than ' + maxLen + ' character, Please input again.';
        };

        validate.getMinLenMsg = function (memberName, lenStr) {
            var minLen = lenStr.indexOf('(') > -1 ? parseInt(lenStr.substring(1, lenStr.length - 1)) : parseInt(lenStr);
            return (easyweb.appDomain.language === "zh-cn") ? '“' + memberName + '”长度不能少于' + minLen + '个字符，请您重新输入。' : '"' + memberName + '" can\'t less than ' + minLen + ' character, Please input again.';
        };

        validate.getLenMsg = function (memberName, lenStr) {
            var len = lenStr.indexOf('(') > -1 ? parseInt(lenStr.substring(1, lenStr.length - 1)) : parseInt(lenStr);
            return (easyweb.appDomain.language === "zh-cn") ? '“' + memberName + '”字符长度必需为' + len + '字符，请您重新输入。' : '"' + memberName + '" must equal to ' + len + ', Please input again.';
        };

        validate.getGreaterThanMsg = function (memberName, minStr) {
            var minNum = minStr.indexOf('(') > -1 ? parseInt(minStr.substring(1, minStr.length - 1)) : parseInt(minStr);
            return (easyweb.appDomain.language === "zh-cn") ? '“' + memberName + '”必需大于等于 ' + minNum + ' ，请您重新输入。' : '"' + memberName + '" must be a number greater than or equal to ' + minNum + ', Please input again.';
        };

        validate.getBetweenMsg = function (memberName, parmStr) {
            var parm = parmStr.substring(1, parmStr.length - 1);
            var min = parseFloat(parm.split('-')[0]);
            var max = parseFloat(parm.split('-')[1]);
            return (easyweb.appDomain.language === "zh-cn") ? '“' + memberName + '”必需介于 ' + min + ' 和 ' + max + ' 之间，请您重新输入。' : '"' + memberName + '" must be a number between ' + min + ' to ' + max + ', Please input again.';
        };

        validate.getInArrayMsg = function (memberName) {
            return (easyweb.appDomain.language === "zh-cn") ? '“' + memberName + '”超出了规定的范围，请您重新输入。' : '"' + memberName + '" must out of the range, Please input again.';
        };
        return validate;
    })();
    easyweb.validate = validate;


})(easyweb || (easyweb = {}));

var appMain;
(function (appMain) {
    //在页面载入完成后，运行App入口Javascript脚本
    window.onload = function () {
        easyweb.appDomain.startMain();
    };
})(appMain || (appMain = {}));


(function () {
    easyweb.binder = {

        getProperty: function (json, name) {
            if (name.indexOf('.') > -1) {
                var pos = name.indexOf('.');
                var name1 = name.substring(0, pos);
                var name2 = name.substring(pos + 1, name.length);
                if (json[name1] != null)
                    return easyweb.binder.getProperty(json[name1], name2);
                else
                    return null;
            }
            else {
                return json[name];
            }
        },

        setProperty: function (json, name, value) {
            if (name.indexOf('.') > -1) {
                var pos = name.indexOf('.');
                var name1 = name.substring(0, pos);
                var name2 = name.substring(pos + 1, name.length);
                if (json[name1] == null || json[name1] == undefined) json[name1] = {};
                easyweb.binder.setProperty(json[name1], name2, value);
            }
            else {
                json[name] = value;
                if (json[name] === 'true') json[name] = true;
                if (json[name] === 'false') json[name] = false;
            }
        },

        bindControlToJson: function (container, defaultValue) {


            var json = defaultValue == null ? {} : $.parseJSON(JSON.stringify(defaultValue));
            var dataList = [];

            container.find('input[type=text],input[type=tel],input[type=hidden],input[type=password],textarea,select').each(function () {
                var memberName = $(this).attr('data-member');
                if (easyweb.validate.isNotNull(memberName)) {
                    var value = $.trim($(this).val()) == "" ? null : $.trim($(this).val());

                    var rule = $(this).attr('data-valid');
                    if (easyweb.validate.isNotNull(rule)) {
                        var rules = rule.split(',');
                        if (rules.indexOf('Money') > -1) {
                            if (value != null) {
                                value = value.toString().replaceAll(',', '');
                            }
                        }
                    }
                    dataList.push({ Id: memberName, Value: value });
                }
            });

            for (var n = 0; n < dataList.length; n++) {
                easyweb.binder.setProperty(json, dataList[n].Id, dataList[n].Value);
            }

            return json;
        },

        bindJsonToControl: function (container, json) {
            container.find('input[type=text],input[type=hidden],input[type=password],textarea,input[type=checkbox]').each(function () {
                var memberName = $(this).attr('data-member');
                if (easyweb.validate.isNotNull(memberName)) {
                    var control = $(this),
                        property = easyweb.binder.getProperty(json, memberName);
                    if (property !== undefined) {
                        if (property == null) property = '';
                        else property = property.toString();
                        if ($(this).attr("type") == "checkbox" && property.toString() == "true") {
                            $(this).prop("checked", true);
                        }
                        else
                            control.val(property);
                    }


                    var rule = control.attr('data-valid');
                    if (easyweb.validate.isNotNull(rule)) {
                        var rules = rule.split(',');
                        if (rules.indexOf('Money') > -1) {
                            initMoneyControl(control);
                        }
                        else if (rules.indexOf('Date') > -1) {
                            initDateControl(control);
                        }
                    }

                }
            });

            container.find('select').each(function () {
                var memberName = $(this).attr('data-member');
                if (easyweb.validate.isNotNull(memberName)) {
                    var property = easyweb.binder.getProperty(json, memberName);
                    if (property !== undefined) {
                        if (property == null) property = '';
                        else property = property.toString();

                        var select = $(this);
                        select.find('option').each(function () {
                            var option = $(this);
                            if (option.attr('value') == property)
                                option.prop('selected', true);
                        });
                    }
                }
            });

            container.find('span,label,li').each(function () {
                var memberName = $(this).attr('data-member');
                if (easyweb.validate.isNotNull(memberName)) {
                    var property = easyweb.binder.getProperty(json, memberName);
                    if (property !== undefined) $(this).text(property);
                }

                if (easyweb.validate.isNotNull(memberName)) {
                    if (easyweb.binder.getProperty(json, memberName) != undefined) $(this).text(easyweb.binder.getProperty(json, memberName));
                }
            });

            container.find('iframe').each(function () {
                var memberName = $(this).attr('data-member');
                if (easyweb.validate.isNotNull(memberName)) {
                    var body = $($(this)[0].contentWindow.document.body);
                    var property = easyweb.binder.getProperty(json, memberName);

                    if (property !== undefined) body.html(property);
                }
            });

            function initMoneyControl(control) {
                var format = function () {
                    var money = easyweb.getMoneyFormat(control.val());
                    control.val(money);
                };
                format();
                control.unbind('blur.ControlMoney'); // 先解除相关事件绑定，防止重复绑定
                control.bind('blur.ControlMoney', function () {
                    var num = control.val().toString().replace(/\$|\,/g, '');
                    if (isNaN(num)) {
                        aweb.dialog.alert('“' + control.data('title') + '”不是一个有效的数字，请您重新输入。');
                        return;
                    }
                    format();
                });
            }

            function initDateControl(control) {
                new easyweb.datePicker(control);

                //control.unbind('focus.ControlDate'); // 先解除相关事件绑定，防止重复绑定
                //control.bind('focus.ControlDate', function () {
                //    // WdatePicker({ dateFmt: 'yyyy-MM-dd' });
                //    new easyweb.datePicker(control);
                //});
            }
        },

        bindGridToJson: function (grids, json) {
            if (grids.dataMember != undefined) {
                var grid = grids;
                if (grid.dataMember != undefined) {
                    easyweb.binder.setProperty(json, grid.dataMember, grid.dataSource);
                }
            }
            else {
                for (var p in grids) {
                    var grid = grids[p];
                    if (grid.dataMember != undefined) {
                        easyweb.binder.setProperty(json, grid.dataMember, grid.dataSource);
                    }
                }
            }
        },

        bindJsonToGrid: function (grids, json) {
            if (grids.dataMember != undefined) {
                var grid = grids;
                if (grid.dataMember != undefined) {
                    grid.dataSource = easyweb.binder.getProperty(json, grid.dataMember);
                    grid.dataBind();
                    grid.refresh();
                }
            }
            else {
                for (var p in grids) {
                    var grid = grids[p];
                    if (grid.dataMember != undefined) {
                        grid.dataSource = easyweb.binder.getProperty(json, grid.dataMember);
                        grid.dataBind();
                        grid.refresh();
                    }
                }
            }
        },

        checkValid: function (container) {
            var dataList = [];
            container.find('input[type=text],input[type=tel],input[type=hidden],input[type=password],textarea,select').each(function () {
                var rule = $(this).attr('data-valid');
                if (easyweb.validate.isNotNull(rule))
                    dataList.push({ Title: $(this).data('title'), Rule: rule, Value: $(this).val(), obj: $(this) });
            });
            for (var n = 0; n < dataList.length; n++) {
                var item = dataList[n];
                var rules = item.Rule.split(',');
                for (var i = 0; i < rules.length; i++) {
                    var rule = rules[i];
                    var pos = Math.max(rule.indexOf('('), rule.indexOf('['));
                    if (!easyweb.validate.isNotNull(rule)) continue;

                    // 无参数型有效性检查，如isInt,isNotNull等。
                    if (pos == -1) {
                        var func = rule[0].toUpperCase() + rule.substr(1);
                        if (!easyweb.validate['is' + func](item.Value)) {
                            var msg = easyweb.validate['get' + func + 'Msg'](item.Title);
                            //item.obj.attr("style", "border-bottom: 1px solid red");

                            easyweb.dialog.alert(msg);
                            return false;
                        }
                        else {
                            //item.obj.attr("style", "");
                        }
                    }
                    else {  // 有参数型有效性检查，如isBetween(100,500),isInArray["A","B","C"]。
                        var func = rule.substring(0, pos);
                        var parm = rule.substring(pos, rule.length);
                        func = func[0].toUpperCase() + func.substr(1);
                        if (!easyweb.validate['is' + func](item.Value, parm)) {
                            var msg = easyweb.validate['get' + func + 'Msg'](item.Title, parm);
                            //item.obj.attr("style", "border-bottom: 1px solid red");
                            easyweb.dialog.alert(msg);
                            return false;
                        } else {
                            //item.obj.attr("style", "");
                        }
                    }
                }
            }
            return true;

        },

        bindSelect: function (control, dataSource, options) {
            var settings = $.extend({
                value: null,
                displayMember: 'Name',
                valueMember: 'Id',
                parentMember: 'ParentId',
                parentSelect: null,
                emptyOption: null,//{ value: "0", text: '=zhe='}
                parentChange: null
            }, options);

            var fillData = function () {
                control.empty();
                if (settings.emptyOption) {
                    option = $('<option value="' + settings.emptyOption.value + '">' + settings.emptyOption.text + '</option>');
                    control.append(option);
                }

                if (settings.parentSelect === null) {
                    for (var i = 0; i < dataSource.length; i++) {
                        var item = dataSource[i],
                            text = item[settings.displayMember],
                            value = item[settings.valueMember],
                            option = $('<option value="' + value + '">' + easyweb.utility.htmlEncode(text) + '</option>');

                        if (settings.value != null && settings.value == value) {
                            option.prop('selected', true);
                        }
                        control.append(option);
                    }
                }
                else {
                    for (var i = 0; i < dataSource.length; i++) {
                        var item = dataSource[i],
                            text = item[settings.displayMember],
                            value = item[settings.valueMember],
                            option = $('<option value="' + value + '">' + text + '</option>');

                        if (settings.value != null && settings.value == value) {
                            option.prop('selected', true);
                        }

                        var parentId = item[settings.parentMember];
                        if (parentId == settings.parentSelect.val() || parentId == '00000000-0000-0000-0000-000000000000') {
                            control.append(option);
                        }
                    }
                    control.change();
                }
            };


            fillData();

            if (settings.parentSelect !== null) {
                settings.parentSelect.change(function () {
                    fillData();
                    if (settings.parentChange != null){
                        settings.parentChange();
                    }
                });
            }
        }



    };
    easyweb.getHtmlByTemplate = function (tmplateHtml, obj, htmlMode, prePer) {
        var newHtml = tmplateHtml;
        for (var o in obj) {
            var perName = prePer == undefined ? "{" + o + "}" : "{" + prePer + "." + o + "}";
            var value = obj[o] === null ? '' : obj[o];
            var type = typeof value;

            if (type == "object") {
                var newPre = prePer == undefined ? o : prePer + "." + o;
                newHtml = easyweb.getHtmlByTemplate(newHtml, value, newPre);
            }
            else {
                if (htmlMode) {
                    newHtml = newHtml.replaceAll(perName, value);
                }
                else {
                    newHtml = newHtml.replaceAll(perName, easyweb.utility.htmlEncode(value));
                }
            }
        }
        return newHtml;
    };
})();

(function () {
    easyweb.tipbox = function (message) {
        var body = $(window.top.document).find('body'),
            instance = this;

        // var tipControl = $('<div class="weui_loading_toast" style="display:block"><div class="weui_mask_transparent"></div><div class="weui_toast"><div class="weui_loading"><div class="weui_loading_leaf weui_loading_leaf_0"></div><div class="weui_loading_leaf weui_loading_leaf_1"></div><div class="weui_loading_leaf weui_loading_leaf_2"></div><div class="weui_loading_leaf weui_loading_leaf_3"></div><div class="weui_loading_leaf weui_loading_leaf_4"></div><div class="weui_loading_leaf weui_loading_leaf_5"></div><div class="weui_loading_leaf weui_loading_leaf_6"></div><div class="weui_loading_leaf weui_loading_leaf_7"></div><div class="weui_loading_leaf weui_loading_leaf_8"></div><div class="weui_loading_leaf weui_loading_leaf_9"></div><div class="weui_loading_leaf weui_loading_leaf_10"></div><div class="weui_loading_leaf weui_loading_leaf_11"></div></div><p class="weui_toast_content"></p></div></div>');
        var tipControl = $('<div id="loadingToast" style="opacity: 1; display: block;">'
            + '<div class="weui-mask_transparent"></div>'
            + '<div class="weui-toast">'
            + '<i class="weui-loading weui-icon_toast"></i>'
            + '<p class="weui-toast__content">数据加载中</p>'
            + '</div>'
            + '</div>');


        this.show = function () {
            tipControl.find('.weui-toast__content').text(message);
            tipControl.appendTo(body);
        }

        this.close = function () {
            tipControl.remove();
        }



    }

    easyweb.tipbox.showBox = function (message) {
        var tip = new easyweb.tipbox(message);
        tip.show();
        return tip;
    };

    easyweb.tipbox.waiting = function (msg) { return easyweb.tipbox.showBox(msg); };

})();

(function () {
    easyweb.dialog = function () { };

    easyweb.dialog.style = 'chevrolet';

    easyweb.dialog.alertDialogContent = {
        'weiui': '<div class="weui_dialog_alert"><div class="weui_mask"></div><div class="weui_dialog"><div class="weui_dialog_hd"><strong class="weui_dialog_title">系统提示</strong></div><div class="weui_dialog_bd dialogMessage"></div><div class="weui_dialog_ft"><a href="javascript:;" class="weui_btn_dialog primary btnOK">确定</a></div></div></div>',
        'chevrolet': '<div class="wechatDialogBox"><div class="wechatMask"></div><div class="wechatDialog"><div class="btnClose button"></div><div class="title"></div><div class="message dialogMessage"></div><div class="btnOK button">确定</div></div></div>'
    };

    easyweb.dialog.alertDialogContentWithNoOK = {
        'weiui': '<div class="weui_dialog_alert"><div class="weui_mask"></div><div class="weui_dialog"><div class="weui_dialog_hd"><strong class="weui_dialog_title">系统提示</strong></div><div class="weui_dialog_bd dialogMessage"></div><div class="weui_dialog_ft"><a href="javascript:;" class="weui_btn_dialog primary btnOK">确定</a></div></div></div>',
        'chevrolet': '<div class="wechatDialogBox"><div class="wechatMask"></div><div class="wechatDialog"><div class="btnClose button"></div><div class="title"></div><div class="message dialogMessage"></div></div></div>'
    };

    easyweb.dialog.confirmDialogContent = {
        'weiui': '<div class="weui_dialog_confirm"><div class="weui_mask"></div><div class="weui_dialog"><div class="weui_dialog_hd"><strong class="weui_dialog_title">系统提示</strong></div><div class="weui_dialog_bd dialogMessage"></div><div class="weui_dialog_ft"> <a href="javascript:;" class="weui_btn_dialog default btnCancel">取消</a><a href="javascript:;" class="weui_btn_dialog primary btnOK">确定</a></div></div></div>',
        'chevrolet': '<div class="wechatDialogBox"><div class="wechatMask"></div><div class="wechatDialog"><div class="btnClose button"></div><div class="title"></div><div class="message dialogMessage"></div><div class="btnOK button con wechatConfirmOK">确定</div><div class="btnCancel button wechatConfirmNO">取消</div></div></div>'
    };

    easyweb.dialog.alert = function (message, callback) {
        alert(message);
        //var body = $(window.top.document).find('body'),
        //    dialog = $(easyweb.dialog.alertDialogContent[easyweb.dialog.style]),
        //    btnOK = dialog.find('.btnOK'),
        //    btnClose = dialog.find('.btnClose');

        //dialog.find('.dialogMessage').text(message);
        //dialog.appendTo(body);

        //btnOK.click(function () {
        //    setTimeout(function () {
        //        dialog.remove();
        //        if ($.isFunction(callback)) {
        //            callback();
        //        }
        //    }, 100);
        //});
        //btnClose.click(function () {
        //    setTimeout(function () {
        //        dialog.remove();
        //        if ($.isFunction(callback)) {
        //            callback();
        //        }
        //    }, 100);
        //});
    }

    easyweb.dialog.alertWithNoOK = function (message, callback) {
        var body = $(window.top.document).find('body'),
            dialog = $(easyweb.dialog.alertDialogContentWithNoOK[easyweb.dialog.style]),
            btnClose = dialog.find('.btnClose');

        dialog.find('.dialogMessage').text(message);
        dialog.appendTo(body);

        btnClose.click(function () {
            setTimeout(function () {
                dialog.remove();
                if ($.isFunction(callback)) {
                    callback();
                }
            }, 100);
        });
    }

    easyweb.dialog.confirm = function (message, btnOKClick, btnCancelClick) {
        var body = $(window.top.document).find('body'),
            dialog = $(easyweb.dialog.confirmDialogContent[easyweb.dialog.style]),
            btnOK = dialog.find('.btnOK'),
            btnCancel = dialog.find('.btnCancel');

        dialog.find('.dialogMessage').text(message);
        dialog.appendTo(body);

        dialog.appendTo(body);

        btnOK.click(function () {
            setTimeout(function () {
                dialog.remove();
                if ($.isFunction(btnOKClick)) {
                    btnOKClick();
                }
            }, 100);
        });

        btnCancel.click(function () {
            setTimeout(function () {
                dialog.remove();
                if ($.isFunction(btnCancelClick)) {
                    btnCancelClick();
                }
            }, 100);
        });
    }

    easyweb.dialog.error = function (message, callback) {
        easyweb.dialog.alert(message, callback);
    }

    easyweb.closeWindow = function () {
        if (window.WeixinJSBridge != undefined) {
            WeixinJSBridge.invoke('closeWindow', {}, function (res) { });
        }
        else {
            window.close();
        }
    }
})();

function getArgs() {
    var argsArr = new Object();
    //获取url中查询字符串参数
    var query = window.location.search;
    //取消查询字符串中的？号
    query = query.substring(1);
    var pairs = query.split("&");
    for (var i = 0; i < pairs.length; i++) {
        var sign = pairs[i].indexOf("=");
        //如果没有找到=号，那么就跳过，跳到下一个字符串（下一个循环）
        if (sign == -1) {
            continue;
        }
        var aKey = pairs[i].substring(0, sign);
        var aValue = pairs[i].substring(sign + 1);
        argsArr[aKey] = aValue;
    }
    return argsArr;
}


