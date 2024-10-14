// Write your Javascript code.

var eServiceOnline = eServiceOnline  || {};

eServiceOnline.Site = function (listUrl, processContextMenuUrl) {
    this.listUrl = listUrl;
    this.processContextMenuUrl = processContextMenuUrl;
    this.baseControllerActionUrls = {};
};

eServiceOnline.Site.Instance = new eServiceOnline.Site();

eServiceOnline.Site.prototype.setControllersActions = function(obj) {
    this.baseControllerActionUrls = obj;
}

eServiceOnline.Site.prototype.MergeStyledCell = function (args) {
    if (args.data[args.column.field] !== null) {
      
        $(args.cell).addClass(args.data[args.column.field].Style);
        $(args.cell).text(args.data[args.column.field].PropertyValue);
        $("table th div").removeClass("e-gridtooltip");

        if (args.data[args.column.field].IsNeedRowMerge === true) {
            args.rowMerge(args.data[args.column.field].RowMergeNumber);
        } 

        $(args.cell).css({ "padding": "0.3em" });

        $(document).delegate(args.cell,'contextmenu',function (e) {
                e.preventDefault();
        });

        $("#matchDialog").hide();
        $(args.cell).mousedown(function (e) {
            $("#tipContent").hide();
            $(args.cell).parent("tr").removeAttr("aria-selected");
            var contextMenus = args.data[args.column.field].ContextMenus;
            //右键为3
            
            $("table td").removeClass("e-cellselectionbackground e-activecell");
            $(args.cell).addClass("e-cellselectionbackground e-activecell");
            if (contextMenus === null)
                return;
            if (3 === e.which) {
                $("#tipContent").html("");
                // ReSharper disable once QualifiedExpressionMaybeNull
                if (contextMenus.length > 0) {
                    var splitLine =
                        "<div style=\"margin-left: 10px; margin-right:10px; height: 0; border: 1px solid #AE4B68;\"></div>";
                    $("#tipContent").append(" <div style='border-top:4px solid #AE4B68;'></div>");
                    for (var i = 0; i < contextMenus.length; i++) {
                        console.log(contextMenus[i].MenuStyle);
                            var contextMenuHtmlString ="" ;
                        if (contextMenus[i].ProcessingMode === 2) {
                            if (contextMenus[i].ControllerName !== null && contextMenus[i].ActionName !== null) {
                                if (contextMenus[i].IsHaveSplitLine === true && contextMenus.length>1) {
                                    if (contextMenus[i].IsDisabled === true) {
                                        contextMenuHtmlString =
                                            splitLine +
                                        "<div> <a class='disableMenuStyle " + contextMenus[i].MenuStyle + "'>" +
                                            contextMenus[i].MenuName.replace(new RegExp('%%%', "g"), "'") +
                                            "</a>  </div>";
                                    } else {
                                        contextMenuHtmlString =
                                            splitLine +
                                        "<div> <a class='contextMenuStyle " + contextMenus[i].MenuStyle + "' onclick='siteInstance.PopsUpWindow(" +
                                            JSON.stringify(contextMenus[i]).replace(new RegExp('\'', "g"),"&#39;") +
                                            ")' >" +
                                            contextMenus[i].MenuName.replace(new RegExp('%%%', "g"), "'") +
                                            "</a></div>";
                                    }
                                    
                                } 
                                else {
                                    if (contextMenus[i].IsDisabled === true) {
                                        contextMenuHtmlString = "<div> <a class='disableMenuStyle " + contextMenus[i].MenuStyle + "' >" +
                                            contextMenus[i].MenuName.replace(new RegExp('%%%', "g"), "'") +
                                            "</a>  </div>";
                                    } else {
                                        contextMenuHtmlString = "<div> <a class='contextMenuStyle " + contextMenus[i].MenuStyle + "' onclick='siteInstance.PopsUpWindow(" + JSON.stringify(contextMenus[i]).replace(new RegExp('\'', "g"),"&#39;") +
                                            ")' >" +
                                            contextMenus[i].MenuName.replace(new RegExp('%%%', "g"), "'") +
                                            "</a></div>";
                                    }
                                   
                                }
                            }
                            else {
                                if (contextMenus[i].IsHaveSplitLine === true && contextMenus.length > 1) {
                                    if (contextMenus[i].IsDisabled === true) {
                                        contextMenuHtmlString = splitLine +
                                            " <div> <a class='disableMenuStyle " + contextMenus[i].MenuStyle + "' >" + contextMenus[i].MenuName.replace(new RegExp('%%%', "g"), "'") + "</a>  </div>";
                                    }else
                                    {
                                        contextMenuHtmlString = splitLine +
                                            " <div> <a class='contextMenuStyle  " + contextMenus[i].MenuStyle + "'>" + contextMenus[i].MenuName.replace(new RegExp('%%%', "g"), "'") + "</a>  </div>";
                                    }
                                   
                                } else {
                                    if (contextMenus[i].IsDisabled === true) {
                                        contextMenuHtmlString =
                                            " <div> <a class='disableMenuStyle " + contextMenus[i].MenuStyle + "' >" + contextMenus[i].MenuName.replace(new RegExp('%%%', "g"), "'") + "</a>  </div>";
                                    } else {
                                        contextMenuHtmlString =
                                            " <div> <a class='contextMenuStyle  " + contextMenus[i].MenuStyle + "' >" + contextMenus[i].MenuName.replace(new RegExp('%%%', "g"), "'") + "</a>  </div>";
                                    }
                                    
                                }
                               
                            }
                           
                        } else if (contextMenus[i].ProcessingMode === 1) {
                            if (contextMenus[i].IsHaveSplitLine === true && contextMenus.length > 1) {
                                if (contextMenus[i].IsDisabled === true) {
                                    contextMenuHtmlString = splitLine +
                                        " <div> <a class='disableMenuStyle " + contextMenus[i].MenuStyle + "'  >" +
                                        contextMenus[i].MenuName.replace(new RegExp('%%%', "g"), "'") +
                                        "</a>  </div>";
                                } else {
                                    contextMenuHtmlString = splitLine +
                                        " <div> <a class='contextMenuStyle  " + contextMenus[i].MenuStyle + "'  onclick='siteInstance.NoPopsUpWindow(" +
                                        JSON.stringify(contextMenus[i]).replace(new RegExp('\'', "g"),"&#39;") +
                                        ")' >" +
                                        contextMenus[i].MenuName.replace(new RegExp('%%%', "g"), "'") +
                                        "</a>  </div>";
                                }
                                
                            } else {
                                if (contextMenus[i].IsDisabled === true) {
                                    contextMenuHtmlString =
                                        " <div> <a class='disableMenuStyle' >" + contextMenus[i].MenuName.replace(new RegExp('%%%', "g"), "'") + "</a>  </div>"; 
                                } else {
                                    contextMenuHtmlString =
                                        " <div> <a class='contextMenuStyle  " + contextMenus[i].MenuStyle + "'  onclick='siteInstance.NoPopsUpWindow(" + JSON.stringify(contextMenus[i]).replace(new RegExp('\'', "g"),"&#39;") + ")' >" + contextMenus[i].MenuName.replace(new RegExp('%%%', "g"), "'") + "</a>  </div>";
                                }
                               
                            }
                        } else if (contextMenus[i].ProcessingMode === 4) {
                            if (contextMenus[i].IsHaveSplitLine === true && contextMenus.length > 1) {
                                if (contextMenus[i].IsDisabled === true) {
                                    contextMenuHtmlString = splitLine +
                                        " <div> <a class='disableMenuStyle " + contextMenus[i].MenuStyle + "'  >" +
                                        contextMenus[i].MenuName.replace(new RegExp('%%%', "g"), "'") +
                                        "</a>  </div>";
                                } else {
                                    contextMenuHtmlString = splitLine +
                                        " <div> <a class='contextMenuStyle  " + contextMenus[i].MenuStyle + "'  onclick='siteInstance.OpenInNewTab(" +
                                        JSON.stringify(contextMenus[i]).replace(new RegExp('\'', "g"), "&#39;") +
                                        ")' >" +
                                        contextMenus[i].MenuName.replace(new RegExp('%%%', "g"), "'") +
                                        "</a>  </div>";
                                }

                            } else {
                                if (contextMenus[i].IsDisabled === true) {
                                    contextMenuHtmlString =
                                        " <div> <a class='disableMenuStyle' >" + contextMenus[i].MenuName.replace(new RegExp('%%%', "g"), "'") + "</a>  </div>";
                                } else {
                                    contextMenuHtmlString =
                                        " <div> <a class='contextMenuStyle  " + contextMenus[i].MenuStyle + "'  onclick='siteInstance.OpenInNewTab(" + JSON.stringify(contextMenus[i]).replace(new RegExp('\'', "g"), "&#39;") + ")' >" + contextMenus[i].MenuName.replace(new RegExp('%%%', "g"), "'") + "</a>  </div>";
                                }

                            }

                        } else if (contextMenus[i].ProcessingMode === 3) {
                            console.log(contextMenus[i]);
                            if (contextMenus[i].IsDisabled === true) {
                                contextMenuHtmlString = "";
                                if (contextMenus[i].IsHaveSplitLine === true && contextMenus.length > 1) {
                                    contextMenuHtmlString += splitLine;
                                }
                                contextMenuHtmlString = " <div> <a class='disableMenuStyle " + contextMenus[i].MenuStyle + "' >" + contextMenus[i].MenuName.replace(new RegExp('%%%', "g"), "'") + "</a>";
                                if (contextMenus[i].MenuList != null && contextMenus[i].MenuList.length > 0) {
                                    //contextMenuHtmlString =
                                    //    "<div class=\"cellmenu\"> <a class='disableMenuStyle " + contextMenus[i].MenuStyle + "'>" +
                                    //    contextMenus[i].MenuName +
                                    //    "</a>";
                                    contextMenuHtmlString =
                                        contextMenuHtmlString +
                                        "<i class=\"e-icon e-arrow-sans-right showcellmenu\"></i> <ul>";
                                    for (var j1 = 0; j1 < contextMenus[i].MenuList.length; j1++) {
                                        contextMenuHtmlString = contextMenuHtmlString +
                                            "<li><a class='disableMenuStyle " + contextMenus[i].MenuList[j1].MenuStyle + "' >" +
                                            contextMenus[i].MenuList[j1].MenuName
                                                .replace(new RegExp('%%%', "g"), "'") +
                                            "</a>";
                                        if (contextMenus[i].MenuList[j1].MenuList != null && contextMenus[i].MenuList[j1].MenuList.length > 0) {
                                            contextMenuHtmlString = contextMenuHtmlString + "<i class='e-icon e-arrow-sans-right showcellmenu'></i><ul>";
                                            for (var k1 = 0; k1 < contextMenus[i].MenuList[j1].MenuList.length; k1++) {
                                                contextMenuHtmlString = contextMenuHtmlString +
                                                    "<li><a class='disableMenuStyle " + contextMenus[i].MenuList[j1].MenuList[k1].MenuStyle + "' >" + contextMenus[i].MenuList[j1].MenuList[k1].MenuName.replace(new RegExp('%%%', "g"), "'") + "</a></li>";
                                            }
                                            contextMenuHtmlString = contextMenuHtmlString + "</ul>";
                                        }
                                        contextMenuHtmlString = contextMenuHtmlString + "</li>";

                                    }
                                    contextMenuHtmlString = contextMenuHtmlString + "</ul> </div>";
                                } else {
                                    contextMenuHtmlString =
                                        " <div> <a class='disableMenuStyle " + contextMenus[i].MenuStyle + "' >" + contextMenus[i].MenuName.replace(new RegExp('%%%', "g"), "'") + "</a>  </div>";
                                } 
                            } else {
                                if (contextMenus[i].MenuList != null && contextMenus[i].MenuList.length > 0) {
                                    contextMenuHtmlString = "";
                                    if (contextMenus[i].IsHaveSplitLine === true && contextMenus.length > 1) {
                                        contextMenuHtmlString += splitLine;
                                    }
                                    contextMenuHtmlString +=
                                        "<div class=\"cellmenu\"> <a class='contextMenuStyle  " + contextMenus[i].MenuStyle + "'>" +
                                        contextMenus[i].MenuName.replace(new RegExp('%%%', "g"), "'") +
                                        "</a>";
                                    contextMenuHtmlString =
                                        contextMenuHtmlString +
                                        "<i class=\"e-icon e-arrow-sans-right showcellmenu\"></i> <ul>";
                                    for (var j = 0; j < contextMenus[i].MenuList.length; j++) {

                                        if (contextMenus[i].MenuList[j].IsDisabled === true) {
                                            contextMenuHtmlString = contextMenuHtmlString +
                                                "<li><a class='disableMenuStyle " + contextMenus[i].MenuList[j].MenuStyle + "' title='"+ contextMenus[i].MenuList[j].MenuTips + "' >" +
                                                contextMenus[i].MenuList[j].MenuName
                                                .replace(new RegExp('%%%', "g"), "'") +
                                                "</a>";
                                        } else {
                                            if (contextMenus[i].MenuList[j].ProcessingMode === 2) {

                                                contextMenuHtmlString = contextMenuHtmlString +
                                                    "<li><a class='contextMenuStyle  " + contextMenus[i].MenuList[j].MenuStyle + "' title='"+ contextMenus[i].MenuList[j].MenuTips + "' onclick='siteInstance.PopsUpWindow(" +
                                                    JSON.stringify(contextMenus[i].MenuList[j]).replace(new RegExp('\'', "g"),"&#39;") +
                                                    ")' >" +
                                                    contextMenus[i].MenuList[j].MenuName.replace(new RegExp('%%%', "g"), "'") +
                                                    "</a>";
                                            } else if (contextMenus[i].MenuList[j].ProcessingMode === 1) {

                                                contextMenuHtmlString = contextMenuHtmlString +
                                                    "<li><a class='contextMenuStyle  " + contextMenus[i].MenuList[j].MenuStyle + "' title='"+ contextMenus[i].MenuList[j].MenuTips  + "' onclick='siteInstance.NoPopsUpWindow(" +
                                                    JSON.stringify(contextMenus[i].MenuList[j]).replace(new RegExp('\'', "g"),"&#39;") +
                                                    ")' >" +
                                                    contextMenus[i].MenuList[j].MenuName.replace(new RegExp('%%%', "g"), "'") +
                                                    "</a>";
                                            } else if (contextMenus[i].MenuList[j].ProcessingMode === 4) {

                                                contextMenuHtmlString = contextMenuHtmlString +
                                                    "<li><a class='contextMenuStyle  " + contextMenus[i].MenuList[j].MenuStyle + "' title='" + contextMenus[i].MenuList[j].MenuTips + "' onclick='siteInstance.OpenInNewTab(" +
                                                    JSON.stringify(contextMenus[i].MenuList[j]).replace(new RegExp('\'', "g"), "&#39;") +
                                                    ")' >" +
                                                    contextMenus[i].MenuList[j].MenuName.replace(new RegExp('%%%', "g"), "'") +
                                                    "</a>";
                                            }
                                        }

                                        if (contextMenus[i].MenuList[j].MenuList != null && contextMenus[i].MenuList[j].MenuList.length > 0) {
                                            contextMenuHtmlString = contextMenuHtmlString + "<i class='e-icon e-arrow-sans-right showcellmenu'></i><ul>";
                                            for (var k = 0; k < contextMenus[i].MenuList[j].MenuList.length; k++) {
                                                if (contextMenus[i].MenuList[j].MenuList[k].IsDisabled === true) {
                                                    contextMenuHtmlString = contextMenuHtmlString +
                                                        "<li><a class='disableMenuStyle " + contextMenus[i].MenuList[j].MenuList[k].MenuStyle + "' title='"+ contextMenus[i].MenuList[j].MenuList[k].MenuTips  + "' >" + contextMenus[i].MenuList[j].MenuList[k].MenuName.replace(new RegExp('%%%', "g"), "'") + "</a></li>";
                                                } else {
                                                    if (contextMenus[i].MenuList[j].MenuList[k].ProcessingMode === 2) {

                                                        contextMenuHtmlString = contextMenuHtmlString +
                                                            "<li><a class='contextMenuStyle " + contextMenus[i].MenuList[j].MenuList[k].MenuStyle + "' title='"+ contextMenus[i].MenuList[j].MenuList[k].MenuTips  + "' onclick='siteInstance.PopsUpWindow(" +
                                                            JSON.stringify(contextMenus[i].MenuList[j].MenuList[k]).replace(new RegExp('\'', "g"),"&#39;") +
                                                            ")' >" +
                                                            contextMenus[i].MenuList[j].MenuList[k].MenuName.replace(new RegExp('%%%', "g"), "'") +
                                                            "</a></li>";
                                                    } else if (
                                                        contextMenus[i].MenuList[j].MenuList[k].ProcessingMode === 1) {

                                                        contextMenuHtmlString = contextMenuHtmlString +
                                                            "<li><a class='contextMenuStyle " + contextMenus[i].MenuList[j].MenuList[k].MenuStyle + "' title='"+ contextMenus[i].MenuList[j].MenuList[k].MenuTips  + "' onclick='siteInstance.NoPopsUpWindow(" +
                                                            JSON.stringify(contextMenus[i].MenuList[j].MenuList[k]).replace(new RegExp('\'', "g"),"&#39;") +
                                                            ")' >" +
                                                            contextMenus[i].MenuList[j].MenuList[k].MenuName.replace(new RegExp('%%%', "g"), "'") +
                                                            "</a></li>";
                                                    } else if (
                                                        contextMenus[i].MenuList[j].MenuList[k].ProcessingMode === 4) {

                                                        contextMenuHtmlString = contextMenuHtmlString +
                                                            "<li><a class='contextMenuStyle " + contextMenus[i].MenuList[j].MenuList[k].MenuStyle + "' title='" + contextMenus[i].MenuList[j].MenuList[k].MenuTips + "' onclick='siteInstance.OpenInNewTab(" +
                                                            JSON.stringify(contextMenus[i].MenuList[j].MenuList[k]).replace(new RegExp('\'', "g"), "&#39;") +
                                                            ")' >" +
                                                            contextMenus[i].MenuList[j].MenuList[k].MenuName.replace(new RegExp('%%%', "g"), "'") +
                                                            "</a></li>";
                                                    } else {
                                                        contextMenuHtmlString = contextMenuHtmlString +
                                                            "<li><a class='disableMenuStyle  " + contextMenus[i].MenuList[j].MenuList[k].MenuStyle + "' title='"+ contextMenus[i].MenuList[j].MenuList[k].MenuTips  + "' >" + contextMenus[i].MenuList[j].MenuList[k].MenuName.replace(new RegExp('%%%', "g"), "'") + "</a></li>";
                                                    }
                                                }
                                            }
                                            contextMenuHtmlString = contextMenuHtmlString + "</ul>";
                                        }
                                        contextMenuHtmlString = contextMenuHtmlString + "</li>";

                                    }
                                    contextMenuHtmlString = contextMenuHtmlString + "</ul> </div>";
                                } else {
                                    contextMenuHtmlString = "";
                                    if (contextMenus[i].IsHaveSplitLine === true && contextMenus.length > 1) {
                                        contextMenuHtmlString += splitLine;
                                    }
                                    contextMenuHtmlString +=
                                        " <div> <a class='disableMenuStyle " + contextMenus[i].MenuStyle + "' >" + contextMenus[i].MenuName.replace(new RegExp('%%%', "g"), "'") + "</a>  </div>";
                                } 
                            }
                        }else
                        {
                            if (contextMenus[i].IsDisabled === true) {
                                contextMenuHtmlString =
                                    " <div> <a class='disableMenuStyle " + contextMenus[i].MenuStyle + "' >" + contextMenus[i].MenuName.replace(new RegExp('%%%', "g"), "'") + "</a>  </div>";
                            } else {
                                contextMenuHtmlString =
                                    " <div> <a class='contextMenuStyle " + contextMenus[i].MenuStyle + "' >" + contextMenus[i].MenuName.replace(new RegExp('%%%', "g"), "'") + "</a>  </div>";
                            }
                            
                        }
//                        console.log(contextMenuHtmlString);
                        $("#tipContent").append(contextMenuHtmlString);
                    }
                    var $screenWidth = $(window).width();
                    var $screenHeigth = $(window).height();
                    var $tipContentWidth = $("#tipContent").width();
                    var $tipContentHeigth = $("#tipContent").height();

                    var siteTop = 50;
                   //Jan 26, 2024 zhangyuan 281_PR_MenuShowIncomplete: Modify Add menu top distance
                    var $tiptop = (e.clientY + 20) > ($screenHeigth - $tipContentHeigth)
                        ? (e.clientY - $tipContentHeigth) > siteTop ? e.clientY - $tipContentHeigth : siteTop
                        : e.clientY + 20;
                    var $tipleft = (e.clientX - 20) > ($screenWidth - $tipContentWidth)
                        ? (e.clientX - $tipContentWidth) > 0 ? e.clientX - $tipContentWidth : 0
                        : e.clientX - 20;
                    

                    $("#tipContent")
                        .css({
                            top: $tiptop,
                            left: $tipleft,
                            display: 'block'
                        });
                    $("#tipContent")
                        .on("mouseleave",
                        function () {
                            $(this).hide();
                        });
                    $(".showcellmenu").on("mouseover", function () {
                        $(this).parent().siblings().find(".showcellmenu").next().hide();
                        var $cellmenuWidth = $(this).next("ul").width();
                        var $cellmenuHeigth = $(this).next("ul").height();
                        var $tipContentTop = $(this).parent().offset().top;
                        var $tipContentLeft = $(this).parent().offset().left;
                        var $marginleft = $(this).parent().width();

                        var $celltop = ($screenHeigth - $tipContentTop) > $cellmenuHeigth ? 0 : ($cellmenuHeigth > $tipContentTop) ? (50 - $tipContentTop) : (36 - $cellmenuHeigth);
                        var $cellleft = ($screenWidth - $tipContentLeft - $marginleft) > $cellmenuWidth ? $marginleft : -$cellmenuWidth;
//                        console.log($tipContentTop);
//                        console.log($tipContentLeft);
//                        console.log($celltop);
//                        console.log($cellleft);
                        $(this).next("ul")
                            .css({
                                top: $celltop,
                                left: $cellleft,
                                display: 'block'
                            });
                        $(this).next("ul").show();
                    });
                    $(".showcellmenu +ul").on("mouseleave", function () {
                        $(this).hide();
                    });
                } 
            }
        });
    } else {
        $(args.cell).text(args.data[args.column.field]);
    }
}


eServiceOnline.Site.prototype.switchMenu = function () {
    var menuList = {
        upcomingJobs: ["headerUpcomingJobs", "UpcomingJobs"],
        productHaul: ["headerProductHaul", "ProductHaul"],
        rigBoard: ["headerRigBoard", "RigBoard"],
        bulkPlant: ["headerBulkPlant", "BulkPlant"],
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

eServiceOnline.Site.prototype.NoPopsUpWindow=function (item) {
    
    var itemurl = this.processContextMenuUrl;
//    alert(this.processContextMenuUrl);
    $.ajax({
        url: itemurl,
        method: "Post",
        data: item,
        success: function (result) {
            console.log((result === true));
            if (result === true) {
                window.location.reload();
            } else {
                alert("Operation fail.");
            }
        }
    });
}
eServiceOnline.Site.prototype.OpenInNewTab = function (item) {

    var itemurl = this.processContextMenuUrl;
    //    alert(this.processContextMenuUrl);
    $.ajax({
        url: itemurl,
        method: "Post",
        data: item,
        success: function (result) {
            var arrPathName = window.location.pathname.split('/');
            var pathName = "";
            if (arrPathName.length > 2) {
                pathName = "/" + arrPathName[1];
            }
            var pdfUrl = window.location.origin + pathName + result;
            window.open(pdfUrl, '_blank');
        }
    });
}

eServiceOnline.Site.prototype.PopsUpWindow=function (item) {
    console.log(item);
 
    var itemurl = this.processContextMenuUrl;

    var disableInputOrButton = function (event) {
            //console.log('on submit');
            $(this).find("input[type='submit']").attr("disabled", "disabled");
            $(event.target).attr("disabled", "disabled");
    }

//    alert(this.processContextMenuUrl+"aaa");
//    var dialogid = "#dialog_" + item.DialogName.toLowerCase();
    $.ajax({
        url: itemurl,
        method: "Post",
        data: item,
        success: function (data) {
            $("#tipContent").hide();
            $(".sj_mask").fadeIn(300,
                function () {
                    $("#basicDialog_title span").html(item.DialogName == null || item.DialogName === "" ? item.MenuName.replace(new RegExp('%%%', "g"), "'") : item.DialogName.replace(new RegExp('%%%', "g"), "'"));
                    $("#basicDialog").html(data);
                    $("#basicDialog").ejDialog("open");

                    var form = $("#basicDialog").find("form");
                    form.on("submit", disableInputOrButton);
                }
            );
//            $(dialogid).html("");
//            $(dialogid).append(data);
//            $(dialogid).show();
            
        }
    }); 
}

eServiceOnline.Site.prototype.CloseDialog=function(){
    $(".sj_mask").fadeOut(300,
        function () {
//            $("#" + elementId).hide();
            $("#basicDialog").ejDialog("close");
           // window.location.reload();
        }
    );
}

eServiceOnline.Site.prototype.RefeshPage = function () {
    setInterval(function () {
        var canRefresh = true;
        if ($("#tipContent").is(":hidden") == false) {
            canRefresh = false;
        }
        if (canRefresh) {
            $(".sj_mask").each(function () {
                if ($(this).is(":hidden") == false) {
                    canRefresh = false;
                }
            });
        }
        if (canRefresh) {
            $(".sj_confirm").each(function () {
                if ($(this).is(":hidden") == false) {
                    canRefresh = false;
                }
            });
        }
        console.log(canRefresh);
        if (canRefresh) {
            window.location.reload();
        }
    }, 1000 * 60 * 15);
}


