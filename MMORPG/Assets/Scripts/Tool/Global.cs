using NUnit.Framework.Internal;
using UnityEngine;

namespace Tool
{
    public class Global
    {
        public class Logger
        {
            public static void Info(object message)
            {
                Debug.Log(message);
            }

            public static void Error(object message)
            {
                Debug.LogError(message);
            }
        }

    }
}
