using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.Core.Events
{
    /// <summary>
    /// A game event container. This container will be used as long as the predicate is fullfiled
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GameEventContainer<T>
        : GameObject
        where T : GameObject
    {
        private T _target;
        private Dictionary<Func<T, bool>, Action<T>> Logic { get; set; }

        protected void When(Func<T, bool> condition, Action<T> handler)
        {
            Logic.Add(condition, handler);
        }

        public GameEventContainer(T target) : base()
        {
            _target = target;
            Logic = new Dictionary<Func<T, bool>, Action<T>>();
        }

        public override void Update(GameTime time)
        {
            var handlers = from kv in Logic
                           where kv.Key(_target)
                           select kv.Value;
            handlers.ToList().ForEach(handler => handler(_target));
        }
    }
}