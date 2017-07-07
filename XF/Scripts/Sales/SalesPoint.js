var XF = XF || {};

(function ($, XF) {
    XF.Products = JSON.parse($('#hfProducts').val());
    XF.InvoiceId = 0;
    XF.addItem = function () {
        var itemIndex = $('.item').size();
        var itemHtml = XF.getHtmlFromTemplate('#newItemTemplate', { index: itemIndex });

        $('#invoiceTable > tbody').append(itemHtml);
        $('#invoiceTable tbody .autocomplete:last').focus();

        $('.item-' + itemIndex + ' .txt-quantity,#txtDiscount').on('change', function () {
            XF.showItemData(itemIndex);
            XF.showTotals();
        });

        $('.item-' + itemIndex + ' .autocomplete').kendoAutoComplete({
            dataSource: XF.Products,
            template: '#=data.split("|")[0]#',
            filter: "contains",
            placeholder: "Select product...",
            change: function (e) {
                XF.showItemData(itemIndex);
                XF.showTotals();
            }
        });

        XF.showItemData(itemIndex);
        XF.showTotals();
    };

    XF.removeItem = function (index) {
        XF.showItemData(index);
        $('.item-' + index).remove();
        XF.showTotals();
    };


    $('.lnk-addnew').on('click', function () {
        XF.addItem();
    });

    XF.saveInvoice = function (data) {
        var url = '/Sales/Save';
        var data = { data: JSON.stringify(data) };
        $.post(url, data, XF.saveInvoiceResponse, 'json');
    };

    XF.saveInvoiceResponse = function (data) {
        if (data.Result) {
            XF.addInfoMessage(data.Message, 'success');
            XF.CurrentInvoiceId = data.Data.InvoiceId;
            location.href = '/Invoices';
        }
        else {
            XF.addInfoMessage(data.Message, 'danger');
        }
    };
    XF.getPayments = function (invoiceId) {
        var result = [];
        $('.payment-item').each(function (index) {
            result.push({
                InvoiceId: invoiceId,
                PaymentOptionId: $(this).find('.payment-option').val(),
                Amount: $(this).find('.payment-amount').val(),
                Date: $(this).find('.payment-date').val(),
            });
        });
        return result;
    };
    XF.getInvoiceDetail = function (invoiceId) {
        var result = [];
        $('.item').each(function (index) {
            result.push({
                InvoiceId: invoiceId,
                ProductId: $(this).find('input.autocomplete').data('kendoAutoComplete').value().split('|')[1],
                Quantity: $(this).find('.txt-quantity').val(),
                UnitPrice: $(this).find('.lbl-price').data('value'),
                InOrder: $(this).find('.lbl-inorder').data('value')
            });
        });
        return result;
    };

    XF.getInvoice = function () {
        var result = {
            Id: XF.InvoiceId,
            Date: new Date($('#txtDate').val()),
            ClientId: $('#cmbClient').val(),
            Discount: $('#txtDiscount').val(),
            PaymentTypeId: $('#cmbPaymentType').val(),
            Tax: $('#lblTax').data('value'),
            Subtotal: $('#lblSubtotal').data('value'),
            Total: $('#lblTotal').data('value'),
            IsDelivery: $('#cmbIsDelivery').val() == '1',
            Address: $('#txtAddress').val(),
            InvoiceDetails: XF.getInvoiceDetail(XF.InvoiceId),
            Payments: XF.getPayments(XF.InvoiceId)

        };
        return result;
    };

    XF.getBalance = function () {
        $('#divFee').hide();
        var invoice = XF.getInvoice();
        if (invoice.PaymentTypeId == 2 && invoice.Payments.length > 0) {
            $('#lblBalance').text('Loading...');
            var url = '/Invoices/GetInvoiceBalance';
            var data = { jsonInvoice: JSON.stringify(invoice) };
            $.getJSON(url, data, XF.getBalanceResponse);
        }
    };

    XF.getBalanceResponse = function (data) {
        if (data.Result) {
            console.log(data);
            var balance = data.Data.Balance;
            $('#lblBalance').text(kendo.format('{0:C}', balance)).data({ value: balance });
            if (data.Data.HasFee) {
                $('#divFee').show();
            }
        }
        else {
            XF.addInfoMessage(data.Message, 'danger');
        }
    };


    XF.AddPaymentOption = function () {
        var paymentIndex = $('.payment-item').size() || 0;
        var html = XF.getHtmlFromTemplate('#paymentOptionsTemplate', { Index: paymentIndex });
        $('#tablePayments tbody').append(html);
        XF.showItemData();
        XF.showTotals();

        $('.payment-amount:last,.payment-date:last').on('change', function () {
            XF.showItemData();
            XF.showTotals();
        });
    };

    XF.removePaymentItem = function (index) {
        $('.payment-item-' + index).remove();
        XF.showTotals();
    };

    XF.showItemData = function (index) {
        console.log($('.item-' + index + ' input.autocomplete').data('kendoAutoComplete').value());
        var price = $('.item-' + index + ' input.autocomplete').data('kendoAutoComplete').value().split('|')[2];
        var stock = $('.item-' + index + ' input.autocomplete').data('kendoAutoComplete').value().split('|')[3];
        var quantity = $('.item-' + index + ' .txt-quantity').val();
        var inorder = quantity - stock;
        inorder = inorder > 0 ? inorder : 0;
        stock = inorder > 0 ? stock : quantity;
        var total = price * quantity;

        $('.item-' + index + ' .lbl-price').text(kendo.format('{0:C}', parseFloat(price)))
                                            .data({ value: price });
        $('.item-' + index + ' .lbl-total').text(kendo.format('{0:C}', total))
                                            .data({ value: total });
        $('.item-' + index + ' .lbl-stock').text(kendo.format('{0}', stock))
                                            .data({ value: stock });
        $('.item-' + index + ' .lbl-inorder').text(kendo.format('{0}', inorder))
                                            .data({ value: inorder });
    };

    XF.showTotals = function () {
        var subtotal = 0.0;
        var discount = parseFloat($('#txtDiscount').val() || 0.0);
        var tax = parseFloat($('#lblTax').data('value'));
        var paymentsAmount = 0;

        $('.item').each(function (index) {
            subtotal += (parseFloat($(this).find('.lbl-total').data('value')) || 0.00);
        });
        $('.payment-amount').each(function () {
            paymentsAmount += parseFloat($(this).val());
        });

        var total = (subtotal - discount) * (1 + tax);
        var balance = total - paymentsAmount;
        var discountPercent = subtotal == 0 ? 0 : (discount * 100 / subtotal).toFixed(2);

        $('#txtDiscountPercent').val(discountPercent);
        $('#lblSubtotal').text(kendo.format('{0:C}', subtotal)).data({ value: subtotal });
        $('#lblTotal').text(kendo.format('{0:C}', total)).data({ value: total });
        $('#lblPaymentsAmount').text(kendo.format('{0:C}', paymentsAmount)).data({ value: paymentsAmount });
        $('#lblBalance').text(kendo.format('{0:C}', balance)).data({ value: balance });
        XF.getBalance();
    };


    XF.addItem();

    //Binding events

    $('#btnSaveInvoice').on('click', function () {
        XF.confirm('This action will create a new Invoice', function () {
            var data = { Invoice: XF.getInvoice() };
            XF.saveInvoice(data);
        });
    });

    $('#btnCancelInvoice').on('click', function () {
        XF.confirm("Are you sure that you want clear this form?", function () {
            location.href = '/Sales';
        });
    });

    $('#txtDiscountPercent').on('change unfocus', function (e) {
        var discountPercent = parseFloat($(this).val()).toFixed(2);
        var subtotal = $('#lblSubtotal').data('value');
        var discount = discountPercent * subtotal / 100;
        $('#txtDiscount').val(discount.toFixed(2))
                        .trigger('change');
    });

    $('#cmbIsDelivery').on('change', function () {

        if ($(this).val() == 1) {
            $('#txtAddress').show();
        } else {
            $('#txtAddress').hide();
        }
    });

    $('#btnAddPaymentOption').on('click', function () {
        XF.AddPaymentOption();
    });

    $('#chkTaxZero').on('change', function () {
        var tax = $('#hfTax').val();
        if ($(this).is(':checked')) {
            tax = 0.0;
        }
        $('#lblTax')
            .text(kendo.format('{0} %', tax * 100))
            .data({ value: tax });
        XF.showItemData();
        XF.showTotals();

    });

    $('#cmbPaymentType').on('change', function () {
        XF.getBalance();
    });

    $('#divFee').hide();

})(jQuery, XF);