var XF = XF || {};
(function ($, XF) {
    
    XF.getData = function () {
        XF.setDataToGrid([]);
        var url = '/Supplies/Supplies';
        var data = {};
        $.getJSON(url, data, XF.getDataResponse);
    };

    XF.getDataResponse = function (data) {
        if (data.Result) {
            XF.setDataToGrid(data.Data);
        }
        else {
            XF.alert(data.Message);
        }
    };

    XF.setDataToGrid = function (data) {
        var dataSource = new kendo.data.DataSource({
            data: data,
            pageSize: 10,
            schema:
            {
                model: {
                    fields: {
                        Id: { type: "number" },
                        Type: { type: "string" },
                        Date: { type: "date" },
                        StringDate: { type: "string" },
                        Number: { type: "string" },
                        Name: { type: "string" },
                        Amount: { type: "number" },
                        ProviderId: { type: "number" },
                        Provider: { type: "string" },
                    }
                }
            }
        });
        XF.Grid.setDataSource(dataSource);
    };

    XF.Grid = $("#grid").kendoGrid({
        navigatable: false,
        pageable: true,
        sortable: true,
        scrollable: true,
        width: '100%',
        resizable: true,
        height: 340,
        columns: [
            { field: "Provider", title: "Provider", width: 200 },
			{ field: "Type", title: "Type", width: 200 },
			{ field: "Date", title: "Date", width: 100, template:'#:kendo.toString(data.Date,"MM/dd/yyyy")#' },
			{ field: "Number", title: "Number", width: 100},
			{ field: "Name", title: "Name", width: 200 },
			{ field: "Amount", title: "Amount", width: 100},
			{ field: "Id", title: "Actions", template: $('#actionsSupplyTemplate').html(), width: 144 },
        ],
        dataBound: function () {
        }
	}).data("kendoGrid");

    XF.addSupplyWindow = $('#AddWindow').kendoWindow({
        width: "400px",
        title: "Add Supply",
        visible: false,
        modal: true,
        actions: [
            "Close"
        ],
    }).data("kendoWindow");

    XF.editSupplyWindow = $('#EditWindow').kendoWindow({
        width: "400px",
        title: "Edit Supply",
        visible: false,
        modal: true,
        actions: [
            "Close"
        ],
    }).data("kendoWindow");

    XF.cleanControls = function () {
        $('#txtType').val('');
        $('#txtDate').val('');
        $('#txtNumber').val('');
        $('#txtName').val('');
        $('#txtAmount').val('');
    };
    XF.getParameters = function () {
        var parameters = {
            Id: $('#hfId').val(),
            Type: $('#txtType').val(),
            Date: $('#txtDate').val(),
            Number: $('#txtNumber').val(),
            Name: $('#txtName').val(),
            Amount: $('#txtAmount').val(),
            ProviderId: $('#cmbProviderId').val()
        };
        return parameters;
    };
    XF.getEditParameters = function () {
        var parameters = {
            Id: $('#hfId').val(),
            Type: $('#txtEditType').val(),
            Date: $('#txtEditDate').val(),
            Number: $('#txtEditNumber').val(),
            Name: $('#txtEditName').val(),
            Amount: $('#txtEditAmount').val(),
            ProviderId: $('#cmbEditProviderId').val()
        };
        return parameters;
    };

    XF.saveSupply = function () {
        var url = '/Supplies/Create';
        var data = XF.getParameters();
        $.getJSON(url, data, XF.saveSupplyResponse);
    };

    XF.saveSupplyResponse = function (data) {
        if (data.Result) {
            XF.getData();
            XF.addSupplyWindow.close();
        }
        else {
            XF.alert(XF.Message);
        }
    };

    XF.editSupply = function () {
        var url = '/Supplies/Edit';
        var data = XF.getEditParameters();
        console.log(data);
        $.getJSON(url, data, XF.editSupplyResponse);
    };

    XF.editSupplyResponse = function (data) {
        if (data.Result) {
            XF.getData();
            XF.editSupplyWindow.close();
        }
        else {
            XF.alert(XF.Message);
        }
    };

    XF.getSupply = function (id) {
        var url = '/Supplies/Supply';
        var data = {Id:id};
        $.getJSON(url, data, XF.getSupplyResponse);
    };

    XF.getSupplyResponse = function (data) {
        if (data.Result) {
            XF.setEditData(data.Data);
            XF.editSupplyWindow.center().open();
        }
        else {
            XF.alert(XF.Message);
        }
    };

    XF.setEditData = function (data) {
        $('#hfId').val(data.Id);
        $('#txtEditType').val(data.Type);
        $('#txtEditDate').val(data.StringDate);
        $('#txtEditNumber').val(data.Number);
        $('#txtEditName').val(data.Name);
        $('#txtEditAmount').val(data.Amount);
        $('#cmbEditProviderId').val(data.ProviderId);
    };

    XF.deleteSupply = function (id) {
        XF.confirm("Are you sure you want to delete this record?",
            function () {
                var url = '/Supplies/Delete/' + id;
                $.getJSON(url, {}, XF.deleteSupplyResponse);
            });
    };


    XF.deleteSupplyResponse = function(data){
        if (data.Result) {
            XF.getData();
        }
        else {
            XF.alert(data.Message);
        }
    }

    XF.init = function () {
        XF.getData();
    };

    XF.init();

    $('#btnSaveSupply').on('click', function () {
        XF.saveSupply();
    });

    $('#btnEditSupply').on('click', function () {
        XF.editSupply();
    });

    

})(jQuery, XF);
