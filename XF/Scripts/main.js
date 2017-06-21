/*
    Commont functions
    
    XF.addInfoMessage('action description','info');
    XF.alert('Message');
    XF.confirm('Message', function(){your code...});
    XF.getHtmlFromTemplate('#selector',{data:data});
*/

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
        var html = XF.getHtmlFromTemplate('#alertTemplate',{text: text});
        $('#alert .content').html(html);
        XF.alertWindow.center().open();
    };

    XF.confirm = function (text, fn) {
        XF.confirmOnClose = fn;
        var html = XF.getHtmlFromTemplate('#confirmTemplate', { text: text });
        $('#confirm .content').html(html);
        XF.confirmWindow.center().open();

    };

    XF.prompt = function (text,value, fn) {
        XF.promptOnClose = fn;
        $('#txtPromptValue').val(value);
        var html = XF.getHtmlFromTemplate('#promptTemplate', { text: text });
        $('#prompt .content').html(html);
        XF.promptWindow.center().open();

    };

    XF.format = function (text) {
        var args = arguments;
        return text.replace(/\{\{|\}\}|\{(\d+)\}/g, function (m, n) {
            if (m === "{{") { return "{"; }
            if (m === "}}") { return "}"; }
            return args[n];
        });
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

    //Prompt window
    XF.promptWindow = $('#prompt').kendoWindow({
        width: "500",
        title: "Application Input",
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

    $('#btnPromptCancel').on('click', function () {
        XF.promptWindow.close();
    });

    $('#btnPromptOK').on('click', function () {
        XF.promptWindow.close();
        XF.promptOnClose($('#txtPromptValue').val());
    });

}(jQuery,XF));