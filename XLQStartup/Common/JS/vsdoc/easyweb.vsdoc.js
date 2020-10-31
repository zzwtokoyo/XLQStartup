easyweb = {};

easyweb.closeWindow = function () {
    /// <summary>关闭当前窗口。</summary>
}



easyweb.application = function () {
    /// <summary>Web应用程序控制器，用于控制Web应用程序的启动，实现动态模块加载、视图切换等功能。</summary>
    /// <field name='configFile' type='string '>统资源清单文件，通常可以为一个JS或一个远程地址</field> 
    /// <field name='settings' type='object'>应用程序的默认全局参数（所有模块都可以访问并设置其中的值）</field> 

    this.configFile = config;

    this.settings = {
        /// <field name='version' type='string '>版本号</field> 
        version: '1.0',
        /// <field name='debugMode' type='bool'>是否启用调试模式，在调试模式下系统会禁用缓存，并显示调试信息</field> 
        debugMode: false,
        /// <field name='loginModuleName' type='string '>系统登录模块名称</field> 
        loginModuleName: 'Login',
        /// <field name='layoutModuleName' type='string '>页面布局模块名称</field> 
        layoutModuleName: 'Layout',
        /// <field name='defaultPages' type='array'></field> 
        defaultPages: ['/', '/default.htm'],
        /// <field name='autoLanguage' type='bool'>是否自动检测并设置平台语言</field> 
        autoLanguage: true,
        /// <field name='language' type='string '>系统默认语言</field> 
        language: 'cn',
    };

    this.loadModule = function (moduleName, parms) {
        /// <summary>方法：切换当前模块中的视图。</summary>
        /// <param name='viewName' type='string'>视图名称</param>
        /// <param name='parms' type='object'>所要传递给视图的对象</param>
    }

    this.changeView = function (viewName, parms) {
        /// <summary>方法：切换当前模块中的视图。</summary>
        /// <param name='viewName' type='string'>视图名称</param>
        /// <param name='parms' type='object'>所要传递给视图的对象</param>
    }

    this.regModule = function (moduleName, moduleClass, isPageModeModule) {
        /// <summary>方法：向app注册功能模块</summary>
        /// <param name='moduleName' type='string'>功能模块名称</param>
        /// <param name='moduleClass' type='function'>功能模块所依赖的对象脚本</param>
        /// <param name='isPageModeModule' type='bool'>是否为页面模式功能模块</param>
    }
}






easyweb.grid = function (table, options) {
    /// <summary>表格控件</summary>
    /// <param name='table' type='html元素'>用于产生table的Html元素</param>
    /// <param name='options' type='object'>表格参数</param>


    /// <field name='allowColumnResize' type='bool'>是否允许可变列宽</field> 
    /// <field name='allowCellEdit' type='bool'>是否允许编辑。当allowCreateRow同时为true时，编辑不更改源数据</field> 
    /// <field name='allowCreateRow' type='bool'>是否允许创建新行</field> 
    /// <field name='allowPaging' type='bool'>是否分页。为true时allowCellEdit属性失效</field> 
    /// <field name='pageSize' type='int'>每个分页的行数</field> 
    /// <field name='showRowNumber' type='bool'>是否显示序号列</field> 
    /// <field name='hideSelection' type='bool'>是否加亮显示选择的行</field> 
    /// <field name='defaultColWidth' type='int'>默认列宽</field> 
    /// <field name='rowBackColor' type='string'>行背景色</field> 
    /// <field name='rowAlternateColor' type='string'>间隔行颜色</field> 
    /// <field name='rowHoverColor' type='string'>鼠标悬停在行上时的背景色</field> 
    /// <field name='rowSelectColor' type='string'>被选择行的背景色</field> 
    /// <field name='htmlControl' type='html元素'>Grid对象在DOM上所对应的html控件</field> 
    /// <field name='serviceUrl' type='url'>用于从服务端获取数据的Url</field> 
    /// <field name='serviceParm' type='object'>serviceUrl的参数</field> 
    /// <field name='dataSource' type='数组'>表格绑定的数据</field> 
    /// <field name='selectIndex' type='int'>当前选择行，从0开始</field> 
    /// <field name='selectItem' type='object'>当前选择行对应的数据</field> 
    /// <field name='rowCount' type='int'>当前页的行数</field> 
    /// <field name='editMode' type='bool'>编辑模式 - false:插入模式、true:改写模式</field> 
    /// <field name='pageBar' type='object'></field> 
    /// <field name='autoADdNewRowWhileEdit' type='bool'>在编辑模式下是否自动添加新行</field> 


    this.allowColumnResize = false;

    this.allowCellEdit = false;

    this.allowCreateRow = false;

    this.allowPaging = false;

    this.pageSize = 50;

    this.showRowNumber = false;

    this.hideSelection = false;

    this.defaultColWidth = 100;

    this.rowBackColor = "#FFFFFF";

    this.rowAlternateColor = "#FFFFFF";

    this.rowHoverColor = "#EAF2FF";

    this.rowSelectColor = "#DFE8F6";

    this.htmlControl = gridView;

    this.serviceUrl = null;

    this.serviceParm = null;

    this.dataSource = [];

    this.selectIndex = -1;

    this.selectItem = null;

    this.rowCount = 0;

    this.editMode = false;

    this.pageBar = null;

    this.autoADdNewRowWhileEdit = false;

    this.attr = {

    }

    this.dataBind = function () {
        /// <summary>方法：绑定数据源至控件</summary>
        /// <returns type='' ></returns> 
    }

    this.destroy = function () {
        /// <summary>方法：销毁控件</summary>
        /// <returns type='' ></returns> 
    }

    this.refresh = function () {
        /// <summary>方法：重置表格布局</summary>
        /// <returns type='' ></returns> 
    }

    this.hideColumn = function (colume) {
        /// <summary>方法：隐藏列</summary>
        /// <param name='colume' type='int'>需隐藏的列的Index</param>
        /// <returns type='' ></returns> 
    }

    this.setColWidth = function (column, newWidth) {
        /// <summary>方法：设置指定列的宽度</summary>
        /// <param name='column' type='int/string'>列的name或者index</param>
        /// <param name='newWidth' type='int'>调整后列的宽度</param>
        /// <returns type='' ></returns> 
    }

    this.createNewRow = function () {
        /// <summary>方法：在选择的行之前增加新行。分页模式下无效。</summary>
        /// <returns type='' ></returns> 
    }

    this.createNewRowToBottom = function () {
        /// <summary>方法：在表格末尾添加新行</summary>
        /// <returns type='' ></returns> 
    }

    this.removeRow = function () {
        /// <summary>方法：删除选择的行。分页模式下无效。 </summary>
        /// <returns type='' ></returns> 
    }



    this.onSelectChange = function () {
        /// <summary>事件：选择的行发生变更事件</summary>
    }

    this.onCellEdit = function () {
        /// <summary>事件：单元格编辑时发生</summary>
    }

    this.onCellClick = function () {
        /// <summary>事件：单元格被单击事件，参数：当前单击的Cell、当前行对应的数据</summary>
    }

    this.onClick = function () {
        /// <summary>事件：行被单击事件，参数：当前行的Index，当前行对应的数据</summary>
    }

    this.onDbClick = function () {
        /// <summary>事件：行被双击事件，参数：当前行的Index，当前行对应的数据</summary>
    }

    this.onDataBinding = function () {
        /// <summary>事件：开始绑定数据时事件</summary>
    }

}

easyweb.binder = {
    /// <summary>数据绑定控件</summary>

    getProperty: function (json, name) {
        /// <summary>方法：根据属性名从Json对象中获取对应的数据</summary>
        /// <param name='json' type='json'>json</param>
        /// <param name='name' type='string'>属性名</param>
        /// <returns type='json' >json对象对应属性的值</returns> 
    },
    setProperty: function (json, name, value) {
        /// <summary>方法：根据属性名给Json对象赋值</summary>
        /// <param name='json' type='json'>json</param>
        /// <param name='name' type='string'>属性名</param>
        /// <param name='value' type='object'>属性的值</param>
        /// <returns type='void' ></returns> 
    },
    bindControlToJson: function (container, defaultValue) {
        /// <summary>方法：将html元素及对应的值绑定成Json对象</summary>
        /// <param name='container' type='htmlElement'>需要绑定数据的html元素</param>
        /// <param name='defaultValue' type='object'>数据的默认值</param>
        /// <returns type='json' >绑定后的Json对象</returns> 
    },
    bindJsonToControl: function (container, json) {
        /// <summary>方法：将Json对象的值绑定至html元素</summary>
        /// <param name='container' type='htmlElement'>需绑定数据的Html元素</param>
        /// <param name='json' type='json'>json</param>
        /// <returns type='void' ></returns> 
    },
    bindGridToJson: function (grids, json) {
        /// <summary>方法：将表格控件的数据绑定成Json对象</summary>
        /// <param name='grids' type='e-grid'>grid控件</param>
        /// <param name='json' type='json'>json</param>
        /// <returns type='void' ></returns> 
    },
    bindJsonToGrid: function (grids, json) {
        /// <summary>方法：将Json对象的值绑定到Grid上</summary>
        /// <param name='grids' type='e-grid'>需绑定的Grid控件</param>
        /// <param name='json' type='json'>需绑定到Grid上的Json数据</param>
        /// <returns type='void' ></returns> 
    },
    checkValid: function (container) {
        /// <summary>方法：检查具有特定属性的input元素中输入的有效性</summary>
        /// <param name='container' type='htmlElement'>待检查的html元素</param>
        /// <returns type='bool'>输入是否有效</returns> 
    }
};


easyweb.dialog = function (options) {
    /// <summary>对话框控件</summary>
    /// <param name='options' type='object'>[可选]用于设置对话框的属性</param>


    /// <field name='clone' type='bool'></field> 
    /// <field name='title' type='string'>标题栏文字</field> 
    /// <field name='showModal' type='bool'></field> 
    /// <field name='buttons' type='array'>对话框上的按键</field> 
    /// <field name='control' type='object'></field> 

    this.clone = true;
    this.title = easyweb.language == "cn" ? "系统提示" : "System Info";
    this.showModal = true;
    this.buttons = [{ text: global.BtnOK }];
    this.control = null;

    this.show = function () {
        /// <summary>方法：显示对话框</summary>
        /// <returns type='void' ></returns> 
    }

    this.close = function () {
        /// <summary>方法：关闭对话框</summary>
        /// <returns type='void' ></returns> 
    }


    this.onShown = function () {
        /// <summary>事件：对话框显示时响应的事件</summary>
    }

    this.onClose = function () {
        /// <summary>事件：对话框关闭时响应的事件</summary>
    }

}
easyweb.dialog.message = function (icon, message, options) {
    /// <summary>消息提示对话框</summary>
    /// <param name='icon' type=''></param>
    /// <param name='message' type=''></param>
    /// <param name='options' type=''></param>
};
easyweb.dialog.showException = function (serviceMethodResult) {
    /// <summary>错误提示提示框</summary>
    /// <param name='serviceMethodResult' type='object'>easyweb.server.run方法返回的数据</param>
};
easyweb.dialog.info = function (message, close) {
    /// <summary>信息提示对话框</summary>
    /// <param name='message' type='string'>对话框显示的信息</param>
    /// <param name='close' type=''></param>
};
easyweb.dialog.alert = function (message, close) {
    /// <summary>警告提示对话框</summary>
    /// <param name='message' type='string'>警告对话框文字</param>
    /// <param name='close' type='function'>关闭对话框</param>
};
easyweb.dialog.error = function (message, close) {
    /// <summary>错误提示对话框</summary>
    /// <param name='message' type='string'>错误对话框显示的文字</param>
    /// <param name='close' type='function'>关闭对话框</param>
};
easyweb.dialog.confirm = function (message, btnYesClick, btnNoClick) {
    /// <summary>确认提示对话框</summary>
    /// <param name='message' type='string'>确认对话框显示现时代的消息</param>
    /// <param name='btnYesClick' type='function'>点击“确定“响应的方法</param>
    /// <param name='btnNoClick' type='function'>点击”取消“响应的方法</param>
};
easyweb.dialog.success = function (success, close) {
    /// <summary>成功提示对话框</summary>
    /// <param name='success' type='string'>对话框显示的成功消息</param>
    /// <param name='close' type='function'>关闭对话框</param>
};
easyweb.dialog.remove = function (message, btnDeleteClick, btnCancelClick) {
    /// <summary>移除提示对话框</summary>
    /// <param name='message' type='string'>对话框显示的消息</param>
    /// <param name='btnDeleteClick' type='function'>点击”删除“后响应的方法</param>
    /// <param name='btnCancelClick' type='function'>点击”取消“后响应的方法</param>
};

easyweb.validate = {
    isNull: function (obj) { }
}

easyweb.server = {
    /// <summary>server工具包。</summary>
    run: function (serviceUrl, parms) {
        /// <summary>调用服务端方法，返回一个serviceMethodResult对象。</summary>
        /// <param name='serviceUrl' type='string'>所要调用方法的地址。如：/Services/ObjectName/MethodName</param>
        /// <param name='parms' type='object'>调用方法所需附带的参数。如：{id:1}</param>
        /// <returns type='object' >返回ServiceMethodResult对象。</returns> 

    },
    get: function (serviceUrl, parms, defaultValue) {
        /// <summary>调用服务端方法，并直接获取方法所返回的对象。</summary>
        /// <param name='serviceUrl' type='string'>所要调用方法的地址。如：/Services/ObjectName/MethodName</param>
        /// <param name='parms' type='object'>调用方法所需附带的参数。如：{id:1}</param>
        /// <param name='defaultValue' type='object'>如果获取失败，所需返回的默认值。</param>
        /// <returns type='object' >返回ServiceMethodResult对象。</returns> 
    },
    isSuccess: function (serviceMethodResult) {
        /// <summary>检测调用服务端调用是否成功。</summary>
        /// <param name='serviceMethodResult' type='object'>服务端返回的ServiceMethodResult对象。</param>
        /// <returns type='bool' >调用是否成功。</returns> 
    },
    async_Run: function (serviceUrl, parms, waitmsg, callback) {
        /// <summary>异步调用服务端方法</summary>
        /// <param name='serviceUrl' type='string'>所要调用方法的地址。如：/Services/ObjectName/MethodName</param>
        /// <param name='parms' type='object'>调用方法所需附带的参数。如：{id:1}</param>
        /// <param name='waitmsg' type='string'>调用方法是的提示消息。</param>
        /// <param name='callback' type='function'>方法调用完成时的回调函数，如:function(contextData){}</param>
        /// <returns type='void' ></returns> 
    },
    load: function (serviceUrl, parms, callback) {
        /// <summary>异步调用服务端方法，装载所需的数据</summary>
        /// <param name='serviceUrl' type='string'>所要调用方法的地址。如：/Services/ObjectName/MethodName</param>
        /// <param name='parms' type='object'>调用方法所需附带的参数。如：{id:1}</param>
        /// <param name='callback' type='function'>方法调用完成时的回调函数，如:function(contextData){}</param>
        /// <returns type='void' ></returns> 

    },
    save: function (serviceUrl, parms, callback) {
        /// <summary>异步调用服务端方法，提交需保存数据。</summary>
        /// <param name='serviceUrl' type='string'>所要调用方法的地址。如：/Services/ObjectName/MethodName</param>
        /// <param name='parms' type='object'>调用方法所需附带的参数。如：{id:1}</param>
        /// <param name='callback' type='function'>方法调用完成时的回调函数，如:function(contextData){}</param>
        /// <returns type='void' ></returns> 

    },
    remove: function (serviceUrl, parms, callback) {
        /// <summary>异步调用服务端方法，提交需删除数据。</summary>
        /// <param name='serviceUrl' type='string'>所要调用方法的地址。如：/Services/ObjectName/MethodName</param>
        /// <param name='parms' type='object'>调用方法所需附带的参数。如：{id:1}</param>
        /// <param name='callback' type='function'>方法调用完成时的回调函数，如:function(contextData){}</param>
        /// <returns type='void' ></returns> 

    }

};

easyweb.repeater = function (control, options) {
    /// <summary>重复Html元素</summary>
    /// <param name='control' type='htmlElement'>用于重复的HtmlElement</param>
    /// <param name='options' type='object'>控件参数设置</param>


    /// <field name='animate' type='bool'>是否采用动画方式显示</field> 
    /// <field name='animateTime' type='int'>动画方式时间</field> 
    /// <field name='dataSource' type='array'>数据源 - 通常为一个对象数组</field> 

    this.animate = fasle;
    this.animateTime = 400;
    this.dataSource = [];

    this.databind = function () {
        /// <summary>方法：将数据绑定到reaper</summary>
        /// <returns type='void' ></returns> 
    }
}

easyweb.tab = function (control, options) {
    /// <summary>Tab控件</summary>
    /// <param name='control' type='htmlElement'>用于生成Tab控件的Html元素</param>
    /// <param name='options' type='object'>tab控件的属性</param>

    this.setActive = function (tabName) {
        /// <summary>方法：设置Tab页为当前页</summary>
        /// <param name='tabName' type='string'>Tab页的名称</param>
        /// <returns type='void' ></returns> 
    }


    this.onChange = function () {
        /// <summary>事件：Tab页切换时触发</summary>
    }

}

easyweb.dragable = function (control) {
    /// <summary>拖拽功能组件</summary>
    /// <param name='control' type='htmlElement'>所要进行拖拽的Html元素</param>

    /// <field name='control' type='JQuery'>所要拖动的控件</field> 
    /// <field name='handle' type='JQuery'>子控件，仅鼠标在该子控件上，才能拖动整个控件，如点击标题栏，拖动整个窗口。</field> 
    /// <field name='onlyH' type='boolean'>仅能横向拖动</field> 
    /// <field name='onlyV' type='boolean'>仅能竖向拖动</field> 
    /// <field name='outParent' type='boolean'>是否可以移出父控件</field> 
    /// <field name='cursor' type='string'>发生拖动时的鼠标样式</field> 
    /// <field name='dragStep' type='number'>拖拽最小位移像素，在开发SliderBar等控件时，可能需要按步长进行拖动。</field> 

    this.onDrag = function (pos) {
        /// <summary>注册正在拖动元素时的处理方法。</summary>
        /// <param name='pos' type='object'>pos参数是一个对象，通过其x,y属性访问具体坐标。</param>
    }

    this.afterDrag = function (pos) {
        /// <summary>注册拖动元素结束时的处理方法。</summary>
        /// <param name='pos' type='object'>pos参数是一个对象，通过其x,y属性访问具体坐标。</param>
    }

    this.bind = function (eventName, handle) {
        /// <summary>添加事件绑定及处理方法。</summary>
        /// <param name='eventName' type='string'>事件名称，如click或click.custom01，当事件名称带有后缀时,可以通过后缀名解除相关绑定。</param>
        /// <param name='handle' type='Function'>事件处理方法。</param>
    }

    this.unbind = function (eventName) {
        /// <summary>解除事件绑定及处理方法。</summary>
        /// <param name='eventName' type='string'>事件名称，如click或.custom01，可以通过后缀名解除相关绑定。</param>
    }
}


app = new easyweb.application();