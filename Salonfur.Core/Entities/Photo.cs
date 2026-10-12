using System.ComponentModel.DataAnnotations;

namespace Salonfur.Core.Entities;

// Image bytes live in the database: Render's disk is wiped on every deploy.
// Query with a projection that skips Data unless you are serving the image.
public class Photo
{
    public int Id { get; set; }

    public int AlbumId { get; set; }
    public Album Album { get; set; } = null!;

    [MaxLength(50)]
    public string ContentType { get; set; } = "";

    public byte[] Data { get; set; } = [];

    public DateTime CreatedAtUtc { get; set; }
}
