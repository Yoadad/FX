var XF = XF || {};

(function ($, XF) {

    var baseUrl = document.URL;

    var _dataSource = new kendo.data.DataSource({
        transport: {
            read:
                {
                    url: baseUrl + "/Products",
                    dataType: "json"
                }
        },
        serverPagin: true,
        serverSorting: true,
        pageSize: $('#hfPageSize').val()
    });

    console.log(_dataSource);

    $("#grid").kendoGrid({
        dataSource: _dataSource,
        sortable: true,
        pageable: true,
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
                { field: "Id" },
                {
                    field: "Code"
                },
                { field: "Name" },
                { field: "SellPrice" },
                { field: "PurchasePrice" },
                { field: "Max" },
                { field: "Min" }
            ]
    });

    function titleFilter(element) {
        element.kendoAutoComplete({
            dataSource: titles
        });
    }

}(jQuery, XF));