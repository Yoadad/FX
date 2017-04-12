var XF = XF || {};

(function ($, XF) {
    
    XF.addItem = function () {
        var itemIndex = $('.item').size();
        var itemHtml = XF.getHtmlFromTemplate('#newItemTemplate', { index: itemIndex });
        $('#invoiceTable tbody').append(itemHtml);
        $('#invoiceTable tbody .cmb-product:last').focus();

        $('.item-' + itemIndex + ' .txt-quantity').on('change', function () {
            XF.showItemData(itemIndex);
        });

        $('.item-' + itemIndex + ' .cmb-product').on('change', function () {
            XF.showItemData(itemIndex);
        });

        XF.showItemData(itemIndex);
        
    };

    XF.removeItem = function (index) {
        $('.item-' + index).remove();
    };

    XF.showItemData = function (index) {
        var price = $('.item-' + index + ' .cmb-product option:selected').data('price');
        var quantity = $('.item-' + index + ' .txt-quantity').val();
        $('.item-' + index + ' .lbl-price').text(kendo.format('{0:C}', parseFloat(price)));
        $('.item-' + index + ' .lbl-total').text(kendo.format('{0:C}', price * quantity));
        XF.showTotals();
    };

    XF.showTotals = function () {
        var subtotal =  0.0;
        $('.item').each(function (index) {
            console.log(index);
            subtotal += parseFloat($(this).find('.lbl-total').text() || 0.00);
        });

        $('#lblSubtotal').text(kendo.format('{0:C}', subtotal));

    };

    XF.addItem();

    $('.lnk-addnew').on('click', function () {
        XF.addItem();
    });

})(jQuery, XF);