using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XF.Locals
{
    public class NotesHub:Hub
    {
        public void TestConnection()
        {
            Clients.Caller.TestConnectionConfirm(string.Format("Connected! [{0}]",DateTime.Now.ToString()));
        }
        public void InformAddClientNote(int noteId)
        {
            Clients.Others.ShowClientNOte(noteId);
        }
    }
}