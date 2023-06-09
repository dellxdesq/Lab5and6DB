namespace Core.TableClasses;

public class DataBaseFactory
{
    private TableFactory _tableFactory;
    
    public DataBaseFactory(TableFactory tableFactory)
    {
        _tableFactory = tableFactory;
    }

    public DataBase CreateInstance(SchemaFile dataBaseConfig)
    {
        var tables = _tableFactory.BuildTables(dataBaseConfig.TablesDescription);
        return new DataBase(dataBaseConfig, tables);
    }
}