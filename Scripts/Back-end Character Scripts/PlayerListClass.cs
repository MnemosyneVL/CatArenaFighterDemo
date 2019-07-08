using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerList
{
    public PlayerManager playerManager;
    public GameObject objectPlayer;
    public Transform playerTransform;

    public PlayerList(GameObject victim)
    {
        objectPlayer = victim;
        playerManager = victim.GetComponent<PlayerManager>();
        playerTransform = victim.transform;
    }
}
