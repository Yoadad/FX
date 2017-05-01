using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XF.Entities;

namespace XF.Models
{
    public class NotesViewModel
    {
        public IEnumerable<Client> Clients { get; set; }
        public IEnumerable<ClientNote> Notes { get; set; }
    }
}