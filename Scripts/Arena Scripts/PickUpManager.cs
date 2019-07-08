using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpManager : MonoBehaviour
{
    //public player list
    public List<PlayerList> currentPlayers = new List<PlayerList>();

    //Editor varibles
    [SerializeField]
    float spawnCoolDown;
    [SerializeField]
    GameObject[] pickUpsPool;
    [SerializeField]
    float playerDistanceSpawn;

    //Misc Fields
    private List<Transform> spawnPoints = new List<Transform>();
    private float coolDownVariable;
    private int currentSpawnPoint;
    private int lastSpawnPoint;

    //Bool switches
    private bool spawnPointFound;
    private bool canSpawn;
    //Audio
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        //{
        //    currentPlayers.Add(new PlayerList(player));
        //}
        foreach (GameObject point in GameObject.FindGameObjectsWithTag("PickUpSpawnPoint"))
        {
            spawnPoints.Add(point.GetComponent<Transform>());
        }
        coolDownVariable = spawnCoolDown;
    }

    // Update is called once per frame
    void Update()
    {
        if (coolDownVariable > 0)
        {
            coolDownVariable -= Time.deltaTime;
        }
        else
        {
            if(!spawnPointFound)
            {
                currentSpawnPoint = Random.Range(0, spawnPoints.Count);
                spawnPointFound = true;
            }
            else
            {
                if (spawnPoints[currentSpawnPoint].childCount>0)
                {
                    spawnPointFound = false;
                }
                else
                {
                    canSpawn = true;
                    foreach(PlayerList player in currentPlayers)
                    {
                        if(player.playerTransform != null)
                        {
                            Debug.Log("Has transform");
                            if (Vector3.Distance(player.playerTransform.position, spawnPoints[currentSpawnPoint].position) < playerDistanceSpawn)
                            {
                                canSpawn = false;
                                spawnPointFound = false;
                            }
                        }
                        else
                        {
                            Debug.Log("Faulty");
                        }
                    }
                    if (canSpawn)
                    {
                        GameObject pickup = Instantiate(pickUpsPool[Random.Range(0, pickUpsPool.Length)], spawnPoints[currentSpawnPoint]);
                        //PickupSpawn Audio
                        audioManager.Play("Pickup_Spawn");

                        spawnPointFound = false;
                        coolDownVariable = spawnCoolDown;
                    }
                    else
                    {
                        spawnPointFound = false;
                    }
                }
            }
        }
    }

    public void SearchForPlayers()
    {
        currentPlayers.Clear();
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            currentPlayers.Add(new PlayerList(player));

        }
    }
}
