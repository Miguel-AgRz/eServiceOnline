// Write your Javascript code.
var eServiceOnline = eServiceOnline || {};

eServiceOnline.Site.prototype.Created = function() {
    this.element.addEventListener("mousedown", function (e) {
        $("#tipContent").hide();
        if (e.button === 2 && e.target.classList.contains("e-rowcell")) {
            var id = e.currentTarget.id;
            var grid = document.getElementById(id).ej2_instances[0];
            var field = grid.columns[parseInt(e.target.getAttribute("aria-colindex"))].field;
            var rowObj = grid.getRowObjectFromUID(ej.base.closest(e.target, '.e-row').getAttribute('data-uid'));
            e.preventDefault();
            grid.selectCell({ rowIndex: rowObj.index, cellIndex: parseInt(e.target.getAttribute("aria-colindex")) },
                true);
            var contextMenus = rowObj.data[field].ContextMenus;
            eServiceOnline.Site.prototype.InitContext(contextMenus,e);
        }
    });
}

eServiceOnline.Site.prototype.QueryCellInfo = function (args) {
    if (args.data[args.column.field] !== null) {
        $(args.cell).addClass(args.data[args.column.field].Style);
        $(args.cell).text(args.data[args.column.field].PropertyValue);
        $("table th div").removeClass("e-gridtooltip");
        $(args.cell).css({ "padding": "0.3em" });
        $(document).delegate(args.cell,'contextmenu',function (e) {
                e.preventDefault();
        });
        $("#matchDialog").hide();
    } else {
        $(args.cell).text(args.data[args.column.field]);
    }
    if (args.column.field == "WorkerName") {
        $(args.cell).attr("title", args.data.Notes.PropertyValue);
    }
    
}

