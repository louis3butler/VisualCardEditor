namespace VisualCardEditor
{
    using PropertyChanged;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [ImplementPropertyChanged]
    public partial class EffectTrigger
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string FireWhen { get; set; }

        [Required]
        [StringLength(50)]
        public string FireText { get; set; }
    }
}
