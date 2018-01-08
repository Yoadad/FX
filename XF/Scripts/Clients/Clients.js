var XF = XF || {};
(function ($,XF){
   
    XF.getClients = function () {
        var url = '/Clients/Clients';
        var data = {};
        $.getJSON(url, data, XF.getClientsResponse);
    };

    XF.getClientsResponse = function(data){
        if (data) {
            XF.createGrid(data,'#grid');
        }
        else {
            XF.alert("The are a problem with the server. Please try to refresh the page");
        }
    };

    XF.getDataSourceGrid = function (data) {
        var dataSource = new kendo.data.DataSource({
            data: data,
            schema: {
                model: {
                    fields: XF.getModel(data)
                }
            },
            pageSize: 10
        });
        return dataSource;
    };

    XF.clearGrid = function (gridName) {
        if ($(gridName).data("kendoGrid")) {
            $(gridName).data("kendoGrid").destroy();
        }
        $(gridName).unbind().html('');
    };


    XF.createGrid = function (data, selector) {

        XF.clearGrid(selector);
        var grid = $(selector).kendoGrid({
            dataSource: XF.getDataSourceGrid(data),
            dataBound: function () {
            },
            filterable: {
                extra: false,
                operators:
                     {
                         string: {
                             contains: "Contains",
                             eq: "Is equal to",
                             neq: "Is not equal to"
                         }
                     }
            },
            scrollable: true,
            pageable: true,
            sortable: true,
            resizable: true,
            filterable: true,
            height: 500,
            columns: XF.getColumns(data)
        });
    };

    XF.getModel = function (data) {
        var result = {};
        for (var prop in data[0]) {
            result[prop] = { type: typeof data[0][prop] };
        }
        return result;
    };
    
    XF.getColumns = function (data) {
        var result = [
            { field: 'Id', title: 'Id', hidden: true, filterable:false },
            { field: 'FirstName', title: 'First Name', hidden: false, filterable: true },
            { field: 'MiddleName', title: 'Middle Name', hidden: false, filterable: true },
            { field: 'LastName', title: 'Last Name', hidden: false, filterable: true },
            { field: 'Email', title: 'Email', hidden: false, filterable: false },
            { field: 'Phone', title: 'Phone Number', hidden: false, filterable: false },
            { field: 'Address', title: 'Address', hidden: false, filterable: false },
            { field: 'Id', title: 'Actions', template: $('#actionsTemplate').html(), filterable: false, width: 244 }
        ];
        return result;
    };

    XF.init = function () {
        XF.getClients();
    };

    XF.init();

})(jQuery,XF);