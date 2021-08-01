using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model.Network;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data
{
    public class ExampleLoginAndRegistrationService
        : IUserAuthenticationService
    {
        private List<UserDataWithLoginToken> SampleDataByLoginToken { get; } = new()
        {
            new UserDataWithLoginToken(){
                Roles = new Dictionary<UserDataRoleSource, UserDataRole>(){ 
                            { UserDataRoleSource.LocalServer, UserDataRole.CanViewWorldByLoginToken },
                            { UserDataRoleSource.SyncServer, UserDataRole.CanSendRemoteStateCommands },
                },
                Username = "admin",
                Password = "5994471ABB01112AFCC18159F6CC74B4F511B99806DA59B3CAF5A9C173CACFC5",
                NewInGame = true // password is "12345" unhashed
            },
            new UserDataWithLoginToken(){
                Roles = new Dictionary<UserDataRoleSource, UserDataRole>(){
                    // { UserDataRoleSource.LocalServer, UserDataRole.CanSendRemoteStateCommands },
                    { UserDataRoleSource.SyncServer, UserDataRole.CanReceiveWork },
                },
                Username = "worker.test.0",
                Password = "5994471ABB01112AFCC18159F6CC74B4F511B99806DA59B3CAF5A9C173CACFC5",
                NewInGame = true // password is "12345" unhashed
            },
            new UserDataWithLoginToken(){
                Roles = new Dictionary<UserDataRoleSource, UserDataRole>(){ 
                    { UserDataRoleSource.SyncServer, UserDataRole.CanViewWorldByLoginToken },
                },
                Username = "remote.test.0",
                Password = "5994471ABB01112AFCC18159F6CC74B4F511B99806DA59B3CAF5A9C173CACFC5",
                NewInGame = true // password is "12345" unhashed
            },
            new UserDataWithLoginToken(){
                Roles = new Dictionary<UserDataRoleSource, UserDataRole>(){ 
                    { UserDataRoleSource.SyncServer, UserDataRole.CanViewWorldByLoginToken },
                },
                Username = "remote.test.1",
                Password = "5994471ABB01112AFCC18159F6CC74B4F511B99806DA59B3CAF5A9C173CACFC5",
                NewInGame = true // password is "12345" unhashed
            },
            new UserDataWithLoginToken(){
                Roles = new Dictionary<UserDataRoleSource, UserDataRole>(){ 
                    { UserDataRoleSource.SyncServer, UserDataRole.CanViewWorldByLoginToken },
                },
                Username = "remote.test.2",
                Password = "5994471ABB01112AFCC18159F6CC74B4F511B99806DA59B3CAF5A9C173CACFC5",
                NewInGame = true // password is "12345" unhashed
            },
            
            /*
             * The flag CanSendRemoteMovementCommands will only work if the game client config has "WithLocalServer" turned on.
             * This is due to the desktop game client.
             * It requests after a successful login either Update or UpdateDelta.  
             */ 
            new UserDataWithLoginToken(){
                Roles = new Dictionary<UserDataRoleSource, UserDataRole>(){ 
                    { UserDataRoleSource.LocalServer, UserDataRole.CanViewWorldByLoginToken },
                    { UserDataRoleSource.SyncServer, UserDataRole.CanSendRemoteMovementCommands },
                },
                Username = "sync.test.0",
                Password = "5994471ABB01112AFCC18159F6CC74B4F511B99806DA59B3CAF5A9C173CACFC5",
                NewInGame = true // password is "12345" unhashed
            },
            new UserDataWithLoginToken(){
                Roles = new Dictionary<UserDataRoleSource, UserDataRole>(){ 
                    { UserDataRoleSource.LocalServer, UserDataRole.CanViewWorldByLoginToken },
                    { UserDataRoleSource.SyncServer, UserDataRole.CanSendRemoteMovementCommands },
                },
                Username = "sync.test.1",
                Password = "5994471ABB01112AFCC18159F6CC74B4F511B99806DA59B3CAF5A9C173CACFC5",
                NewInGame = true // password is "12345" unhashed
            },
            new UserDataWithLoginToken(){
                Roles = new Dictionary<UserDataRoleSource, UserDataRole>(){ 
                    { UserDataRoleSource.LocalServer, UserDataRole.CanViewWorldByLoginToken },
                    { UserDataRoleSource.SyncServer, UserDataRole.CanSendRemoteMovementCommands },
                },
                Username = "sync.test.2",
                Password = "5994471ABB01112AFCC18159F6CC74B4F511B99806DA59B3CAF5A9C173CACFC5",
                NewInGame = true // password is "12345" unhashed
            }
        };

        private StaticLoginTokenGeneratorService<UserData> LoginTokenGeneratorService { get; } = new StaticLoginTokenGeneratorService<UserData>();

        public ExampleLoginAndRegistrationService()
        {
            //because the login token must be generated after instantiation
            foreach (var user in SampleDataByLoginToken)
                user.LoginToken = LoginTokenGeneratorService.GetToken(user);
        }

        private string GiveMeTheHashedPassword(string password)
        {
            var passwordAsBytes = Encoding.UTF8.GetBytes(password);
            using (var hasher = new SHA256Managed())
                passwordAsBytes = hasher.ComputeHash(passwordAsBytes);
            return BitConverter.ToString(passwordAsBytes).Replace("-", string.Empty);
        }

        public string GetLoginToken(UserData userData)
        {
            if (string.IsNullOrWhiteSpace(userData.Password))
                throw new ArgumentNullException("Password");

            var hashedPassword = GiveMeTheHashedPassword(userData.Password);

            var foundUsers = from user in SampleDataByLoginToken
                             where user.Username == userData.Username &&
                                   user.Password == hashedPassword
                             select user;

            if (!foundUsers.Any())
                throw new UserNotFoundException();

            var foundUser = foundUsers.FirstOrDefault();
            return foundUser.LoginToken;
        }

        public string Register(UserData userData)
        {
            var userDataWithLoginToken = new UserDataWithLoginToken()
            {
                Username = userData.Username,
                Password = userData.Password,
                LoginToken = LoginTokenGeneratorService.GetToken(userData)
            };

            SampleDataByLoginToken.Add(userDataWithLoginToken);
            return userDataWithLoginToken.LoginToken;
        }

        public Dictionary<UserDataRoleSource, UserDataRole> GetRoles(UserData userData)
        {
            if (string.IsNullOrWhiteSpace(userData.Password))
                throw new ArgumentNullException("Password");
            var hashedPassword = GiveMeTheHashedPassword(userData.Password);

            var foundUsers = from user in SampleDataByLoginToken
                             where user.Username == userData.Username &&
                                   user.Password == hashedPassword
                             select user;

            if (!foundUsers.Any())
                throw new UserNotFoundException();

            var foundUser = foundUsers.FirstOrDefault();
            return foundUser.Roles;
        }

        public UserDataWithLoginToken GetByLoginTokenHack(string loginToken)
            => SampleDataByLoginToken.Find(user => user.LoginToken == loginToken);
    }
}