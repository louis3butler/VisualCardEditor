namespace VisualCardEditor
{
    using PropertyChanged;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Condition")]
    [ImplementPropertyChanged]
    public partial class Condition
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string ConditionName { get; set; }

        [Required]
        [StringLength(50)]
        public string ConditionText { get; set; }
    }
}
