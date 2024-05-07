using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QFramework;

public class LocalPlayerAbility : PlayerAbility
{
    public virtual IEnumerable<MethodInfo> GetStateConditions()
    {
        return from method in GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            where method.HasAttribute<StateConditionAttribute>()
            select method;
    }

}
