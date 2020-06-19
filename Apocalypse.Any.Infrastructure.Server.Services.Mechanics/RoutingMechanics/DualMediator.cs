using System;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.RoutingMechanics
{
    public class DualMediator<T1, T2, TObjectType, TEventType>
        where T1 : TObjectType
        where T2 : TObjectType
    {
        private T1 T1Instance { get; set; }
        private T2 T2Instance { get; set; }
        private Action<TObjectType, TEventType> NotifyFunc { get; set; }

        public DualMediator(
            T1 t1,
            T2 t2,
            Action<TObjectType, TEventType> notifyFunc)
        {
            T1Instance = t1;
            T2Instance = t2;
            NotifyFunc = notifyFunc ?? throw new ArgumentNullException(nameof(notifyFunc));
        }

        public void Notify(TObjectType sender, TEventType evnt)
        {
            NotifyFunc?.Invoke(sender, evnt);
        }
    }
}