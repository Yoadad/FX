var XF = XF || {};

(function ($, XF) {
    $('#cmbClient').on('change', function () {
        location.href = "/ClientNotes/Index/" + $(this).val();
    });
})(jQuery, XF);