using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCAttack_Box : MonoBehaviour
{
    public PlayerManager pManager;

    //Fields assigned on launch
    public float stunTimeVariable;
    public float dmgPerSecVariable;
    public float secToDestroy = 4f;
    public GameObject projectileOwner;

    //Misc Variable
    public GameObject coveringBox;
    private GameObject instantiatedObject;
    private float secVariable;
    private float invisVariable;

    // Start is called before the first frame update
    private void Start()
    {
        secVariable = secToDestroy;
        invisVariable = stunTimeVariable;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject != projectileOwner)
        {
            if(other.gameObject.tag.Equals("Player"))
            {
                PlayerManager victimManager = other.GetComponent<PlayerManager>();
                if (victimManager != null)
                {
                    instantiatedObject = Instantiate(coveringBox, other.transform);
                    instantiatedObject.transform.position = new Vector3(instantiatedObject.transform.position.x, instantiatedObject.transform.position.y + 0.5f, instantiatedObject.transform.position.z);


                    victimManager.ContinuousDamage(pManager, dmgPerSecVariable, stunTimeVariable);
                    victimManager.Immobile(false, stunTimeVariable);

                    Destroy(instantiatedObject, stunTimeVariable);
                }
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Not Player");
                //Destroy(gameObject);
            }     
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(secVariable <=0)
        {
            Destroy(gameObject);
        }
        else
        {
            secVariable -= Time.deltaTime;
        }
    }
}
