using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Validation;
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
        [Required]
        [LabelText("@TargetObjectLabel")]
        public GameObject TargetObject;

        [PropertyOrder(10)]
        [Required]
        [LabelText("@TargetComponentLabel")]
        [ValueDropdown("GetTargetComponentsDropdown")]
        public Component TargetComponent;

        [PropertyOrder(20)]
        [Required]
        [LabelText("@TargetMemberLabel")]
        [ValueDropdown("GetComponentMemberNamesDropdown")]
        public string TargetMemberName = string.Empty;

        public MemberInfo TargetMember { get; protected set; }
        public BindingFlags MemberBindingFlags { get; set; }

        public virtual string TargetComponentLabel => "Target Component";
        public virtual string TargetObjectLabel => "Target Object";
        public virtual string TargetMemberLabel => "Target Member";
        public virtual bool ShowIncludePrivateInInspector { get; set; } = true;

        protected Func<MemberInfo, bool> ExtraMemberFilter;

        public MemberPicker(BindingFlags memberBindingFlags = BindingFlags.Instance | BindingFlags.Public)
        {
            MemberBindingFlags = memberBindingFlags;
            ExtraMemberFilter = _ => true;
        }

        public virtual void Initialize()
        {
            if (!TargetObject || !TargetComponent || string.IsNullOrEmpty(TargetMemberName))
            {
                return;
            }

            if (TryGetMember(out var member))
            {
                TargetMember = member;
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

        public virtual void StartExtraMemberFilter(Func<MemberInfo, bool> filter)
        {
            ExtraMemberFilter = filter;
        }

        public virtual void StopExtraMemberFilter()
        {
            ExtraMemberFilter = _ => true;
        }

#if UNITY_EDITOR


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
                .Where(ExtraMemberFilter)
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
