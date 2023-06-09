namespace Core.TableClasses;

public class DataBase
{
    public DataBase(SchemaFile file, List<Table> tables)
    {
        File = file;
        Tables = tables;
    }
    public SchemaFile File { get; }
    public List<Table> Tables { get; }
    public string Name
    {
        get => File.DataBaseFile.Name;
        set
        {
            var destination = File.DataBaseFile.FullName.Substring(0, 
                File.DataBaseFile.FullName.IndexOf(File.DataBaseFile.Name)) + value;
            System.IO.File.Move(File.DataBaseFile.FullName, destination);
            File.DataBaseFile = new FileInfo(destination);
        }
    }

    public override string ToString()
    {
        return File.DataBaseFile.Name;
    }
}