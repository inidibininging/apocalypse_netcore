using Apocalypse.Any.Domain.Common.Model;
using System;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Language
{
    public class MechanicSingleEntityWithSingleTargetArgs<TEntity, TTargetEntity>
    : MechanicSingleEntityArgs<TEntity>
    where TEntity : CharacterEntity, new()
    where TTargetEntity : CharacterEntity, new()
    {
        public TTargetEntity Target { get; private set; }

        public MechanicSingleEntityWithSingleTargetArgs(
            TEntity subjectEntity,
            TTargetEntity targetEntity) : base(subjectEntity)
        {
            if (targetEntity == null)
                throw new ArgumentNullException(nameof(targetEntity));
            Target = targetEntity;
        }
    }
}