namespace BitcoinRhExplorer.Entities.Users
{
    public enum UserRole
    {
        SuperAdmin = 1,
        Admin = 2,
        Editor = 3,
        User = 4
    }

    public class Role : RichEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
