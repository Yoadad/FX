var XF = XF || {};

(function ($, XF) {
    XF.InvoiceId = 0;
    XF.addItem = function () {
        var itemIndex = $('.item').size();
        var itemHtml = XF.getHtmlFromTemplate('#newItemTemplate', { index: itemIndex });
        $('#invoiceTable tbody').append(itemHtml);
        $('#invoiceTable tbody .cmb-product:last').focus();

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
                                            .data({ value: price});
        $('.item-' + index + ' .lbl-total').text(kendo.format('{0:C}', total))
                                            .data({ value: total });
    };

    XF.showTotals = function () {
        var subtotal = 0.0;
        var discount = parseFloat($('#txtDiscount').val() || 0.0);
        var tax = parseFloat($('#lblTax').data('value')); 
        $('.item').each(function (index) {
            subtotal += (parseFloat( $(this).find('.lbl-total').data('value')) || 0.00);
        });
        var total = subtotal * (1 + tax)-discount;

        $('#lblSubtotal').text(kendo.format('{0:C}', subtotal)).data({value:subtotal});
        $('#lblTotal').text(kendo.format('{0:C}', total)).data({value:total});
    };

    XF.addItem();

    $('.lnk-addnew').on('click', function () {
        XF.addItem();
    });

    XF.saveInvoice = function (data) {
        var url = '/Sales/Save';
        var data = { data: JSON.stringify(data) };
        $.post(url, data, XF.saveInvoiceResponse,'json');
    };

    XF.saveInvoiceResponse = function(data) {
        if (data.Result) {
            XF.addInfoMessage(data.Message,'success');
            XF.CurrentInvoiceId = data.Data.InvoiceId;
            location.href = '/Invoices';
        }
        else {
            XF.addInfoMessage(data.Message, 'danger');
        }
    };

    XF.getInvoiceDetail = function (invoiceId) {
        var result = [];
        $('.item').each(function (index) {
            result.push({
                InvoiceId: invoiceId,
                ProductId: $(this).find('.cmb-product').val(),
                Quantity: $(this).find('.txt-quantity').val(),
                UnitPrice: $(this).find('.lbl-price').data('value'),
            });
        });
        return result;
    };

    XF.getInvoice = function () {
        var result = {
            Id:XF.InvoiceId,
            Date: new Date($('#txtDate').val()),
            ClientId: $('#cmbClient').val(),
            Discount: $('#txtDiscount').val(),
            PaymentTypeId: $('#cmbPaymentType').val(),
            Tax: $('#lblTax').data('value'),
            Subtotal: $('#lblSubtotal').data('value'),
            total: $('#lblTotal').data('value'),
            InvoiceDetails: XF.getInvoiceDetail(XF.InvoiceId)
        };
        return result;
    };


    $('#btnSaveInvoice').on('click', function () {
        XF.confirm('This action will create a new Invoice', function () {
            var data = { Invoice: XF.getInvoice() };
            XF.saveInvoice(data);
        });
    });

})(jQuery, XF);