var XF = XF || {};

(function ($, XF) {

    XF.getDailyReport = function () {
        var url = '/Reports/daily';
        var data = { date: $('#txtDate').val(), hasStyles :false};
        $.get(url, data, XF.getDailyReportResponse);
    };

    XF.getDailyReportResponse = function (data) {
        $('#reportViewer').html(data);
    };

    $('#btnFilter').on('click', function () {
        XF.getDailyReport();
    });

    $('#btnPrint').on('click', function () {
        var url = "/Reports/PrintDaily?date=" + $('#txtDate').val() + '&hasStyles=true'
        var win = window.open(url, '_blank');
        win.focus();
    });

})(jQuery, XF);