using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using SDE.ApplicationConfiguration;
using SDE.Core;
using SDE.Editor.Generic.Lists;
using SDE.Editor.Generic.UI.FormatConverters;
using SDE.Editor.Generic.YamlModel;
using TokeiLibrary.WPF.Styles;

namespace SDE.View.Dialogs
{
    /// <summary>
    /// Interaction logic for EvolutionDialog.xaml
    /// </summary>
    public partial class EvolutionDialog : TkWindow, IInputWindow
    {
        public delegate void ShowItemRequirementDialogDelegate(string jsonData);

        public ShowItemRequirementDialogDelegate OnShowItemRequirementDialog;

        private readonly bool _autoFill;
        private readonly List<TextBox> _boxes = new List<TextBox>();
        private readonly bool _partialFill;
        private readonly List<TextBlock> _previews = new List<TextBlock>();
        public List<PetEvolutionItemModel> m_Model = new List<PetEvolutionItemModel>();
        private List<SelectTupleProperty<int>> targetTupple=new List<SelectTupleProperty<int>>();
        public EvolutionDialog(string text) : this(text, false)
        {
        }

        public EvolutionDialog(string text, bool autoFill) 
            : base("Level edit", "cde.ico", SizeToContent.Height, ResizeMode.CanResize)
        {
            _autoFill = autoFill;
            InitializeComponent();
            Extensions.SetMinimalSize(this);
            int max = 10;

            m_Model = JsonConvert.DeserializeObject<List<PetEvolutionItemModel>>(text);
            if (m_Model == null)
                m_Model = new List<PetEvolutionItemModel>();
            //int numOfColumns = ((max - 1) / 10) + 1;
            //numOfColumns = numOfColumns > 3 ? 3 : numOfColumns;
            int numOfColumns = 3;


            //Width = 300 * numOfColumns;

            //for (int i = 0; i < numOfColumns; i++)
            //{
            //    _upperGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(-1, GridUnitType.Auto) });
            //}

            //for (int i = 0; i < max; i++)
            //{
            //    SelectTupleProperty<int> tupleItem = new SelectTupleProperty<int>();
            //    tupleItem.DBAttribute = ServerPetAttributes.Evolution;
            //    tupleItem.DBAttribute.AttachedObject = ServerDbs.Mobs;
            //    if (m_Model != null && m_Model.Count > 0)
            //    {
            //        tupleItem.DisplayTextBox.Text = m_Model[i].Target;
            //    }

            //    Button btn = new Button();
            //    btn.Content = "Item Req.";
            //    btn.Click += delegate (object sender, RoutedEventArgs args)
            //     {
            //         if (OnShowItemRequirementDialog != null)
            //             OnShowItemRequirementDialog(Text);
            //     };

            //    tupleItem.ContentGrid.SetValue(Grid.RowProperty, i % 10);
            //    btn.SetValue(Grid.RowProperty, i % 10);
            //    tupleItem.ContentGrid.SetValue(Grid.ColumnProperty, 0);
            //    btn.SetValue(Grid.ColumnProperty, 1);

            //    _upperGrid.Children.Add(tupleItem.ContentGrid);
            //    _upperGrid.Children.Add(btn);
            //}

            Width = 300 * numOfColumns;

            for (int i = 0; i < numOfColumns; i++)
            {
                _upperGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(-1, GridUnitType.Auto) });
                //_upperGrid.ColumnDefinitions.Add(new ColumnDefinition());
                //_upperGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(-1, GridUnitType.Auto) });
            }

            for (int i = 0; i < max; i++)
            {
                Label label = new Label();
                label.Content = "Evo. Target " + (i + 1);
                label.Padding = new Thickness(0);
                label.Margin = new Thickness(3);
                label.VerticalAlignment = VerticalAlignment.Center;

                label.SetValue(Grid.RowProperty, i % 10);
                label.SetValue(Grid.ColumnProperty, 0);

                _upperGrid.Children.Add(label);
            }

            for (int i = 0; i < max; i++)
            {
                //Label preview = new Label();
                //preview.Padding = new Thickness(0);
                //preview.Margin = new Thickness(3);
                //preview.VerticalAlignment = VerticalAlignment.Center;
                //preview.HorizontalAlignment = HorizontalAlignment.Right;

                //TextBlock preview2 = new TextBlock();
                //preview2.Padding = new Thickness(0);
                //preview2.Margin = new Thickness(7, 6, 0, 6);
                //preview2.TextAlignment = TextAlignment.Left;
                //preview2.IsHitTestVisible = false;
                //preview2.VerticalAlignment = VerticalAlignment.Center;
                //preview2.Foreground = Brushes.DarkGray;

                TextBox box = new TextBox();
                box.Margin = new Thickness(3, 6, 3, 6);
                box.VerticalAlignment = VerticalAlignment.Center;

                Button btn = new Button();
                btn.Content = "Item Req.";
                btn.Click += delegate (object sender, RoutedEventArgs args)
                 {
                     OnShowItemRequirementDialog?.Invoke(Text);
                 };

                box.SetValue(Grid.RowProperty, i % 10);
                btn.SetValue(Grid.RowProperty, i % 10);
                //preview2.SetValue(Grid.RowProperty, i % 10);
                //preview.SetValue(Grid.RowProperty, i % 10);

                box.SetValue(Grid.ColumnProperty, 1);
                btn.SetValue(Grid.ColumnProperty, 2);
                //preview2.SetValue(Grid.ColumnProperty, (i / 10) * 3 + 1);
                //preview.SetValue(Grid.ColumnProperty, (i / 10) * 3 + 2);

                _upperGrid.Children.Add(box);
                _upperGrid.Children.Add(btn);

                box.KeyDown += delegate
                {
                    if (Keyboard.IsKeyDown(Key.Enter))
                    {
                        if (!SdeAppConfiguration.UseIntegratedDialogsForLevels)
                            DialogResult = true;

                        Close();
                    }
                };

                box.GotKeyboardFocus += delegate
                {
                    if (Keyboard.IsKeyDown(Key.Tab))
                        box.SelectAll();
                };

                if (m_Model.Count > 0)
                {
                    if (i < m_Model.Count)
                        box.Text = JsonConvert.SerializeObject(m_Model[i]);
                    else
                        box.Text = "{}";
                }
                else
                {
                    box.Text = "{}";
                }

                _boxes.Add(box);

                box.TextChanged += delegate
                {
                    OnValueChanged();
                };
            }

            //_updatePreviews();

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
