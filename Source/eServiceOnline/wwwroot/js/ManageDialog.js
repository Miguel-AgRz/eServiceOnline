var ManageDialog = (function () {

    function ManageDialog(dialogName, validateFunction) {

        if (validateFunction && !(validateFunction instanceof ValidationsChain))
            throw "validateFunction must be an object of type ValidationsChain";

        this.dialogName = dialogName;
        this.validationFn = validateFunction;
    }

    ManageDialog.prototype = {
        openDialog: function () {
            var self = this;
            //console.log("opened called"); 
          
            var divs = document.getElementsByTagName("div");
            var max = 0;
            for (var i = 0; i < divs.length; i++) {
                max = Math.max(max, divs[i].style.zIndex || 0);
            }
            $(".sj_confirm").css({ "z-index": max + 1 });

            $(".sj_confirm").fadeIn(300,
                function () {
                    $("#" + self.dialogName).ejDialog("open");
                }
            );
            $(".confirmDialog_button").show();
        },
        closeDialog: function (item) {
            var self = this;
            //console.log("close dialog");

            if (item === "Yes") {
                self.callValidationFn(true);
                $("#" + self.dialogName).ejDialog("close");

                $("#sj_createProductHual").trigger("click");
            }
            else if (item === "No") {
                $("#" + self.dialogName).ejDialog("close");

                self.callValidationFn(false);
            }

            $(".sj_confirm").hide();
        },
        callValidationFn: function (value)
        {
            if (this.validationFn) { 
                this.validationFn.setPass(value);
            }
        }
    }
    return ManageDialog;
}());