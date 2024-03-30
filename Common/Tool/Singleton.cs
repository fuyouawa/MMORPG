using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Tool
{
    public class Singleton<T> where T : new()
    {
        private Singleton() { }

		private static T? _instance;
        public static T Instance => _instance ??= new T();
	}
}
