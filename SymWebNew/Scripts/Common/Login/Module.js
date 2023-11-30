var gbModuleId = 0;
var ModuleManager = {
    getModule: function (projectId) {
        var objModuleList = "";
        var serviceUrl = "../Module/SelectModuleByUserPermission/?projectId=" + projectId;
        var jsonParam = "";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);
        function onSuccess(jsonData) {
            objModuleList = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objModuleList;
    },
    SetModule: function (moduleId) {
        var objModuleList = "";
        var serviceUrl = "../Home/SetModule/?moduleId=" + moduleId;
        var jsonParam = "";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);
        function onSuccess(jsonData) {
            objModuleList = jsonData;
            CurrentUser = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objModuleList;
    }
};


var ModuleHelper = {
    GetModuleInformation: function (projectId) {
        $(window).resize(function () {
            ModuleHelper.HtmlbodyHeightUpdate();
        });
        $(window).scroll(function () {
            ModuleHelper.HtmlbodyHeightUpdate();
        });

        var objModuleList = ModuleManager.getModule(projectId);
        //  ModuleHelper.PopulateModules(objModuleList);
        ModuleHelper.PopulateModulesNew(objModuleList);

        $("#navSidebarMenu11").click(function () {
            $("#navSidebarMenu11").addClass("leftZero");
        }).mouseleave(function () {
            $("#navSidebarMenu11").removeClass("leftZero");
        });
        ModuleHelper.HtmlbodyHeightUpdate();

        $('[data-toggle="tooltip"]').tooltip();//for disply text hover on module
    },

    PopulateModules: function (modules) {
        $.map(modules, function (module) {
            $("#blockDiv").append(ModuleHelper.CreateBlock(module));
        });
    },
    CreateBlock: function (module) {
        var str = "<div onclick='ModuleHelper.Goto(" + module.ModuleId + ");' class='parentDiv'> " +
                      "<span style='color:" + module.IconColor + "' class='" + module.IconName + " icon'></span>" +
                      "<span class='moduleName' >" + module.ModuleName.toUpperCase() + "</span>" +
                 "</div>";
        return str;
    },



    GetSessionModuleId: function () {
        if (CurrentUser != null) {
            gbModuleId = CurrentUser.ModuleId;
        }

    },
    GetNavHight: function () {
        var height = 0;
        $("#ulMenu li").each(function () {
            height += 51;
        });
        return height;
    },
    HtmlbodyHeightUpdate: function () {
        //var height1 = $('.nav').height();
        //var height2 = $('.randerBody').height();
        //$('html').height(Math.max(height1, height2) + 10);
        //$('body').height(Math.max(height1, height2) + 10);

        var height3 = $(window).height() - 100;
        var height1 = $('.nav').height();
        var height2 = $('.randerBody').height()+100;
        if (height2 > height3) {
            $('html').height(Math.max(height1, height3, height2) + 10);
            $('body').height(Math.max(height1, height3, height2) + 10);
        }
        else {
            $('html').height(Math.max(height1, height3, height2));
            $('body').height(Math.max(height1, height3, height2));
        }
    },
    PopulateModulesNew: function (modules) {
        var flag = true, className = "";
        var menulink = "<li class='liMenu activeMenu'><a href='../Home/Home'>DASHBOARD <span style='font-size:16px; cursor:pointer;' class='pull-right hidden-xs showopacity fa fa-home' data-toggle='tooltip' data-title='DASHBOARD'></span></a></li>";
        $("#ulMenu").append(menulink);
        var menulink = "<li class='liMenu '><a href='../Home/Vendor'>VENDOR <span style='font-size:16px; cursor:pointer;' class='pull-right hidden-xs showopacity fa fa-shopping-cart' data-toggle='tooltip' data-title='VENDOR'></span></a></li>";
        $("#ulMenu").append(menulink);
        var menulink = "<li class='liMenu '><a href='../Home/Customer'>CUSTOMER <span style='font-size:16px; cursor:pointer;' class='pull-right hidden-xs showopacity fa fa-user' data-toggle='tooltip' data-title='CUSTOMER'></span></a></li>";
        $("#ulMenu").append(menulink);
        var menulink = "<li class='liMenu '><a href='../Home/Issue'>ISSUE <span style='font-size:16px; cursor:pointer;' class='pull-right hidden-xs showopacity fa fa-hashtag' data-toggle='tooltip' data-title='ISSUE'></span></a></li>";
        $("#ulMenu").append(menulink);
        $.map(modules, function (module) {
            $("#ulMenu").append("<li class='liMenu " + className + "' moduleId=" + module.ModuleId + "><a href='../Dashboard/Dashboard?mid=" + module.ModuleId + "'>" + module.ModuleName.toUpperCase() + "<span style='font-size:16px; cursor:pointer;'  data-toggle='tooltip' data-title=" + module.ModuleName.toUpperCase() + "  class='pull-right hidden-xs showopacity " + module.IconName + "'></span></a></li>");
        });
    },
};