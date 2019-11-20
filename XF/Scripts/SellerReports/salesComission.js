var XF = XF || {};

(function ($, XF) {

    XF.getData = function () {
        var url = '/SellerReports/SellerComission';
        var data = {
            startDate: $('#txtStartDate').val(),
            endDate: $('#txtEndDate').val()
        };
        $.getJSON(url, data, XF.getDataResponse);
    };

    XF.getDataResponse = function (data) {
        if (data.Response) {
            XF.setData(data.Data);
        }
        else {
            XF.alert(data.Message, 'danger', 'danger');
        }
    };
    XF.setData = function (data) {
        var sections = XF.getSections(data);
        $('.report').reporting({
            data: data,
            sections: sections,
            orientation: 'portrait',
            urlProxy: "/SellerReports/PdfInTab",
            height: 550
        });
    };

    XF.getSections = function (data) {
        data.Detail.push({
            SellPrice: data.TotalSellPrice,
            Comission: data.TotalComission,
            InvoiceDate: '',
            Client: '',
            InvoiceId:''
        });
        var sections = [
            {
                contents: [{ template: '<h2 class="text-center">Sales Comission <br /><br /> <small> #=data.StartDate# &nbsp;to&nbsp;#=data.EndDate# </small>  </h2>' }]
                , detail: {
                    columnNumber: 1
                    , data: data.Detail
                    , fields: [
                        {
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

    $('#btnPrint').on('click', function () {
        $('.report').reporting('open-pdf');
    });

    $('#btnFilter').on('click', function () {
        XF.getData();
    });
})(jQuery, XF);
