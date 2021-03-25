using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.JsonAdapter;
using Newtonsoft.Json;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data
{
    public class WebAuthenticationClientService : IUserAuthenticationService
    {
        private enum Method
        {
            Get,
            Post,
            Put,
            Delete
        }

        private readonly string _apiPath;
        // private const string ControllerPath = "/api/ExampleAuth";
        private IStringSerializationAdapter _serializerAdapter = new JsonSerializerAdapter();
        public WebAuthenticationClientService(string apiPath)
        {
            _apiPath = apiPath;
        }
        
        private async Task<string> GetClient<TRequest>(string path, TRequest instance, Method method)
        {
            using (HttpClientHandler handler = new HttpClientHandler()
            {
                UseDefaultCredentials = false
            })
            using (HttpClient client = new HttpClient(handler))
            {
                try  
                {  
                    client.BaseAddress = new Uri($"{_apiPath}");
                    Console.WriteLine($"{_apiPath}{path}");
                    // client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (method == Method.Get) return await client.GetStringAsync($"{path}{instance}");
                    
                    var content = new StringContent(_serializerAdapter.SerializeObject(instance), Encoding.UTF8,"application/json");
                    if (method == Method.Post) return await (await client.PostAsync(path, content)).Content.ReadAsStringAsync();

                    if (method == Method.Put || method == Method.Delete) throw new NotSupportedException();
                }  
                catch (Exception ex)  
                {  
                    throw ex;  
                }
            }
            

            return string.Empty;
        }
        
        private string GetLoginByToken(UserData userData)
        {
            var request = GetClient($"{nameof(GetLoginByToken)}", userData, Method.Post);
            request.Wait();
            return request.Result?.Replace("\"","");
        }
    

        public string Register(UserData userData)
        {
            var request = GetClient($"{nameof(Register)}", userData, Method.Post);
            request.Wait();
            return request.Result;
        }

        public Dictionary<UserDataRoleSource, UserDataRole> GetRoles(UserData userData)
        {
            var request = GetClient($"{nameof(GetRoles)}", userData, Method.Post);
            request.Wait();
            var roles = request.Result;
            return _serializerAdapter.DeserializeObject<Dictionary<UserDataRoleSource, UserDataRole>>(request.Result);
        }

        public UserDataWithLoginToken GetByLoginTokenHack(string loginToken)
        {
            var request = GetClient(nameof(GetByLoginTokenHack), new LoginTokenHolder() { LoginToken =  loginToken } , Method.Post);
            request.Wait();
            return _serializerAdapter.DeserializeObject<UserDataWithLoginToken>(request.Result);
        }
        

        public string GetLoginToken(UserData userData)
        {
            return GetLoginByToken(userData);
        }
    }
}