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
using Microsoft.Scripting.Ast;
using Newtonsoft.Json;
using SDE.ApplicationConfiguration;
using SDE.Core;
using SDE.Editor.Engines.TabNavigationEngine;
using SDE.Editor.Generic;
using SDE.Editor.Generic.Lists;
using SDE.Editor.Generic.TabsMakerCore;
using SDE.Editor.Generic.UI.FormatConverters;
using SDE.Editor.Generic.YamlModel;
using Tamir.SharpSsh.jsch;
using TokeiLibrary;
using TokeiLibrary.WPF.Styles;

namespace SDE.View.Dialogs
{
    /// <summary>
    /// Interaction logic for EvolutionDialog.xaml
    /// </summary>
    public partial class EvolutionDialog : TkWindow, IInputWindow
    {
        public delegate void ShowItemRequirementDialogDelegate(string jsonData, int index);

        public ShowItemRequirementDialogDelegate OnShowItemRequirementDialog;

        
        private readonly List<TextBox> _boxes = new List<TextBox>();
        private readonly List<MenuItem> _menuItems = new List<MenuItem>();
        private readonly bool _partialFill;
        public List<PetEvolutionItemModel> m_Model = new List<PetEvolutionItemModel>();
        
        private readonly List<MenuItem> _menuSelectfrom = new List<MenuItem>();
        private MetaTable<int> mobTable;
        public EvolutionDialog(string text) : this(text, false)
        {
        }

        public EvolutionDialog(string text, bool autoFill) 
            : base("Level edit", "cde.ico", SizeToContent.Height, ResizeMode.CanResize)
        {
            
            InitializeComponent();
            Extensions.SetMinimalSize(this);
            ShowInTaskbar = false;
            Topmost = true;
            Owner = Application.Current.MainWindow;//WpfUtilities.TopWindow;


            int max = 10;
            mobTable = SdeEditor.Instance.ProjectDatabase.GetMetaTable<int>(ServerDbs.Mobs);
            m_Model = JsonConvert.DeserializeObject<List<PetEvolutionItemModel>>(text);
            if (m_Model == null)
                m_Model = new List<PetEvolutionItemModel>();
            //int numOfColumns = ((max - 1) / 10) + 1;
            //numOfColumns = numOfColumns > 3 ? 3 : numOfColumns;
            int numOfColumns = 4;
            Width = 400;

            //Button btnClose = new Button();
            //btnClose.Content = "Close";
            //btnClose.Click+= delegate(object sender, RoutedEventArgs args)
            //{
            //    Close();
            //};

            for (int i = 0; i < numOfColumns; i++)
            {
                _upperGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(-1, GridUnitType.Auto) });
                //_upperGrid.ColumnDefinitions.Add(new ColumnDefinition());
                //_upperGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(-1, GridUnitType.Auto) });
            }

            //for (int i = 0; i <= max; i++)
            //{
            //    _upperGrid.RowDefinitions.Add(new RowDefinition() {Height = new GridLength(-1, GridUnitType.Auto)});
            //}
            for (int i = 0; i < max; i++)
            {
                Label label = new Label();
                label.Content = "Evo.Target " + (i + 1);
                label.Padding = new Thickness(0);
                label.Margin = new Thickness(3);
                label.VerticalAlignment = VerticalAlignment.Center;

                label.SetValue(Grid.RowProperty, i % 10);
                label.SetValue(Grid.ColumnProperty, 0);

                _upperGrid.Children.Add(label);
            }


            for (int i = 0; i < max; i++)
            {
                TextBox boxTarget = new TextBox();
                boxTarget.Margin = new Thickness(3, 6, 3, 6);
                boxTarget.VerticalAlignment = VerticalAlignment.Center;
                boxTarget.Width = 200;

                Button btnContextMenu = new Button();
                btnContextMenu.Width = 22;
                btnContextMenu.Height = 22;
                btnContextMenu.Margin = new Thickness(0, 3, 3, 3);
                btnContextMenu.Content = new Image { Source = ApplicationManager.PreloadResourceImage("arrowdown.png"), Stretch = Stretch.None };
                btnContextMenu.ContextMenu = new ContextMenu();
                btnContextMenu.ContextMenu.Placement = PlacementMode.Bottom;
                btnContextMenu.ContextMenu.PlacementTarget = btnContextMenu;
                btnContextMenu.PreviewMouseRightButtonUp += _disableButton;
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

                Button btn = new Button();
                btn.Content = "Item Req.";
                btn.Tag = i;

                btn.Click += delegate (object sender, RoutedEventArgs args)
                {
                    int indexTag = (int)btn.Tag;
                    if (indexTag < m_Model.Count)
                    {
                        string jsonValue = JsonConvert.SerializeObject(m_Model[indexTag].ItemRequirements);
                        OnShowItemRequirementDialog?.Invoke(jsonValue, indexTag);
                    }
                    else
                    {
                        OnShowItemRequirementDialog?.Invoke("{}", indexTag);
                    }
                };

               
                boxTarget.SetValue(Grid.RowProperty, i % 10);
                btnContextMenu.SetValue(Grid.RowProperty, i % 10);
                btn.SetValue(Grid.RowProperty, i % 10);

                boxTarget.SetValue(Grid.ColumnProperty, 1);
                btnContextMenu.SetValue(Grid.ColumnProperty, 2);
                btn.SetValue(Grid.ColumnProperty, 3);

                _upperGrid.Children.Add(boxTarget);
                _upperGrid.Children.Add(btnContextMenu);
                _upperGrid.Children.Add(btn);

                boxTarget.KeyDown += delegate
                {
                    if (Keyboard.IsKeyDown(Key.Enter))
                    {
                        if (!SdeAppConfiguration.UseIntegratedDialogsForLevels)
                            DialogResult = true;

                        Close();
                    }
                };
                boxTarget.GotKeyboardFocus += delegate
                {
                    if (Keyboard.IsKeyDown(Key.Tab))
                        boxTarget.SelectAll();
                };

                if (m_Model.Count > 0)
                {
                    if (i < m_Model.Count)
                        //boxTarget.Text = JsonConvert.SerializeObject(m_Model[i]);
                    {
                        boxTarget.Text = m_Model[i].Target;
                    }
                    else
                        boxTarget.Text = "";
                }
                else
                {
                    boxTarget.Text = "";
                }

                _boxes.Add(boxTarget);
                _menuItems.Add(menuItem);
                _menuSelectfrom.Add(selectFromList);

                boxTarget.TextChanged += delegate
                {
                    OnValueChanged();
                };
            }

            //btnClose.SetValue(Grid.ColumnProperty, 3);
            //btnClose.SetValue(Grid.RowProperty, max);
            //_upperGrid.Children.Add(btnClose);

            if (_boxes.Count > 0)
            {
                _boxes[0].Loaded += delegate
                {
                    Keyboard.Focus(_boxes[0]);
                    _boxes[0].SelectAll();
                };
            }
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

        }

        //protected GDbTabWrapper<int, ReadableTuple<int>> _tab;
        private void _selectFromList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int index = (int)((MenuItem) sender).Tag;
                TextBox textbox = _boxes[index];
                
                Table<int, ReadableTuple<int>> btable = SdeEditor.Instance.ProjectDatabase.GetMetaTable<int>((ServerDbs)ServerDbs.Mobs);

                SelectFromDialog select = new SelectFromDialog(btable, (ServerDbs)ServerDbs.Mobs, textbox.Text, ServerMobAttributes.SpriteName);
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
            Button _button = (Button) sender;
            int value = (int) _button.Tag;

            

            //((MenuItem)_button.ContextMenu.Items[0]).IsEnabled = Int32.TryParse(_boxes[value].Text, out value) && value > 0;
            int mobId;
            bool isExist = SDE.Editor.Utils.IsMobExistBySpriteName<int>(mobTable, _boxes[value].Text, out mobId);

            ((MenuItem) _button.ContextMenu.Items[0]).IsEnabled = isExist;
            try
            {
                string val = "Unknown";

                ServerDbs sdb = (ServerDbs)ServerDbs.Mobs;
                
                MetaTable<int> table = SdeEditor.Instance.ProjectDatabase.GetMetaTable<int>(sdb);
                if (isExist)
                {
                    Tuple tuple = table.TryGetTuple(mobId);

                    if (tuple != null)
                    {
                        val = tuple.GetValue(table.AttributeList.Attributes.FirstOrDefault(p => p.IsDisplayAttribute) ?? table.AttributeList.Attributes[1]).ToString();
                    }

                    MenuItem _select = _menuSelectfrom[value];
                    _select.Header = String.Format("Select '{0}'", val);
                }
                
            }
            catch
            {
            }

            _button.ContextMenu.IsOpen = true;
        }

        private void menuItem_onClick(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem) sender;
            
            int value;
            Editor.Utils.IsMobExistBySpriteName<int>(mobTable, _boxes[(int) menuItem.Tag].Text, out value);

            //Int32.TryParse(_boxes[(int)menuItem.Tag].Text, out value);

            if (value <= 0)
                return;

            TabNavigation.Select((ServerDbs)ServerDbs.Mobs, value);
        }

        private void _disableButton(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
        
        public string Text
        {
            get
            {
                if (m_Model.Count == 0)
                {
                    return "{}";
                }
                else
                {
                    return JsonConvert.SerializeObject(m_Model);
                }

            }
        }

        public void UpdateItemRequirementFromDialog(List<PetEvolutionItemRequirementItemModel> itemReqList, int index)
        {
            m_Model[index].ItemRequirements = new List<PetEvolutionItemRequirementItemModel>(itemReqList);
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
            //if (e.Key == Key.Escape)
            //    Close();
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
