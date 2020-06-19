namespace Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Data
{
    public class ExampleLoginTokenGeneratorService
    {
        public string GetToken(object data)
        {
            return System.Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}