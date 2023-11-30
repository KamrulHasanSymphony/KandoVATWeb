
var mnManager = {
    
    getMenu: function (nModuleId) {
        var objMenuList = "";
        var serviceURL = "../Menu/SelectMenuByUserPermission/?nModuleId=" + nModuleId;
        var jsonParam = "";// "moduleId=" + moduleId;
        AjaxManager.GetJsonResult(serviceURL, jsonParam, false, false, onSuccess, onFailed);
        function onSuccess(jsonData) {
          
            objMenuList = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }

        return objMenuList;
    },
    
    Logoff: function () {
        var serviceURL = "../Home/Logoff";
        window.location.href = serviceURL;
    },
 
};



var mnHelper = {
    GetMenuInformation: function (nModuleId) {
        if (nModuleId > 0) {
            $("#kendoMenu li, #quickLinkUl li").remove();
            var objMenuList = mnManager.getMenu(nModuleId);
            mnHelper.populateMenus(objMenuList);
           // mnHelper.CreateQuickLink(objMenuList);

            //dashboard thumb menu
            // mnHelper.populateSubMenu(objMenuList);

        }
       
        //UserLoginManager.getCurrentUser();
    },

    youLogedInAs: function() {
        var jsonParam = '';

        var serviceURL = "../Home/GetUserTypeByUserId";

        AjaxManager.GetJsonResult(serviceURL, jsonParam, false, false, onSuccess, onFailed);
        function onSuccess(jsonData) {

            var logedInAs = "";
            var groupName = "";
            if (jsonData != undefined) {
                for (var i = 0; i < jsonData.length; i++) {
                    groupName += " " + jsonData[i].GroupName + " &";
                }
                var splitlogedInAs = groupName.slice(0, -1);
                logedInAs = "| You loged in as " + splitlogedInAs;
                $("#lblLogedinAs").html(logedInAs);

            }

        }

        function onFailed(error) {
            window.alert(error.statusText);
        }
    },


  
    populateMenus: function(menus) {
        var dynamicmenuArray = [];
        var chiledMenuArray = [];

       // var menulink = "<li><a href='../Home/Home'>DASHBOARD</a></li>";
        var menulink = "";
        for (var i = 0; i < menus.length; i++) {
          
            if (menus[i].ParentMenu == null || menus[i].ParentMenu == 0) {
                menulink += "<li class='has-sub'>";
                if (menus[i].MenuPath == null || menus[i].MenuPath == "") {
                    menulink += '<a  class="dropdown-toggle" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false" href="#">' + menus[i].MenuName + '</a>';
                } else if (menus[i].MenuPath != null && menus[i].ParentMenu == 0) {
                    menulink += '<a role="button" aria-haspopup="true" aria-expanded="false" href="' + menus[i].MenuPath + '">' + menus[i].MenuName + '</a>';
                }
                else {
                    menulink += '<a class="dropdown-toggle" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false" href="' + menus[i].MenuPath + '">' + menus[i].MenuName + '</a>';
                }
                menulink += mnHelper.addchiledMenu(menus[i], menus[i].MenuId, menus, "has-sub dropdown-submenu");//dropdown-submenu
                menulink += "</li>";
            }
        }

        var menu = $("#kendoMenu").kendoMenu().data("kendoMenu");
        menu.append(menulink);
        $("#kendoMenu").kendoMenu();
        $("#kendoMenu").removeClass("k-header").removeClass("k-widget");
      
        $(".childHover").hover(function () {
            $(this).css("background-color", "#4da6ff");
        }, function () {
            $(this).css("background-color", "#286090");
        });
    },
  

    addchiledMenu: function (objMenuOrginal, menuId, objMenuList,cls) {
        var menulink = '<ul>';
        var added = false;
        for (var j = 0; j < objMenuList.length; j++) {
            if (objMenuList[j].ParentMenu == menuId) {
                menulink += '<li class="'+cls+'">';
                if (objMenuList[j].MenuPath == null) {
                    menulink += objMenuList[j].MenuName;
                }
                else {
                    menulink += "<a href='" + objMenuList[j].MenuPath + "?mId=" + objMenuList[j].ModuleId + "' class='childHover'>" + objMenuList[j].MenuName + "</a>";
                }
                menulink += mnHelper.addchiledMenu(objMenuList[j], objMenuList[j].MenuId, objMenuList, "has-sub dropdown-submenu");
                menulink += "</li>";
                added = true;
            }
        }
        menulink += '</ul>';
        if (added == false) {
            menulink = "";
        }

        return menulink;
    },

    CreateQuickLink: function (menus) {
        var quickLink = "";
        for (var i = 0; i < menus.length; i++) {
            if (menus[i].ToDo == 1) {
                quickLink += "<li class='quicklinkbtn'>";
                quickLink += '<a  role="button" style=" color:#ffffff;  font-size: 1.5em;"  href="' + menus[i].MenuPath + '">' + menus[i].MenuName + '</a>';
                quickLink += "</li>";
            }
        }
        $("#quickLinkUl").append(quickLink);
    },

    //dashboard menu

    populateSubMenu: function (menuList) {
      
        var index = 0;
        $.map(menuList, function (menu) {
            $("#blockChildDiv").append(mnHelper.CreateBlock(menu, index));
            $("#parentDiv" + index).click(function () {
                window.location.href = menu.MenuPath;
            });
            index++;
        });
    },

    CreateBlock: function (menu, index) {
        var menuPath = menu.MenuPath;
        var str = "";
        if (menuPath !== "") {
            str = '<div class="parentDiv" id="parentDiv' + index + '"> ' +
                   "<span style='color:" + "red" + "' class='" + "" + " icon'></span>" +
                   "<span class='moduleName'>" + menu.MenuName.toUpperCase() + "</span>" +
              "</div>";
        }
        return str;
    }
};