var XF = XF || {};

(function ($, XF) {
    XF.logData = {};
    XF.addInfoMessage = function (message, type) {
        var type = type || 'info';
        var html = XF.getHtmlFromTemplate(  '#infoMessageTemplate'
                                            , {
                                                message: message
                                                ,type: type
                                            });

        $('#info').prepend(html).find('.alert:first').hide().slideDown(500);
        XF.log(message, type);
    };

    XF.log = function (message, type) {url
        var url = '/Logs/Add';
        var data = {
            Message: message + ' |  data:' + JSON.stringify(XF.logData)
            , Type: type
            , UserId: '?'
        };
        $.post(url,data,XF.logResponse,'json');
    };

    XF.logResponse = function (data) {
        if (!data.Result) {
            alert(data.Message);
        }
    };

    XF.getHtmlFromTemplate = function (selector, data) {
        var template = doT.template($(selector).html());
        var html = template(data);
        return html;
    };


}(jQuery,XF));