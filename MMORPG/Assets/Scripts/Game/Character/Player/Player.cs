using MMORPG;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Character Character;
    public GameInputControls InputControls;

    private void Awake()
    {
        InputControls = new();
    }

    private void OnEnable()
    {
        InputControls.Enable();
    }

    private void OnDisable()
    {
        InputControls.Disable();
    }
}
