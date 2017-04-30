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
            $('#notes').append(html);
        }
        else {
            console.log(data.Message);
        }
    };

})(jQuery, XF);