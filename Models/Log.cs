using System;
using System.Collections.Generic;

namespace JournalApi.Models;

public partial class Log
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public string Message { get; set; } = null!;

    public string Type { get; set; } = null!;
}
