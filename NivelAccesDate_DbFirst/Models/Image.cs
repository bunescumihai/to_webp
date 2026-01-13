﻿using System;
using System.Collections.Generic;

namespace NivelAccesDate_DbFirst.Models;

public partial class Image
{
    public int Id { get; set; }

    public string Md5 { get; set; } = null!;

    public string Path { get; set; } = null!;

    public int Size { get; set; }

    public string Format { get; set; } = null!;

    public virtual ICollection<Conversion> ConversionImageIdFromNavigations { get; set; } = new List<Conversion>();

    public virtual ICollection<Conversion> ConversionImageIdToNavigations { get; set; } = new List<Conversion>();
}
