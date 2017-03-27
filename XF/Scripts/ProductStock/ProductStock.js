var XF = XF || {};

(function ($, XF) {

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


}(jQuery, XF));