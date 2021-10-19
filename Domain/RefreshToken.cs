using System;

namespace Domain
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public User User { get; set; }
        public string Token { get; set; }
    }
}