using System;
using System.Collections.Generic;

namespace NivelAccesDate_DbFirst.Models;

public partial class Plan
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Price { get; set; }

    /// <summary>
    /// Maximum number of conversions allowed per day
    /// </summary>
    public int Limit { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
