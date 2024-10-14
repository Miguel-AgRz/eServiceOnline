// Write your Javascript code.

var eServiceOnline = eServiceOnline  || {};

eServiceOnline.UpcomingJobs = function (listUrl) {
    this.listUrl = listUrl;
};

eServiceOnline.UpcomingJobs.Instance = new eServiceOnline.UpcomingJobs();


eServiceOnline.UpcomingJobs.prototype.MergeStyledCell = function (args) {
    $(document).delegate(args.cell, 'contextmenu', function (e) {
        e.preventDefault();
    });
    if (args.data[args.column.field] !== null) {
        $(args.cell).addClass(args.data[args.column.field].Style);
        $(args.cell).html(args.data[args.column.field].PropertyValue);
        $("table th div").removeClass("e-gridtooltip");
    } else {
        $(args.cell).text(args.data[args.column.field]);
    }
}



