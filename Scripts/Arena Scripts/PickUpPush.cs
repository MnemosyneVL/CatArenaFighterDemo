using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpPush : MonoBehaviour
{
    //Variables seen through the editor
    [SerializeField]
    float pushPower;
    [SerializeField]
    float influenceRadius;
    [SerializeField]
    float lifeTime;
    
    public PickUpManager manager;
    //Misc Fields
    private Transform objectTransform;
    //Audio
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        objectTransform = GetComponent<Transform>();
        manager = FindObjectOfType<PickUpManager>();
    }
    private void Update()
    {
        if(lifeTime>0)
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
            foreach(PlayerList player in manager.currentPlayers)
            {
                if(Vector3.Distance(objectTransform.position, player.playerTransform.position) <= influenceRadius)
                {
                    Debug.Log("there are players in range");
                    if(player.objectPlayer.GetInstanceID() != other.gameObject.GetInstanceID())
                    {
                        player.playerManager.Push(player.playerTransform.position - objectTransform.position, pushPower);
                    }
                }
            }
        }
        //Push Audio
        audioManager.Play("Pickup_Tornado");

        Destroy(gameObject);
    }
}
