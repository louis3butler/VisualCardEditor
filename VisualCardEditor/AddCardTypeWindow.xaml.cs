using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VisualCardEditor
{
    /// <summary>
    /// Interaction logic for AddCardTypeWindow.xaml
    /// </summary>
    public partial class AddCardTypeWindow : Window
    {
        CardType ct;
        public AddCardTypeWindow()
        {
            ct = new CardType();
            ct.Name = "<Enter Name>";
            InitializeComponent();
            grid1.DataContext = ct;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (ChampionsDB db = new ChampionsDB())
            {

                var result = from CardType tmp in db.CardTypes where tmp.TypeGroup == 1 orderby tmp.Id select tmp;
                foreach (CardType a in result)
                {
                    ComboBoxItem cbi = new ComboBoxItem();
                    cbi.Tag = a.Id;
                    cbi.Content = a.Name;
                    typeGroupComboBox.Items.Add(cbi);
                }
            }


        }

        private void AddCardType_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddCardType_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (ChampionsDB db = new ChampionsDB())
                {
                    CardType a = db.CardTypes.Create();
                    a.Name = ct.Name;
                    a.TypeGroup = ct.TypeGroup;
                    db.Entry(a).State = System.Data.Entity.EntityState.Added;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Failed to add Card Type: {0}", ex.ToString()));
            }
            this.Close();
        }
    }
}
