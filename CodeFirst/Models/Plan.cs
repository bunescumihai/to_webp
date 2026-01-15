namespace CodeFirst.Models;

public class Plan
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    /// <summary>
    /// Maximum number of conversions allowed per day
    /// </summary>
    public int Limit { get; set; } 

    public int Price { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

