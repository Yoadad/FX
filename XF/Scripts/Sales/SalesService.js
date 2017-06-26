var XF = XF || {};

(function ($, XF) {
    XF.feePercent = 0.10;
    XF.payments = [];
    XF.monthDiff = function (d1, d2) {
        var months;
        months = (d2.getFullYear() - d1.getFullYear()) * 12;
        months -= d1.getMonth() + 1;
        months += d2.getMonth();
        return months <= 0 ? 0 : months;
    };

   


})(jQuery, XF);