var XF = XF || {};
(function ($, XF) {
    var baseUrl = document.URL;
    var _dataSource = new kendo.data.DataSource({
        transport: {
            read: function (option) {
                $.ajax({
                    url: '/Purchases/Orders',
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
                        , Created: { type: "date" }
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
                { field: "Id", template: $('#orderDetailTemplate').html() },
                { field: "Created", format: "{0:MM/dd/yyyy}", filterable: true },
                { field: "Date", format: "{0:MM/dd/yyyy}", filterable: true },
                { field: "ProviderName", filterable: false },
                { field: "Total", filterable: false },
                { command:{ text: "Received", click: receivedAction }, title: " "}
            ]
    });

    XF.ChangeStatus = function (data) {
        var url = 'ChangeStatus'
        var data = { data: JSON.stringify(data) };
       
        $.post(url, data, XF.SaveChangeStatusResponse, 'json');
    };

    XF.SaveChangeStatusResponse = function (data) {
        if (data.Result) {
            XF.addInfoMessage(data.Message, 'success');
        }
        else {
            XF.addInfoMessage(data.Message, 'danger');
        }
    };

    XF.getPurchaseOrder = function (dataItem) {
        
        var result = {
            Id: dataItem.PurchaseOrderId
        };

        return result;
    };
    XF.removeRow = function (dataItem)
    {
        var dataSource = $("#grid").data("kendoGrid").dataSource;
        dataSource.remove(dataItem);
        dataSource.sync();
    };

    function receivedAction(e)
    {
        e.preventDefault();

        var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
  
        XF.confirm('This action will update this order as Received', function () {
            var data = { PurchaseOrder: XF.getPurchaseOrder(dataItem) };
            XF.ChangeStatus(data);
                  XF.removeRow(dataItem);
        });
       
    }

    function titleFilter(element) {
        element.kendoAutoComplete({
            dataSource: titles
        });
    }
})(jQuery, XF);