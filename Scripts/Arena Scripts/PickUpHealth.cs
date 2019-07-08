using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpHealth : MonoBehaviour
{
    //Variables modified from editor
    [SerializeField]
    float healingAmount;
    [SerializeField]
    float lifeTime;
    //Audio
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Update()
    {
        if (lifeTime > 0)
        {
            lifeTime -= Time.deltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Player"))
        {
            other.gameObject.GetComponent<PlayerManager>().Heal(healingAmount);
            //Heal Audio
            audioManager.Play("Pickup_Heal");

            Destroy(gameObject);
        }
    }
}
