using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCAttack_Rogue : CCAttack
{
    //Editor Fields
    //Variables for projectile launching
    [SerializeField]
    private float dmgPerSec;

    [SerializeField]
    private float poisonTime;

    [SerializeField]
    private float smokeFieldTime;

    [SerializeField]
    private float projectileLifeTime;

    [SerializeField]
    private GameObject character;

    [SerializeField]
    private GameObject smokeBombPrefab;

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
        characterTransform = character.GetComponent<Transform>();
        destinationPoint.localPosition = new Vector3(0.1f, 0f, throwDistance);
        isSetToFire = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActivated)
        {
            if (isSetToFire)
            {
                if (takenControl)
                {
                    if (anticipationVariable <= 0)
                    {
                        //Calculations
                        damageVariable = dmgPerSec * dmgMultiplyer;
                        throwVector = CalcThrowVector(destinationPoint.position, spawnPoint.position, projectileFlyTime);
                        //Projectile instantiation
                        Debug.Log("Instantiates smoke bomb");
                        GameObject instantiatedObject = Instantiate(smokeBombPrefab, spawnPoint.position, characterTransform.rotation);
                        instantiatedObject.GetComponent<Rigidbody>().velocity = throwVector;
                        CCAttack_SmokeBomb bombScript = instantiatedObject.GetComponent<CCAttack_SmokeBomb>();
                        //bombScript.pManager = attackingPlayer;
                        bombScript.secToDestroy = projectileFlyTime;
                        bombScript.poisonTimeVariable = poisonTime;
                        bombScript.smokeTimeVariable = smokeFieldTime;
                        bombScript.dmgPerSecVariable = damageVariable;
                        bombScript.projectileOwner = character;
                        //CCstart audio
                        if (audioManager != null)
                        {
                            audioManager.Play("Rogue_CC");
                        }
                        //Reset
                        isActivated = false;
                        takenControl = false;
                        anticipationVariable = anticipationTime;

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
