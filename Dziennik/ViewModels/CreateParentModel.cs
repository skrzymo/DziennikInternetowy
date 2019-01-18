using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dziennik.ViewModels
{
    public class CreateParentModel
    {
        public string name { get; set; }
        public string surname { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string phone { get; set; }
        public DateTime? birthDate { get; set; }
    }
}