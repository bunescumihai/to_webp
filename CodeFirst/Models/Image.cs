namespace CodeFirst.Models;

public class Image
{
    public int Id { get; set; }

    public string Md5 { get; set; } = null!;

    public string Path { get; set; } = null!;

    public int Size { get; set; }

    public string Format { get; set; } = null!;

    public virtual ICollection<Conversion> ConversionsFrom { get; set; } = new List<Conversion>();

    public virtual ICollection<Conversion> ConversionsTo { get; set; } = new List<Conversion>();
}

