namespace Core.TableClasses;

public class Table
{
    public Table(FileInfo file, List<object> data, Type type, 
        List<TableProperty> properties)
    {
        File = file;
        ElementsType = type;
        Data = data;
        Properties = properties;
    }

    public string Name
    {
        get => File.Name;
        set
        {
            var destination = File.FullName.Substring(0, File.FullName.IndexOf(File.Name)) + value;
            System.IO.File.Move(File.FullName, destination);
            File = new FileInfo(destination);
        }
    }

    public FileInfo File { get; private set; }
    public Tuple<int, int> Shape => Tuple.Create(ElementsType.GetProperties().Length, Data.Count);
    public Type ElementsType { get; }
    public List<object> Data { get; set; }
    public List<TableProperty> Properties { get; set; }   

    public int CountNotNullProperties()
    {
        return Properties.Count(property => !property.IsNullOrEmpty());
    }
}