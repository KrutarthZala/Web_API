using System.ComponentModel.DataAnnotations;

namespace Product_CRUD_Web_API.Models
{
    public class UserModel
    {
        [Key]
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;
        public string? UserJWTToken { get; set; }
        public DateTime? JWTTokenIssueDate { get; set; }
        public DateTime? JWTTokenExpiryDate { get; set; }
    }
}
