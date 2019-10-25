using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Database;
using ErrorManager;
using Newtonsoft.Json;
using SDE.ApplicationConfiguration;
using SDE.Core;
using SDE.Editor.Engines.TabNavigationEngine;
using SDE.Editor.Generic;
using SDE.Editor.Generic.Lists;
using SDE.Editor.Generic.UI.FormatConverters;
using SDE.Editor.Generic.YamlModel;
using TokeiLibrary;
using TokeiLibrary.WPF.Styles;

namespace SDE.View.Dialogs
{
    /// <summary>
    /// Interaction logic for ItemRequirementDialog.xaml
    /// </summary>
    public partial class ItemRequirementDialog : TkWindow, IInputWindow
    {
        private List<PetEvolutionItemRequirementItemModel> m_Model = new List<PetEvolutionItemRequirementItemModel>();
        List<TextBox> textBoxItems = new List<TextBox>();
        private List<TextBox> textBoxAmmounts = new List<TextBox>();
        private List<Button> btnContextMenus = new List<Button>();
        private MetaTable<int> itemTables;

        public ItemRequirementDialog(string jsonData, Window parentWindow=null)
            : base("Item Requirement edit", "cde.ico", SizeToContent.Height, ResizeMode.CanResize)
        {
            InitializeComponent();
            Extensions.SetMinimalSize(this);
            ShowInTaskbar = false;
            Topmost = true;
            Owner = parentWindow ?? Application.Current.MainWindow;//WpfUtilities.TopWindow;
            itemTables = SdeEditor.Instance.ProjectDatabase.GetMetaTable<int>(ServerDbs.Items);
            int max = 10;

            if (!string.IsNullOrEmpty(jsonData))
                m_Model = JsonConvert.DeserializeObject<List<PetEvolutionItemRequirementItemModel>>(jsonData);
            else
                m_Model = new List<PetEvolutionItemRequirementItemModel>();
            int numOfColumns = 5;
            Width = 100 * numOfColumns;

            for (int i = 0; i < numOfColumns; i++)
            {
                _upperGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(-1, GridUnitType.Auto) });
                //_upperGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(-1, GridUnitType.Auto) });
                //_upperGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(-1, GridUnitType.Auto) });
                //_upperGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(-1, GridUnitType.Auto) });
            }
            for (int i = 0; i < max; i++)
            {
                Label label = new Label();
                label.Content = "Item " + (i + 1);
                label.Padding = new Thickness(0);
                label.Margin = new Thickness(3);
                label.VerticalAlignment = VerticalAlignment.Center;

                TextBox boxItem = new TextBox();
                boxItem.Margin = new Thickness(3, 6, 3, 6);
                boxItem.VerticalAlignment = VerticalAlignment.Center;
                boxItem.Width = 200;

                Button btnContextMenu = new Button();
                btnContextMenu.Width = 22;
                btnContextMenu.Height = 22;
                btnContextMenu.Margin = new Thickness(0, 3, 3, 3);
                btnContextMenu.Content = new Image { Source = ApplicationManager.PreloadResourceImage("arrowdown.png"), Stretch = Stretch.None };
                btnContextMenu.ContextMenu = new ContextMenu();
                btnContextMenu.ContextMenu.Placement = PlacementMode.Bottom;
                btnContextMenu.ContextMenu.PlacementTarget = btnContextMenu;
                //btnContextMenu.PreviewMouseRightButtonUp += _disableButton;
                btnContextMenu.Tag = i;
                btnContextMenu.Click += btnContextMenu_Clicked;

                MenuItem menuItem = new MenuItem();
                menuItem.Header = "Select ''";
                menuItem.Icon = new Image { Source = ApplicationManager.PreloadResourceImage("find.png"), Stretch = Stretch.Uniform, Width = 16, Height = 16 };
                menuItem.Click += menuItem_onClick;
                menuItem.Tag = i;

                MenuItem selectFromList = new MenuItem();
                selectFromList.Header = "Select...";
                selectFromList.Icon = new Image { Source = ApplicationManager.PreloadResourceImage("treeList.png"), Stretch = Stretch.None };
                selectFromList.Click += _selectFromList_Click;
                selectFromList.Tag = i;

                btnContextMenu.ContextMenu.Items.Add(menuItem);
                btnContextMenu.ContextMenu.Items.Add(selectFromList);
                ;
                Label label2 = new Label();
                label2.Content = "Amount " + (i + 1);
                label2.Padding = new Thickness(0);
                label2.Margin = new Thickness(3);
                label2.VerticalAlignment = VerticalAlignment.Center;

                TextBox boxAmount = new TextBox();
                boxAmount.Margin = new Thickness(3, 6, 3, 6);
                boxAmount.VerticalAlignment = VerticalAlignment.Center;
                boxAmount.Width = 50;

                label.SetValue(Grid.RowProperty, i % 10);
                boxItem.SetValue(Grid.RowProperty, i % 10);
                btnContextMenu.SetValue(Grid.RowProperty, i % 10);
                label2.SetValue(Grid.RowProperty, i % 10);
                boxAmount.SetValue(Grid.RowProperty, i % 10);

                label.SetValue(Grid.ColumnProperty, 0);
                boxItem.SetValue(Grid.ColumnProperty, 1);
                btnContextMenu.SetValue(Grid.ColumnProperty, 2);
                label2.SetValue(Grid.ColumnProperty, 3);
                boxAmount.SetValue(Grid.ColumnProperty, 4);

                if (m_Model.Count > 0)
                {
                    if (i < m_Model.Count)
                    {
                        boxItem.Text = m_Model[i].Item;
                        boxAmount.Text = m_Model[i].Amount.ToString();
                    }
                    else
                    {
                        boxItem.Text = "";
                        boxAmount.Text = "0";
                    }
                }
                else
                {
                    boxItem.Text = "";
                    boxAmount.Text = "0";
                }

                _upperGrid.Children.Add(label);
                _upperGrid.Children.Add(boxItem);
                _upperGrid.Children.Add(btnContextMenu);
                _upperGrid.Children.Add(label2);
                _upperGrid.Children.Add(boxAmount);

                
                textBoxItems.Add(boxItem);
                textBoxAmmounts.Add(boxAmount);
                btnContextMenus.Add(btnContextMenu);
            }
            //Button btnClose = new Button();
            //btnClose.Content = "Close";
            //btnClose.Click += delegate (object sender, RoutedEventArgs args)
            //{
            //    Close();
            //};

            //btnClose.SetValue(Grid.ColumnProperty, 3);
            //btnClose.SetValue(Grid.RowProperty, max % 10);
            //_upperGrid.Children.Add(btnClose);

            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private void _selectFromList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = (int)((MenuItem)sender).Tag;
                TextBox textbox = textBoxItems[index];

                Table<int, ReadableTuple<int>> btable = SdeEditor.Instance.ProjectDatabase.GetMetaTable<int>((ServerDbs)ServerDbs.Items);

                SelectFromDialog select = new SelectFromDialog(btable, (ServerDbs)ServerDbs.Mobs, textbox.Text, ServerItemAttributes.AegisName);
                select.ShowInTaskbar = false;
                select.Topmost = true;
                select.Owner = this;//WpfUtilities.TopWindow;

                if (select.ShowDialog() == true)
                {
                    textbox.Text = (string)select.ValueByAttribute;
                }
            }
            catch (Exception err)
            {
                ErrorHandler.HandleException(err);
            }
        }

        
        private void btnContextMenu_Clicked(object sender, RoutedEventArgs e)
        {
            Button _button = (Button)sender;
            int value = (int)_button.Tag;

            //((MenuItem)_button.ContextMenu.Items[0]).IsEnabled = Int32.TryParse(_boxes[value].Text, out value) && value > 0;
            int mobId;
            bool isExist = SDE.Editor.Utils.IsItemExistByAegisName<int>(itemTables, textBoxItems[value].Text, out mobId);
            ((MenuItem)_button.ContextMenu.Items[0]).IsEnabled = isExist;
            try
            {
                string val = "Unknown";

                ServerDbs sdb = (ServerDbs)ServerDbs.Items;

                MetaTable<int> table = SdeEditor.Instance.ProjectDatabase.GetMetaTable<int>(sdb);
                if (isExist)
                {
                    Tuple tuple = table.TryGetTuple(mobId);

                    if (tuple != null)
                    {
                        val = tuple.GetValue(table.AttributeList.Attributes.FirstOrDefault(p => p.IsDisplayAttribute) ?? table.AttributeList.Attributes[1]).ToString();
                    }

                    ((MenuItem)_button.ContextMenu.Items[1]).Header= String.Format("Select '{0}'", val);
                }

            }
            catch
            {
            }

            _button.ContextMenu.IsOpen = true;
        }
        private void menuItem_onClick(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            int value;
            Editor.Utils.IsMobExistBySpriteName<int>(itemTables, textBoxItems[(int)menuItem.Tag].Text, out value);

            //Int32.TryParse(_boxes[(int)menuItem.Tag].Text, out value);

            if (value <= 0)
                return;

            TabNavigation.Select((ServerDbs)ServerDbs.Mobs, value);
        }

        public List<PetEvolutionItemRequirementItemModel> ItemRequirement
        {
            get
            {
                List<PetEvolutionItemRequirementItemModel> itemReq = new List<PetEvolutionItemRequirementItemModel>();

                for (int i = 0; i < textBoxItems.Count; i++)
                {
                    if (!string.IsNullOrEmpty(textBoxItems[i].Text) && !string.IsNullOrEmpty(textBoxAmmounts[i].Text))
                    {
                        string aegisName = textBoxItems[i].Text;
                        int amount = 0;
                        if (int.TryParse(textBoxAmmounts[i].Text, out amount))
                        {
                            PetEvolutionItemRequirementItemModel itemList = new PetEvolutionItemRequirementItemModel();
                            itemList.Item = aegisName;
                            itemList.Amount = amount;
                            itemReq.Add(itemList);
                        }
                    }

                }

                return itemReq;
            }

        }
        public string Text
        {
            get
            {
                List<PetEvolutionItemRequirementItemModel> itemReq = ItemRequirement;

                if (itemReq.Count == 0)
                {
                    return "{}";
                }
                else
                {

                    return JsonConvert.SerializeObject(itemReq);
                }

            }
        }

        public Grid Footer { get { return _footerGrid; } }
        public event Action ValueChanged;

        public void OnValueChanged()
        {
            Action handler = ValueChanged;
            if (handler != null) handler();
        }

        
        protected override void GRFEditorWindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void _buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void _buttonOk_Click(object sender, RoutedEventArgs e)
        {
            //if (!SdeAppConfiguration.UseIntegratedDialogsForLevels)
            //    DialogResult = _boxes.Count != 0;
            DialogResult = true;
            Close();
        }
    }
}
