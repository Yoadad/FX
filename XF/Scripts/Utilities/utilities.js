var XF = XF || {};
(function ($, XF) {

    XF.getData = function () {
        XF.setDataToGrid([]);
        var url = '/Utilities/Utilities';
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
                        Split: { type: "string" },
                        OriginalAmount: { type: "number" },
                        PaidAmount: { type: "number" },
                        Description: { type: "string" },
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
            { field: "Description", title: "Description", width: 200 },
			{ field: "Type", title: "Type", width: 200 },
			{ field: "Date", title: "Date", width: 100, template: '#:kendo.toString(data.Date,"MM/dd/yyyy")#' },
			{ field: "Number", title: "Number", width: 100 },
			{ field: "Name", title: "Name", width: 200 },
			{ field: "Split", title: "Split", width: 100 },
            { field: "OriginalAmount", title: "Original Amount", width: 100 },
            { field: "PaidAmount", title: "Paid Amount", width: 100 },
			{ field: "Id", title: "Actions", template: $('#actionsUtilityTemplate').html(), width: 144 },
        ],
        dataBound: function () {
        }
    }).data("kendoGrid");

    XF.addUtilityWindow = $('#AddWindow').kendoWindow({
        width: "400px",
        title: "Add Utility",
        visible: false,
        modal: true,
        actions: [
            "Close"
        ],
    }).data("kendoWindow");

    XF.editUtilityWindow = $('#EditWindow').kendoWindow({
        width: "400px",
        title: "Edit Utility",
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
        $('#txtSplit').val('');
        $('#txtOriginalAmount').val('');
        $('#txtPaidAmount').val('');
        $('#txtDescription').val('');
    };
    XF.getParameters = function () {
        var parameters = {
            Id: $('#hfId').val(),
            Type: $('#txtType').val(),
            Date: $('#txtDate').val(),
            Number: $('#txtNumber').val(),
            Name: $('#txtName').val(),
            Split: $('#txtSplit').val(),
            OriginalAmount: $('#txtOriginalAmount').val(),
            PaidAmount: $('#txtPaidAmount').val(),
            Description: $('#txtDescription').val()
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
            Split: $('#txtEditSplit').val(),
            OriginalAmount: $('#txtEditOriginalAmount').val(),
            PaidAmount: $('#txtEditPaidAmount').val(),
            Description: $('#txtEditDescription').val()

        };
        return parameters;
    };

    XF.saveUtility = function () {
        var url = '/Utilities/Create';
        var data = XF.getParameters();
        $.getJSON(url, data, XF.saveUtilityResponse);
    };

    XF.saveUtilityResponse = function (data) {
        if (data.Result) {
            XF.getData();
            XF.addUtilityWindow.close();
        }
        else {
            XF.alert(XF.Message);
        }
    };

    XF.editUtility = function () {
        var url = '/Utilities/Edit';
        var data = XF.getEditParameters();
        console.log(data);
        $.getJSON(url, data, XF.editUtilityResponse);
    };

    XF.editUtilityResponse = function (data) {
        if (data.Result) {
            XF.getData();
            XF.editUtilityWindow.close();
        }
        else {
            XF.alert(data.Message);
        }
    };

    XF.getUtility = function (id) {
        var url = '/Utilities/Utility';
        var data = { Id: id };
        $.getJSON(url, data, XF.getUtilityResponse);
    };

    XF.getUtilityResponse = function (data) {
        if (data.Result) {
            XF.setEditData(data.Data);
            XF.editUtilityWindow.center().open();
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
        $('#txtEditSplit').val(data.Split);
        $('#txtEditOriginalAmount').val(data.OriginalAmount);
        $('#txtEditPaidAmount').val(data.PaidAmount);
        $('#txtEditDescription').val(data.Description);
    };

    XF.deleteUtility = function (id) {
        XF.confirm("Are you sure you want to delete this record?",
            function () {
                var url = '/Utilities/Delete/' + id;
                $.getJSON(url, {}, XF.deleteUtilityResponse);
            });
    };


    XF.deleteUtilityResponse = function (data) {
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

    $('#btnSaveUtility').on('click', function () {
        XF.saveUtility();
    });

    $('#btnEditUtility').on('click', function () {
        XF.editUtility();
    });



})(jQuery, XF);
