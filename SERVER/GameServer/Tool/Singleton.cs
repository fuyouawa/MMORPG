using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Tool
{
    internal class SingletonCreator
    {
        public static T CreateSingleton<T>() where T : class
        {
            var type = typeof(T);
            var ctorInfos = type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

            var ctor = Array.Find(ctorInfos, c => c.GetParameters().Length == 0)
                ?? throw new Exception($"单例({type})必须要有一个非public的无参构造函数!");
#if DEBUG
            var publicCtorInfos = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            if (publicCtorInfos.Length != 0)
                throw new Exception($"单例({type})不能有public构造函数!");
#endif
            var inst = ctor.Invoke(null) as T;
            Debug.Assert(inst != null);
            return inst;
        }
    }

    /// <summary>
    /// 单例模式
    /// 注意: 一定要有一个私有的构造函数!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> where T : Singleton<T>
    {
        protected static T? _instance;
        //static object _mutex = new();

        public static T Instance
        {
            get
            {
                //lock (_mutex)
                //{
                    _instance ??= SingletonCreator.CreateSingleton<T>();
                //}
                return _instance;
            }
        }

        public virtual void Dispose()
        {
            _instance = null;
        }
    }
}
