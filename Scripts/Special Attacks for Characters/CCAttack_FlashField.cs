using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCAttack_FlashField : MonoBehaviour
{
    //delivered fields
    public float flashDmg;
    public float stunTime;
    public float flashTime;
    public GameObject immuneCharacter;


    private void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {

        if (flashTime > 0)
        {
            flashTime -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            if (other.gameObject != immuneCharacter)
            {
                PlayerList target = new PlayerList(other.gameObject);
                if (target.playerManager != null)
                {
                    if (flashDmg > 0)
                    {

                        target.playerManager.DealDamage(immuneCharacter.GetComponent<PlayerManager>(), flashDmg);
                    }
                    target.playerManager.Immobile(true, stunTime);
                }
            }
        }
    }
}
