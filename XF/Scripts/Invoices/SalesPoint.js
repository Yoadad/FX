var XF = XF || {};

(function ($, XF) {
    XF.CurrentProduct = {};
    XF.addConcept = function (product) {
        var newConcept = XF.getHtmlFromTemplate('#invoiceConcept', product);
        $('#invoiceTable').append(newConcept);
    };

    $('#btnAdd').on('click', function () {
        var product = $.extend({
            Units: $('#txtUnits').val()
        }, XF.CurrentProduct);
        XF.addConcept(product);
    });

    var productAutocomplete = $('#txtProduct').kendoAutoComplete({
        dataTextField: "Name",
        filter: "startswith",
        minLength: 2,       
        dataSource: {
            type: "json",
            serverFiltering: true,
            transport: {
                read: "Products/ByName"
            }
        },
        dataBound: function (e) {

            console.log(e);
        }
    });


})(jQuery, XF);