using System;

using System.Security.Cryptography;
using System.Text;
using Apocalypse.Any.Domain.Common.Model.Network;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data
{
    /// <summary>
    /// Generates a token based on a key
    /// </summary>
    public class StaticLoginTokenGeneratorService<T>
        where T : UserData
    {
        private const string password = "123456abcdefghijklmnopqrstuvwxyz";

        public string GetToken(T subject) {
            using (var md5 = MD5.Create()) return Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(subject.Username)));
        }

    }
}
