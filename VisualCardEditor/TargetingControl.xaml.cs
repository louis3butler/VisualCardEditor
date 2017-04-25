using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VisualCardEditor
{
    /// <summary>
    /// Interaction logic for TargetingControl.xaml
    /// </summary>
[ImplementPropertyChanged]
    public partial class TargetingControl : UserControl
    {
        public event RoutedEventHandler InfoChanged;

        public TargetingControl()
        {
            InitializeComponent();
        }
        /*
        public static readonly DependencyProperty PluralProperty =
            DependencyProperty.Register("Plural", typeof(int),
            typeof(TargetingControl), new PropertyMetadata(""));

        public static readonly DependencyProperty OwnerProperty =
            DependencyProperty.Register("Owner", typeof(int),
            typeof(TargetingControl), new PropertyMetadata(""));

        public static readonly DependencyProperty CardTypeProperty =
            DependencyProperty.Register("CardType", typeof(int),
            typeof(TargetingControl), new PropertyMetadata(""));
            */
        private void TypeBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("cardtypeitem"))
            {
                var obj = e.Data.GetData("cardtypeitem");
                if (obj.GetType() == typeof(int))
                {
                    BindingExpression bindingExpression = CardTypeBox.GetBindingExpression(TextBlock.TextProperty);
                    EffectTarget et = bindingExpression.DataItem as EffectTarget;
                    et.CardType = Convert.ToInt32(obj);
                    if (InfoChanged != null) InfoChanged(sender, e);
                }
            }
        }

        private void Type_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("cardtypeitem") || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InfoChanged != null) InfoChanged(sender, e);
        }
    }
}
