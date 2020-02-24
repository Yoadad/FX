var XF = XF || {};

(function ($, XF) {

    XF.getSellerComission = function () {
        var url = '/ReportsBeta/SellerComission';
        var data = {
            startDate: $('#txtStartDate').val(),
            endDate: $('#txtEndDate').val(),
            sellerId: $('#ddlUser').val()
        };
        $.getJSON(url, data, XF.getSellerComissionResponse);
    };
    XF.getSellerComissionResponse = function (data) {
        if (data.Response) {
            var sections = XF.getSectionsByReport(10, data.Data);//10 == seller comission
            XF.setData(data.Data, sections);
        }
        else {
            XF.alert(data.Message, 'danger', 'danger');
        }
    };
    XF.getSalesByDate = function () {
        var url = '/ReportsBeta/SalesRange';
        var data = {
            startDate: $('#txtStartDate').val(),
            endDate: $('#txtEndDate').val(),
            sellerId: $('#ddlUser').val()
        };
        $.getJSON(url, data, XF.getSalesByDateResponse);
    };
    XF.getSalesByDateResponse = function (data) {
        if (data.Response) {
            var sections = XF.getSectionsByReport(6, data.Data);//6 == sales by date
            XF.setData(data.Data, sections);
        }
        else {
            XF.alert(data.Message, 'danger', 'danger');
        }
    };

    XF.getSales = function () {
        var url = '/ReportsBeta/Sales';
        var data = {
            startDate: $('#txtStartDate').val(),
            endDate: $('#txtEndDate').val(),
            sellerId: $('#ddlUser').val()
        };
        $.getJSON(url, data, XF.getSalesResponse);
    };

    XF.getSalesResponse = function (data) {
        if (data.Response) {
            var sections = XF.getSectionsByReport(4, data.Data);//4 == sales
            XF.setData(data.Data, sections);
        }
        else {
            XF.alert(data.Message, 'danger', 'danger');
        }
    };

    XF.getProfit = function () {
        var url = '/ReportsBeta/Profit';
        var data = {
            startDate: $('#txtStartDate').val(),
            endDate: $('#txtEndDate').val(),
            sellerId: $('#ddlUser').val()
        };
        $.getJSON(url, data, XF.getProfitResponse);
    };

    XF.getProfitResponse = function (data) {
        if (data.Response) {
            var sections = XF.getSectionsByReport(5, data.Data);//5 == profit
            XF.setData(data.Data, sections);
        }
        else {
            XF.alert(data.Message, 'danger', 'danger');
        }
    };

    XF.getComission = function () {
        var url = '/ReportsBeta/Comission';
        var data = {
            startDate: $('#txtStartDate').val(),
            endDate: $('#txtEndDate').val(),
            sellerId: $('#ddlUser').val()
        };
        $.getJSON(url, data, XF.getComissionResponse);
    };

    XF.getComissionResponse = function (data) {
        if (data.Response) {
            var sections = XF.getSectionsByReport(7, data.Data);//7 == comission
            XF.setData(data.Data, sections);
        }
        else {
            XF.alert(data.Message, 'danger', 'danger');
        }
    };

    XF.getDeliveryByDate = function () {
        var url = '/ReportsBeta/Delivery';
        var data = {
            startDate: $('#txtStartDate').val(),
            endDate: $('#txtEndDate').val()
        };
        $.getJSON(url, data, XF.getDeliveryByDateResponse);
    };
    XF.getDeliveryByDateResponse = function (data) {
        if (data.Response) {
            var sections = XF.getSectionsByReport(2, data.Data);//2 == delivery
            XF.setData(data.Data, sections);
        }
        else {
            XF.alert(data.Message, 'danger', 'danger');
        }
    };

    XF.getPickUpByDate = function () {
        var url = '/ReportsBeta/PickUp';
        var data = {
            startDate: $('#txtStartDate').val(),
            endDate: $('#txtEndDate').val()
        };
        $.getJSON(url, data, XF.getPickUpByDateResponse);
    };
    XF.getPickUpByDateResponse = function (data) {
        if (data.Response) {
            var sections = XF.getSectionsByReport(3, data.Data);//3 == pick up
            XF.setData(data.Data, sections);
        }
        else {
            XF.alert(data.Message, 'danger', 'danger');
        }
    };

    XF.getDaily = function () {
        var url = '/ReportsBeta/Daily';
        var data = {
            date: $('#txtStartDate').val(),
            sellerId: $('#ddlUser').val()
        };
        $.getJSON(url, data, XF.getDailyResponse);
    };
    XF.getDailyResponse = function (data) {
        if (data.Response) {
            var sections = XF.getSectionsByReport(1, data.Data);//1 == Daily
            XF.setData(data.Data, sections);
        }
        else {
            XF.alert(data.Message, 'danger', 'danger');
        }
    };

    /////////////////////////////
    XF.getSectionsByReport = function (reportId, data) {
        var sections;
        if (reportId == 1 ) {
            sections = XF.getSectionsDaily(data);
        }
        else if (reportId == 2 || reportId == 3) {
            sections = XF.getSectionsDeliveryOrPickUp(data);
        }
        else if (reportId == 4) {
            sections = XF.getSectionsSales(data);
        }
        else if (reportId == 5) {
            sections = XF.getSectionsProfit(data);
        }
        else if (reportId == 6) {
            sections = XF.getSectionsSalesByDate(data);
        }
        else if (reportId == 7) {
            sections = XF.getSectionsComission(data);
        }
        else if (reportId == 10) {
            sections = XF.getSectionsSellerComission(data);
        }
        return sections;
    };

    XF.setData = function (data, sections) {
        $('.report').reporting({
            data: data,
            sections: sections,
            orientation: $('#cmbReports').val() == 4 ? 'landscape':'portrait',
            urlProxy: "/ReportsBeta/PdfToPrintInTab",
            height: 550
        });
    };

    XF.getSectionsSellerComission = function (data) {
        var sections = [
            {
                contents: [{ template: '<h3 class="text-center">Sales Comission <br /><br /> <small> #=data.StartDate# &nbsp;to&nbsp;#=data.EndDate# </small>  </h3>' }]
                , detail: {
                    columnNumber: 1
                    , data: data.Detail
                    , footer: [{ name: 'Comission', template: $('#template-footer-template').html() }, { name: 'SellPrice', template: $('#template-footer-template').html() }]
                    , fields: [
                        {
                            name: 'Seller',
                            titleTemplate: '<th class="text-left">Seller</th>',
                            template: '<td class="text-left"><strong>#=data.Seller#</strong></td>'
                        }
                        , {
                            name: 'Client',
                            titleTemplate: '<th class="text-left">Client Name</th>',
                            template: '<td class="text-left"><strong>#=data.Client#</strong></td>'
                        }
                        , {
                            name: 'InvoiceId',
                            titleTemplate: '<th class="text-center">Invoice No.</th>',
                            template: '<td class="text-center"><strong>#=data.InvoiceId#</strong></td>'
                        }
                        , {
                            name: 'Date',
                            titleTemplate: '<th class="text-center">Date</th>',
                            template: '<td class="text-center">#=data.InvoiceDate#</td>'
                        }
                        , {
                            name: 'SellPrice',
                            titleTemplate: '<th class="text-right">Sell Price</th>',
                            template: '<td class="text-right">#=kendo.toString(data.SellPrice,"c")#</td>'
                        }
                        , {
                            name: 'Comission',
                            titleTemplate: '<th class="text-right">Comission</th>',
                            template: '<td class="text-right">#=kendo.toString(data.Comission,"c")#</td>'
                        }
                    ]
                }
            }
        ];
        return sections;
    };

    XF.getSectionsSalesByDate = function (data) {
        var sections = [
            {
                contents: [{ template: '<h3 class="text-center">Sales by Date Balance <br /><br /> <small> #=data.StartDate# &nbsp;to&nbsp;#=data.EndDate# </small>  </h3>' }]
                , detail: {
                    columnNumber: 1
                    , data: data.Detail
                    , footer: [{ name: 'Cash', template: $('#template-footer-template').html() },
                                { name: 'CC', template: $('#template-footer-template').html() },
                                { name: 'Debit', template: $('#template-footer-template').html() },
                                { name: 'Check', template: $('#template-footer-template').html() },
                                { name: 'Finance', template: $('#template-footer-template').html() },
                                { name: 'NewLayaway', template: $('#template-footer-template').html() },
                                { name: 'Subtototal', template: $('#template-footer-template').html() },
                                { name: 'Total', template: $('#template-footer-template').html() },
                                { name: 'TaxDue', template: $('#template-footer-template').html() },
                    ]
                    , fields: [
                        {
                            name: 'Seller',
                            titleTemplate: '<th class="text-center">Seller</th>',
                            template: '<td class="text-left"><strong>#=data.Seller#</strong></td>'
                        }
                        , {
                            name: 'InvoiceId',
                            titleTemplate: '<th class="text-center">Inv ID</th>',
                            template: '<td class="text-center"><strong>#=data.InvoiceId#</strong></td>'
                        }
                        , {
                            name: 'Date',
                            titleTemplate: '<th class="text-center">Date</th>',
                            template: '<td class="text-center"><strong>#=data.Date#</strong></td>'
                        }
                        , {
                            name: 'Customer',
                            titleTemplate: '<th class="text-center">Customer</th>',
                            template: '<td class="text-left">#=data.Customer#</td>'
                        }
                        , {
                            name: 'Cash',
                            titleTemplate: '<th class="text-center">Cash</th>',
                            template: '<td class="text-right">#=kendo.toString(data.Cash,"c")#</td>'
                        }
                        , {
                            name: 'CC',
                            titleTemplate: '<th class="text-center">CC</th>',
                            template: '<td class="text-right">#=kendo.toString(data.CC,"c")#</td>'
                        }
                        , {
                            name: 'Debit',
                            titleTemplate: '<th class="text-center">Debit</th>',
                            template: '<td class="text-right">#=kendo.toString(data.Debit,"c")#</td>'
                        }
                        , {
                            name: 'Check',
                            titleTemplate: '<th class="text-center">Check</th>',
                            template: '<td class="text-right">#=kendo.toString(data.Check,"c")#</td>'
                        }
                        , {
                            name: 'Finance',
                            titleTemplate: '<th class="text-center">Finance</th>',
                            template: '<td class="text-right">#=kendo.toString(data.Finance,"c")#</td>'
                        }
                        , {
                            name: 'NewLayaway',
                            titleTemplate: '<th class="text-center">New<br/>Layaway</th>',
                            template: '<td class="text-right">#=kendo.toString(data.NewLayaway,"c")#</td>'
                        }
                        , {
                            name: 'Subtototal',
                            titleTemplate: '<th class="text-center">Subtototal</th>',
                            template: '<td class="text-right">#=kendo.toString(data.Subtototal,"c")#</td>'
                        }
                        , {
                            name: 'Total',
                            titleTemplate: '<th class="text-center">Total</th>',
                            template: '<td class="text-right">#=kendo.toString(data.Total,"c")#</td>'
                        }
                        , {
                            name: 'TaxDue',
                            titleTemplate: '<th class="text-center">Tax Due</th>',
                            template: '<td class="text-right">#=kendo.toString(data.TaxDue,"c")#</td>'
                        }
                    ]
                }
            }
        ];
        return sections;
    };

    XF.getSectionsSales = function (data) {
        var sections = [
            {
                contents: [
                    { template: '<h3 class="text-center">Sales Report<br /><br /> <small> #=data.StartDate# &nbsp;to&nbsp;#=data.EndDate# </small>  </h3>' }
                ]
            },
            {
                contents: [
                    { template: $('#page-sales-template').html() }
                ]
            }
        ];
        return sections;
    };

    XF.getSectionsDaily = function (data) {
        var sections = [
            {
                contents: [{ template: '<h2 class="text-center">Daily Balance <br /><small>#=data.Seller#</small><br /><small class="text-right">#=data.Date#</small><br /></h2>' }]
                , detail: {
                    columnNumber: 1
                    , data: data.Detail
                    , footer: [{ name: 'Cash', template: $('#template-footer-template').html() },
                                { name: 'CC', template: $('#template-footer-template').html() },
                                { name: 'Debit', template: $('#template-footer-template').html() },
                                { name: 'Check', template: $('#template-footer-template').html() },
                                { name: 'Finance', template: $('#template-footer-template').html() },
                                { name: 'NewLayaway', template: $('#template-footer-template').html() },
                                { name: 'Subtototal', template: $('#template-footer-template').html() },
                                { name: 'Total', template: $('#template-footer-template').html() },
                                { name: 'TaxDue', template: $('#template-footer-template').html() },
                    ]
                    , fields: [
                        {
                            name: 'Seller',
                            titleTemplate: '<th class="text-left" style="width:233px;">Seller</th>',
                            template: '<td class="text-left"><strong>#=data.Seller#</strong></td>'
                        }
                        , {
                            name: 'InvoiceId',
                            titleTemplate: '<th class="text-center">Inv ID</th>',
                            template: '<td class="text-center"><strong>#=data.InvoiceId#</strong></td>'
                        }
                        , {
                            name: 'Customer',
                            titleTemplate: '<th class="text-left">Customer</th>',
                            template: '<td class="text-left"><strong>#=data.Customer#</strong></td>'
                        }
                        , {
                            name: 'Cash',
                            titleTemplate: '<th class="text-center">Cash</th>',
                            template: '<td class="text-right"><strong>#=kendo.toString(data.Cash,\'c\')#</strong></td>'
                        }
                        , {
                            name: 'CC',
                            titleTemplate: '<th class="text-center">CC</th>',
                            template: '<td class="text-right"><strong>#=kendo.toString(data.CC,\'c\')#</strong></td>'
                        }
                        , {
                            name: 'Debit',
                            titleTemplate: '<th class="text-center">Debit</th>',
                            template: '<td class="text-right"><strong>#=kendo.toString(data.Debit,\'c\')#</strong></td>'
                        }
                        , {
                            name: 'Check',
                            titleTemplate: '<th class="text-center">Check</th>',
                            template: '<td class="text-right"><strong>#=kendo.toString(data.Check,\'c\')#</strong></td>'
                        }
                        , {
                            name: 'Finance',
                            titleTemplate: '<th class="text-center">Finance</th>',
                            template: '<td class="text-right"><strong>#=kendo.toString(data.Finance,\'c\')#</strong></td>'
                        }
                        , {
                            name: 'NewLayaway',
                            titleTemplate: '<th class="text-center">New<br/>Layaway</th>',
                            template: '<td class="text-right"><strong>#=kendo.toString(data.NewLayaway,\'c\')#</strong></td>'
                        }
                        , {
                            name: 'Total',
                            titleTemplate: '<th class="text-center">Total</th>',
                            template: '<td class="text-right"><strong>#=kendo.toString(data.Total,\'c\')#</strong></td>'
                        }
                        , {
                            name: 'TaxDue',
                            titleTemplate: '<th class="text-center">Tax Due</th>',
                            template: '<td class="text-right"><strong>#=kendo.toString(data.TaxDue,\'c\')#</strong></td>'
                        }
                    ]
                }
            }
        ];
        return sections;
    };

    XF.getSectionsDeliveryOrPickUp = function (data) {
        var sections = [
            {
                contents: [{ template: '<h2 class="text-center">Delivery Items<br /><small>#=data.StartDate# &nbsp;to&nbsp;#=data.EndDate#</small><br/></h2>' }]
                , detail: {
                    columnNumber: 1
                    , data: data.Detail
                    , fields: [
                        {
                            name: 'ClientName',
                            titleTemplate: '<th class="text-center" style="width:233px;">Client\'s Name</th>',
                            template: '<td class="text-left"><strong>#=data.ClientName#</strong></td>'
                        }
                        , {
                            name: 'InvoiceId',
                            titleTemplate: '<th class="text-center">Invoice ID</th>',
                            template: '<td class="text-center"><strong>#=data.InvoiceId#</strong></td>'
                        }
                        , {
                            name: 'Date',
                            titleTemplate: '<th class="text-center">Date Of Delivery</th>',
                            template: '<td class="text-center"><strong>#=data.Date#</strong></td>'
                        }
                    ]
                }
            }
        ];
        return sections;
    };

    XF.getSectionsProfit = function (data) {
        var sections = [
            {
                contents: [{ template: '<h2 class="text-center">    Profit Report    <br />    <small>#=data.StartDate# &nbsp;to&nbsp;#=data.EndDate#</small>    <br /></h2>' }]
                , detail: {
                    columnNumber: 1
                    , data: data.Detail
                    , footer: [ { name: 'PurchasePrice', template: $('#template-footer-template').html() },
                                { name: 'SellPrice', template: $('#template-footer-template').html() },
                                { name: 'Profit', template: $('#template-footer-template').html() }
                    ]
                    , fields: [
                        {
                            name: 'Date',
                            titleTemplate: '<th class="text-center">Date</th>',
                            template: '<td class="text-left"><strong>#=data.Date#</strong></td>'
                        }
                        , {
                            name: 'InvoiceId',
                            titleTemplate: '<th class="text-center">Invoice ID</th>',
                            template: '<td class="text-center"><strong>#=data.InvoiceId#</strong></td>'
                        }
                        , {
                            name: 'PurchasePrice',
                            titleTemplate: '<th class="text-center">Purchase Price</th>',
                            template: '<td class="text-right"><strong>#=kendo.toString(data.PurchasePrice,\'c\')#</strong></td>'
                        }
                        , {
                            name: 'SellPrice',
                            titleTemplate: '<th class="text-center">Sell Price</th>',
                            template: '<td class="text-right"><strong>#=kendo.toString(data.SellPrice,\'c\')#</strong></td>'
                        }
                        , {
                            name: 'Profit',
                            titleTemplate: '<th class="text-center">Profit</th>',
                            template: '<td class="text-right"><strong>#=kendo.toString(data.Profit,\'c\')#</strong></td>'
                        }

                    ]
                }
            }
        ];
        return sections;
    };

    XF.getSectionsComission = function (data) {
        var sections = [
            {
                contents: [{ template: '<h2 class="text-center">    Comissions Report    <br />  <br />    <small>#=data.StartDate# &nbsp;to&nbsp;#=data.EndDate#</small>    <br /></h2>' }]
                , detail: {
                    columnNumber: 1
                    , data: data.Detail
                    , footer: [ { name: 'PurchasePrice', template: $('#template-footer-template').html() },
                                { name: 'SellPrice', template: $('#template-footer-template').html() },
                                { name: 'Profit', template: $('#template-footer-template').html() },
                                { name: 'Comission', template: $('#template-footer-template').html() }
                    ]
                    , fields: [
                        {
                            name: 'Seller',
                            titleTemplate: '<th class="text-left" style="width:233px;">Seller</th>',
                            template: '<td class="text-left"><strong>#=data.Seller#</strong></td>'
                        }
                        , {
                            name: 'InvoiceId',
                            titleTemplate: '<th class="text-center">Invoice Id</th>',
                            template: '<td class="text-center"><strong>#=data.InvoiceId#</strong></td>'
                        }
                        , {
                            name: 'Date',
                            titleTemplate: '<th class="text-center">Date</th>',
                            template: '<td class="text-center"><strong>#=data.Date#</strong></td>'
                        }
                        , {
                            name: 'PurchasePrice',
                            titleTemplate: '<th class="text-center">Purchase Price</th>',
                            template: '<td class="text-right"><strong>#=kendo.toString(data.PurchasePrice,\'c\')#</strong></td>'
                        }
                        , {
                            name: 'SellPrice',
                            titleTemplate: '<th class="text-center">Sell Price</th>',
                            template: '<td class="text-right"><strong>#=kendo.toString(data.SellPrice,\'c\')#</strong></td>'
                        }
                        , {
                            name: 'Profit',
                            titleTemplate: '<th class="text-center">Profit</th>',
                            template: '<td class="text-right"><strong>#=kendo.toString(data.Profit,\'c\')#</strong></td>'
                        }
                        , {
                            name: 'Comission',
                            titleTemplate: '<th class="text-center">Comission</th>',
                            template: '<td class="text-right"><strong>#=kendo.toString(data.Comission,\'c\')#</strong></td>'
                        }
                    ]
                }
            }
        ];
        return sections;
    };

    $('#btnPrint').on('click', function () {
        $('.report').reporting('open-pdf');
    });

    $('#btnFilter').on('click', function () {
        $('.report').html('<h5>Loading...</h5>');
        var selectedReport = $('#cmbReports').val();
        if (selectedReport == 1) {
            XF.getDaily();
        }
        else if (selectedReport == 2) {
            XF.getDeliveryByDate();
        }
        else if (selectedReport == 3) {
            XF.getPickUpByDate();
        }
        else if (selectedReport == 4) {
            XF.getSales();
        }
        else if (selectedReport == 5) {
            XF.getProfit();
        }
        else if (selectedReport == 6) {
            XF.getSalesByDate();
        }
        else if (selectedReport == 7) {
            XF.getComission();
        }
        else if (selectedReport == 10) {
            XF.getSellerComission();
        }
    });

    $('#cmbReports').on('change', function () {
        $('#lblEndDate').hide();
        if ($(this).val() > 1) {
            $('#lblEndDate').show();
        }
    });

    $('#lblEndDate').hide();

})(jQuery, XF);
