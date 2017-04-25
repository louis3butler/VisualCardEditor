namespace VisualCardEditor
{
    using PropertyChanged;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [ImplementPropertyChanged]
    public partial class Card
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public string FlavorText { get; set; }

        public int Cost { get; set; }

        public int Attack { get; set; }

        public int Defense { get; set; }

        public int? CardLevel { get; set; }

        public string Notes { get; set; }

        public int Speed { get; set; }

        public int Reach { get; set; }

        [StringLength(20)]
        public string Version { get; set; }

        public int DiscardCount { get; set; }

        public int DiscardType { get; set; }

        public int SacrificeCount { get; set; }

        public int SacrificeType { get; set; }

        public void CopyTo(Card tmp)
        {
            tmp.Id = this.Id;
            if (this.Name == null) { tmp.Name = null; }
            else { tmp.Name = string.Copy(this.Name); }
            tmp.FlavorText = this.FlavorText;
            tmp.Cost = this.Cost;
            tmp.Attack = this.Attack;
            tmp.Defense = this.Defense;
            tmp.CardLevel = this.CardLevel;
            tmp.Notes = this.Notes;
            tmp.Speed = this.Speed;
            tmp.Reach = this.Reach;
            if (this.Version == null) { tmp.Version = null; }
            else { tmp.Version = string.Copy(this.Version); }
            tmp.DiscardCount = this.DiscardCount;
            tmp.DiscardType = this.DiscardType;
            tmp.SacrificeCount = this.SacrificeCount;
            tmp.SacrificeType = this.SacrificeType;
        }

    }
}
