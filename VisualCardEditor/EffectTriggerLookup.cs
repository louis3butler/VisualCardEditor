﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VisualCardEditor
{
    public class EffectTriggerLookup : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                if ((int)value == 0) return "";
                using (ChampionsDB db = new ChampionsDB())
                {
                    EffectTrigger et = db.EffectTriggers.Find(value);
                    if (et == null) return null;
                    return et.FireWhen;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                using (ChampionsDB db = new ChampionsDB())
                {
                    CardType ct = db.CardTypes.Where(a => a.Name.Equals((string)value)).FirstOrDefault();
                    if (ct == null) return 0;
                    return ct.Id;
                }
            }
            return 0;
        }
    }


}
