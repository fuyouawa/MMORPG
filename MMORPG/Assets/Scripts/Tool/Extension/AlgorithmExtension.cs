using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class AlgorithmExtension
{
    public static T AssertNotEqual<T>(this T self, T val)
    {
        Debug.Assert(!self.Equals(val));
        return self;
    }

    public static T AssertNotNull<T>(this T self)
    {
        Debug.Assert(self != null);
        return self;
    }

    public static T AssertEqual<T>(this T self, T val)
    {
        Debug.Assert(self.Equals(val));
        return self;
    }
}