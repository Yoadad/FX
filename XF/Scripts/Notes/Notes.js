var XF = XF || {};

(function ($, XF) {

    XF.initNotesConnection = function () {
        $.connection.hub.start()
        .done(function (message) {
            $.connection.notesHub.server.testConnection();
            XF.informAddClientNote = function (noteId) {
                $.connection.notesHub.server.informAddClientNote(noteId);
            }
        })
        .fail(function () {
            console.log('Connection FAIL!');
        });

        $.connection.notesHub.client.testConnectionConfirm = function (message) {
            console.log(message);
        };

        $.connection.notesHub.client.informAddClientNote = function (noteId) {
            console.log(noteId);
            XF.showNote(noteId);
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
            XF.informAddClientNote(data.Data.NoteId);
        }
        else {
            console.log(data.Message);
        }
    };

    XF.showNote = function (noteId) {
        var url = '/Notes/Get';
        var data = {noteId:noteId};
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

    $('#btnAddNote').on('click', function () {
        var clientId = $('#cmbNoteClients').val();
        var text = $('#txtNoteText').val();
        if (!clientId) {
            XF.alert("Please select a client related with this note", 'warning', 'glyphicon-warning-sign');
            return;
        }
        if (!text || $.trim(text) == '') {
            XF.alert("Please add a note in the text box", 'warning', 'glyphicon-warning-sign');
            return;
        }

        XF.addNote(clientId,text)
    });

})(jQuery, XF);