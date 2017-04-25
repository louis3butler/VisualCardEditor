namespace VisualCardEditor
{
    using PropertyChanged;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;
    [ImplementPropertyChanged]
    public partial class CardEffect
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(32)]
        public string Name { get; set; }

        [Required]
        public string EffectText { get; set; }

        public int EffectTrigger { get; set; }

        public int TriggerTarget { get; set; }

        public int EffectTarget { get; set; }

        public int FireCondition { get; set; }

        public int ConditionValue { get; set; }

        public int Effect { get; set; }

        public int Value1 { get; set; }

        public int Value2 { get; set; }

        public int Duration { get; set; }

        public int Cost { get; set; }

        public int DiscardCount { get; set; }

        public int DiscardType { get; set; }

        public int SacrificeCount { get; set; }

        public int SacrificeType { get; set; }

        public int Deplete { get; set; }

        public void CopyTo(CardEffect tmp)
        {
            tmp.Id = this.Id;
            if (this.Name == null) { tmp.Name = null; }
            else { tmp.Name = string.Copy(this.Name); }
            if (this.EffectText == null) { tmp.EffectText = null; }
            else { tmp.EffectText = string.Copy(this.EffectText); }
            tmp.EffectTrigger = this.EffectTrigger;
            tmp.TriggerTarget = this.TriggerTarget;
            tmp.EffectTarget = this.EffectTarget;
            tmp.FireCondition = this.FireCondition;
            tmp.ConditionValue = this.ConditionValue;
            tmp.Effect = this.Effect;
            tmp.Value1 = this.Value1;
            tmp.Value2 = this.Value2;
            tmp.Duration = this.Duration;
            tmp.Cost = this.Cost;
            tmp.DiscardCount = this.DiscardCount;
            tmp.DiscardType = this.DiscardType;
            tmp.SacrificeCount = this.SacrificeCount;
            tmp.SacrificeType = this.SacrificeType;
            tmp.Deplete = this.Deplete;
        }


    }
}
