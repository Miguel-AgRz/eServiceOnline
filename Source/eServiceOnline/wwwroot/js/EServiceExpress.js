var EServiceExpress = EServiceExpress || {};

EServiceExpress.Common = function () {
};

EServiceExpress.Common.Instance = new EServiceExpress.Common();

EServiceExpress.Common.prototype.ModalAlertDialog = function (dialogId, title, content, closeEnevt, firstUrl) {
    
    $("#" + dialogId).remove();
    var dialogDiv = $("<div id='" + dialogId + "' title='" + title +"' hidden></div>");
    var pContent = $("<p>" + content +" </p>");
    $(dialogDiv).append(pContent);
    $(document.body).append(dialogDiv);

    $("#" + dialogId + "Footer").remove();
    var dialogFooterTemplate = $("<script id='" + dialogId + "Footer" + "'  type='text/ x - jsrender'></script>");
    
    var dialogFooterDiv = $("<div class='footerspan' style='float:right'></div>");
    var dialogBtnOk = $('<button id="' +
        dialogId +
        'BtnOk" class="e-button e-js e-ntouch e-btn-mini e-btn e-select e-widget e-txt e-corner" tabindex="" type="button" role="button" aria-describedby="OK" aria-disabled="false" style="height: 30px; width: 70px;">OK</button>');
    var dialogBtnCancel = $('<button id="' +
        dialogId +
        'BtnCancel" class="e-button e-js e-ntouch e-btn-mini e-btn e-select e-widget e-txt e-corner" tabindex="" type="button" role="button" aria-describedby="Cancel" aria-disabled="false" style="height: 30px; width: 70px;">Cancel</button>');
    dialogFooterDiv.append(dialogBtnOk);
    dialogFooterDiv.append(dialogBtnCancel);
    dialogFooterTemplate.append(dialogFooterDiv);
    $(document.body).append(dialogFooterTemplate);

    $(document).unbind("click").on('click', "#" + dialogId + "BtnOk", function () {
        if (closeEnevt!=null) {
            closeEnevt(firstUrl);
        }
        EServiceExpress.Common.Instance.CloseDialog(dialogId);
    }).on('click', "#" + dialogId + "BtnCancel", function () {
        EServiceExpress.Common.Instance.CloseDialog(dialogId);
    });

    $("#" + dialogId)
        .ejDialog({
            title: title,
            showOnInit: false,
            actionButtons: ["close"],
            close: function () {
                EServiceExpress.Common.Instance.CloseDialog(dialogId);
            },
            enableModal: true,
            showFooter: true,
            IsResponsive: true,
            footerTemplateId: dialogId + "Footer"
        });

    $("#" + dialogId).ejDialog("open");
}

EServiceExpress.Common.prototype.CloseDialog = function (dialogId) {
    $("#" + dialogId).ejDialog("close");
    $("#" + dialogId + "_wrapper").remove();
    $("#" + dialogId + "Footer").remove();
}

EServiceExpress.Common.prototype.ModalConfirmDialog = function (dialogId, title, content, closeEnevt, info, firstUrl, lastUrl) {

    $("#" + dialogId).remove();
    var dialogDiv = $("<div id='" + dialogId + "' title='" + title + "' hidden></div>");
    var pContent = $("<p>" + content + " </p>");
    $(dialogDiv).append(pContent);
    $(document.body).append(dialogDiv);

    $("#" + dialogId + "Footer").remove();
    var dialogFooterTemplate = $("<script id='" + dialogId + "Footer" + "'  type='text/ x - jsrender'></script>");

    var dialogFooterDiv = $("<div class='footerspan' style='float:right'></div>");
    //var dialogBtnOk = $("<button  id='" + dialogId + "BtnOk" + "' >OK</button >");

    var dialogBtnYes = $('<button id="' +
        dialogId +
        'BtnYes" class="e-button e-js e-ntouch e-btn-mini e-btn e-select e-widget e-txt e-corner" tabindex="" type="button" role="button" aria-describedby="Yes" aria-disabled="false" style="height: 30px; width: 70px;">YES</button>');
    var dialogBtnNo = $('<button id="' +
        dialogId +
        'BtnNo" class="e-button e-js e-ntouch e-btn-mini e-btn e-select e-widget e-txt e-corner" tabindex="" type="button" role="button" aria-describedby="No" aria-disabled="false" style="height: 30px; width: 70px;">NO</button>');
    dialogFooterDiv.append(dialogBtnYes);
    dialogFooterDiv.append(dialogBtnNo);
    dialogFooterTemplate.append(dialogFooterDiv);
    $(document.body).append(dialogFooterTemplate);

    $(document).unbind("click").on('click', "#" + dialogId + "BtnYes", function () {
        $(this).attr("disabled", "disabled");
        info.Submit = 'yes';
        closeEnevt(info, firstUrl, lastUrl);
        EServiceExpress.Common.Instance.CloseDialog(dialogId);
    }).on('click', "#" + dialogId + "BtnNo", function () {
        EServiceExpress.Common.Instance.CloseDialog(dialogId);
    });

    $("#" + dialogId)
        .ejDialog({
            title: title,
            showOnInit: false,
            zIndex: 11000,
            actionButtons: ["close"],
            close: function() {
                EServiceExpress.Common.Instance.CloseDialog(dialogId);
            },
            enableModal: true,
            showFooter: true,
            IsResponsive: true,
            footerTemplateId: dialogId + "Footer"
        });

    $("#" + dialogId).ejDialog("open");
};

EServiceExpress.Common.prototype.LogOut = function (itemUrl) {
    $.ajax({
        url: itemUrl,
        type: "Post",
        data: {
        },
        dateType: "json",
        async: false,
        success: function () {
        }
    });
}