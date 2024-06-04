using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MMORPG.Tool
{
    public class MemberPicker
    {
        [PropertyOrder(0)]
        [LabelText("@TargetObjectLabel")]
        public GameObject TargetObject;

        [PropertyOrder(10)]
        [LabelText("@TargetComponentLabel")]
        [ValueDropdown("GetTargetComponentsDropdown")]
        public Component TargetComponent;

        [PropertyOrder(20)]
        [LabelText("@TargetMemberLabel")]
        [ValueDropdown("GetComponentMemberNamesDropdown")]
        public string TargetMemberName = string.Empty;

        public MemberInfo TargetMember { get; protected set; }
        public BindingFlags MemberBindingFlags { get; set; }

        public bool IsValid { get; protected set; }

        public virtual string TargetComponentLabel => "Target Component";
        public virtual string TargetObjectLabel => "Target Object";
        public virtual string TargetMemberLabel => "Target Member";

        public MemberPicker(BindingFlags memberBindingFlags = BindingFlags.Instance | BindingFlags.Public)
        {
            MemberBindingFlags = memberBindingFlags;
        }

        public virtual void Initialize()
        {
            IsValid = false;
            if (!TargetObject || !TargetComponent || string.IsNullOrEmpty(TargetMemberName))
            {
                return;
            }

            if (TryGetMember(out var member))
            {
                TargetMember = member;
                IsValid = true;
                OnInitialized();
            }
        }

        public virtual bool TryGetMember(out MemberInfo member)
        {
            member = null;

            if (!TargetObject || !TargetComponent || string.IsNullOrEmpty(TargetMemberName))
                return false;

            var splits = TargetMemberName.Split('/');
            if (splits.Length != 2)
            {
                return false;
            }
            if (Enum.TryParse(splits[0], out MemberTypes type))
            {
                var name = splits[1];

                member = TargetComponent.GetType().GetMember(name, MemberBindingFlags)
                    .FirstOrDefault(x => x.MemberType == type);
                if (member != null)
                    return true;
            }
            return false;
        }

        protected virtual void OnInitialized()
        {

        }

        public override string ToString()
        {
            if (TargetComponent == null || string.IsNullOrEmpty(TargetMemberName))
                return "None Member";
            var member = TargetMember;
            if (TargetMember == null)
            {
                if (!TryGetMember(out member))
                {
                    return "None Member";
                }
            }
            return $"{TargetComponent.GetType().Name}.{member.Name}[{member.MemberType}]";
        }

#if UNITY_EDITOR
        private string _prevTargetMemberName;


        [OnInspectorInit]
        protected virtual void OnInspectorInit()
        {
            _prevTargetMemberName = string.Empty;
        }

        [OnInspectorGUI]
        protected virtual void OnInspectorGUI()
        {
            if (_prevTargetMemberName != TargetMemberName)
            {
                if (TryGetMember(out var targetMember))
                {
                    TargetMember = targetMember;
                }
                _prevTargetMemberName = TargetMemberName;
            }
        }

        [OnInspectorDispose]
        protected virtual void OnInspectorDispose()
        {
            _prevTargetMemberName = string.Empty;
        }

        protected virtual IEnumerable GetTargetComponentsDropdown()
        {
            var total = new ValueDropdownList<Component>() { { "None", null } };
            if (TargetObject == null)
                return total;
            total.AddRange(TargetObject.GetComponents<Component>()
                .Select(x => new ValueDropdownItem<Component>(x.GetType().Name, x)));
            return total;
        }


        protected virtual IEnumerable GetComponentMemberNamesDropdown()
        {
            var total = new ValueDropdownList<string>() { { "None", string.Empty } };
            if (TargetComponent == null)
                return total;

            total.AddRange(TargetComponent.GetType().GetMembers(MemberBindingFlags)
                .Where(MemberFilter)
                .Select(MemberDropdownSelector));
            return total;
        }


        protected virtual bool MemberFilter(MemberInfo member)
        {
            return true;
        }

        protected virtual ValueDropdownItem<string> MemberDropdownSelector(MemberInfo member)
        {
            return new($"{member.MemberType}/{ReflectHelper.GetGeneralMemberValueType(member).Name}/{member.Name}",
                $"{member.MemberType}/{member.Name}");
        }
#endif
    }
}
