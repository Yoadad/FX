var XF = XF || {};

(function ($, XF) {
    XF.confirmResult = false; 
    XF.confirmOnClose = function(){};
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
        var template = kendo.template($(selector).html());
        var html = template(data);
        return html;
    };

    XF.alert = function (text,type,icon) {
        var html = XF.getHtmlFromTemplate('#alertTemplate',
        {
            text: text,
            type: type || 'info',
            icon: icon || 'glyphicon-info-sign'
        });
        $('#alert .content').html(html);
        XF.alertWindow.center().open();
    };

    XF.confirm = function (text,fn) {
        XF.confirmOnClose = fn;
        var html = XF.getHtmlFromTemplate('#confirmTemplate',{text: text});
        $('#confirm .content').html(html);
        XF.confirmWindow.center().open();

    };

    //Common controlls

    //Alert window
    XF.alertWindow = $('#alert').kendoWindow({
        width: "600px",
        title: "Application Message",
        visible: false,
        modal:true,
        actions: [
            "Close"
        ],
    }).data("kendoWindow");

    $('#btnAlertOK').on('click', function () {
        XF.alertWindow.close();
    });

    //Confirm window
    XF.confirmWindow = $('#confirm').kendoWindow({
        width: "500",
        title: "Application Message",
        visible: false,
        modal: true,
        actions: [
            "Close"
        ],
    }).data("kendoWindow");

    $('#btnConfirmCancel').on('click', function () {
        XF.confirmWindow.close();
    });
    $('#btnConfirmOK').on('click', function () {
        XF.confirmWindow.close();
        XF.confirmOnClose();
    });

    //kendoWindow({
    //    width: "600px",
    //    title: "About Alvar Aalto",
    //    visible: false,
    //    actions: [
    //        "Pin",
    //        "Minimize",
    //        "Maximize",
    //        "Close"
    //    ],
    //    close: onClose
    //}).data("kendoWindow").center().open();

}(jQuery,XF));