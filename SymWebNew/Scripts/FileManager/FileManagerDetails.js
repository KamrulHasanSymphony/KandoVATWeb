
var FileManagerDetailsManager = {
    GetAllFilesData: function () {
        var objEmpEdu = "";
        var jsonParam = "";
        var serviceUrl = "../FileManager/GetAllFilesData/";
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);

        function onSuccess(jsonData) {
            objEmpEdu = jsonData;
        }
        function onFailed(error) {
            alert(error.statusText);
        }
        return objEmpEdu;
    },
};

var FileManagerDetailsHelper = {
    InitFileManagerDetails: function () {
        // FileManagerDetailsHelper.GetAllFiles();

      
        FileManagerDetailsHelper.GenerateFileManager();
        var filemanager = $("#filemanager").getKendoFileManager();
        filemanager.executeCommand({ command: "TogglePaneCommand", options: { type: "preview" } });
        filemanager.toolbar.fileManagerDetailsToggle.switchInstance.toggle();

        //filemanager.commands.MyCustomCommand = filemanager.FileManagerCommand.extend({
        //    exec: function () {
        //        kendo.alert('Custom command in ContextMenu has been clicked');
        //    }
        //});

    },

    GetAllFiles:function() {
        var data = FileManagerDetailsManager.GetAllFilesData();
    },

    GenerateFileManager:function() {
        $("#filemanager").kendoFileManager({
            dataSource: {
                schema: kendo.data.schemas.filemanager,
                transport: {
                    read: {
                        url: "../FileManager/Read",
                        method: "POST"
                    },
                    create: {
                        url: "../FileManager/Create",
                        method: "POST"
                    },
                    update: {
                        url: "../FileManager/Update",
                        method: "POST"
                    },
                    destroy: {
                        url: "../FileManager/Destroy",
                        method: "POST"
                    }
                }
            },
            uploadUrl: "../FileManager/Upload",
            toolbar: {
                items: [
                    { name: "createFolder" },
                    { name: "upload" },
                    { name: "sortDirection" },
                    { name: "sortField" },
                    { name: "changeView" },
                    { name: "spacer" },
                    { name: "details" },
                    { name: "search" }
                ]
            },
            contextMenu: {
                items: [
                    { name: "rename" },
                    { name: "delete" },
                   // { name: "custom", text: "Download", command: "FileManagerDetailsHelper.DownloadFile", spriteCssClass: "k-icon k-i-info" }
                    
                ]
            },
            draggable: true,
            resizable: true
        });
    },

    DownloadFile:function(obj) {
        debugger;
    }


};