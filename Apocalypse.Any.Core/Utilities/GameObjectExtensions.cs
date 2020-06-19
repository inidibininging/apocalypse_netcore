using Apocalypse.Any.Core.Drawing;
using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Core.Utilities
{
    public static class GameObjectExtensions
    {
        public static bool IsVisual(this IUpdateableLite obj)
        {
            return obj.GetType().GetInterface(nameof(IVisualGameObject)) != null;
        }

        public static void ExtendVisual(this IUpdateableLite obj, Action drawFn)
        {
        }

        public static IGameObjectDictionary ForEach(this IGameObjectDictionary gameObj, Action<IUpdateableLite> delegateAction)
        {
            lock (gameObj)
            {
                var keys = new List<string>(gameObj.Keys);
                keys.ForEach(key => delegateAction(gameObj[key]));
                //var enumerator = gameObj.GetEnumerator();
                //while (enumerator.MoveNext())
                //    delegateAction(enumerator.Current.Value);
            }
            return gameObj;
        }
    }
}