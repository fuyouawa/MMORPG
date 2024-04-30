using UnityEngine;

public abstract class CharacterCondition : MonoBehaviour
{
    public Character Character { get; set; }
    public string ExtraParam { get; set; }

    public abstract bool OnStateCondition();

    public virtual void OnStateEnter() { }

    public virtual void OnStateUpdate() { }

    public virtual void OnStateFixedUpdate() { }

    public virtual void OnStateNetworkFixedUpdate() { }

    public virtual void OnStateExit() { }
}
