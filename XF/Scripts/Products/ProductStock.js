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
            XF.addInfoMessage('Stock updated', 'success');
            //$('#cmbLocation option:selected').data('stock',$('#txtStock').val())
        }
        else {
            XF.addInfoMessage(data.Message,'danger');
        }
    };

    $('#btnUpdate').on('click', function () {
        XF.confirm('This action will update the current product stock, are you sure that you want to continue?',
        function () {
            $('.stock').each(function () {
                var locationId = $(this).find('.location-stock').data('locationid');
                var productId = $('#hfProductId').val();
                var stock = $(this).find('.location-stock').val();
                console.log(locationId);
                console.log(stock);
                XF.SetStock(locationId, productId, stock);
            });
        });
    });

    //$('#cmbLocation').on('change', function () {
    //    $('#txtStock').val($(this).find('option:selected').data('stock'));
    //});

}(jQuery, XF));