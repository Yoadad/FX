var XF = XF || {};

$(document).ready(function ($,XF) {
    XF.selectedUseId = 0;
    XF.comissions = [
        { key: "0.05", value: "5%" },
        { key: "0.10", value: "10%" },
        { key: "0.12", value: "12%" },
        { key: "0.15", value: "15%" }
    ];

    XF.setRole = function (userId, roleId, isCheked) {
        var url = '/Users/SetRole';
        var data = { userId: userId, roleId: roleId, isChecked: isCheked };
        XF.logData = data;
        $.post(url, data, XF.setRoleResponse,'json');
    };

    XF.setRoleResponse = function(data){
        if (data.Result) {
            var message = 'Set/Remove role to user';
            XF.addInfoMessage(message, 'success');
        }
        else {
            XF.addInfoMessage(data.Message,'danger');
        }
    };

    XF.setComission = function (userId, comission) {
        var url = '/Users/SetComission';
        var data = { UserId: userId, Comission: comission};
        XF.logData = data;
        $.post(url, data, XF.setComissionResponse, 'json');
    };

    XF.setComissionResponse = function (data) {
        if (data.Result) {
            var value = kendo.toString(data.Data.Comission, '#.00');
            $('.xf-lnk-comission[data-userid="' + data.Data.UserId + '"]')
                .text(value);
            var message = 'Set comission to user';
            XF.addInfoMessage(message, 'success');
        }
        else {
            XF.addInfoMessage(data.Message, 'danger');
        }
    };

    $('.xf-chk-role').click(function () {
        var userId = $(this).data('userid');
        var roleId = $(this).data('roleid');
        var isChecked = $(this).is(':checked');
        XF.setRole(userId,roleId,isChecked);
    });

    $('.xf-lnk-comission').on('click', function () {
        XF.selectedUseId = $(this).data('userid');
        var value = kendo.toString($(this).text(), '#.00');
        XF.options(XF.comissions, 
                    value,
                    'Comission:',
                    function (value) {
            XF.setComission(XF.selectedUseId,value);
        });
    });

}(jQuery,XF));



