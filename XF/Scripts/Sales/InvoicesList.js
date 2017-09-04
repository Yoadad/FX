var XF = XF || {};

(function ($, XF) {

    var _dataSource = new kendo.data.DataSource({
        transport: {
            read: function (option) {
                $.ajax({
                    url: '/Invoices/Invoices',
                    dataType: 'json',
                    data:
                        {
                            skip: option.data.skip,
                            take: option.data.take,
                            pageSize: option.data.pageSize,
                            page: option.data.page,
                            sorting: JSON.stringify(option.data.sort),
                            filter: JSON.stringify(option.data.filter)
                        },
                    success: function (result) {
                        option.success(result);
                    },
                    error: function (result) {
                        console.log('Error on server call.');
                    }
                });
            }
        },
        serverFiltering: true,
        serverSorting: true,
        serverPaging: true,
        pageSize: $('#hfPageSize').val(),
        schema:
            {
                data: "data",
                total: "total",
                model: {
                    fields: {
                        Date: { type: "date" }
                    }
                }
            }

    });


    $("#grid").kendoGrid({
        dataSource: _dataSource,
        sortable: {
            mode: "single",
            allowUnsort: false
        },
        pageable: { refresh: true, pageSizes: true },
        dataBound: function () {
            $('.chk-released').on('click', function () {
                XF.releasedInvoice($(this).data('invoiceid'), $(this).is(':checked'));
            });
        },
        filterable: {
            extra: false,
            operators:
                 {
                     string: {
                         //contains: "Contains",
                         eq: "Is equal to",
                         neq: "Is not equal to"
                     }
                 }
        },
        //selectable: "multiple cell",
        columns:
            [
                { field: "Id", template: $('#invoiceDetailTemplate').html() },
                //{ field: "Date", template: '#=kendo.toString(Date, \"MM/dd/yyyy\")#', filterable:true },
                { field: "Date", format: "{0:MM/dd/yyyy}", filterable: true },
                { field: "ClientName", title: "Client Name", filterable: true, width: 155 },
                { field: "PaymentType", title: "Payment Type", filterable: false, width: 155 },
                { field: "Subtotal", filterable: false },
                { field: "Tax", filterable: false },
                { field: "Discount", filterable: false },
                { field: "Total", filterable: false },
                { field: "ClientEmail", filterable: false, hidden: true },
                { field: "Actions", template: $('#PrintInvoiceDetailTemplate').html(), filterable: false }
            ]
    });

    function titleFilter(element) {
        element.kendoAutoComplete({
            dataSource: titles
        });
    }

    XF.confirmEmail = function (invoiceId, email) {
        XF.prompt("Send Invoice to:", email, function (eml) {
            XF.sendInvoiceEmail(invoiceId, eml);
        });
    };

    XF.sendInvoiceEmail = function (invoiceId, email) {
        var url = '/Sales/EmailInvoice';
        var data = { id: invoiceId, email: email };
        $.post(url, data, XF.sendInvoiceEmailResponse, 'json');
    };

    XF.sendInvoiceEmailResponse = function (data) {
        if (data.Result) {
            XF.addInfoMessage(data.Message, 'success');
        }
        else {
            XF.addInfoMessage(data.Message, 'danger');
        }
    };

    XF.releasedInvoice = function (invoiceId, isReleased) {
        var url = '/Invoices/Release';
        var data = { id: invoiceId, isReleased: isReleased };
        $.post(url, data, XF.releasedInvoiceResponse, 'json');
    };

    XF.releasedInvoiceResponse = function (data) {
        if (data.Result) {
            XF.addInfoMessage(data.Message, 'success');
            $('#grid').data('kendoGrid').dataSource.read();
        }
        else {
            XF.addInfoMessage(data.Message, 'danger');
        }
    };

    XF.refundInvoice = function (invoiceId,amount) {
        XF.prompt("This action will refund this invoice, please enter the amount:", amount, function () {
            var url = '/Invoices/Refund';
            var data = { id: invoiceId, amount: $('#txtPromptValue').val() };
            $.post(url, data, XF.refundInvoiceResponse, 'json');
        });
    };

    XF.refundInvoiceResponse = function (data) {
        if (data.Result) {
            XF.addInfoMessage(data.Message, 'success');
            $('#grid').data('kendoGrid').dataSource.read();
        }
        else {
            XF.addInfoMessage(data.Message, 'danger');
        }
    };


}(jQuery, XF));