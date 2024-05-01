using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerInputType
{
    Nothing,
    Move
}

public class PlayerConditionInput : CharacterCondition
{
    [CharacterStateParam]
    public PlayerInputType InputType { get; set; }

    public override bool OnStateCondition()
    {
        throw new System.NotImplementedException();
    }
}
