using Core.TableClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace Laba5and6.Views
{
    public partial class NewTable : Window
    {
        public NewTable(ObservableCollection<DataBase> databases)
        {
            InitializeComponent();
            Properties = new ObservableCollection<TableProperty>();
            GridNewTable.ItemsSource = Properties;
            DataBases.ItemsSource = databases;
        }

        public ObservableCollection<TableProperty> Properties { get; set; }

        private void BtnAdd_OnClick(object sender, RoutedEventArgs e)
        {          
            if(TableName.Text.Length == 0)
            {
                MessageBox.Show("Не указано имя таблицы");
                return;
            }
            if(GridNewTable.Items.Count == 0)
            {
                MessageBox.Show("Не указаны поля");
                return;
            }
            if(DataBases.SelectedItem == null)
            {
                MessageBox.Show("Не выбрана база данных");
                return;
            }
            var database = (DataBase)DataBases.SelectedItem;
            var tableFactory = new TableFactory();
            var properties = MakeConfig(GridNewTable.Items);
            var tableName = TableName.Text;
            var path = database.File.DataBaseFile.FullName
                .Replace(database.ToString(), "") + tableName + ".csv";
            var tempDict = new Dictionary<FileInfo, Dictionary<string, Type>>() { { new FileInfo(path), properties } };

            var table = tableFactory.BuildTables(tempDict).First();
            database.Tables.Add(table);

        }

        private Dictionary<string, Type> MakeConfig(IEnumerable properties)
        {
            var lProperties = new List<TableProperty>();
            var config = new Dictionary<string, Type>();

            try
            {
                foreach (TableProperty property in properties)
                {
                    config.TryAdd(property.Name, Type.GetType("System." + property.PrType));
                }
            }
            catch
            {

            }

            return config;
        }
    }
}
