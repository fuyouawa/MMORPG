using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AnimatorParam<T> where T : struct, IConvertible
{
    private Animator _animator;
    private string _paramName;

    public T Value
    {
        get
        {
            var type = typeof(T);
            if (type == typeof(float))
                return (T)Convert.ChangeType(_animator.GetFloat(_paramName), typeof(float));
            else if (type == typeof(bool))
                return (T)Convert.ChangeType(_animator.GetBool(_paramName), typeof(bool));
            else if (type == typeof(int))
                return (T)Convert.ChangeType(_animator.GetInteger(_paramName), typeof(int));
            else
                throw new Exception($"Animator不支持的类型:{typeof(T)}");
        }
        set
        {
            if (value is float f)
                _animator.SetFloat(_paramName, f);
            else if (value is bool b)
                _animator.SetBool(_paramName, b);
            else if(value is int i)
                _animator.SetInteger(_paramName, i);
            else
                throw new Exception($"Animator不支持的类型:{typeof(T)}");
        }
    }

    public void Init(Animator animator, string paramName)
    {
        _animator = animator;
        _paramName = paramName;
    }
}
