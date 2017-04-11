var XF = XF || {};

(function ($, XF) {
    var baseUrl = document.URL;
    XF.CurrentProduct = {};
    XF.addConcept = function (product) {
        var gridDataSource = $('#grid').data('kendoGrid').dataSource;
        gridDataSource.add(product);
        gridDataSource.sync();
        console.log(gridDataSource);
    };

    $('#btnAdd').on('click', function () {
        var product = $.extend({
            Units: $('#txtUnits').val(),
            Concept: $('#txtProduct').val(),
            Price: 0.00,
            Total: 0.00
        }, XF.CurrentProduct);

        
        $.ajax({
            url: baseUrl + '/GetProduct',
            method: 'GET',
            dataType: 'json',
            data: { concept: JSON.stringify(product.Concept) }
        })
        .done(function (data) {
            //console.log(data.products);
            product.Price = data.products.PurchasePrice;
            product.Total = product.Units * product.Price;
            XF.addConcept(product);
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            console.log('Error status: ' + textStatus + ' ' + jqXHR.statusText())
        });
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
    
    var products = new kendo.data.DataSource({
     
    });
    function onDataBonDataBindingound(e)
    {
        alert(e.model.Concept);
    }
    //declaring the grid structure
    $('#grid').kendoGrid({
        dataSource: products,
        //dataBinding: onDataBinding,
        columns:
            [
                { field: "Id", hidden: true },
                { field: "Concept" },
                { field: "Units" },
                { field: "Price" },
                { field: "Total" }

                //{field:"footer", width:100, footerTemplate:'<p>MyFooter</p>'}
            ]
    });

})(jQuery, XF);