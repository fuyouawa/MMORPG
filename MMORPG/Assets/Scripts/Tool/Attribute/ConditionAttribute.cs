using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ConditionAttribute : PropertyAttribute
{
	public string ConditionBoolean = "";
	public bool Hidden = false;
	public bool Negative = false;

	public ConditionAttribute(string conditionBoolean)
	{
		this.ConditionBoolean = conditionBoolean;
		this.Hidden = false;
	}

	public ConditionAttribute(string conditionBoolean, bool hideInInspector)
	{
		this.ConditionBoolean = conditionBoolean;
		this.Hidden = hideInInspector;
		this.Negative = false;
	}

	public ConditionAttribute(string conditionBoolean, bool hideInInspector, bool negative)
	{
		this.ConditionBoolean = conditionBoolean;
		this.Hidden = hideInInspector;
		this.Negative = negative;
	}

}