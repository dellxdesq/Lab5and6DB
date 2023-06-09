using System.Data.Common;
using System.Text.Json;
using Core.Reflection;
using Core.TableClasses;

namespace Core.Managers;

public static class FileManager
{
    public static List<List<T>> ReadFileData<T>(this FileInfo file)
    {
        var fileData = new List<List<T>>();
        
        if (file.Exists)
        {
            var fileRows = File.ReadAllLines(file.FullName);
            foreach (var str in fileRows)
            {
                var list = new List<T>();
                var splitString = str.Split(",");
                
                foreach (var item in splitString)
                {
                    list.Add((T)Convert.ChangeType(item, typeof(T)));
                }
                
                fileData.Add(list);
            }
        }

        return fileData;
    }

    public static void Save(this DataBase dataBase)
    {
        SaveConfig(dataBase);
        foreach (var table in dataBase.Tables)
        {
            SaveTable(table);
        }
    }

    private static void SaveTable(Table table)
    {
        var data = new List<string>();
        using var writer = new StreamWriter(table.File.FullName);
        
        foreach (var element in table.Data)
        {
            var properties = from property in ReflectionBuilder.GetProperties(element)
                select property.ToString();

            ManageColumnCsv(properties, table, writer);
        }
    }

    private static void ManageColumnCsv(IEnumerable<string> properties, Table table, StreamWriter writer)
    {
        var strProperties = String.Join(",", properties);
        var numberOfColumns = strProperties.Count(c => c == ',') + 1;

        if (numberOfColumns <= table.CountNotNullProperties())
        {
            writer.Write(strProperties);
            
            for (var i = 0; i < table.CountNotNullProperties() - numberOfColumns ; i++)
            {
                writer.Write(',');
            }
            
            writer.WriteLine();
        }
        else
        {
            var listProps = properties.ToList();
            for (var i = 0; i < numberOfColumns - table.CountNotNullProperties(); i++)
            {
                listProps.RemoveAt(listProps.Count - 1);
            }

            writer.WriteLine(String.Join(",", listProps));
        }
    }


    private static void SaveConfig(DataBase database)
    {
        var description = MakeStringConfig(database);
        var strConfig = JsonSerializer.Serialize(description);

        using var writer = new StreamWriter(database.File.DataBaseFile.FullName);
        writer.WriteLine(strConfig);
    }

    private static Dictionary<string, Dictionary<string, string>> MakeStringConfig(DataBase database)
    {
        var description = new Dictionary<string, Dictionary<string, string>>();

        foreach (var table in database.Tables)
        {
            var strMetaData = new Dictionary<string, string>();

            foreach (var property in table.Properties)
            {
                if(!property.IsNullOrEmpty())
                    strMetaData.Add(property.Name, "System." + property.PrType);
            }
            description.Add(table.File.FullName, strMetaData);
        }

        return description;
    }
}