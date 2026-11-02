using System;
using UnityEngine;

public class TestInteractible : MonoBehaviour, IInteractionObject
{
    private GameObject player;

    private void Awake()
    {
        player = GameObject.Find("Player");
    }

    public void Interact()
    {
        var pos = player.transform.position;

        pos.y += 1;

        player.transform.position = pos;
    }
}