var XF = XF || {};

(function ($, XF) {

    XF.InvoiceId = $('#hfInvoiceId').val();
    XF.addItem = function () {
        var itemIndex = $('.item').size();
        var itemHtml = XF.getHtmlFromTemplate('#newItemTemplate', { index: itemIndex });

        $('#invoiceTable > tbody').append(itemHtml);
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


    $('.lnk-addnew').on('click', function () {
        XF.addItem();
    });

    XF.saveInvoice = function (data) {
        var url = '/Sales/Update';
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
                Id: $(this).data('paymentid'),
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
                Id: $(this).data('detailid'),
                InvoiceId: invoiceId,
                Note: $(this).find('.txt-note').val(),
                ProductId: $(this).find('.cmb-product').val(),
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
            Payments: XF.getPayments(XF.InvoiceId),
            SNAP: $('#txtSNAP').val(),
            DeliveryFee: $('#txtDeliveryFee').val(),
            InstalationFee: $('#txtInstalationFee').val()
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

    XF.confirmEmail = function (invoiceId, email) {
        XF.prompt("Send Invoice to:", email, function (eml) {
            XF.sendInvoiceEmail(invoiceId, eml);
        });
    };

    XF.sendInvoiceEmail = function (invoiceId, email) {
        var url = '/Sales/EmailInvoice';
        var data = { id: invoiceId, email: email };
        $.post(url, data, XF.sendInvoiceEmailResponse, 'json');
    };

    XF.sendInvoiceEmailResponse = function (data) {
        if (data.Result) {
            XF.addInfoMessage(data.Message, 'success');
        }
        else {
            XF.addInfoMessage(data.Message, 'danger');
        }
    };

    XF.showItemData = function (index) {
        var price = $('.item-' + index + ' .cmb-product option:selected').data('price');
        var stock = $('.item-' + index + ' .cmb-product option:selected').data('stock');
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
        var snapFee = parseFloat($('#txtSNAP').val());
        var deliveryFee = parseFloat($('#txtDeliveryFee').val());
        var instalationFee = parseFloat($('#txtInstalationFee').val());

        $('.item').each(function (index) {
            subtotal += (parseFloat($(this).find('.lbl-total').data('value')) || 0.00);
        });
        $('.payment-amount').each(function () {
            paymentsAmount += parseFloat($(this).val());
        });

        var total = ((subtotal - discount) * (1 + tax)) + snapFee + deliveryFee + instalationFee;
        var balance = total - paymentsAmount;
        var discountPercent = subtotal == 0 ? 0 : (discount * 100 / subtotal).toFixed(2);

        if (Math.abs(balance) <= 0.09) {
            balance = 0.0;
        }

        $('#txtDiscountPercent').val(discountPercent);
        $('#lblSubtotal').text(kendo.format('{0:C}', subtotal)).data({ value: subtotal });
        $('#lblTotal').text(kendo.format('{0:C}', total)).data({ value: total });
        $('#lblPaymentsAmount').text(kendo.format('{0:C}', paymentsAmount)).data({ value: paymentsAmount });
        $('#lblBalance').text(kendo.format('{0:C}', balance)).data({ value: balance });
        XF.getBalance();
    };

    //Binding events

    $('#btnSaveInvoice').on('click', function () {
        XF.confirm('This action will update the Invoice', function () {
            var data = {
                Invoice: XF.getInvoice()
            };
            XF.saveInvoice(data);
        });
    });

    $('#btnCancelInvoice').on('click', function () {
        XF.confirm("Are you sure that you want cancel?", function () {
            location.href = "/Invoices";
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

    $('.item').each(function (index) {
        XF.showItemData(index);
        $('.item-' + index + ' .txt-quantity,#txtDiscount,.item-' + index + ' .cmb-product,#txtSNAP,#txtDeliveryFee,#txtInstalationFee').on('change', function () {
            XF.showItemData(index);
            XF.showTotals();
        });
    });

    XF.convertToInvoice = function (id) {
        XF.confirm("This action will convert Estimate to Invoice, do you want to continue?",function () {
            var url = "/Invoices/ConvertToInvoice";
            var data = { id: id };
            $.post(url, data, XF.convertToInvoiceResponse,'json');
        });    
    };

    XF.convertToInvoiceResponse = function (data) {
        if (data.Result) {
            XF.addInfoMessage(data.Message, 'success');
            location.href = '/Invoices/Details/' + data.InvoiceId;
        }
        else {
            XF.addInfoMessage(data.Message, 'danger');
        }
    };
    

    XF.showTotals();

    $('#chkTaxZero').on('change', function () {
        console.log(':)');
        var tax = $('#hfTax').val();
        if (!$(this).is(':checked')) {
            tax = 0.0;
        }
        $('#lblTax')
            .text(kendo.format('{0:P}', tax))
            .data({
                value: tax
            });
    });


    $('#cmbIsDelivery').trigger('change');

    $('#btnSendEmail').on('click', function () {
        XF.confirmEmail($('#hfInvoiceId').val(), $('#hfEmail').val());
    });

    $('.payment-amount:last,.payment-date:last').on('change', function () {
        XF.showItemData();
        XF.showTotals();
    });


})(jQuery, XF);