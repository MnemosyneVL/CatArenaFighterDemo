using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCAttack_SmokeField : MonoBehaviour
{
    //delivered fields
    public float poisonDmg = 5f;
    public float poisonTime;
    public float smokeTime;
    public GameObject immuneCharacter;
    public float period = 1f;
    private float actionTime;
    private bool canDamage;

    //Misc Fields
    private CapsuleCollider influenceArea;

    [SerializeField]
    List<PlayerList> trappedPlayers = new List<PlayerList>();

    private void Start()
    {
        actionTime = 0f;
        influenceArea = GetComponent<CapsuleCollider>();
        trappedPlayers.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if (actionTime > period)
        {
            actionTime = 0f;
            canDamage = true;
        }
        else
        {
            canDamage = false;
            actionTime += Time.deltaTime;
        }

        if(trappedPlayers.Count > 0)
        {
            foreach (PlayerList player in trappedPlayers)
            {
                if (canDamage)
                {
                    if (player.playerManager != null)
                    {
                        player.playerManager.DealDamage(immuneCharacter.GetComponent<PlayerManager>(), poisonDmg / 6f);
                    }
                }
            }
        }

        if (smokeTime>0)
        {
            smokeTime -= Time.deltaTime;
        }
        else
        {
            influenceArea.enabled = false;
            for (int i = 0; i < trappedPlayers.Count; i++)
            {
                if (trappedPlayers[i].playerManager != null)
                {
                    trappedPlayers[i].playerManager.ContinuousDamage(immuneCharacter.GetComponent<PlayerManager>(), poisonDmg, poisonTime);
                }
                 trappedPlayers.RemoveAt(i);
            }
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            if(other.gameObject != immuneCharacter)
            {

                  trappedPlayers.Add(new PlayerList(other.gameObject));
                
            }
        }
    }
    //If object leaves trap's range it is removed from the list and is no longer effected by the trap 
    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < trappedPlayers.Count; i++)
        {
            if (other.gameObject.Equals(trappedPlayers[i].objectPlayer))
            {
                if (trappedPlayers[i].playerManager != null)
                {
                    trappedPlayers[i].playerManager.ContinuousDamage(immuneCharacter.GetComponent<PlayerManager>(), poisonDmg, poisonTime);

                }
                trappedPlayers.RemoveAt(i);
            }
        }
    }

}
