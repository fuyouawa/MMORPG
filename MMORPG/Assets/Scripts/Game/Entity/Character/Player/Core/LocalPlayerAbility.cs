using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QFramework;

namespace MMORPG.Game
{
    public class LocalPlayerAbility : PlayerAbility
    {
        public virtual IEnumerable<MethodInfo> GetStateConditions()
        {
            return from method in GetType()
                    .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                where method.HasAttribute<StateConditionAttribute>()
                select method;
        }

        public virtual bool OnStateCondition() { return true; }
    }
}
