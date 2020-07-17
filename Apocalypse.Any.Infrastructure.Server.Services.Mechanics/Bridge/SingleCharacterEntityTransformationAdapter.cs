using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;
using System;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Bridge
{
    //SPOT THE PATTERN.
    public class SingleCharacterEntityTransformationAdapter<TSourceType, TCharacterEntity>
        : ISingleCharacterEntityMechanic<TCharacterEntity>
        where TCharacterEntity : CharacterEntity, new()
    {
        public bool Active { get; set; } = true;
        private Func<TSourceType> GetSource { get; set; }
        private ISingleEntityTransformationMechanic<TSourceType, TCharacterEntity> SingleEntityTransformationMechanic { get; set; }

        public SingleCharacterEntityTransformationAdapter(
            ISingleEntityTransformationMechanic<TSourceType, TCharacterEntity> singleEntityTransformationMechanic,
            Func<TSourceType> getSource)
        {
            SingleEntityTransformationMechanic = singleEntityTransformationMechanic ?? throw new ArgumentNullException(nameof(singleEntityTransformationMechanic));
            GetSource = getSource ?? throw new ArgumentNullException(nameof(getSource));
        }

        public TCharacterEntity Update(TCharacterEntity singularEntity)
        {
            return SingleEntityTransformationMechanic.Update(GetSource());
        }
    }
}