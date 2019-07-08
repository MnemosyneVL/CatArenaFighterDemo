using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCAttack_Ranger : CCAttack
{
    //Editor Fields
    //Variables for projectile launching
    [SerializeField]
    private float dmgPerSec;

    [SerializeField]
    private float stunTime;

    [SerializeField]
    private float projectileLifeTime;

    [SerializeField]
    private GameObject character;

    [SerializeField]
    private GameObject trapBoxPrefab;

    //Attack timers, forces and vectors

    [SerializeField]
    private float anticipationTime;

    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private Transform destinationPoint;

    [SerializeField]
    private float throwDistance;

    [SerializeField]
    private float projectileFlyTime;

    [SerializeField]
    private float localGravity;

    //Sequence control fields
    private bool takenControl = false;
    private bool isSetToFire;
    private bool readyToThrow;
    //private float coolDownVariable;
    private float anticipationVariable;

    //Other Variables
    private Vector3 throwVector;
    private float damageVariable;
    private Transform characterTransform;

    //Audio
    private AudioManager audioManager;

    [SerializeField] private Transform hand;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }


    // Start is called before the first frame update
    void Start()
    {
        anticipationVariable = anticipationTime;
        //coolDownVariable = coolDown;
        characterTransform = character.GetComponent<Transform>();
        destinationPoint.localPosition = new Vector3(0.1f, 0f, throwDistance);
        //throwVector = CalcThrowVector(destinationPoint.position,spawnPoint.position,projectileFlyTime);
        isSetToFire = true;
    }

    // Update is called once per frame
    void Update()
    {
        //if(coolDownVariable<=0)
        //{
        //    isSetToFire = true;
        //}
        //else
        //{
        //    coolDownVariable -= Time.deltaTime;
        //}
        if (isActivated)
        {
            if (isSetToFire)
            {
                if(takenControl)
                {
                    if (anticipationVariable <= 0)
                    {
                        //Calculations
                        damageVariable = dmgPerSec * dmgMultiplyer;
                        //Projectile instantiation
                        Debug.Log("Instantiates box");
                        GameObject instantiatedObject = Instantiate(trapBoxPrefab, spawnPoint.position, characterTransform.rotation);
                        instantiatedObject.GetComponent<Rigidbody>().velocity = throwVector;
                        CCAttack_Box boxScript = instantiatedObject.GetComponent<CCAttack_Box>();
                        boxScript.pManager = attackingPlayer;
                        boxScript.stunTimeVariable = stunTime;
                        boxScript.dmgPerSecVariable = damageVariable;
                        boxScript.secToDestroy = projectileLifeTime;
                        boxScript.projectileOwner = character;
                        //CCstart audio
                        if (audioManager != null)
                        {
                            audioManager.Play("Ranger_CC");
                        }
                        //Reset
                        //isSetToFire = false;
                        isActivated = false;
                        takenControl = false;
                        anticipationVariable = anticipationTime;
                        //coolDownVariable = coolDown;

                    }
                    else
                    {
                        anticipationVariable -= Time.deltaTime;
                    }
                }
                else
                {
                    character.GetComponent<PlayerManager>().Immobile(false, anticipationVariable);
                    throwVector = CalcThrowVector(destinationPoint.position, spawnPoint.position, projectileFlyTime);
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

        transform.position = hand.position;
        transform.eulerAngles = hand.eulerAngles;
    }

    //Throw Velocity Formula
    Vector3 CalcThrowVector(Vector3 destination, Vector3 startPoint, float time)
    {
        Vector3 distance = destination - startPoint;
        Vector3 distanceProjection = distance;
        distanceProjection.y = 0f;
        //Assign distances
        float lengthY = distance.y;
        float lengthProjection = distanceProjection.magnitude;
        //Calculate velocity
        float veloX = lengthProjection / time;
        float veloY = lengthY / time + 0.5f * localGravity * time;
        //Calculate fly vector
        Vector3 launchVector = distanceProjection.normalized;
        launchVector *= veloX;
        launchVector.y = veloY;

        return launchVector;
    }
}
