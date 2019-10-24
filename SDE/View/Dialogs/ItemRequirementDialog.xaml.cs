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
    /// Interaction logic for ItemRequirementDialog.xaml
    /// </summary>
    public partial class ItemRequirementDialog : TkWindow, IInputWindow
    {
        private List<PetEvolutionItemRequirementItemModel> m_Model = new List<PetEvolutionItemRequirementItemModel>();
        List<TextBox> boxes = new List<TextBox>();
        
        public ItemRequirementDialog()
        {
            InitializeComponent();
            Extensions.SetMinimalSize(this);
            int max = 10;

            string jsonData = Text;
            if (string.IsNullOrEmpty(jsonData))
                m_Model = JsonConvert.DeserializeObject<List<PetEvolutionItemRequirementItemModel>>(jsonData);
            else
                m_Model = new List<PetEvolutionItemRequirementItemModel>();
            int numOfColumns = 4;
            Width = 300 * numOfColumns;

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

                SelectTupleProperty<int> tuple = new SelectTupleProperty<int>();
                tuple.DBAttribute = ServerPetAttributes.Evolution;
                tuple.DBAttribute.AttachedObject = ServerDbs.Items;
                if (m_Model != null && m_Model.Count > 0)
                    tuple.DisplayTextBox.Text = m_Model[i].Item;
                
                //TextBox box = new TextBox();
                //box.Margin = new Thickness(3, 6, 3, 6);
                //box.VerticalAlignment = VerticalAlignment.Center;
                //    ;
                Label label2 = new Label();
                label2.Content = "Amount " + (i + 1);
                label2.Padding = new Thickness(0);
                label2.Margin = new Thickness(3);
                label2.VerticalAlignment = VerticalAlignment.Center;

                TextBox box2 = new TextBox();
                box2.Margin = new Thickness(3, 6, 3, 6);
                box2.VerticalAlignment = VerticalAlignment.Center;

                label.SetValue(Grid.RowProperty, i % 10);
                tuple.SetValue(Grid.RowProperty, i % 10);
                //box.SetValue(Grid.RowProperty, i % 10);
                label2.SetValue(Grid.RowProperty, i % 10);
                box2.SetValue(Grid.RowProperty, i % 10);

                label.SetValue(Grid.ColumnProperty, 0);
                tuple.SetValue(Grid.ColumnProperty, 1);
                //box.SetValue(Grid.ColumnProperty, 1);
                label2.SetValue(Grid.ColumnProperty, 2);
                //box.SetValue(Grid.ColumnProperty, 3);

                _upperGrid.Children.Add(label);
                //_upperGrid.Children.Add(box);
                _upperGrid.Children.Add(tuple.ContentGrid);
                _upperGrid.Children.Add(label2);
                _upperGrid.Children.Add(box2);

                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
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
