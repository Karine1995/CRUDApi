namespace CRUD.DAL.Models
{
    public class UserSession : BaseEntity
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public bool IsExpired { get; set; }
        public string RefreshToken { get; set; }
        public User User { get; set; }
    }
}
