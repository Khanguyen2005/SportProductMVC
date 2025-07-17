using System;
using System.Collections.Generic;

namespace SportProductMVC.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? NamePro { get; set; }

    public string? DecriptionPro { get; set; }

    public string? Category { get; set; }

    public decimal? Price { get; set; }

    public string? ImagePro { get; set; }

    public DateOnly ManufacturingDate { get; set; }
}
