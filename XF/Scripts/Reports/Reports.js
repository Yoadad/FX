var XF = XF || {};

(function ($, XF) {

    XF.getDailyReport = function () {
        var url = '/Reports/daily';
        var data = { date: $('#txtStartDate').val(), hasStyles: false
    };
        $.get(url, data, XF.getDailyReportResponse);
    };

    XF.getDailyReportResponse = function (data) {
        $('#reportViewer').html(data);
    };

    XF.getWeeklyReport = function () {
        var url = '/Reports/Weekly';
        var data = { startDate: $('#txtStartDate').val(), endDate: $('#txtEndDate').val(), hasStyles: false };
        $.get(url, data, XF.getWeeklyReportResponse);
    };

    XF.getWeeklyReportResponse = function (data) {
        $('#reportViewer').html(data);
    };

    XF.getSalesReport = function () {
        var url = '/Reports/Sales';
        var data = { startDate: $('#txtStartDate').val(), endDate: $('#txtEndDate').val(), hasStyles: false };
        $.get(url, data, XF.getSalesReportsResponse);
    };

    XF.getSalesReportsResponse = function (data) {
        $('#reportViewer').html(data);
    };


    $('#btnFilter').on('click', function () {
        $('#reportViewer').html('<span class="label label-warning">Loading...</div>');
        var reportType = $('#cmbReports').val();
        if (reportType == 1) {
            XF.getDailyReport();
        }
        else if (reportType == 2) {
            XF.getWeeklyReport();
        }
        else if (reportType == 6) {
            XF.getSalesReport();
        }
    });

    $('#cmbReports').on('change', function () {
        $('#lblEndDate').hide();
        if ($(this).val()>2) {
            $('#lblEndDate').show();
        }
    });

    $('#btnPrint').on('click', function () {
        var url = "/Reports/PrintDaily?date=" + $('#txtDate').val() + '&hasStyles=true'
        var win = window.open(url, '_blank');
        win.focus();
    });

    $('#lblEndDate').hide();
})(jQuery, XF);