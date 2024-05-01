using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIsPressingMove : CharacterCondition
{
    public override bool OnStateCondition()
    {
        return Character.Player.InputControls.Player.Move.IsPressed();
    }
}
