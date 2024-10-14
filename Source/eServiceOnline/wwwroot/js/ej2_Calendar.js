// Write your Javascript code.
eServiceOnline.Site.prototype.CreatedCalenderContext = function () {
    this.element.addEventListener("mousedown", function (e) {     
        $("#tipContent").hide();
        var url = $("#MenuUrl").val();
        var param;
        if (e.button === 2 && e.target.classList.contains("e-work-cells")) {
            var  date = e.target.dataset.date;
            var index = e.target.dataset.groupIndex;
            var  text = $(e.currentTarget).find("td[data-group-index='" + index + "']")[0].innerText.trim();
          
             param = ["Calendar", "Add", $(this).attr("id"), date, text];      
            eServiceOnline.Site.prototype.GetContextMenuList(url,param, e);         
        }
        if (e.button === 2 && e.target.classList.contains("e-subject") || e.button === 2 && e.target.classList.contains("e-time")) {
            var field = "";
            var id;
            if (e.path[3].className == "e-appointment" || e.path[3].className == "e-appointment e-appointment-border" || e.path[3].className =="e-appointment e-lib e-draggable") {
                field = e.path[3].attributes[1].value;
                id = field.split("_")[1];
            } else {
                field = e.path[1].attributes[1].value;
                id = field;
            }          
            param = ["Calendar", "UpdateOrDelete", $(this).attr("id"), id];      
            eServiceOnline.Site.prototype.GetContextMenuList(url, param, e);         
        }
        else {
            return false;
        }
    });
}

