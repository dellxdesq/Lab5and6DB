using System.Text.Json;
using Core.Loggers;
using Core.Managers;
using Core.TableClasses;

namespace Core.Testers;

public class FileTester
{
    private ILogger _logger;
    
    public FileTester(ILogger logger)
    {
        _logger = logger;
    }

    public bool Test(FileInfo dataBaseFile, out SchemaFile? databaseDescription)
    {
        databaseDescription = null;

        if (!dataBaseFile.Exists)
        {
            _logger.Log("Db file does not exist.");
            return false;
        }

        var untestedConfig = ReadDataBaseFile(dataBaseFile);
        if (untestedConfig is null)
        {
            _logger.Log("Db file is invalid");
            return false;
        }

        var answer = TestConfig(untestedConfig,
            out var dataBaseConfig);

        foreach (var (tableFile, tableConfig) in dataBaseConfig)
        {
            answer = answer && TestTable(tableFile) && TestColumnDataTypeEquality(tableFile, tableConfig);
        }

        if (answer)
        {
            databaseDescription = new SchemaFile(dataBaseFile, dataBaseConfig);
        }

        return answer;
    }

    private Dictionary<string, Dictionary<string, string>>? ReadDataBaseFile(FileInfo dataBaseFile)
    {
        using StreamReader stream = new StreamReader(dataBaseFile.FullName);
        string json = stream.ReadToEnd();
        return JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json);
    }

    private bool TestConfig(Dictionary<string, Dictionary<string, string>> config,
        out Dictionary<FileInfo, Dictionary<string, Type>> dataBaseConfig)
    {
        var answer = true;
        dataBaseConfig = new Dictionary<FileInfo, Dictionary<string, Type>>();
        
        foreach (var (tableFilePath, tableMetadata) in config)
        {
            
            var file = new FileInfo(tableFilePath);
            answer = answer &&
                     TestMetaData(tableMetadata, out var metaData) && 
                     file.Exists &&
                     dataBaseConfig.TryAdd(file, metaData);
        }

        if (!answer)
        {
            _logger.Log("Problems with config.");
        }
        return answer;
    }

    private bool TestMetaData(Dictionary<string, string> tableMetaData, 
        out Dictionary<string, Type> metaData)
    {
        var answer = true;
        metaData = new Dictionary<string, Type>();
        
        foreach (var (columnName, dataType) in tableMetaData)
        {
            var type = Type.GetType(dataType);
            
            answer = answer && 
                     type is not null &&
                     columnName.Length >= 1 &&
                     metaData.TryAdd(columnName, type);
        }

        if (!answer)
        {
            _logger.Log("Problems with table metadata.");
        }
        return answer;
    }
    
    private bool TestTable(FileInfo tableFile)
    {
        if (!tableFile.Exists)
            return false;
        
        var fileData = File.ReadAllLines(tableFile.FullName);
        var length = 0;
        if (fileData.Length > 0)
            length = fileData[0].Split(",").Length;

        for (var i = 0; i < fileData.Length; ++i)
        {
            var strArrLength = fileData[i].Split(",").Length;
            if (length != strArrLength)
            {
                _logger.Log($"File {tableFile.Name} with wrong dimensions. " +
                            $"Found {i+1} row of size {strArrLength}, " +
                            $"whereas 1st row of size {length}.\n");
                
                return false;
            }
        }

        return true;
    }

    private bool TestColumnDataTypeEquality(FileInfo tableFile, Dictionary<string, Type> tableConfig)
    {
        var fileData = tableFile.ReadFileData<string>();

        if (fileData.Count == 0 || fileData[0].Count == 0)
            return true;

        var types = tableConfig.Values.ToArray();
        
        for (var i = 0; i < tableConfig.Values.Count; ++i)
        {
            for (var j = 0; j < fileData.Count; ++j)
            {
                try
                {
                    var obj = fileData[j][i];
                    if(obj.Length > 0)
                    {
                        Convert.ChangeType(fileData[j][i], types[i]);
                    }
                }
                catch
                {
                    _logger.Log($"{fileData[j][i]} couldn't be converted to {types[i].FullName}");
                    return false;
                }
            }
        }

        return true;
    }
}