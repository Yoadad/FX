var XF = XF || {};

(function ($, XF) {

    $('#grid').kendoGrid({
        columns:
            [                
                { field: "Concept" },
                { field: "Quantity" },
                { field: "Price"},
                { field: "Subtotal"}
            ]
    });

})(jQuery, XF);