using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Tool
{
    public class Singleton<T> where T : new()
    {
        public Singleton() { Debug.Assert(_instance == null); }

		private static T? _instance;
        public static T Instance => _instance ??= new T();
	}
}
