namespace Apocalypse.Any.Domain.Common.Model.Network
{
    public class PressReleaseUpdateData
    {
        public int SectorKey { get; set; }
        public string LoginToken { get; set; }
        public int Command { get; set; }
    }
}