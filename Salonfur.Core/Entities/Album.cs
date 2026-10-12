using System.ComponentModel.DataAnnotations;

namespace Salonfur.Core.Entities;

public class Album
{
    public int Id { get; set; }

    [MaxLength(80)]
    public string Title { get; set; } = "";

    [MaxLength(300)]
    public string? Description { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public List<Photo> Photos { get; set; } = [];
}
