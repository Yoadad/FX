var XF = XF || {};

(function ($, XF) {

    var baseUrl = document.URL;

    XF.SetStock = function (productId,stock) {
        var url = '/Products/SetStock';
        var data = { productId: productId, stock: stock };
        $.post(url,data, XF.SetStockResponse,'json');
    };

    XF.SetStockResponse = function (data) {
        if (data.Result) {
            XF.addInfoMessage('Stock updated');
        }
        else {
            XF.addInfoMessage(data.Message,'danger');
        }
    };

    $('#btnUpdate').on('click', function () {
        if (confirm('This action is going to update the current product stock, are you sure that you want to continue?')) {
            var productId = $('#hfProductId').val();
            var stock = $('#txtStock').val();
            console.log(productId + ','+ stock);
            
            XF.SetStock(productId, stock);
        }
    });

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
        filterable:{
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
                { field: "Id"},
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