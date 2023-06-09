namespace Core.TableClasses;

public class TableProperty
{
    private PropertyType _pr;
    public string Name { get; set; }
    public PropertyType PrType
    {
        get => _pr;
        set => _pr = value;
    }

    public void Set(string name, string typeName)
    {
        Name = name;
        Enum.TryParse(typeName, out _pr);
    }
    
    public bool IsNullOrEmpty()
    {
        return Name is null ||
               Name.Length == 0;
    }
}

public enum PropertyType
{
    String,
    Int32,
    Char,
    Boolean
}