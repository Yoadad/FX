var XF = XF || {};

(function ($, XF) {

    var baseUrl = document.URL;

    XF.SetStock = function (locationId, productId, stock) {
        var url = '/Products/SetStock';
        var data = XF.logData = { locationId: locationId, productId: productId, stock: stock };
        $.post(url,data, XF.SetStockResponse,'json');
    };

    XF.SetStockResponse = function (data) {
        if (data.Result) {
            XF.addInfoMessage('Stock updated','success');
        }
        else {
            XF.addInfoMessage(data.Message,'danger');
        }
    };

    $('#btnUpdate').on('click', function () {
        XF.confirm('This action will update the current product stock, are you sure that you want to continue?',
        function () {
            var locationId = $('#hfLocationId').val();
            var productId = $('#hfProductId').val();
            var stock = $('#txtStock').val();
            XF.SetStock(locationId,productId, stock);
        });
    });

}(jQuery, XF));