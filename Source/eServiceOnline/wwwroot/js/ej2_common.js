var eServiceOnline = eServiceOnline || {};

eServiceOnline.Site = function (listUrl, processContextMenuUrl, getMenuUrl) {
    this.listUrl = listUrl;
    this.processContextMenuUrl = processContextMenuUrl;
    this.getMenuUrl = getMenuUrl;
};

eServiceOnline.Site.Instance = new eServiceOnline.Site();

eServiceOnline.Site.prototype.switchMenu = function () {
    var menuList = {
        upcomingJobs: ["headerUpcomingJobs", "UpcomingJobs"],
        productHaul: ["headerProductHaul", "ProductHaul"],
        rigBoard: ["headerRigBoard", "RigBoard"],
        bulkPlantBoard: ["headerBulkPlant", "BulkPlant"],
        CrewBoard: ["headerResource", "Resource"],
        Calendar: ["headerCalendar", "Calendar"]
    };
    var arrPathName = window.location.pathname.split('/');
    var pathName = arrPathName[1];

    $.each(menuList, function (i, ele) {
        if ($.inArray(pathName, ele) !== -1) {
            $("#" + ele[0]).addClass("active").siblings().removeClass("active");
        }
    });
};

eServiceOnline.Site.prototype.switchMenu();

eServiceOnline.Site.prototype.NoPopsUpWindow = function (item) {

    var itemurl = this.processContextMenuUrl;
    $.ajax({
        url: itemurl,
        method: "Post",
        data: item,
        success: function (result) {
            if (result === true) {
                window.location.reload();
            } else {
                alert("Operation fail.");
            }
        }
    });
}

eServiceOnline.Site.prototype.PopsUpWindow = function (item) {
    console.log(item);
    var itemurl = this.processContextMenuUrl;
    $.ajax({
        url: itemurl,
        method: "Post",
        data: item,
        success: function (data) {
            $("#tipContent").hide();

            $(".sj_mask").fadeIn(300,
                function () {
                    $("#basicDialog_title").html(item.MenuName.replace(new RegExp('%%%', "g"), "'"));
                    $("#basicDialog_dialog-content").html(data);
                    var dialog = document.getElementById("basicDialog").ej2_instances[0];
                    dialog.show();
                }
            );
        }
    });
}

eServiceOnline.Site.prototype.CloseDialog = function () {
    $(".sj_mask").fadeOut(300, function () { });
}

eServiceOnline.Site.prototype.CloseConfirmDialog = function () {
    $(".sj_mask").fadeOut(300, function () {
        var dialog = document.getElementById("basicDialog").ej2_instances[0];
        dialog.hide();
    });
}
eServiceOnline.Site.prototype.GetContextMenuList = function(url,param, e) {

    $.ajax({
        url: url,
        method: "Post",
        data: {
            "param": param
        },      
        success: function(result) {
            if (result != "") {
                eServiceOnline.Site.prototype.InitContext(result, e);
            }
        }
    });
};
eServiceOnline.Site.prototype.InitContext = function (result, e) {
    var contextMenus = result;
    if (contextMenus === null) return;
    $("#tipContent").html("");

    if (contextMenus.length > 0) {
        var splitLine =
            "<div style=\"margin-left: 10px; margin-right:10px; height: 0; border: 1px solid #AE4B68;\"></div>";
        $("#tipContent").append(" <div style='border-top:4px solid #AE4B68;'></div>");
        for (var i = 0; i < contextMenus.length; i++) {

            var contextMenuHtmlString = "";
            if (contextMenus[i].ProcessingMode === 2) {
                if (contextMenus[i].ControllerName !== null && contextMenus[i].ActionName !== null) {
                    if (contextMenus[i].IsHaveSplitLine === true && contextMenus.length > 1) {
                        if (contextMenus[i].IsDisabled === true) {
                            contextMenuHtmlString =
                                splitLine +
                                "<div> <a class='disableMenuStyle'>" +
                                contextMenus[i].MenuName +
                                "</a>  </div>";
                        } else {
                            contextMenuHtmlString =
                                splitLine +
                                "<div> <a class='contextMenuStyle' onclick='siteInstance.PopsUpWindow(" +
                                JSON.stringify(contextMenus[i]) +
                                ")' >" +
                                contextMenus[i].MenuName +
                                "</a></div>";
                        }

                    } else {
                        if (contextMenus[i].IsDisabled === true) {
                            contextMenuHtmlString = "<div> <a class='disableMenuStyle' >" +
                                contextMenus[i].MenuName +
                                "</a>  </div>";
                        } else {
                            contextMenuHtmlString =
                                "<div> <a class='contextMenuStyle' onclick='siteInstance.PopsUpWindow(" +
                                JSON.stringify(contextMenus[i]) +
                                ")' >" +
                                contextMenus[i].MenuName +
                                "</a></div>";
                        }

                    }
                } else {
                    if (contextMenus[i].IsHaveSplitLine === true && contextMenus.length > 1) {
                        if (contextMenus[i].IsDisabled === true) {
                            contextMenuHtmlString = splitLine +
                                " <div> <a class='disableMenuStyle' >" +
                                contextMenus[i].MenuName +
                                "</a>  </div>";
                        } else {
                            contextMenuHtmlString = splitLine +
                                " <div> <a class='contextMenuStyle' >" +
                                contextMenus[i].MenuName +
                                "</a>  </div>";
                        }

                    } else {
                        if (contextMenus[i].IsDisabled === true) {
                            contextMenuHtmlString =
                                " <div> <a class='disableMenuStyle' >" +
                                contextMenus[i].MenuName +
                                "</a>  </div>";
                        } else {
                            contextMenuHtmlString =
                                " <div> <a class='contextMenuStyle'  >" +
                                contextMenus[i].MenuName +
                                "</a>  </div>";
                        }

                    }

                }

            } else if (contextMenus[i].ProcessingMode === 1) {
                if (contextMenus[i].IsHaveSplitLine === true && contextMenus.length > 1) {
                    if (contextMenus[i].IsDisabled === true) {
                        contextMenuHtmlString = splitLine +
                            " <div> <a class='disableMenuStyle'  >" +
                            contextMenus[i].MenuName +
                            "</a>  </div>";
                    } else {
                        contextMenuHtmlString = splitLine +
                            " <div> <a class='contextMenuStyle'  onclick='siteInstance.NoPopsUpWindow(" +
                            JSON.stringify(contextMenus[i]) +
                            ")' >" +
                            contextMenus[i].MenuName +
                            "</a>  </div>";
                    }

                } else {
                    if (contextMenus[i].IsDisabled === true) {
                        contextMenuHtmlString =
                            " <div> <a class='disableMenuStyle' >" + contextMenus[i].MenuName + "</a>  </div>";
                    } else {
                        contextMenuHtmlString =
                            " <div> <a class='contextMenuStyle'  onclick='siteInstance.NoPopsUpWindow(" +
                            JSON.stringify(contextMenus[i]) +
                            ")' >" +
                            contextMenus[i].MenuName +
                            "</a>  </div>";
                    }

                }

            } else if (contextMenus[i].ProcessingMode === 3) {
                if (contextMenus[i].IsDisabled === true) {
                    contextMenuHtmlString = " <div> <a class='disableMenuStyle' >" +
                        contextMenus[i].MenuName +
                        "</a>  </div>";
                    if (contextMenus[i].MenuList != null && contextMenus[i].MenuList.length > 0) {
                        contextMenuHtmlString =
                            "<div class=\"cellmenu\"> <a class='disableMenuStyle'>" +
                            contextMenus[i].MenuName +
                            "</a>";
                        contextMenuHtmlString =
                            contextMenuHtmlString +
                            "<i class=\"e-icon e-arrow-sans-right showcellmenu\"></i> <ul>";
                        for (var j1 = 0; j1 < contextMenus[i].MenuList.length; j1++) {
                            contextMenuHtmlString = contextMenuHtmlString +
                                "<li><a class='disableMenuStyle' >" +
                                contextMenus[i].MenuList[j1].MenuName
                                .replace(new RegExp('%%%', "g"), "'") +
                                "</a>";
                            if (contextMenus[i].MenuList[j1].MenuList != null &&
                                contextMenus[i].MenuList[j1].MenuList.length > 0) {
                                contextMenuHtmlString =
                                    contextMenuHtmlString +
                                    "<i class='e-icon e-arrow-sans-right showcellmenu'></i><ul>";
                                for (var k1 = 0; k1 < contextMenus[i].MenuList[j1].MenuList.length; k1++) {
                                    contextMenuHtmlString = contextMenuHtmlString +
                                        "<li><a class='disableMenuStyle' >" +
                                        contextMenus[i].MenuList[j1].MenuList[k1].MenuName
                                        .replace(new RegExp('%%%', "g"), "'") +
                                        "</a></li>";
                                }
                                contextMenuHtmlString = contextMenuHtmlString + "</ul>";
                            }
                            contextMenuHtmlString = contextMenuHtmlString + "</li>";

                        }
                        contextMenuHtmlString = contextMenuHtmlString + "</ul> </div>";
                    } else {
                        contextMenuHtmlString =
                            " <div> <a class='disableMenuStyle' >" + contextMenus[i].MenuName + "</a>  </div>";
                    }
                } else {
                    if (contextMenus[i].MenuList != null && contextMenus[i].MenuList.length > 0) {
                        contextMenuHtmlString = "<div class=\"cellmenu\"> <a class='contextMenuStyle'>" +
                            contextMenus[i].MenuName +
                            "</a>";
                        contextMenuHtmlString = contextMenuHtmlString +
                            "<i class=\"e-icon e-arrow-sans-right showcellmenu\"></i> <ul>";
                        for (var j = 0; j < contextMenus[i].MenuList.length; j++) {

                            if (contextMenus[i].MenuList[j].IsDisabled === true) {
                                contextMenuHtmlString = contextMenuHtmlString +
                                    "<li><a class='disableMenuStyle' >" +
                                    contextMenus[i].MenuList[j].MenuName.replace(new RegExp('%%%', "g"), "'") +
                                    "</a>";
                            } else {
                                if (contextMenus[i].MenuList[j].ProcessingMode === 2) {

                                    contextMenuHtmlString = contextMenuHtmlString +
                                        "<li><a class='contextMenuStyle' onclick='siteInstance.PopsUpWindow(" +
                                        JSON.stringify(contextMenus[i].MenuList[j]) +
                                        ")' >" +
                                        contextMenus[i].MenuList[j].MenuName.replace(new RegExp('%%%', "g"), "'") +
                                        "</a>";
                                } else if (contextMenus[i].MenuList[j].ProcessingMode === 1) {

                                    contextMenuHtmlString = contextMenuHtmlString +
                                        "<li><a class='contextMenuStyle' onclick='siteInstance.NoPopsUpWindow(" +
                                        JSON.stringify(contextMenus[i].MenuList[j]) +
                                        ")' >" +
                                        contextMenus[i].MenuList[j].MenuName.replace(new RegExp('%%%', "g"), "'") +
                                        "</a>";
                                }
                            }

                            if (contextMenus[i].MenuList[j].MenuList != null &&
                                contextMenus[i].MenuList[j].MenuList.length > 0) {
                                contextMenuHtmlString =
                                    contextMenuHtmlString +
                                    "<i class='e-icon e-arrow-sans-right showcellmenu'></i><ul>";
                                for (var k = 0; k < contextMenus[i].MenuList[j].MenuList.length; k++) {
                                    if (contextMenus[i].MenuList[j].MenuList[k].IsDisabled === true) {
                                        contextMenuHtmlString = contextMenuHtmlString +
                                            "<li><a class='disableMenuStyle' >" +
                                            contextMenus[i].MenuList[j].MenuList[k].MenuName
                                            .replace(new RegExp('%%%', "g"), "'") +
                                            "</a></li>";
                                    } else {
                                        if (contextMenus[i].MenuList[j].MenuList[k].ProcessingMode === 2) {

                                            contextMenuHtmlString = contextMenuHtmlString +
                                                "<li><a class='contextMenuStyle' onclick='siteInstance.PopsUpWindow(" +
                                                JSON.stringify(contextMenus[i].MenuList[j].MenuList[k]) +
                                                ")' >" +
                                                contextMenus[i].MenuList[j].MenuList[k].MenuName
                                                .replace(new RegExp('%%%', "g"), "'") +
                                                "</a></li>";
                                        } else if (
                                            contextMenus[i].MenuList[j].MenuList[k].ProcessingMode === 1) {

                                            contextMenuHtmlString = contextMenuHtmlString +
                                                "<li><a class='contextMenuStyle' onclick='siteInstance.NoPopsUpWindow(" +
                                                JSON.stringify(contextMenus[i].MenuList[j].MenuList[k]) +
                                                ")' >" +
                                                contextMenus[i].MenuList[j].MenuList[k].MenuName
                                                .replace(new RegExp('%%%', "g"), "'") +
                                                "</a></li>";
                                        } else {
                                            contextMenuHtmlString = contextMenuHtmlString +
                                                "<li><a class='disableMenuStyle' >" +
                                                contextMenus[i].MenuList[j].MenuList[k].MenuName
                                                .replace(new RegExp('%%%', "g"), "'") +
                                                "</a></li>";
                                        }
                                    }
                                }
                                contextMenuHtmlString = contextMenuHtmlString + "</ul>";
                            }
                            contextMenuHtmlString = contextMenuHtmlString + "</li>";

                        }
                        contextMenuHtmlString = contextMenuHtmlString + "</ul> </div>";
                    } else {
                        contextMenuHtmlString = " <div> <a class='disableMenuStyle' >" +
                            contextMenus[i].MenuName +
                            "</a>  </div>";
                    }
                }
            } else {
                if (contextMenus[i].IsDisabled === true) {
                    contextMenuHtmlString = " <div> <a class='disableMenuStyle' >" +
                        contextMenus[i].MenuName +
                        "</a>  </div>";
                } else {
                    contextMenuHtmlString = " <div> <a class='contextMenuStyle' >" +
                        contextMenus[i].MenuName +
                        "</a>  </div>";
                }

            }
            $("#tipContent").append(contextMenuHtmlString);
        }
        var $screenWidth = $(window).width();
        var $screenHeigth = $(window).height();
        var $tipContentWidth = $("#tipContent").width();
        var $tipContentHeigth = $("#tipContent").height();

        var $tiptop = (e.clientY + 20) > ($screenHeigth - $tipContentHeigth)
            ? (e.clientY - $tipContentHeigth) > 0
            ? e.clientY - $tipContentHeigth
            : 0
            : e.clientY + 20;
        var $tipleft = (e.clientX - 20) > ($screenWidth - $tipContentWidth)
            ? (e.clientX - $tipContentWidth) > 0
            ? e.clientX - $tipContentWidth
            : 0
            : e.clientX - 20;

        $("#tipContent").css({
            top: $tiptop,
            left: $tipleft,
            display: 'block'
        });
        $("#tipContent").on("mouseleave",
            function() {
                $(this).hide();
            });
        $(".showcellmenu").on("mouseover",
            function() {
                $(this).parent().siblings().find(".showcellmenu").next().hide();
                var $cellmenuWidth = $(this).next("ul").width();
                var $cellmenuHeigth = $(this).next("ul").height();
                var $tipContentTop = $(this).parent().offset().top;
                var $tipContentLeft = $(this).parent().offset().left;
                var $marginleft = $(this).parent().width();

                var $celltop = ($screenHeigth - $tipContentTop) > $cellmenuHeigth
                    ? 0
                    : ($cellmenuHeigth > $tipContentTop)
                    ? (50 - $tipContentTop)
                    : (36 - $cellmenuHeigth);
                var $cellleft = ($screenWidth - $tipContentLeft - $marginleft) > $cellmenuWidth
                    ? $marginleft
                    : -$cellmenuWidth;
                $(this)
                    .next("ul")
                    .css({
                        top: $celltop,
                        left: $cellleft,
                        display: 'block'
                    });
                $(this).next("ul").show();
            });
        $(".showcellmenu +ul").on("mouseleave",
            function() {
                $(this).hide();
            });
    }
}

