using Laba5and6.Models;
using Laba5and6.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Core.Reflection;
using Core.TableClasses;
using Laba5and6.Models;
using Laba5and6.ViewModels;
using Table = Core.TableClasses.Table;

namespace Laba5and6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainModel mainModel = new MainModel();
            DataContext = new MainWindowViewModel(mainModel);
            BtnDeleteTable.IsEnabled = false;
        }

        private void UpdateDataGrid(object sender, RoutedEventArgs e)
        {
            if (Explorer.SelectedItem is null)
                return;

            if (Explorer.SelectedItem is Table)
            {
                var table = ((Table)Explorer.SelectedItem);
                if (table.Data.Count == 0)
                {
                    var description = ReflectionBuilder.GetDescription(table);
                    var builder = new ReflectionBuilder("System." + table.ElementsType.Name, description);
                    table.Data.Add(builder.CreateEmptyInstance());
                }

                BtnDeleteTable.IsEnabled = true;
                Data.ItemsSource = table.Data;
                Description.ItemsSource = ((Table)Explorer.SelectedItem).Properties;

            }
            else
            {
                Data.ItemsSource = ((DataBase)Explorer.SelectedItem).Tables;
                BtnDeleteTable.IsEnabled = false;
            }
        }

        private void BtnDeleteTable_OnClick(object sender, RoutedEventArgs e)
        {
            var dataBases = ((MainWindowViewModel)DataContext).DataBases;
            foreach (var database in dataBases)
            {
                if (database.Tables.Contains(((Table)Explorer.SelectedItem)))
                    database.Tables.Remove(((Table)Explorer.SelectedItem));
            }

        }

        private void BtnCreateDb_OnClick(object sender, RoutedEventArgs e)
        {            
            if (DbName.Text.Length > 0)
            {
                var tableFactory = new TableFactory();
                var factory = new DataBaseFactory(tableFactory);
                var schema = new SchemaFile(new FileInfo(DbName.Text),
                    new Dictionary<FileInfo, Dictionary<string, Type>>());
                var database = factory.CreateInstance(schema);
                ((MainWindowViewModel)DataContext).DataBases.Add(database);
            }
            else
            {
                MessageBox.Show("Нет названия базы данных");
            }
        }
    }
}
