var Verify = Verify || {};

Verify.VerifyDailog = function () {
};

Verify.VerifyDailog.prototype.verifyEmpty = function (obj) {
    if ($(obj).val() !== '') {
        $(obj).next().hide();
    }
};

Verify.VerifyDailog.prototype.printGrid = function (url) {

    $("#FlatGrid_printGrid").on("click", function () {

        if ($("#FlatGrid table tr[aria-selected='true']").length <= 0) {
            alert("No records selected for print operation");
            return;
        }
		var tempwindow = window.location.protocol + "//" + window.location.host + url + "?productHaulLoadId=" +
            $("#FlatGrid table tr[aria-selected='true'] td").eq(1).text() +
            "&callSheetNumber=" +
            $("#FlatGrid table tr[aria-selected='true'] td").eq(2).text();
        var a = $("<a href='" + tempwindow + "' target='_blank' >test</a>").get(0);
        var e = document.createEvent('MouseEvents');
        e.initEvent('click', true, true);
        a.dispatchEvent(e);
    });
    
}



//Tobey added
Verify.VerifyDailog.prototype.pdfExport = function (url) {

    $("#FlatGrid_pdfExport").on("click", function () {

        if ($("#FlatGrid table tr[aria-selected='true']").length <= 0) {
            alert("No records selected for print operation");
            return;
        }
        var tempwindow = window.location.protocol + "//" + window.location.host + url + "?productHaulLoadId=" +
            $("#FlatGrid table tr[aria-selected='true'] td").eq(1).text() +
            "&callSheetNumber=" +
            $("#FlatGrid table tr[aria-selected='true'] td").eq(2).text();
        var a = $("<a href='" + tempwindow + "' target='_blank' >test</a>").get(0);
        var e = document.createEvent('MouseEvents');
        e.initEvent('click', true, true);
        a.dispatchEvent(e);
    });

}


Verify.VerifyDailog.prototype.add = function (itemurl, item) {

    $("#FlatGrid_add").on("click", function () {
        $.ajax({
            url: itemurl,
            method: "Post",
            data: item,
            success: function (data) {
                $("#tipContent").hide();
                $(".sj_mask").fadeIn(300,
                    function () {
                        $("#basicDialog_title span").html("Add New Record");
                        $("#basicDialog").html(data);
                        $("#basicDialog").ejDialog("open");
                    }
                );
            }
        });
    });
}

Verify.VerifyDailog.prototype.edit = function (url) {

    $("#FlatGrid_edit").on("click", function () {

        if ($("#FlatGrid table tr[aria-selected='true']").length <= 0) {
            alert("No records selected for print operation");
            return;
        }
        var tempwindow = window.location.protocol + "//" + window.location.host + url + "?productHaulId=" +
            $("#FlatGrid table tr[aria-selected='true'] td").eq(1).text() +
            "&callSheetNumber=" +
            $("#FlatGrid table tr[aria-selected='true'] td").eq(2).text();
        var a = $("<a href='" + tempwindow + "' target='_blank' >test</a>").get(0);
        var e = document.createEvent('MouseEvents');
        e.initEvent('click', true, true);
        a.dispatchEvent(e);
    });

}

Verify.VerifyDailog.prototype.delete = function (itemUrl) {
    
    $("#FlatGrid_delete").on("click", function () {
        if ($("#FlatGrid table tr[aria-selected='true']").length <= 0) {
            alert("No records selected for delete operation");
            return;
        }
        EServiceExpress.Common.prototype.ModalAlertDialog("Warning_Dialog", "Prompt", "Are you sure you want to delete record?", Verify.VerifyDailog.prototype.deleteFun, itemUrl);
    });
}

Verify.VerifyDailog.prototype.deleteFun = function (itemUrl) {

    $.ajax({
        type: 'post',
        url: itemUrl,
        data: {
            'productHaulId': $("#FlatGrid table tr[aria-selected='true'] td").eq(1).text()
        },
        success: function (data) {
            history.go(0);
        }
    });
    
}



Verify.VerifyDailog.prototype.save = function (info,firstUrl,lastUrl) {
    
    $.ajax({
        type: 'post',
        url: window.location.protocol + "//" + window.location.host + firstUrl,
        data: {
            'ProductHaulId': info.ProductHaulId,
            'CallSheetNumber': info.CallSheetNumber,
            'BaseBlendSectionId': info.BaseBlendSectionId,
            'IsTotalBlendTonnage': info.IsTotalBlendTonnage,
            'Amount': info.Amount,
            'MixWater': info.MixWater,
            'BinId': info.BinId,
            'BinNumber': info.BinNumber,
            'Comments': info.Comments,
            'Submit': info.Submit,
            'DriverId': info.DriverId,
            'BulkUnitId': info.BulkUnitId,
            'BulkUnitName': info.BulkUnitName,
            'TractorUnitId': info.TractorUnitId,
            'TractorUnitName': info.TractorUnitName,
            'PreferedName': info.PreferedName,
            'ExpectedOnLocationTime': info.ExpectedOnLocationTime 
        },
        async: false, 
        success: function (data) {
            $("#EditDialog_Flat_Save").removeAttr("disabled");
            
            if (data === undefined || data === null) {
                EServiceExpress.Common.prototype.ModalAlertDialog("Warning_Dialog", "Warning", "Add a product load failed.");
                $("#EditDialog_FlatGrid_Save").trigger("click");
            } else if (data === "AmountExceedsTheLimit") {
                EServiceExpress.Common.prototype.ModalConfirmDialog("Warning_Dialog", "Warning", "Scheduled blend amount is more than selected blend required amount.<br /><br /> Click “Yes” to continue ,Click “No” to return.", Verify.VerifyDailog.prototype.save, info, firstUrl, lastUrl);
            } else if (data === "MixWaterIsRequired") {
                $('input[name=MixWater]').parent().next().show();
            } else {
                var dataAry = data.split(",");
                var productHaulId = dataAry[0];
                var callSheetNumber = dataAry[1];

                Verify.VerifyDailog.prototype.openNewWindow(lastUrl, productHaulId, callSheetNumber);
//                var tempwindow = window.location.protocol + "//" + window.location.host + lastUrl + "?productHaulId=" + productHaulId + "&callSheetNumber=" + callSheetNumber;
//                var a = $("<a href='" + tempwindow + "' target='_blank' >test</a>").get(0);
//                var e = document.createEvent('MouseEvents');
//                e.initEvent('click', true, true);
//                a.dispatchEvent(e);

                history.go(0);
            }
        }
    });
}

Verify.VerifyDailog.prototype.openNewWindow = function (url, pId, number) {
    var tempwindow = window.location.protocol + "//" + window.location.host + url + "?productHaulId=" + pId + "&callSheetNumber=" + number;
    window.open(tempwindow, "_blank");
}



