var XF = XF || {};
(function () {


    XF.getDetails = function () {
        var details = [];
        $('.item').each(function () {
            details.push({
                Id: $(this).data('detailid'),
                ProductId: $(this).find('.item-product').val(),
                UnitPrice: $(this).find('.item-price').data('value'),
                Quantity: $(this).find('.item-quantity').val(),
                PurchaseOrderId: $('#hfOrderId').val()
            });
        });
        return details;
    };

    XF.saveOrderDetail = function () {
        var url = '/Purchases/UpdateDetail';
        var data = { data: JSON.stringify(XF.getDetails()) };
        $.post(url, data, XF.saveOrderDetailResponse,'json');
    };

    XF.saveOrderDetailResponse = function(data) {
        if (data.Result) {
            XF.addInfoMessage(data.Message,'success');
        }
        else {
            XF.addInfoMessage(data.Message,'danger');
        }
    };


    XF.setSubtotalItem = function ($item) {
        var price = parseFloat($item.find('.item-product option:selected').data('price'));
        var quantity = parseInt($item.find('.item-quantity').val())
        var subtotal = price * quantity;
        $item.find('.item-price').data('value', price);
        $item.find('.item-price').text(kendo.toString(price, "c"));
        $item.find('.item-subtotal').data('value', subtotal);
        $item.find('.item-subtotal').text(kendo.toString(subtotal, "c"));
        return subtotal;
    };

    XF.setTotals = function () {
        var total = 0;
        $('.item').each(function () {
            var subtotal = XF.setSubtotalItem($(this));
            total += subtotal;
        });
        $('#lblTotal').data('value',total);
        $('#lblTotal').text(kendo.toString(total, "c"));
    };

    $('.item-product,.item-quantity').on('change', function () {
        XF.setTotals();
    });

    $('#btnSave').on('click', function () {

        XF.confirm("This action will update the order, are you sure you want to continue?", function () {
            XF.saveOrderDetail();
        });
        
    });

})(jQuery, XF);