namespace Apocalypse.Any.Infrastructure.Server.Services.Data
{
    public class ExampleLoginTokenGeneratorService
    {
        public string GetToken(object data)
        {
            return System.Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}