﻿var XF = XF || {};

$(document).ready(function ($,XF) {

    XF.setRole = function (userId, roleId, isCheked) {
        var url = '/Users/SetRole';
        var data = { userId: userId, roleId: roleId, isChecked: isCheked };
        $.post(url, data, XF.setRoleResponse,'json');
    };

    XF.setRoleResponse = function(data){
        if (!data.Result) {            
            alert(data.Message);
        }
    };

    $('.xf-chk-role').click(function () {
        var userId = $(this).data('userid');
        var roleId = $(this).data('roleid');
        var isChecked = $(this).is(':checked');
        XF.setRole(userId,roleId,isChecked);
    });


}(jQuery,XF));



