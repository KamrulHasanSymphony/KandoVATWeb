$(document).ready(function () {
    $("#calendar").kendoCalendar({
    });
    //loading menu
    var moduleId = window.location.search.substring(1).split('=')[1];
    CommonDashboardHelper.LoadMenu(moduleId);
    CommonDashboardHelper.InitCommonDashBoard();

});

var CommonDashboardHelper = {
    InitCommonDashBoard: function () {
        CommonDashboardHelper.LoadDashBoardSetting();
    },
    LoadMenu: function (moduleId) {
       // mnHelper.GetMenuInformation(CurrentUser.ModuleId);
        mnHelper.GetMenuInformation(moduleId);
       $(".liMenu").removeClass("activeMenu");
       $(".liMenu[ModuleId=" + CurrentUser.ModuleId + "]").addClass("activeMenu");

       $("#navSidebarMenu11").click(function () {
           $(this).addClass("leftZero");
       }).mouseleave(function () {
           $(this).removeClass("leftZero");
       });
   },

    LoadDashBoardSetting:function() {
        if (CurrentUser != null) {
            var moduleId = CurrentUser.ModuleId;
            if (moduleId === 5) {//attendance
              
              
            }
        }
    }
}