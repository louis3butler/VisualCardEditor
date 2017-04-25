using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VisualCardEditor
{
    public partial class MainWindow : Window
    {
        public void BuildEffectText()
        {
            using (ChampionsDB db = new ChampionsDB())
            {
                string WhenStr = "<When>", CostStr = "<Cost>", ActStr = "<Action>", TriggerStr = "<Trigger>", TargetStr = "<Target>", DurationStr = "<Duration>";
                string DiscardStr = "",
                       DiscardTypeStr = "",
                       SacrificeStr = "",
                       SacrificeTypeStr = "";
                try
                {
                    if (CurrentEffect.DiscardCount > 0)
                    {
                        DiscardTypeStr = CardTypeNameLookup.CardTypeName(CurrentEffect.DiscardType);
                        DiscardStr = string.Format(", Discard {0} {1}{2}", CurrentEffect.DiscardCount, DiscardTypeStr, (CurrentEffect.DiscardCount == 1) ? "" : (DiscardTypeStr[DiscardTypeStr.Length - 1] == 's') ? "es" : "s");
                    }

                    if (CurrentEffect.SacrificeCount > 0)
                    {
                        SacrificeTypeStr = CardTypeNameLookup.CardTypeName(CurrentEffect.SacrificeType);
                        DiscardStr = string.Format(", Sacrifice {0} {1}{2}", CurrentEffect.SacrificeCount, SacrificeTypeStr, (CurrentEffect.SacrificeCount == 1) ? "" : (SacrificeTypeStr[SacrificeTypeStr.Length - 1] == 's') ? "es" : "s");
                    }

                    CostStr = string.Format("{0}{1}{2}{3}", (CurrentEffect.Cost > 0) ? CurrentEffect.Cost.ToString() : "", (CurrentEffect.Deplete == 0) ? "" : (CurrentEffect.Cost == 0) ? "<D>" : ", <D>", DiscardStr, SacrificeStr);
                }
                catch (Exception)
                {
                    // Error creating CostStr
                }
                try
                {
                    if (CurrentEffect.EffectTrigger != 0)
                    {
                        //WhenStr = (from EffectTrigger a in db.EffectTriggers where a.Id == CurrentEffect.EffectTrigger select a.FireText).FirstOrDefault() as string;
                        WhenStr = EffectTriggersList.First(c => c.Id == CurrentEffect.EffectTrigger).FireText;
                        if (CurrentEffect.TriggerTarget != 0)
                        {
                            if (CurrentTrigger == null)
                            {
                                CurrentTrigger = (from EffectTarget et in db.EffectTargets where et.Id == CurrentEffect.TriggerTarget select et).FirstOrDefault();
                            }
                            if (CurrentTrigger != null)
                            {
                                if (CurrentTrigger.CardType != 0)
                                {
                                    string TargetType = CardTypesList.First(c => c.Id == CurrentTrigger.CardType).Name;
                                    if (CurrentTrigger.Plural == 1 && CurrentTrigger.Owner == 1)
                                    {
                                        TriggerStr = string.Format("Any {0}", TargetType);
                                    }
                                    else
                                    {

                                        TriggerStr = string.Format("{0} of {1} {2}{3}",
                                        (CurrentTrigger.Plural == 1) ? "One" : (CurrentTrigger.Plural == 2) ? "Any" : (CurrentTrigger.Plural == 3) ? "All" : "***",
                                        (CurrentTrigger.Owner == 1) ? "Any" : (CurrentTrigger.Owner == 2) ? "Your" : (CurrentTrigger.Owner == 3) ? "Opponent's" : "***",
                                        TargetType, (TargetType[TargetType.Length - 1] == 's') ? "es" : "s"
                                        );
                                    }
                                }

                            }
                        }
                        if (CurrentEffect.EffectTarget != 0)
                        {
                            if (CurrentTarget == null)
                            {
                                CurrentTarget = (from EffectTarget et in db.EffectTargets where et.Id == CurrentEffect.EffectTarget select et).FirstOrDefault();
                            }
                            if (CurrentTarget != null)
                            {
                                if (CurrentTarget.CardType != 0)
                                {
                                    string TargetType = CardTypesList.First(c => c.Id == CurrentTarget.CardType).Name;

                                    if (CurrentTarget.Plural == 1 && CurrentTarget.Owner == 1)
                                    {
                                        TargetStr = string.Format("Any {0}", TargetType);
                                    }
                                    else
                                    {
                                        TargetStr = string.Format("{0} of {1} {2}{3},",
                                            (CurrentTarget.Plural == 1) ? "One" : (CurrentTarget.Plural == 2) ? "Any" : (CurrentTarget.Plural == 3) ? "All" : "***",
                                            (CurrentTarget.Owner == 1) ? "Any" : (CurrentTarget.Owner == 2) ? "Your" : (CurrentTarget.Owner == 3) ? "Opponent's" : "***",
                                            TargetType, (TargetType[TargetType.Length - 1] == 's') ? "es" : "s"
                                            );
                                    }
                                }
                            }
                        }

                        WhenStr = WhenStr.Replace("*", TriggerStr);
                        if (CurrentEffect.Cost == 0 && CurrentEffect.Deplete == 0 && CurrentEffect.DiscardCount == 0 && CurrentEffect.SacrificeCount == 0)
                        {
                            WhenStr = WhenStr.Replace("When", "Whenever");
                        }

                    }

                    if (CurrentEffect.Effect != 0)
                    {
                        try
                        {
                            //ActStr = (from Effect e in db.Effects where e.Id == CurrentEffect.Effect select e.Name).FirstOrDefault();
                            ActStr = EffectsList.First(c => c.Id == CurrentEffect.Effect).Name;
                            ActStr = string.Format(ActStr, TargetStr, CurrentEffect.Value1, CurrentEffect.Value2);
                            if (CurrentTarget.Plural == 3)// multiple
                            {
                                ActStr = ActStr.Replace("(s)", "");
                                ActStr = ActStr.Replace("(es)", "");
                            }
                            else
                            {
                                ActStr = ActStr.Replace("(s)", "s");
                                ActStr = ActStr.Replace("(es)", "s");
                            }
                            if (CurrentEffect.Value1 >1)// multiple
                            {
                                ActStr = ActStr.Replace("(-s)", "s");
                            }
                            else
                            {
                                ActStr = ActStr.Replace("(-s)", "");
                            }
                        }
                        catch (Exception)
                        {
                            ActStr = "<UNABLE TO LOAD EFFECT.NAME>";
                        }

                        switch (CurrentEffect.Duration)
                        {
                            case 0:
                                DurationStr = "";
                                break;
                            case 1:
                                DurationStr = " until End of Turn";
                                break;
                            default:
                                DurationStr = string.Format(" for {0} rounds", CurrentEffect.Duration);
                                break;
                        }

                    }


                    CurrentEffect.EffectText = CostStr + " : " + WhenStr + ", " + ActStr + DurationStr +".";
                }
                catch (Exception)
                {
                    CurrentEffect.EffectText = "** ERROR BUILDING EFFECTTEXT **";
                }
            }
        }

    }
}
