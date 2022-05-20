namespace BitcoinRhExplorer.Entities.Users
{
    public enum UserProviderType
    {
        Default = 0,
        Facebook = 1,
        Twitter = 2,
        Google = 3
    }

    public class User : RichEntity
    {
        private string userName;
        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
            }
        }

        private string email;
        public string Email
        {
            get { return email; }
            set
            {
                email = value;
            }
        }

        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string GeneratedPassword { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool IsTwoFactor { get; set; }

        public string Phone { get; set; }

        public string Language { get; set; }

        public long RoleId { get; set; }
        public Role Role { get; set; }

        public string AvatarFileName { get; set; }

        public int ProviderTypeId { get; set; }
        public UserProviderType Provider
        {
            get { return (UserProviderType)ProviderTypeId; }
            set { ProviderTypeId = (int)value; }
        }
        public string ProviderId { get; set; }
        public string ProviderUrl { get; set; }
        public string ProviderLocation { get; set; }
        public string ProviderGender { get; set; }
        public string ProviderBirthday { get; set; }

        public override string ToString()
        {
            return UserName;
        }

        public bool Equals(User user)
        {
            if (this.UserName != user.UserName) return false;
            if (this.Email != user.Email) return false;
            if (this.Password != user.Password) return false;
            if (this.FirstName != user.FirstName) return false;
            if (this.LastName != user.LastName) return false;
            if (this.Phone != user.Phone) return false;
            if (this.RoleId != user.RoleId) return false;
            if (this.AvatarFileName != user.AvatarFileName) return false;
            if (this.Id != user.Id) return false;

            return true;
        }
    }
}
