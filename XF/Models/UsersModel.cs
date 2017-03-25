using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XF.Entities;

namespace XF.Models
{
    public class UsersViewModel
    {
        public IEnumerable<AspNetUser> Users { get; set; }
        public IEnumerable<AspNetRole> Roles { get; set; }
    }
}