using System;
using System.Collections.Generic;

namespace PAWProject.Models.Entities;

public partial class Secret
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Value { get; set; } = null!;

    public string? Description { get; set; }

    public int? SourceId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Source? Source { get; set; }
}
