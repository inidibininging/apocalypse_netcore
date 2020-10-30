using Microsoft.Xna.Framework.Graphics;

namespace Apocalypse.Any.Core.Utilities
{
    public static class ObjectExtensions
    {
        public static bool IsNull(this object obj)
        {
            return obj == null;
        }

        // public static (T1 t1, T2 t2) TryConvert<T1, T2>(this object obj) =>
        //     (obj is T1 obj1 ? obj1 : default(T1), 
        //         obj is T2 obj2 ? obj2 : default(T2));
        // public static (T1 t1, T2 t2, T3 t3) TryConvert<T1, T2, T3>(this object obj) =>
        //     (obj is T1 obj1 ? obj1 : default(T1), 
        //         obj is T2 obj2 ? obj2 : default(T2),
        //         obj is T3 obj3 ? obj3 : default(T3));
        // public static (T1 t1, T2 t2, T3 t3, T4 t4) TryConvert<T1, T2, T3, T4>(this object obj) =>
        //     (obj is T1 obj1 ? obj1 : default(T1), 
        //         obj is T2 obj2 ? obj2 : default(T2),
        //         obj is T3 obj3 ? obj3 : default(T3),
        //         obj is T4 obj4 ? obj4 : default(T4));
        // public static (T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) TryConvert<T1, T2, T3, T4, T5>(this object obj) =>
        //     (obj is T1 obj1 ? obj1 : default(T1), 
        //         obj is T2 obj2 ? obj2 : default(T2),
        //         obj is T3 obj3 ? obj3 : default(T3),
        //         obj is T4 obj4 ? obj4 : default(T4),
        //         obj is T5 obj5 ? obj5 : default(T5));
    }
}