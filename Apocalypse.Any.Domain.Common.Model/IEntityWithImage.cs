using Apocalypse.Any.Domain.Common.Model.Network;

namespace Apocalypse.Any.Domain.Common.Model
{
    public interface IEntityWithImage
    {
        string Name { get; set; }
        ImageData CurrentImage { get; set; }
    }
}