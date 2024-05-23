using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Tool
{
    [Flags]
    public enum ValuePickerMemberTypeMasks
    {
        None = 0,
        Field = 1,
        Property = 1 << 1,
        Method = 1 << 2,
        Variable = Field | Property,
        All = Field | Property | Method
    }

    [Serializable]
    public class ValuePicker
    {
        public GameObject TargetObject;
        [ValueDropdown("GetTargetComponentsDropdown")]
        public Component Component;
        public ValuePickerMemberTypeMasks MemberTypeMasks = ValuePickerMemberTypeMasks.Variable;
        [ValueDropdown("GetComponentMembersDropdown")]
        public string Member;

        public object GetValue()
        {
            throw new NotImplementedException();
        }

        public void SetValue(object value)
        {
            throw new NotImplementedException();
        }

#if UNITY_EDITOR
        private IEnumerable GetTargetComponentsDropdown()
        {
            var total = new ValueDropdownList<Component>() { { "None", null } };
            if (TargetObject == null)
                return total;
            total.AddRange(TargetObject.GetComponents<Component>()
                .Select(x => new ValueDropdownItem<Component>(x.GetType().Name, x)));
            return total;
        }

        private IEnumerable GetComponentMembersDropdown()
        {
            var total = new ValueDropdownList<string>() { { "None", string.Empty } };
            if (Component == null)
                return total;
            if ((MemberTypeMasks & ValuePickerMemberTypeMasks.Field) != 0)
            {
                //TODO GetComponentMembersDropdown
            }
            return total;
        }
#endif
    }
}
