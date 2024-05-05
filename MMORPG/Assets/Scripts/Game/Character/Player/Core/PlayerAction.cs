using System;
using System.Collections;
using System.Linq;
using Sirenix.OdinInspector;

[Serializable]
public class PlayerAction
{
    //TODO AbilityName check valid
    [Required("Can't be none!")]
    [VerticalGroup("Ability")]
    [ValueDropdown("GetAbilityDropdown")]
    [HideLabel]
    public string AbilityName = string.Empty;

    //TODO Delay
    [TableColumnWidth(50, false)]
    public float Delay;

    public PlayerAbility Ability {get; private set; }
    public PlayerState OwnerState { get; set; }

    public void Initialize(PlayerState state)
    {
        OwnerState = state;
        Ability = OwnerState.Brain.GetAttachAbilities().First(x => x.GetType().Name == AbilityName);
    }

    public IEnumerable GetAbilityDropdown()
    {
        var total = new ValueDropdownList<string> { { "None Ability", string.Empty } };
        if (OwnerState != null)
        {
            total.AddRange(OwnerState.Brain.GetAttachAbilities().Select((x, i) =>
                new ValueDropdownItem<string>($"{i} - {x.GetType().Name}", x.GetType().Name))
            );
        }
        return total;
    }
}
