﻿using System;
using System.Collections.Generic;

namespace NivelAccesDate_DbFirst.Models;

public partial class Conversion
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime Datetime { get; set; }

    public int ImageIdFrom { get; set; }

    public int ImageIdTo { get; set; }

    public virtual Image ImageIdFromNavigation { get; set; } = null!;

    public virtual Image ImageIdToNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
