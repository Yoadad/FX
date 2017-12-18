var XF = XF || {};

(function ($, XF) {

    var _dataSource = new kendo.data.DataSource({
        transport: {
            read: function (option) {
                $.ajax({
                    url: '/Products/InventoryProducts',
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
                total: "total"
            }

    });

    
    $("#grid").kendoGrid({
        dataSource: _dataSource,
        sortable:  {
            mode: "single",
            allowUnsort: false
           },
        pageable: {refresh:true, pageSizes: true},
        filterable: {
            extra:false,
           operators:
                {
                    string: {
                        contains: "Contains",
                        eq: "Is equal to",
                        neq: "Is not equal to"
                    }
                }
        },
        //selectable: "multiple cell",
        columns:
            [
                { field: "Id", hidden: true },
                { field: "Code",template:$("#codeTemplate").html() },
                { field: "Name" },
                { field: "ProviderName",title:'Provider Name', hidden: false },
                { field: "SellPrice",title:"Sell Price", filterable: false },
                { field: "PurchasePrice",title:"Puchase Price", filterable: false },
                { field: "Max", filterable: false },
                { field: "Min", filterable: false },
                { field: "Stock", filterable: false, template: $('#stockTemplate').html() },
            ]
    });

    function titleFilter(element) {
        element.kendoAutoComplete({
            dataSource: titles
        });
    }

}(jQuery, XF));