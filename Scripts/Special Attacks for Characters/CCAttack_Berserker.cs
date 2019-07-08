using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCAttack_Berserker : CCAttack
{
    //Editor Fields
    [SerializeField]
    private float dmg;

    [SerializeField]
    private float pushPower;

    [SerializeField]
    private float stunTime;

    [SerializeField]
    private float anticipationTime;

    [SerializeField]
    private float dashTime;

    [SerializeField]
    private float dashSpeed;

    [SerializeField]
    private GameObject character;

    [SerializeField]
    private Transform destinationPoint;

    [SerializeField]
    private float dashDistance;

    [SerializeField]
    private CapsuleCollider dashCollider;

    //Sequence control fields
    private bool takenControl = false;
    private bool isSetToAttack = false;
    private bool readyToDash;
    //private float coolDownVariable;
    private float anticipationVariable;

    //List for dash affected players
    [SerializeField]
    List<PlayerList> trappedPlayers = new List<PlayerList>();

    //Misc Variables
    private CharacterController controller;
    private Transform characterTransform;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private float lerpVariable;
    private float damageVariable;

    //Audio
    private AudioManager audioManager;
	[SerializeField] private Animator animator;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = character.GetComponent<CharacterController>();
        dashCollider.enabled = false;
        characterTransform = character.GetComponent<Transform>();
        anticipationVariable = anticipationTime;
        isSetToAttack = true;
        //coolDownVariable = coolDown;
        destinationPoint.localPosition = new Vector3(0, 0, dashDistance);
    }

	private void OnTriggerEnter(Collider other)
    {
        if(readyToDash)
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                if (other.gameObject.Equals(character))
                {
                    Debug.Log("I am dashing");
                }
                else
                {
                    trappedPlayers.Add(new PlayerList(other.gameObject));
                }
            }
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < trappedPlayers.Count; i++)
        {
            if (other.gameObject.Equals(trappedPlayers[i].objectPlayer))
            {
                if (trappedPlayers[i].playerManager != null)
                {
                    trappedPlayers[i].playerManager.Immobile(true, stunTime);
                    trappedPlayers.RemoveAt(i);
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        //if (coolDownVariable <= 0)
        //{
        //    isSetToAttack = true;
        //}
        //else
        //{
        //    coolDownVariable -= Time.deltaTime;
        //}
        if (isActivated)
        {
            if (isSetToAttack)
            {
                if(takenControl)
                {
                    if (anticipationVariable <= 0)
                    {
                        if (readyToDash)
                        {
                            if (lerpVariable < 1)
                            {
                                lerpVariable += Time.deltaTime / dashTime;
                                controller.Move((endPosition - startPosition) * Time.deltaTime * dashSpeed); //= Vector3.Lerp(startPosition, endPosition, lerpVariable);
                                if (trappedPlayers.Count > 0)
                                {
                                    foreach (PlayerList player in trappedPlayers)
                                    {
                                        if (player.playerManager != null)
                                        {
                                            if (player.playerManager != attackingPlayer)
                                            {
                                                // if other player dies
                                                if (player.playerManager.DealDamage(attackingPlayer, dmg * Time.deltaTime))
                                                {
                                                    // current player has killed a character
                                                    attackingPlayer.KilledCharacter();
                                                }
                                                player.playerManager.Push(player.playerTransform.position - characterTransform.position, pushPower);
                                            }
                                        }

                                    }
                                }
                            }
                            else
                            {
                                //Reset
                                dashCollider.enabled = false;
                                //isSetToAttack = false;
                                isActivated = false;
                                readyToDash = false;
                                takenControl = false;
                                anticipationVariable = anticipationTime;
                                //coolDownVariable = coolDown;
                                trappedPlayers.Clear();

								animator.SetBool("CCDashing", false);
							}

                        }
                        else
                        {
                            //Calculations
                            damageVariable = dmg * dmgMultiplyer;
                            //Preparation
                            dashCollider.enabled = true;
                            startPosition = characterTransform.position;
                            endPosition = destinationPoint.position;
                            //CCBerserker audio
                            if (audioManager != null)
                            {
                                audioManager.Play("Berserker_CC");
                            }
                            readyToDash = true;
                            lerpVariable = 0f;

							animator.SetBool("CCDashing", true);
						}

                    }
                    else
                    {
                        anticipationVariable -= Time.deltaTime;
                    }
                }
                else
                {
                    character.GetComponent<PlayerManager>().Immobile(false, anticipationVariable);
                    //CCstart audio
                    if (audioManager != null)
                    {
                        audioManager.Play("Character_CCStart");
                    }

                    takenControl = true;
                }
            }
            else
            {
                isActivated = false;
			}
        }
    }
}
