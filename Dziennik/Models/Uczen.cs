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
    
    public partial class Uczen
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Uczen()
        {
            this.Ocenas = new HashSet<Ocena>();
        }
    
        public int id_ucznia { get; set; }
        public int id_osoby { get; set; }
        public int id_rodzica { get; set; }
        public Nullable<int> id_klasy { get; set; }
    
        public virtual Klasa Klasa { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ocena> Ocenas { get; set; }
        public virtual Osoba Osoba { get; set; }
        public virtual Rodzic Rodzic { get; set; }
    }
}