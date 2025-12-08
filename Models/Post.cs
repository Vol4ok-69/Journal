using System;
using System.Collections.Generic;

namespace JournalApi.Models;

public partial class Post
{
    public int Id { get; set; }

    public string Post1 { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
