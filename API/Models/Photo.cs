using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }

        public required string URL { get; set; }

        public bool IsMain { get; set; }

        public string? PublicId { get; set; }


        //Navigation Properties

        public int AppUserID { get; set; }

        public AppUser AppUser { get; set; } = null!;
    }
}