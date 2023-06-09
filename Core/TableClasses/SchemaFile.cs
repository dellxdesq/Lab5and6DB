namespace Core.TableClasses;

public class SchemaFile
{
    public SchemaFile(FileInfo dataBaseFile, Dictionary<FileInfo, Dictionary<string, Type>> tablesDescription)
    {
        DataBaseFile = dataBaseFile;
        TablesDescription = tablesDescription;
    }

    public FileInfo DataBaseFile { get; set; }

    public Dictionary<FileInfo, string> TablesPathsTypes
    {
        get
        {
            var dict = new Dictionary<FileInfo, string>();
            foreach (var (file, config) in TablesDescription)
            {
                dict.TryAdd(file, file.Name.Substring(0, file.Name.LastIndexOf(".csv")));
            }

            return dict;
        }
    }

    public Dictionary<FileInfo, Dictionary<string, Type>> TablesDescription { get; set; }
}