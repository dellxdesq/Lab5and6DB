using System.Collections.ObjectModel;
using System.Windows.Input;
using Core.TableClasses;
using Laba5and6.Models;

namespace Laba5and6.ViewModels;

public class MainWindowViewModel
{
    private readonly MainModel _model;
    private ICommand _addDataBaseCommand;
    private ICommand _saveCommand;
    private ICommand _createTableCommand;

    public ICommand AddDataBaseCommand
    {
        get
        {
            if (_addDataBaseCommand == null)
            {
                _addDataBaseCommand = new RelayCommand(
                    param => _model.AddDataBase(),
                    param => true
                    );
            }
            return _addDataBaseCommand;
        }
    }

    public ICommand SaveCommand
    {
        get
        {
            if (_saveCommand == null)
            {
                _saveCommand = new RelayCommand(
                    param => _model.Save(),
                    param => true
                );
            }
            return _saveCommand;
        }
    }

    public ICommand CreateTableCommand
    {
        get
        {
            if (_createTableCommand == null)
            {
                _createTableCommand = new RelayCommand(
                    param => _model.CreateTable(),
                    param => true
                    );
            }

            return _createTableCommand;
        }
    }

    public MainWindowViewModel(MainModel model)
    {
        _model = model;
        DataBases = _model.DataBases;
    }

    public ObservableCollection<DataBase> DataBases { get; set; }
}