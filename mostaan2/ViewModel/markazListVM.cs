using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mostaan2.ViewModel
{
    public class markazListVM
    {
        public List<Model.markaz> markazList { get; set; }
        public List<Model.bakhsh> bakhshList { get; set; }
        public List<Model.user> userList { get; set; }
    }
}