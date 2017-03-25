using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XF.Entities;

namespace XF.Models
{
    public class UserRoleModel
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public bool IsChecked { get; set; }
    }

    public class UserRolesViewModel
    {
        public IEnumerable<KeyValuePair<string,string>> Users { get; set; }
        public IEnumerable<KeyValuePair<string, string>> Roles { get; set; }
    }
}