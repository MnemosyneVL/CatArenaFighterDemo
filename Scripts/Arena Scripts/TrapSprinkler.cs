using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSprinkler : MonoBehaviour
{

    //Trap Variables adjustable via editor
    [SerializeField]
    float trapDamage;
    [SerializeField]
    float pushPower;
    [SerializeField]
    float rotationSpeed;
    [SerializeField]
    float workTime;
    [SerializeField]
    float maxAngle;
    [SerializeField]
    float minAngle;

    //Trap public variables
    public bool isWorking;

    //Trap misc fields
    private Transform trapTransform;
    private BoxCollider trapInfluenceArea;
    [SerializeField]
    ParticleSystem particles;
    private float workTimeVariable;
    private bool timeSet = false;
    

    //Rotation fields
    private bool rotateLeft = false;

    //List that stores all traped players
    [SerializeField]
    List<PlayerList> trappedPlayers = new List<PlayerList>();

    private void Start()
    {
        trapTransform = GetComponent<Transform>();
        trapInfluenceArea = GetComponent<BoxCollider>();
        trapInfluenceArea.enabled = false;
        particles.gameObject.SetActive(false);
        rotateLeft = false;
    }

    private void Update()
    {
        if(isWorking)
        {
            if(timeSet)
            {
                if(workTimeVariable >0)
                {
                    workTimeVariable -= Time.deltaTime;
                }
                else
                {
                    isWorking = false;
                    timeSet = false;
                    trapInfluenceArea.enabled = false;
                    particles.gameObject.SetActive(false);

                }
                if(!rotateLeft)
                {
                    if (trapTransform.eulerAngles.y < maxAngle)
                    {
                        trapTransform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
                    }
                    else
                    {
                        rotateLeft = true;
                    }
                }
                else
                {
                    if (trapTransform.eulerAngles.y > minAngle)
                    {
                        trapTransform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
                    }
                    else
                    {
                        rotateLeft = false;
                    }
                }
                if (trappedPlayers.Count > 0)
                {
                    foreach (PlayerList player in trappedPlayers)
                    {
                        if(player.playerManager != null)
                        {
                            player.playerManager.DealDamage(null, trapDamage * Time.deltaTime);
                            player.playerManager.Push(player.playerTransform.position - trapTransform.position, pushPower);
                        }
                        
                    }
                }
            }
            else
            {
                workTimeVariable = workTime;
                trapInfluenceArea.enabled = true;
                particles.gameObject.SetActive(true);
                timeSet = true;
            }
            
        }
    }
    //If object enters trap's range and this object can be influenced by the trap (like player), it's added to the list 
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Player"))
        {
            trappedPlayers.Add(new PlayerList(other.gameObject));
        }
    }
    //If object leaves trap's range it is removed from the list and is no longer effected by the trap 
    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < trappedPlayers.Count; i++)
        {
            if(other.gameObject.Equals(trappedPlayers[i].objectPlayer))
            {
                trappedPlayers.RemoveAt(i);
            }
        }
    }

}
