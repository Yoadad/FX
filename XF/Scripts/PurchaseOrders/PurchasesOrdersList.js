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
                //{ field: "Date", template: '#=kendo.toString(Date, \"MM/dd/yyyy\")#', filterable:true },
                { field: "Date", format: "{0:MM/dd/yyyy}", filterable: false },
                { field: "Provider", filterable: false },
                { field: "Subtotal", filterable: false },
                { field: "Tax", filterable: false },
                { field: "Discount", filterable: false },
                { field: "Total", filterable: false },
            ]
    });

    function titleFilter(element) {
        element.kendoAutoComplete({
            dataSource: titles
        });
    }
})(jQuery, XF);