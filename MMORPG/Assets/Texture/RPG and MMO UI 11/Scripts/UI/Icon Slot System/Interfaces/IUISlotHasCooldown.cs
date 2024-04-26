namespace DuloGames.UI
{
    using UnityEngine.Events;

    public interface IUISlotHasCooldown
    {
        UISpellInfo GetSpellInfo();
        UISlotCooldown cooldownComponent { get; }
        void SetCooldownComponent(UISlotCooldown cooldown);
    }
}
