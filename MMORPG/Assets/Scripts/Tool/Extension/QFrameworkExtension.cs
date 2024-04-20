using PimDeWitte.UnityMainThreadDispatcher;
using QFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class QFrameworkExtension
{
    public static IUnRegister RegisterEventInUnityThread<T>(this ICanRegisterEvent self, Action<T> onEvent)
    {
        return self.RegisterEvent<T>(e =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => onEvent(e));
        });
    }
}