using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data
{
    public class ExampleLocalLoginAndRegistrationService
        : IUserAuthenticationService
    {
        private List<UserDataWithLoginToken> SampleDataByLoginToken { get; } = new List<UserDataWithLoginToken>()
        {
            new UserDataWithLoginToken(){
                Roles = UserDataRole.CanViewWorldByLoginToken
                        | UserDataRole.CanSendRemoteStateCommands,
                Username = "admin",
                Password = "5994471ABB01112AFCC18159F6CC74B4F511B99806DA59B3CAF5A9C173CACFC5",
                NewInGame = true // password is "12345" unhashed
            },
            new UserDataWithLoginToken(){
                Roles = UserDataRole.CanReceiveWork,
                Username = "workerbee0",
                Password = "5994471ABB01112AFCC18159F6CC74B4F511B99806DA59B3CAF5A9C173CACFC5",
                NewInGame = true // password is "12345" unhashed
            },
            new UserDataWithLoginToken(){
                Roles = UserDataRole.CanViewWorldByLoginToken,
                Username = "foo0",
                Password = "5994471ABB01112AFCC18159F6CC74B4F511B99806DA59B3CAF5A9C173CACFC5",
                NewInGame = true // password is "12345" unhashed
            },
            new UserDataWithLoginToken(){
                Roles = UserDataRole.CanViewWorldByLoginToken,
                Username = "foo1",
                Password = "5994471ABB01112AFCC18159F6CC74B4F511B99806DA59B3CAF5A9C173CACFC5",
                NewInGame = true // password is "12345" unhashed
            },
            new UserDataWithLoginToken(){
                Roles = UserDataRole.CanViewWorldByLoginToken,
                Username = "foo2",
                Password = "5994471ABB01112AFCC18159F6CC74B4F511B99806DA59B3CAF5A9C173CACFC5",
                NewInGame = true // password is "12345" unhashed
            },
            new UserDataWithLoginToken(){
                Roles = UserDataRole.CanViewWorldByLoginToken,
                Username = "foo3",
                Password = "5994471ABB01112AFCC18159F6CC74B4F511B99806DA59B3CAF5A9C173CACFC5",
                NewInGame = true // password is "12345" unhashed
            },
            new UserDataWithLoginToken(){
                Roles = UserDataRole.CanSendRemoteMovementCommands,
                        Username = "movement_test",
                Password = "5994471ABB01112AFCC18159F6CC74B4F511B99806DA59B3CAF5A9C173CACFC5",
                NewInGame = true // password is "12345" unhashed
            }

        };

        private ExampleLoginTokenGeneratorService LoginTokenGeneratorService { get; } = new ExampleLoginTokenGeneratorService();

        public ExampleLocalLoginAndRegistrationService()
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

        public UserDataRole GetRoles(UserData userData)
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
                    => SampleDataByLoginToken.Find(user => {
                        //Console.WriteLine(user.LoginToken);
                        return user.LoginToken == loginToken;
                    });
    }
    
}
