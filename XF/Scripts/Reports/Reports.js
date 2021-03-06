﻿var XF = XF || {};

(function ($, XF) {

    XF.getDailyReport = function () {
        var url = '/Reports/daily';
        var data = {
            date: $('#txtStartDate').val(),
            hasStyles: false,
            userId: $('#ddlUser').val()
        };
        $.get(url, data, XF.getDailyReportResponse);
    };

    XF.getDailyReportResponse = function (data) {
        $('#reportViewer').html(data);
    };

    XF.getSalesRangeReport = function () {
        var url = '/Reports/SalesRange';
        var data = {
            startDate: $('#txtStartDate').val(),
            endDate: $('#txtEndDate').val(),
            hasStyles: false,
            userId: $('#ddlUser').val()
        };
        $.get(url, data, XF.getSalesRangeReportResponse);
    };

    XF.getSalesRangeReportResponse = function (data) {
        $('#reportViewer').html(data);
    };

    XF.getDeliveryReport = function () {
        var url = '/Reports/Delivery';
        var data = {
            startDate: $('#txtStartDate').val(),
            endDate: $('#txtEndDate').val(),
            hasStyles: false,
            userId: $('#ddlUser').val()
        };
        $.get(url, data, XF.getDeliveryReportResponse);
    };

    XF.getDeliveryReportResponse = function (data) {
        $('#reportViewer').html(data);
    };

    XF.getPickUpReport = function () {
        var url = '/Reports/PickUp';
        var data = {
            startDate: $('#txtStartDate').val(),
            endDate: $('#txtEndDate').val(),
            hasStyles: false,
            userId: $('#ddlUser').val()
        };
        $.get(url, data, XF.getPickUpReportResponse);
    };

    XF.getPickUpReportResponse = function (data) {
        $('#reportViewer').html(data);
    };


    XF.getSalesReport = function () {
        var url = '/Reports/Sales';
        var data = {
            startDate: $('#txtStartDate').val(),
            endDate: $('#txtEndDate').val(),
            hasStyles: false,
            userId: $('#ddlUser').val()
        };
        $.get(url, data, XF.getSalesReportsResponse);
    };

    XF.getSalesReportsResponse = function (data) {
        $('#reportViewer').html(data);
    };

    XF.getProfitReport = function () {
        var url = '/Reports/Profit';
        var data = {
            startDate: $('#txtStartDate').val(),
            endDate: $('#txtEndDate').val(),
            hasStyles: false,
            userId: $('#ddlUser').val()
        };
        $.get(url, data, XF.getProfitReportResponse);
    };

    XF.getProfitReportResponse = function (data) {
        $('#reportViewer').html(data);
    };

    XF.getComissionsReport = function () {
        var url = '/Reports/Comissions';
        var data = {
            startDate: $('#txtStartDate').val(),
            endDate: $('#txtEndDate').val(),
            hasStyles: false,
            userId: $('#ddlUser').val()
        };
        $.get(url, data, XF.getComissionsReportResponse);
    };

    XF.getComissionsReportResponse = function (data) {
        $('#reportViewer').html(data);
    };

    XF.getSuppliesReport = function () {
        var url = '/Reports/Supplies';
        var data = {
            startDate: $('#txtStartDate').val(),
            endDate: $('#txtEndDate').val(),
            hasStyles: false,
            userId: $('#ddlUser').val()
        };
        $.get(url, data, XF.getSuppliesReportResponse);
    };

    XF.getSuppliesReportResponse = function (data) {
        $('#reportViewer').html(data);
    };

    XF.getUtilitiesReport = function () {
        var url = '/Reports/Utilities';
        var data = {
            startDate: $('#txtStartDate').val(),
            endDate: $('#txtEndDate').val(),
            hasStyles: false,
            userId: $('#ddlUser').val()
        };
        $.get(url, data, XF.getUtilitiesReportResponse);
    };

    XF.getUtilitiesReportResponse = function (data) {
        $('#reportViewer').html(data);
    };

    $('#btnFilter').on('click', function () {
        $('#reportViewer').html('<span class="label label-warning">Loading...</div>');
        var reportType = $('#cmbReports').val();
        if (reportType == 1) {
            XF.getDailyReport();
        }
        else if (reportType == 2) {
            XF.getDeliveryReport();
        }
        else if (reportType == 3) {
            XF.getPickUpReport();
        }
        else if (reportType == 4) {
            XF.getSalesReport();
        }
        else if (reportType == 5) {
            XF.getProfitReport();
        }
        else if (reportType == 6) {
            XF.getSalesRangeReport();
        }
        else if (reportType == 7) {
            XF.getComissionsReport();
        }
        else if (reportType == 8) {
            XF.getSuppliesReport();
        }
        else if (reportType == 9) {
            XF.getUtilitiesReport();
        }
    });

    $('#cmbReports').on('change', function () {
        $('#lblEndDate').hide();
        if ($(this).val()>1) {
            $('#lblEndDate').show();
        }
    });

    $('#btnPrint').on('click', function () {
        var url = "";
        var reportType = $('#cmbReports').val();
        if (reportType == 1) {
            url = "/Reports/PrintDaily?date=" + $('#txtStartDate').val() + '&hasStyles=true&userId=' + $('#ddlUser').val();
        }
        else if (reportType == 2) {
            url = "/Reports/PrintDelivery?startDate=" + $('#txtStartDate').val() + "&endDate=" + $('#txtEndDate').val() + '&hasStyles=true&userId=' + $('#ddlUser').val();
        }
        else if (reportType == 3) {
            url = "/Reports/PrintPickUp?startDate=" + $('#txtStartDate').val() + "&endDate=" + $('#txtEndDate').val() + '&hasStyles=true&userId=' + $('#ddlUser').val();
        }
        else if (reportType == 4) {
            url = "/Reports/PrintSales?startDate=" + $('#txtStartDate').val() + "&endDate=" + $('#txtEndDate').val() + '&hasStyles=true&userId=' + $('#ddlUser').val();
        }
        else if (reportType == 5) {
            url = "/Reports/PrintProfit?startDate=" + $('#txtStartDate').val() + "&endDate=" + $('#txtEndDate').val() + '&hasStyles=true&userId=' + $('#ddlUser').val();
        }
        else if (reportType == 6) {
            url = "/Reports/PrintSalesRange?startDate=" + $('#txtStartDate').val() + "&endDate=" + $('#txtEndDate').val() + '&hasStyles=true&userId=' + $('#ddlUser').val();
        }
        else if (reportType == 7) {
            url = "/Reports/PrintComissions?startDate=" + $('#txtStartDate').val() + "&endDate=" + $('#txtEndDate').val() + '&hasStyles=true&userId=' + $('#ddlUser').val();
        }
        else if (reportType == 8) {
            url = "/Reports/PrintSupplies?startDate=" + $('#txtStartDate').val() + "&endDate=" + $('#txtEndDate').val() + '&hasStyles=true&userId=' + $('#ddlUser').val();
        }
        else if (reportType == 9) {
            url = "/Reports/PrintUtilities?startDate=" + $('#txtStartDate').val() + "&endDate=" + $('#txtEndDate').val() + '&hasStyles=true&userId=' + $('#ddlUser').val();
        }
        var win = window.open(url, '_blank');
        win.focus();
    });

    $('#lblEndDate').hide();
})(jQuery, XF);