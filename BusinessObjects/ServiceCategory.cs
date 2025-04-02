using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class ServiceCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
