using System;
using System.Collections.Generic;

namespace JournalApi.Models;

public partial class EmployeePost
{
    public int Id { get; set; }

    public string Post { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
