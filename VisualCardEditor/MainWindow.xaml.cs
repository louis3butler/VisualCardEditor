using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Data.Entity;
using System.ComponentModel;
using System.Globalization;
using PropertyChanged;

namespace VisualCardEditor
{
    [ImplementPropertyChanged]
    public class TabSaveStatus
    {
        bool cardHasChanges;
        bool cardEffectHasChanges;
        public bool CardHasChanges
        {
            get
            {
                return cardHasChanges;
            }

            set
            {
                cardHasChanges = value;
            }
        }
        public bool CardEffectHasChanges
        {
            get
            {
                return cardEffectHasChanges;
            }

            set
            {
                cardEffectHasChanges = value;
            }
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point dragStart;
        ChampionsDB db;
        Card CurrentCard = new Card();
        CardEffect CurrentEffect = new CardEffect();
        TabSaveStatus TabStatus = new TabSaveStatus();

        ObservableCollection<Card> CardsList;
        ObservableCollection<CardType> CardTypesList;
        ObservableCollection<CardEffect> CardEffectsList;

        #region Initialization
        public MainWindow()
        {
            db = new ChampionsDB();
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainTabControl.DataContext = TabStatus;
            FillCardsList();
            FillCardTypesList();
            FillCardEffectsList();

            if (CardsList.Count() == 0)
            {
                AddCard("<New Card>");
                FillCardsList();
            }

            if (CardEffectsList.Count()==0)
            {
                AddCardEffect("<New Card Effect");
                FillCardEffectsList();
            }

            cardsListView.SelectedIndex = 0;
            cardTypesListView.SelectedIndex = 0;
            cardEffectsListView.SelectedIndex = 0;

            SetupCardGrid(CardsList[0]);
            SetupCardEffectsGrid(CardEffectsList[0]);

            LoadTriggersList();
            LoadEffectsList();
            LoadTargetsList();

            TabStatus.CardHasChanges = false;
            TabStatus.CardEffectHasChanges = false;

        }

        #endregion Initialization

        #region LISTFILL // List Population
        private void FillCardsList()
        {
            var result = db.Cards.OrderBy(c => c.Name);
            CardsList = new ObservableCollection<Card>(result);
            cardsListView.DataContext = CardsList;
        }
        private void FillCardTypesList()
        {
            var result = db.CardTypes.OrderBy(c => c.Name);
            CardTypesList = new ObservableCollection<CardType>(result);
            cardTypesListView.DataContext = CardTypesList;
        }
        private void FillCardEffectsList()
        {
            var result = db.CardEffects.OrderBy(c => c.Name);
            CardEffectsList = new ObservableCollection<CardEffect>(result);
            cardEffectsListView.DataContext = CardEffectsList;
        }
        #endregion LISTFILL
        
        #region DragDrop
        private void TypeBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("cardtypeitem"))
            {
                var obj = e.Data.GetData("cardtypeitem");
                if (obj.GetType() == typeof(int))
                {
                    int i = Convert.ToInt32(obj);
                    try
                    {
                        CardTypesJoin ctj = db.CardTypesJoins.Create();
                        ctj.Card = CurrentCard.Id;
                        ctj.CardType = i;
                        db.Entry(ctj).State = System.Data.Entity.EntityState.Added;
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Failed to load Card Types: {0}", ex.ToString()));
                    }
                    BuildCardTypesJoinStr();
                }
                TabStatus.CardHasChanges = true;
            }
        }

        private void DiscardTypeBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("cardtypeitem"))
            {
                var obj = e.Data.GetData("cardtypeitem");
                if (obj.GetType() == typeof(int))
                {
                    CurrentCard.DiscardType = Convert.ToInt32(obj);
                }
                TabStatus.CardHasChanges = true;
            }
        }

        private void Type_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("cardtypeitem") || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private static T FindAnchestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            while (current != null);
            return null;
        }

        private void cardTypesListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragStart = e.GetPosition(null);
        }

        private void cardTypesListView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = dragStart - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed && (
                Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the dragged ListViewItem
                ListView listView = sender as ListView;
                ListViewItem listViewItem =
                    FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);

                // Find the data behind the ListViewItem
                if (listViewItem != null)
                {
                    CardType c = listViewItem.Content as CardType;

                    int i = c.Id;
                    // Initialize the drag & drop operation
                    DataObject dragData = new DataObject("cardtypeitem", i);
                    DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
                }
            }
        }

        private void SacrificeTypeBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("cardtypeitem"))
            {
                var obj = e.Data.GetData("cardtypeitem");
                if (obj.GetType() == typeof(int))
                {
                    int i = Convert.ToInt32(obj);
                    CurrentCard.SacrificeType = i;
                }
                TabStatus.CardHasChanges = true;

            }
        }

        private void EffectsBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("cardeffectsitem"))
            {
                var obj = e.Data.GetData("cardeffectsitem");
                if (obj.GetType() == typeof(int))
                {
                    int i = Convert.ToInt32(obj);
                    try
                    {
                        CardEffectsJoin cej = db.CardEffectsJoins.Create();
                        cej.Card = CurrentCard.Id;
                        cej.CardEffect = i;
                        db.Entry(cej).State = System.Data.Entity.EntityState.Added;
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format("Failed to load Card Effects: {0}", ex.ToString()));
                    }
                    BuildCardEffectsJoinStr();
                }
                TabStatus.CardHasChanges = true;
            }
        }

        private void EffectsBox_DragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("cardeffectsitem") || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void cardEffectsListView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = dragStart - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed && (
                Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                // Get the dragged ListViewItem
                ListView listView = sender as ListView;
                ListViewItem listViewItem =
                    FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);

                // Find the data behind the ListViewItem
                if (listViewItem != null)
                {
                    //                    System.Data.DataRowView temp = listView.ItemContainerGenerator.ItemFromContainer(listViewItem) as System.Data.DataRowView;
                    CardEffect c = listViewItem.Content as CardEffect;

                    int i = c.Id;
                    // Initialize the drag & drop operation
                    DataObject dragData = new DataObject("cardeffectsitem", i);
                    DragDrop.DoDragDrop(listViewItem, dragData, DragDropEffects.Move);
                }
            }
        }

        private void cardEffectsListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragStart = e.GetPosition(null);
        }

        private void ce_SacrificeTypeBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("cardtypeitem"))
            {
                var obj = e.Data.GetData("cardtypeitem");
                if (obj.GetType() == typeof(int))
                {
                    int i = Convert.ToInt32(obj);
                    CurrentEffect.SacrificeType = i;
                }
                CurrentEffect.BuildEffectText();
                TabStatus.CardEffectHasChanges = true;
            }
        }

        private void ce_DiscardTypeBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("cardtypeitem"))
            {
                var obj = e.Data.GetData("cardtypeitem");
                if (obj.GetType() == typeof(int))
                {
                    CurrentEffect.DiscardType = Convert.ToInt32(obj);
                }
                CurrentEffect.BuildEffectText();
                TabStatus.CardEffectHasChanges = true;
            }
        }

        #endregion DragDrop

        #region StringBuild
        private void BuildCardTypesJoinStr()
        {
            int lastgroup = 0;
            var resultList =
                from ctj in db.CardTypesJoins
                join CardType ct in db.CardTypes
                on ctj.CardType equals ct.Id
                where ctj.Card == CurrentCard.Id
                orderby ct.TypeGroup, ct.Name
                select new { ct.Name, ct.TypeGroup, ctj.Card, ctj.CardType };
            TypeBox.Inlines.Clear();
            if (resultList.Count() == 0)
            {
                TypeBox.Text = "* Drag Card Types Here *";
                return;
            }
            foreach (var a in resultList)
            {
                Hyperlink hl = new Hyperlink(new Run(a.Name))
                {
                    NavigateUri = new Uri(string.Format("{0},{1}", a.Card, a.CardType), UriKind.Relative),
                };
                hl.Click += remove_ctj;
                if (lastgroup == a.TypeGroup)
                {
                    TypeBox.Inlines.Add(", ");
                }
                if (lastgroup == 1 && a.TypeGroup == 2)
                {
                    TypeBox.Inlines.Add(" - ");
                }
                TypeBox.Inlines.Add(hl);
                lastgroup = a.TypeGroup;
            }
        }
        private void remove_ctj(object sender, RoutedEventArgs e)
        {
            var link = (Hyperlink)sender;
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            string[] s = link.NavigateUri.ToString().Split(delimiterChars);
            int cardnum = Convert.ToInt32(s[0]);
            int cardtypenum = Convert.ToInt32(s[1]);
            CardTypesJoin ctj = (from CardTypesJoin tmp in db.CardTypesJoins where tmp.Card == cardnum && tmp.CardType == cardtypenum select tmp).FirstOrDefault();
            db.Entry(ctj).State = System.Data.Entity.EntityState.Deleted;
            db.SaveChanges();
            BuildCardTypesJoinStr();
            TabStatus.CardHasChanges = true;
        }

        private void BuildCardEffectsJoinStr()
        {
            var resultList =
                from cej in db.CardEffectsJoins
                join CardEffect ce in db.CardEffects
                on cej.CardEffect equals ce.Id
                where cej.Card == CurrentCard.Id
                orderby ce.Effect
                select new { ce.EffectText, cej.CardEffect };
            EffectsBox.Inlines.Clear();
            if (resultList.Count() == 0)
            {
                EffectsBox.Text = "* Drag Effects Here *";
                return;
            }
            foreach (var a in resultList)
            {
                Hyperlink hl = new Hyperlink(new Run(a.EffectText))
                {
                    NavigateUri = new Uri(a.CardEffect.ToString(), UriKind.Relative),
                };
                hl.Click += remove_cej;
                EffectsBox.Inlines.Add(hl);
                EffectsBox.Inlines.Add("\n");
            }
        }
        private void remove_cej(object sender, RoutedEventArgs e)
        {
            var link = (Hyperlink)sender;
            int cardnum = CurrentCard.Id;
            int cardeffectsnum = Convert.ToInt32(link.NavigateUri.ToString());
            using (ChampionsDB db = new ChampionsDB())
            {
                CardEffectsJoin ctj = (from CardEffectsJoin tmp in db.CardEffectsJoins where tmp.Card == cardnum && tmp.CardEffect == cardeffectsnum select tmp).FirstOrDefault();
                db.Entry(ctj).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
            BuildCardEffectsJoinStr();
            TabStatus.CardHasChanges = true;
        }

        #endregion StringBuild

        #region AddDelete

        private Card AddCard(string sNewName)
        {
            Card c = null;
            try
            {
                c = db.Cards.Create();
                c.Name = sNewName;
                db.Entry(c).State = EntityState.Added;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error adding card: \n" + e);
                return null;
            }
            return c;
        }
        private CardType AddCardType(string sNewName, int tg = 1)
        {
            CardType ct = null;
            try
            {
                ct = db.CardTypes.Create();
                ct.Name = sNewName;
                ct.TypeGroup = tg;
                db.Entry(ct).State = EntityState.Added;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error adding cardtype: \n" + e);
                return null;
            }
            return ct;
        }
        
        private CardEffect AddCardEffect(string sNewName)
        {
            CardEffect ce = null;
            try
            {
                ce = db.CardEffects.Create();
                ce.Name = sNewName;
                ce.EffectText = "";
                ce.Value = "";
                db.Entry(ce).State = EntityState.Added;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error adding cardeffect: \n" + e);
                return null;
            }
            return ce;
        }
        
        private int DeleteCard(Card c)
        {
            int id = c.Id;
            try
            {
                db.Entry(c).State = EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error deleting card: \n" + e);
                return 0;
            }
            return id;
        }
        private int DeleteCardType(CardType ct)
        {
            int id = ct.Id;
            try
            {
                db.Entry(ct).State = EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error deleting cardtype: \n" + e);
                return 0;
            }
            return id;
        }

        private int DeleteCardEffect(CardEffect ce)
        {
            int id = ce.Id;
            try
            {
                db.Entry(ce).State = EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error deleting cardeffect: \n" + e);
                return 0;
            }
            return id;
        }

        #endregion AddDelete
        
        #region EventHandlers_SelectionChanged
        private void cardsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        
        private void ce_effectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentEffect.BuildEffectText();
            TabStatus.CardEffectHasChanges = true;
        }

        private void cardLevelTextBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabStatus.CardHasChanges = true;
        }

        private void cardEffectsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ce_targetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentEffect.BuildEffectText();
            TabStatus.CardEffectHasChanges = true;
        }

        private void ce_triggerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentEffect.BuildEffectText();
            TabStatus.CardEffectHasChanges = true;
        }

        private void ce_conditionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentEffect.BuildEffectText();
            TabStatus.CardEffectHasChanges = true;
        }

        #endregion EventHandlers_SelectionChanged

        #region DropDown_Fillers
        private void LoadEffectsList()
        {
            ce_effectComboBox.Items.Clear();
            ComboBoxItem cbi = new ComboBoxItem();
            var result = from Effect eff in db.Effects orderby eff.Name select eff;
            foreach (Effect e in result)
            {
                cbi = new ComboBoxItem();

                cbi.Tag = e.Id;
                cbi.Content = e.ShortName;
                ce_effectComboBox.Items.Add(cbi);
            }
        }
        private void LoadConditionsList()
        {
            ce_effectComboBox.Items.Clear();
            ComboBoxItem cbi = new ComboBoxItem();
            var result = from Condition co in db.Conditions orderby co.ConditionName select co;
            foreach (Condition co in result)
            {
                cbi = new ComboBoxItem();

                cbi.Tag = co.Id;
                cbi.Content = co.ConditionName;
                ce_effectComboBox.Items.Add(cbi);
            }
        }
        private void LoadTriggersList()
        {
            ce_triggerComboBox.Items.Clear();
            ComboBoxItem cbi;

            var result = from EffectTrigger trig in db.EffectTriggers orderby trig.FireWhen select trig;
            foreach (EffectTrigger t in result)
            {
                cbi = new ComboBoxItem();
                cbi.Tag = t.Id;
                cbi.Content = t.FireText;
                ce_triggerComboBox.Items.Add(cbi);
            }
        }
        private void LoadTargetsList()
        {
            ce_targetComboBox.Items.Clear();
            ce_triggerTargetComboBox.Items.Clear();
            ComboBoxItem cbi;

            var result = from Target tgt in db.Targets orderby tgt.Owner, tgt.CardType select tgt;
            foreach (Target t in result)
            {
                cbi = new ComboBoxItem();
                cbi.Tag = t.Id;
                cbi.Content = t.Name;
                ce_targetComboBox.Items.Add(cbi);
            }
            foreach (Target t in result)
            {
                cbi = new ComboBoxItem();
                cbi.Tag = t.Id;
                cbi.Content = t.Name;
                ce_triggerTargetComboBox.Items.Add(cbi);
            }
        }

        #endregion DropDown_Fillers

        #region FormSetup
        private void SetupCardEffectsGrid(CardEffect tgt)
        {
            var result = (from CardEffect lookup in db.CardEffects where lookup.Id == tgt.Id select lookup).FirstOrDefault();
            CardEffect tmp = result as CardEffect;
            tmp.CopyTo(CurrentEffect);
            CardEffectEditGrid.DataContext = CurrentEffect;
            BuildCardEffectsJoinStr();
            CurrentEffect.BuildEffectText();
            TabStatus.CardEffectHasChanges = false;
        }
        private void SetupCardGrid(Card tgt)
        {
            var result = (from Card lookup in db.Cards where lookup.Id == tgt.Id select lookup).FirstOrDefault();
            Card tmp = result as Card;
            tmp.CopyTo(CurrentCard);
            CardEditGrid.DataContext = CurrentCard;
            BuildCardTypesJoinStr();
            TabStatus.CardHasChanges = false;
        }
        #endregion FormSetup

        #region LoadSave
        private void SaveCard(object sender, RoutedEventArgs e)
        {
            var result = (from Card lookup in db.Cards where lookup.Id == CurrentCard.Id select lookup).FirstOrDefault();
            Card temp = result as Card;
            CurrentCard.CopyTo(temp);
            db.Entry(temp).State = EntityState.Modified;
            db.SaveChanges();
            TabStatus.CardHasChanges = false;
        }
        private void SaveCardEffect(object sender, RoutedEventArgs e)
        {
            var result = (from CardEffect lookup in db.CardEffects where lookup.Id == CurrentEffect.Id select lookup).FirstOrDefault();
            CardEffect temp = result as CardEffect;
            CurrentEffect.CopyTo(temp);
            db.Entry(temp).State = EntityState.Modified;
            db.SaveChanges();
            TabStatus.CardEffectHasChanges = false;
        }
        #endregion LoadSave

        #region EventHandlers_Change
        private void CardTextElementChanged(object sender, TextChangedEventArgs e)
        {
            TabStatus.CardHasChanges = true;
        }
        
        private void CardEffectTextElementChanged(object sender, TextChangedEventArgs e)
        {
            CurrentEffect.BuildEffectText();
            TabStatus.CardEffectHasChanges = true;
        }
        
        private void ce_DepleteCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CurrentEffect.BuildEffectText();
            TabStatus.CardEffectHasChanges = true;
        }
        #endregion

        #region EventHandlers_Buttons
        private void CardDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Card c = cardsListView.SelectedItem as Card;
            if (CardsList.Count < 2)
            {
                MessageBox.Show("Cannot delete the last card in the collection.");
                return;
            }
            int rid = DeleteCard(c);
            if (rid > 0)
            {
                FillCardsList();
                if (rid == CurrentCard.Id)
                {
                    SetupCardGrid(cardsListView.SelectedItem as Card);
                }
            }
        }

        private void CardAddButton_Click(object sender, RoutedEventArgs e)
        {
            SetupCardGrid(AddCard("* New Card *"));
            FillCardsList();
        }

        private void CardTypeDeleteButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void CardTypeAddButton_Click(object sender, RoutedEventArgs e)
        {
            AddCardTypeWindow CTW = new AddCardTypeWindow();
            CTW.ShowDialog();
            FillCardsList();
        }

        private void CardEffectsDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            CardEffect ce = cardEffectsListView.SelectedItem as CardEffect;
            if (MessageBox.Show("Are you sure", "Delete CardEffect", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                db.Entry(ce).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
                FillCardEffectsList();
            }
        }

        private void CardEffectsAddButton_Click(object sender, RoutedEventArgs e)
        {
            SetupCardEffectsGrid(AddCardEffect("* New CardEffect *"));
            FillCardEffectsList();
        }
        #endregion EventHandlers_Buttons

        #region EventHandlers_Combo
        private void DiscardCountCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentCard.DiscardCount == 0) CurrentCard.DiscardType = 0;
            TabStatus.CardHasChanges = true;
        }

        private void ce_durationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentEffect.BuildEffectText();
            TabStatus.CardEffectHasChanges = true;
        }

        private void SacrificeCountCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentCard.SacrificeCount == 0) CurrentCard.SacrificeType = 0;
            TabStatus.CardHasChanges = true;
        }

        #endregion EventHandlers_Combo
        private void cardsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TabStatus.CardHasChanges)
            {
                if (MessageBox.Show("Save Changes?", "Card data has unsaved changes", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {

                }
                else
                {
                    SaveCard(this, new RoutedEventArgs());
                }
            }
            SetupCardGrid(cardsListView.SelectedItem as Card);
        }
        private void cardEffectsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TabStatus.CardEffectHasChanges)
            {
                if (MessageBox.Show("Save Changes?", "Card Effect data has unsaved changes", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                }
                else
                {
                    SaveCardEffect(this, new RoutedEventArgs());
                }
            }

            SetupCardEffectsGrid(cardEffectsListView.SelectedItem as CardEffect);
        }

        private void ce_SacrificeCountCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentEffect.SacrificeCount == 0) CurrentEffect.SacrificeType = 0;
            CurrentEffect.BuildEffectText();
            TabStatus.CardEffectHasChanges = true;
        }

        private void ce_DiscardCountCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentEffect.DiscardCount == 0) CurrentEffect.DiscardType = 0;
            CurrentEffect.BuildEffectText();
            TabStatus.CardEffectHasChanges = true;
        }

        private void ce_triggerTargetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentEffect.BuildEffectText();
            TabStatus.CardEffectHasChanges = true;
        }
    }
}
