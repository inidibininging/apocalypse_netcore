using System;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using StackExchange.Redis;

namespace Apocalypse.Any.Infrastructure.Server.Adapters.Redis
{
    public abstract class GameSectorOwnerRedisMechanicBase : IGameSectorOwnerRedisMechanic
    {
        public string RedisHost { get; set; }
        public int RedisPort { get; set; }

        protected DateTime ExecutionTime { get; set; }
        public bool Executing { get; private set; }
        public IGameSectorsOwner Update(IGameSectorsOwner entity)
        {
            // Console.WriteLine(this.GetType().FullName);
            if (CanExecute()){
                DoWrapper(entity);
            }
            else
            {
                //DO A LITTLE DANCE
            }
            return entity;
        }

        private void ResetTimer(int seconds = 5)
        {
            ExecutionTime = DateTime.Now.AddSeconds(seconds);
        }
        protected bool CanExecute()
        {
            return DateTime.Now > ExecutionTime && !Executing && !string.IsNullOrWhiteSpace(RedisHost);
        }
        private bool HasValidRedisConfig() => !string.IsNullOrWhiteSpace(RedisHost) && RedisPort != 0;
        private void DoWrapper(IGameSectorsOwner entity)
        {
            if(!HasValidRedisConfig())
                return;
            Executing = true;
            UpdateImplementation(entity);
            Executing = false;
            ResetTimer();
        }
        protected abstract void UpdateImplementation(IGameSectorsOwner entity);
    }
}
