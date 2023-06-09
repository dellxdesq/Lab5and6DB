using Core.Managers;
using Core.Reflection;

namespace Core.TableClasses;

public class TableFactory
{
    public List<Table> BuildTables(Dictionary<FileInfo, Dictionary<string, Type>> tableConfig)
    {
        var tables = new List<Table>();
        
        foreach (var (tableFile, tableMetadata) in tableConfig)
        {
            var builder = new ReflectionBuilder(tableFile.Name, tableMetadata);
            tables.Add(BuildTable(tableFile, tableMetadata, builder));
        }

        return tables;
    }

    public Table BuildTable(FileInfo tableFile, 
        Dictionary<string, Type> tableMetadata, 
        ReflectionBuilder builder)
    {
        var preparedData = new List<object>();
        var tableData = tableFile.ReadFileData<object>();
        var properties = MakeProperties(tableMetadata);

        foreach (var elementArray in tableData)
        {
            preparedData.Add(builder.CreateInstance(elementArray.ToArray()));
        }

        return new Table(tableFile, preparedData, builder.BuildedType, properties);
    }

    private List<TableProperty> MakeProperties(Dictionary<string, Type> tableConfig)
    {
        var properties = new List<TableProperty>();

        foreach (var (name, type) in tableConfig)
        {
            var property = new TableProperty();
            property.Set(name, type.Name);
            
            properties.Add(property);
        }

        return properties;
    }
}