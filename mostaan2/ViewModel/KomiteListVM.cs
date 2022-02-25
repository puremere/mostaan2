using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mostaan2.ViewModel
{
    public class KomiteListVM
    {
        public List<Model.markaz> markazList { get; set; }
        public List<Model.komite> komiteList { get; set; }
        public List<Model.user> userList { get; set; }
    }
}