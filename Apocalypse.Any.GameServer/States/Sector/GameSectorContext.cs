using System;
using System.Collections.Generic;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Common;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.GameServer.States.Sector
{
    public class GameSectorContext : StateMachine<string, IGameSectorLayerService>
    {
        public GameSectorContext(
            IStateGetService<string, IGameSectorLayerService> getService,
            IStateSetService<string, IGameSectorLayerService> setService,
            IStateNewService<string, IGameSectorLayerService> newService)
            : base(getService, setService, newService)
        {

        }

        public bool RegisterByType<T>()
        where T : IState<string,IGameSectorLayerService>
        => SetService.Set(Activator.CreateInstance<T>(),typeof(T).Name);
        public bool RegisterByType<T,TParam1>(TParam1 param1)
        where T : IState<string,IGameSectorLayerService>
        => SetService.Set((T)Activator.CreateInstance(
            typeof(T), param1),
            typeof(T).Name);
        public bool RegisterByType<T,TParam1,TParam2>(
            TParam1 param1,
            TParam2 param2)
        where T : IState<string,IGameSectorLayerService>
        => SetService.Set((T)Activator.CreateInstance(
            typeof(T),
            param1,
            param2),
            typeof(T).Name);
        public bool RegisterByType<T,TParam1,TParam2,TParam3>(
            TParam1 param1,
            TParam2 param2,
            TParam3 param3)
        where T : IState<string,IGameSectorLayerService>
        => SetService.Set((T)Activator.CreateInstance(
            typeof(T),
            param1,
            param2,
            param3),
            typeof(T).Name);
        public bool RegisterByType<T,TParam1,TParam2,TParam3,TParam4>(
            TParam1 param1,
            TParam2 param2,
            TParam3 param3,
            TParam4 param4)
        where T : IState<string,IGameSectorLayerService>
        => SetService.Set((T)Activator.CreateInstance(
            typeof(T),
            param1,
            param2,
            param3,
            param4),
            typeof(T).Name);
        public bool RegisterByType<T,TParam1,TParam2,TParam3,TParam4,TParam5>(
            TParam1 param1,
            TParam2 param2,
            TParam3 param3,
            TParam4 param4,
            TParam5 param5)
        where T : IState<string,IGameSectorLayerService>
        => SetService.Set((T)Activator.CreateInstance(
            typeof(T),
            param1,
            param2,
            param3,
            param4,
            param5),
            typeof(T).Name);
        

    }
}