using System.ComponentModel.DataAnnotations.Schema;

namespace AuthModule.Model
{
    public class User
    {
        public int Id {  get; set; }
        public string? UserName {  get; set; }
        public string? Password {  get; set; }
        [NotMapped]
        public bool IsAuthenticated { get; set; } = false;
    }
}