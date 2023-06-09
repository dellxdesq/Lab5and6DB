using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using Core.Loggers;
using Core.Managers;
using Core.TableClasses;
using Core.Testers;
using Laba5and6.Views;
using Microsoft.Win32;

namespace Laba5and6.Models;

public class MainModel
{
    public MainModel()
    {
        DataBases = new ObservableCollection<DataBase>();
        DbFactory = new DataBaseFactory(new TableFactory());
        Tester = new FileTester(Logger.GetInstance());
    }
    
    public ObservableCollection<DataBase> DataBases { get; set; }
    public DataBaseFactory DbFactory { get; set; }
    public FileTester Tester { get; set; }

    public bool AddDataBase()
    {
        var file = OpenDataBaseFile();
        if (file == null) 
            return false;

        var database = CreateDataBase(file);
        
        if (database is null)
            return false;
        
        DataBases.Add(database);
        return true;
    }

    public void ReOpenDataBases(List<FileInfo> databaseFiles)
    {
        foreach (var file in databaseFiles)
        {
            var database = CreateDataBase(file);
            if (database is null)
                continue;
        
            DataBases.Add(database);
        }
    }

    private FileInfo? OpenDataBaseFile()
    {
        var fileDialog = new OpenFileDialog();
        if (fileDialog.ShowDialog() == true)
        { 
            return new FileInfo(fileDialog.FileName);
        }

        return null;
    }
    
    private DataBase? CreateDataBase(FileInfo dataBaseFile)
    {
        Tester.Test(dataBaseFile, out var schemaFile);

        foreach (var message in Logger.messages)
        {
            MessageBox.Show(message);
        }
        
        if(schemaFile is not null)
            return DbFactory.CreateInstance(schemaFile);

        return null;
    }

    public void Save()
    {
        foreach (var database in DataBases)
        {
            FileManager.Save(database);
        }

        var databaseFiles = new List<FileInfo>();
        foreach (var database in DataBases)
        {
            databaseFiles.Add(database.File.DataBaseFile);
        }
        
        DataBases.Clear();
        ReOpenDataBases(databaseFiles);
    }

    public void CreateTable()
    {
        var createTableWindow = new NewTable(DataBases);
        createTableWindow.Show();
    }
}