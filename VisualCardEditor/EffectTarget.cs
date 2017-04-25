namespace VisualCardEditor
{
    using PropertyChanged;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("EffectTargets")]
    [ImplementPropertyChanged]
    public partial class EffectTarget
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Plural { get; set; }

        public int Owner { get; set; }

        public int CardType { get; set; }
        public void CopyTo(EffectTarget tmp)
        {
            if (tmp == null) tmp = new EffectTarget();
            tmp.Id = this.Id;
            tmp.Plural = this.Plural;
            tmp.Owner = this.Owner;
            tmp.CardType = this.CardType;
        }
    }
}
