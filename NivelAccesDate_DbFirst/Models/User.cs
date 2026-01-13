﻿using System;
using System.Collections.Generic;

namespace NivelAccesDate_DbFirst.Models;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int PlanId { get; set; }

    public string Role { get; set; } = null!;

    public virtual ICollection<Conversion> Conversions { get; set; } = new List<Conversion>();

    public virtual Plan Plan { get; set; } = null!;
}
