//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Dziennik.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Ocena
    {
        public int id_oceny { get; set; }
        public int ocena1 { get; set; }
        public int id_przedmiotu { get; set; }
        public int id_ucznia { get; set; }
        public System.DateTime data { get; set; }
        public Nullable<int> id_testu { get; set; }
    
        public virtual Przedmiot Przedmiot { get; set; }
        public virtual Uczen Uczen { get; set; }
        public virtual Test Test { get; set; }
    }
}
