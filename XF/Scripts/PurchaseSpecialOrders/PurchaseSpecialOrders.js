var XF = XF || {};
(function () {
    XF.PurchaseId = 0;
    XF.addItem = function () {
        var itemIndex = $('.item').size();
        var itemHtml = XF.getHtmlFromTemplate('#newItemTemplate', { index: itemIndex });
        $('#purchaseSpecialTable tbody').append(itemHtml);
        $('#purchaseSpecialTable tbody .cmb-product:last').focus();

        $('.item-' + itemIndex + ' .txt-quantity,#txtDiscount,.item-' + itemIndex + ' .cmb-product').on('change', function () {
            XF.showItemData(itemIndex);
            XF.showTotals();
        });

        XF.showItemData(itemIndex);
        XF.showTotals();

    };
    XF.removeItem = function (index) {
        $('.item-' + index).remove();
    };

    XF.showItemData = function (index) {
        var price = $('.item-' + index + ' .cmb-product option:selected').data('price');
        var quantity = $('.item-' + index + ' .txt-quantity').val();
        var total = price * quantity;

        $('.item-' + index + ' .lbl-price').text(kendo.format('{0:C}', parseFloat(price)))
                                            .data({ value: price });
        $('.item-' + index + ' .lbl-total').text(kendo.format('{0:C}', total))
                                            .data({ value: total });
    };

    XF.showTotals = function () {
        var subtotal = 0.0;
        var discount = parseFloat($('#txtDiscount').val() || 0.0);
        var tax = parseFloat($('#lblTax').data('value'));
        $('.item').each(function (index) {
            subtotal += (parseFloat($(this).find('.lbl-total').data('value')) || 0.00);
        });
        var total = subtotal * (1 + tax) - discount;
        var discountPercent = subtotal == 0 ? 0 : (discount * 100 / subtotal).toFixed(2);

        $('#txtDiscountPercent').val(discountPercent);
        $('#lblSubtotal').text(kendo.format('{0:C}', subtotal)).data({ value: subtotal });
        $('#lblTotal').text(kendo.format('{0:C}', total)).data({ value: total });
    };

    XF.addItem();

    $('.lnk-addnew').on('click', function () {
        XF.addItem();
    });

    XF.savePurchase = function (data)
    {
        var Url = '/PurchaseSpecialOrders/Save';
        var data = { data: JSON.stringify(data) };
        $.post(Url, data, XF.savePurchaseResponse);
    };

    XF.savePurchaseResponse = function (data) {
        if (data.Result) {
            XF.addInfoMessage(data.Message, 'success');
            location.href = "/Purchases/List";
        }
        else {
            XF.addInfoMessage(data.Message, 'danger');
        }
    };

    XF.getPurchaseOrderDetail = function (purchaseId) {
        var result = [];
        $('.item').each(function (index) {
            result.push({
                PurchaseId: purchaseId,
                ProductId: $(this).find('.cmb-product').val(),
                Quantity: $(this).find('.txt-quantity').val(),
                UnitPrice: $(this).find('.lbl-price').data('value'),
            });
        });
        return result;
    };

    XF.getProviderDetail = function() {
        var result = {
            Id: $('#cmbProvider').val(),
            Name: $('#cmbProvider :selected').text()
        };
        return result;
    };

    XF.getPurchase = function () {
        var result =
            {
                Id: XF.PurchaseId,
                Date: new Date($('#txtDate').val()),
                Discount: $('#txtDiscount').val(),
                Subtotal: $('#lblSubtotal').data('value'),
                Tax: $('#lblTax').data('value'),
                Total: $('#lblTotal').data('value'),
                PurchaseOrderDetails: XF.getPurchaseOrderDetail(XF.PurchaseId),
                ProviderId: $('#cmbProvider').val()
            };

        return result;
    };

    $('#btnSavePurchase').on('click', function () {
        XF.confirm('This action will create a new Purchase Order', function () {
            var data = { PurchaseOrder: XF.getPurchase() };
            XF.savePurchase(data);
        });
    });
    $('#btnCancelPurchaseSpecial').on('click', function () {
        XF.confirm('This action will set the default data, Do you want to clear this form?', function () {
            location.reload();
        });
    });
    $('#txtDiscountPercent').on('change unfocus', function (e) {
        var discountPercent = parseFloat($(this).val()).toFixed(2);
        var subtotal = $('#lblSubtotal').data('value');
        var discount = discountPercent * subtotal / 100;
        $('#txtDiscount').val(discount.toFixed(2))
                        .trigger('change');
    });
})(jQuery, XF);