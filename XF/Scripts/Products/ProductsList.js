var XF = XF || {};

(function ($, XF) {

    var baseUrl = document.URL;

    var _dataSource = new kendo.data.DataSource({
        transport: {
            read: function (option) {
                console.log(JSON.stringify(option.data.sort));
                $.ajax({
                    url: baseUrl + '/Products',
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
        sortable: true,
        pageable: {refresh:true, pageSizes: true},
        filterable: {
            extra: false,
            operators:
                {
                    string: {
                        startwitstartswith: "Starts with",
                        eq: "Is equal to",
                        neq: "Is not equal to"
                    }
                }
        },
        //selectable: "multiple cell",
        columns:
            [
                { field: "Id" , hidden: true },
                { field: "Code" },
                { field: "Name" },
                { field: "SellPrice", filterable: false },
                { field: "PurchasePrice", filterable: false },
                { field: "Max", filterable: false },
                { field: "Min", filterable: false }
            ]
    });

    function titleFilter(element) {
        element.kendoAutoComplete({
            dataSource: titles
        });
    }

}(jQuery, XF));