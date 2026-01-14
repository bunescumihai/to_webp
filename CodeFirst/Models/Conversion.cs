namespace CodeFirst.Models;

public class Conversion
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ImageIdFrom { get; set; }

    public int ImageIdTo { get; set; }

    public DateTime Datetime { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Image ImageFrom { get; set; } = null!;

    public virtual Image ImageTo { get; set; } = null!;
}

