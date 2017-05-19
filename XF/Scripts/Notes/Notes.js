var XF = XF || {};

(function ($, XF) {

    XF.initNotesConnection = function () {
        $.connection.hub.start()
        .done(function (message) {
            $.connection.notesHub.server.testConnection();
            XF.informAddClientNote = function (noteId,clientId) {
                $.connection.notesHub.server.informAddClientNote(noteId, clientId);
            }
        })
        .fail(function () {
            console.log('Connection FAIL!');
        });

        $.connection.notesHub.client.testConnectionConfirm = function (message) {
            console.log(message);
        };

        $.connection.notesHub.client.informAddClientNote = function (noteId,clientId) { 
            XF.showNote(noteId,clientId);
        };
    };
    
    XF.initNotesConnection();


    XF.addNote = function (clientId, text) {
        var url = '/Notes/Add';
        var data = { ClientId: clientId, Text: text };
        $.post(url, data, XF.addNoteResponse,'json');
    };

    XF.addNoteResponse = function (data) {
        if (data.Result) {
            XF.informAddClientNote(data.Data.NoteId, data.Data.ClientId);
        }
        else {
            console.log(data.Message);
        }
    };

    XF.showNote = function (noteId, clientId) {
        var url = '/Notes/Get';
        var data = {noteId:noteId,clientId:clientId};
        $.post(url,data,XF.showNoteResponse,'json');
    };

    XF.showNoteResponse = function (data) {
        if (data.Result) {
            var html = XF.getHtmlFromTemplate('#noteTemplate', data.Data);
            $('#clientNotes').prepend(html);
        }
        else {
            console.log(data.Message);
        }
    };

    XF.ClearControls = function () {
        $('#txtNoteText').val('');
    };

    XF.filterNotes = function (clientId) {
        $('div.note').hide();
        $('.client-' + clientId).show();
    };

    $('#btnAddNote').on('click', function () {
        var clientId = $('#cmbNoteClients').val();
        var text = $('#txtNoteText').val();
        if (!clientId || $.trim(clientId) == '') {
            XF.alert("Please select client related with this note", 'warning', 'glyphicon-warning-sign');
            return;
        }
        if (!text || $.trim(text) == '') {
            XF.alert("Please add a note in the text box", 'warning', 'glyphicon-warning-sign');
            return;
        }

        XF.addNote(clientId, text)
        XF.ClearControls();
    });

    $('#cmbNoteClients').on('change', function () {
        console.log(':)');
        XF.filterNotes($(this).val());
    });

})(jQuery, XF);