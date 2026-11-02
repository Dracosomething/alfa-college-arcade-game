using UnityEngine;

public class TestAbility : MonoBehaviour, IAbilityObject
{
    private GameObject player;

    private void Awake()
    {
        player = GameObject.Find("Player");
    }
    
    public bool ActivateAbility()
    {
        var pos = player.transform.position;

        pos.y += 1;

        player.transform.position = pos;
        return true;
    }
}