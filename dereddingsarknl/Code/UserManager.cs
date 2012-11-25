using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dereddingsarknl.Models;
using dereddingsarknl.Extensions;
using System.Security.Cryptography;
using System.IO;

namespace dereddingsarknl
{
  public class UserManager
  {
    private IndexFile _userIndex;
    private DataManager _dataManager;

    public UserManager(DataManager dataManager)
    {
      _userIndex = dataManager.GetFile(DataFolders.Users, IndexFiles.Users).OpenIndex();
      _dataManager = dataManager;
    }

    public User GetUser(UserToken token)
    {
      if(token != null)
      {
        var file = _dataManager.GetFile(DataFolders.UsersToken, f => f.Name.EndsWith(token.Guid));
        var tokenIndex = file.OpenIndex();
        var item = tokenIndex.Items
                    .FirstOrDefault(i => /*i.First() == ipadress &&*/ i.Skip(1).First() == token.Token && i.Skip(2).First() == token.Generated);

        return item != null ? GetUserFromTokenFile(file.Name) : null;
      }
      else
      {
        return null;
      }
    }

    private User GetUserFromTokenFile(string tokenFileName)
    {
      var indexLine = _userIndex.Items.FirstOrDefault(i => tokenFileName.StartsWith(i.First().Replace("@", "-"), StringComparison.InvariantCultureIgnoreCase));

      if(indexLine == null)
      {
        return null;
      }
      else
      {
        return User.Create(indexLine);
      }
    }

    private string GetGuidFromUser(User user)
    {
      var begin = user.Email.Replace("@", "-") + "__";
      var file = _dataManager.GetFile(DataFolders.UsersToken, f => f.Name.StartsWith(begin));

      return file.Name.Substring(begin.Length);
    }

    public UserToken StoreNewToken(User user, string ipaddress)
    {
      var token = Salt();
      var generated = DateTime.UtcNow.ToString("yyyyMMddTHH:mm:ss");
      var guid = GetGuidFromUser(user);

      var tokenFileName = user.Email.Replace("@", "-") + "__" + guid;

      var file = _dataManager.GetFile(DataFolders.UsersToken, tokenFileName);
      file.AppendCsvValues(ipaddress, token, generated);

      return new UserToken
      {
        Generated = generated,
        Token = token,
        Guid = guid,
        IpAddress = ipaddress
      };
    }

    public void RemoveToken(User user, UserToken token)
    {
      if(token != null)
      {
        var tokenFileName = user.Email.Replace("@", "-") + "__" + token.Guid;
        var tokenIndex = _dataManager.GetFile(DataFolders.UsersToken, tokenFileName).OpenIndex();
        tokenIndex.Remove(i => i.Skip(1).First() == token.Token);
      }
    }

    public User GetApiUser(ApiToken token)
    {
      if(token != null)
      {
        var file = _dataManager.GetFile(DataFolders.UsersToken, f => f.Name.EndsWith(token.Guid));
        var tokenIndex = file.OpenIndex();
        var item = tokenIndex.Items
                    .FirstOrDefault(i => i.First() == "API" && i.Skip(1).First() == token.Token);

        return item != null ? GetUserFromTokenFile(file.Name) : null;
      }

      return null;
    }


    /// <summary>
    /// Genereer voor de gegeven gebruiker een speciaal, eeuwig durend token.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public ApiToken StoreNewApiToken(User user)
    {
      var token = Salt();
      var generated = DateTime.UtcNow.ToString("yyyyMMddTHH:mm:ss");
      var guid = GetGuidFromUser(user);

      var tokenFileName = user.Email.Replace("@", "-") + "__" + guid;

      var file = _dataManager.GetFile(DataFolders.UsersToken, tokenFileName);
      file.AppendCsvValues("API", token, generated);

      return new ApiToken() { Guid = guid, Token = token };
    }

    public User GetUserFromResetPasswordToken(string token)
    {
      var file = _dataManager.GetFileBasedOnContent(DataFolders.UsersResetPasswords, l => l.StartsWith("\"" + token));
      // TODO check for valid token
      if(file != null)
      {
        return GetUserFromEmail(file.Name);
      }

      return null;
    }

    public string StoreResetToken(string email)
    {
      return StoreResetToken(email, false);
    }

    public string StoreResetToken(string email, bool newAccount)
    {
      string token = Guid.NewGuid().ToString("N");

      var tokenFileName = email.Replace("@", "-");

      var file = _dataManager.GetNewFile(DataFolders.UsersResetPasswords, tokenFileName);
      // Voor nieuwe accounts is het reset token 2 maanden actief.
      file.AppendCsvValues(token, newAccount ? DateTime.Now.AddMonths(2).Ticks : DateTime.Now.Ticks);

      return token;
    }

    protected string Salt()
    {
      var buf = new byte[16];
      new RNGCryptoServiceProvider().GetBytes(buf);
      return Convert.ToBase64String(buf);
    }

    protected string HashPassword(string password, string salt)
    {
      var crypto = new Rfc2898DeriveBytes(password, System.Text.Encoding.Default.GetBytes(salt), 10000);
      var hash = crypto.GetBytes(32);
      return Convert.ToBase64String(hash);
    }

    private string NewPassword()
    {
      var passwordLength = 10;
      string allowedLetterChars = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";
      string allowedNumberChars = "23456789!@$?_-";
      char[] chars = new char[passwordLength];
      Random rd = new Random();

      bool useLetter = true;
      for(int i = 0; i < passwordLength; i++)
      {
        if(useLetter)
        {
          chars[i] = allowedLetterChars[rd.Next(0, allowedLetterChars.Length)];
          useLetter = false;
        }
        else
        {
          chars[i] = allowedNumberChars[rd.Next(0, allowedNumberChars.Length)];
          useLetter = true;
        }
      }

      return new string(chars);
    }

    public int GetUserCount()
    {
      return _userIndex.Items.Count();
    }

    public User GetUserFromEmail(string email)
    {
      var normalizedEmail = email.Replace("@", "-");

      var indexLine = (_userIndex.Items.FirstOrDefault(i =>
        i.First().Replace("@", "-")
         .Equals(normalizedEmail.Trim(), StringComparison.InvariantCultureIgnoreCase)));

      if(indexLine == null)
      {
        return null;
      }
      else
      {
        return User.Create(indexLine);
      }
    }

    public bool IsEmailAlreadyTaken(string email)
    {
      return _userIndex.Contains(i => i.First().Equals(email.Trim(), StringComparison.InvariantCultureIgnoreCase));
    }

    public void Add(string name, string email, string extras)
    {
      string password = NewPassword();
      string salt = Salt();
      string passwordHash = HashPassword(password, salt);

      _userIndex.Add(User.CreateIndexLine(email, name, passwordHash, salt, extras));

      // create a token file:
      var guid = Guid.NewGuid().ToString("N");
      var tokenFileName = email.Replace("@", "-") + "__" + guid;
      _dataManager.GetNewFile(DataFolders.UsersToken, tokenFileName);
    }

    public bool CheckPassword(User user, string password)
    {
      return user.PasswordHash == HashPassword(password, user.Salt);
    }

    public IEnumerable<User> GetUsers()
    {
      return _userIndex.Items.Select(i => dereddingsarknl.Models.User.Create(i));
    }

    public void Update(User user, string password)
    {
      string salt = Salt();
      string passwordHash = HashPassword(password, salt);
      var indexLine = string.Format("\"{0}\", \"{1}\", \"{2}\", \"{3}\", \"{4}\"", user.Email, user.Name, passwordHash, salt, user.Extras);

      _userIndex.Update(i => i.First().Equals(user.Email, StringComparison.InvariantCultureIgnoreCase)
                  , indexLine);
    }
  }
}