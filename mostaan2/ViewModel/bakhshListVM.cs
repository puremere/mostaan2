using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mostaan2.ViewModel
{
   
    public class bakhshListVM
    {
        public List<Model.bakhsh> bakhshList { get; set; }
        public List<Model.user> userList { get; set; }

        public string add { get; set; }
        public string edit { get; set; }
    }
}