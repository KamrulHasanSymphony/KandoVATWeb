$(document).ready(function() {
    UserProjectHelper.IntiUserProject();
});


var UserProjectManager = {

};

var UserProjectHelper= {
    IntiUserProject: function () {
        $("#btnMerhcandising").click(function() {
            window.location.href = "../Home/Home";
        });
    }
}