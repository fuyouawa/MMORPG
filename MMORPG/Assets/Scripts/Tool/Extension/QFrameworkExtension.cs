using PimDeWitte.UnityMainThreadDispatcher;
using QFramework;
using System;

namespace MMORPG.Tool
{
    public static class QFrameworkExtension
    {
        public static IUnRegister RegisterEventInUnityThread<T>(this ICanRegisterEvent self, Action<T> onEvent)
        {
            return self.RegisterEvent<T>(e => { UnityMainThreadDispatcher.Instance().Enqueue(() => onEvent(e)); });
        }
    }
}
