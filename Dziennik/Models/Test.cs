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
    
    public partial class Test
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Test()
        {
            this.Pytanies = new HashSet<Pytanie>();
            this.Ocenas = new HashSet<Ocena>();
        }
    
        public int id_testu { get; set; }
        public int id_nauczyciela { get; set; }
        public string nazwa { get; set; }
        public int id_przedmiotu { get; set; }
        public System.TimeSpan czas_trwania { get; set; }
        public int ilosc_pytan { get; set; }
        public int max_punktow { get; set; }
    
        public virtual Nauczyciel Nauczyciel { get; set; }
        public virtual Przedmiot Przedmiot { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Pytanie> Pytanies { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ocena> Ocenas { get; set; }
    }
}
