using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using BitcoinRhExplorer.EF.Interfaces;
using BitcoinRhExplorer.Entities.Users;
using BitcoinRhExplorer.Library;

namespace BitcoinRhExplorer.Server.Business
{
    public class UserComponent : BaseDbComponent<User>
    {
        public UserComponent(IDbContextScopeFactory dbContextScopeFactory, IAmbientDbContextLocator ambientDbContextLocator) :
            base(dbContextScopeFactory, ambientDbContextLocator)
        {
        }

        public User Create(string email, string userName, string password, string passwordVerify, int roleId)
        {
            User user = new User();

            if (password == passwordVerify)
            {
                user.Email = email;
                user.UserName = userName;
                user.Password = GetHashPassword(password);
                user.RoleId = roleId;
            }
            else
            {
                throw new BitcoinRhExplorerException(Resources.Resources.Registration_Error_InvalidPassword);
            }

            return user;
        }
        
        public User GetByUserName(string userName)
        {
            IEnumerable<User> items;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                items = _repository.FindBy(u => u.UserName == userName)
                    .ToList();
            }

            if (items.IsAny())
            {
                return items.First();
            }
            else
            {
                return null;
            }
        }

        public User GetByEmail(string email)
        {
            IEnumerable<User> items;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                items = _repository.FindBy(u => u.Email == email)
                    .ToList();
            }

            if (items.IsAny())
            {
                return items.First();
            }
            else
            {
                return null;
            }
        }

        public User GetByProviderId(string provider, string providerId)
        {
            var providerTypeId = (int)Enum.Parse(typeof(UserProviderType), provider, true);

            IEnumerable<User> items;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                items = _repository.FindBy(u => u.ProviderId == providerId && u.ProviderTypeId == providerTypeId)
                    .ToList();
            }

            if (items.IsAny())
            {
                return items.First();
            }
            else
            {
                return null;
            }
        }

        public User LoginUserByUserName(string username, string password)
        {
            var hashedPassword = GetHashPassword(password);

            IEnumerable<User> items;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                items = _repository.FindBy(u => u.UserName == username && u.Password == hashedPassword)
                    .ToList();
            }

            if (items.IsAny())
            {
                return items.First();
            }
            else
            {
                throw new BitcoinRhExplorerException(Resources.Resources.Registration_Error_Invalid);
            }
        }

        public User LoginUserByEmail(string email, string password)
        {
            var hashedPassword = GetHashPassword(password);

            IEnumerable<User> items;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                items = _repository.FindBy(u => u.Email == email && u.Password == hashedPassword)
                    .ToList();
            }

            if (items.IsAny())
            {
                return items.First();
            }
            else
            {
                throw new BitcoinRhExplorerException(Resources.Resources.Registration_Error_Invalid);
            }
        }

        public string GetHashPassword(string plain)
        {
            var builder = new StringBuilder();
            foreach (byte b in GetHash(plain)) builder.Append(b.ToString("X2"));

            return builder.ToString();
        }

        private byte[] GetHash(string plain)
        {
            HashAlgorithm algorithm = SHA1.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(plain));
        }

        public bool ArePasswordsOk(string password, string confirmPassword)
        {
            bool result = false;

            if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(confirmPassword))
            {
                if (password == confirmPassword)
                {
                    result = true;
                }
            }

            if (!result)
            {
                throw new BitcoinRhExplorerException(Resources.Resources.Registration_Error_InvalidPassword);
            }

            return result;
        }

        public void IsUserNameValid(string username)
        {
            if (username.Length < 2)
            {
                throw new BitcoinRhExplorerException(Resources.Resources.Registration_Error_UserNameTooShort);
            }
        }

        public void IsEmailValid(string email)
        {
            string regularExpresion = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            bool result = Regex.IsMatch(email, regularExpresion, RegexOptions.IgnoreCase);
            
            if (!result)
            {
                throw new BitcoinRhExplorerException(Resources.Resources.Registration_Error_WrongEmail);
            }
        }

        public void IsUserNameExists(string userName)
        {
            IEnumerable<User> items;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                items = _repository.FindBy(u => u.UserName == userName)
                    .ToList();
            }

            if (items.IsAny())
            {
                throw new BitcoinRhExplorerException(Resources.Resources.Registration_Error_UserAlreadyExists);
            }
        }

        public void IsEmailExists(string email)
        {
            IEnumerable<User> items;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                items = _repository.FindBy(u => u.Email == email)
                    .ToList();
            }

            if (items.IsAny())
            {
                throw new BitcoinRhExplorerException(Resources.Resources.Registration_Error_UserEmailAlreadyExists);
            }
        }

        public int GetCount(string token)
        {
            IEnumerable<User> items;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                items = _repository.FindBy(u => (token == null) ||
                     (u.UserName.Contains(token) ||
                      u.FirstName.Contains(token) ||
                      u.LastName.Contains(token) ||
                      u.Email.Contains(token)))
                    .ToList();
            }

            return items.Count();
        }

        public IEnumerable<User> GetByRange(string token, int? offset, int limit)
        {
            IEnumerable<User> items;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                items = _repository.FindBy(u => (token == null) ||
                                                (u.UserName.Contains(token) ||
                                                 u.FirstName.Contains(token) ||
                                                 u.LastName.Contains(token) ||
                                                 u.Email.Contains(token)))
                    .AsEnumerable()
                    .Skip((int) offset.GetValueOrDefault(0))
                    .Take(limit)
                    .ToList();
            }

            return items;
        }

        public IEnumerable<User> GetByRange(int companyId, int? offset, int limit)
        {
            IEnumerable<User> items;

            using (_dbContextScopeFactory.CreateReadOnly())
            {
                items = _repository.GetAll()
                    .Skip((int) offset.GetValueOrDefault(0))
                    .Take(limit)
                    .ToList();
            }

            return items;
        }

        public string SaveAvatar(HttpPostedFileBase file, string allowedExtensions, string targetPath)
        {
            string result = string.Empty;

            try
            {
                result = FileHelper.SaveFile(file, allowedExtensions, targetPath);
            }
            catch (BitcoinRhExplorerException ex)
            {
                throw new BitcoinRhExplorerException(ex.Message);
            }
            catch (Exception)
            {
                throw new BitcoinRhExplorerException(Resources.Resources.Error_SaveAvatarToDisk);
            }

            return result;
        }

        public string GetProviderData(IDictionary<string, string> extraData, string[] keys)
        {
            var output = string.Empty;

            foreach (var key in keys)
            {
                if (extraData.ContainsKey(key))
                {
                    output = extraData[key];
                    break;
                }
            }

            return output;
        }

        public string GenerateNewPassword()
        {
            Guid g = Guid.NewGuid();
            string newPassword = Convert.ToBase64String(g.ToByteArray());
            newPassword = newPassword.Replace("=", "");
            newPassword = newPassword.Replace("+", "");

            return newPassword.Substring(1, 8);
        }
    }
}
