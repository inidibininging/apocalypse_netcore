using Apocalypse.Any.Domain.Common.Model;
using System;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Language
{
    public class MechanicSingleEntityArgs<TEntity>
        where TEntity : CharacterEntity, new()
    {
        public TEntity Subject { get; private set; }

        public MechanicSingleEntityArgs(TEntity subjectEntity)
        {
            if (subjectEntity == null)
                throw new ArgumentNullException(nameof(subjectEntity));
            Subject = subjectEntity;
        }
    }
}