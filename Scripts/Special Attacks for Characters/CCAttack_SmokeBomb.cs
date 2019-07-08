using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCAttack_SmokeBomb : MonoBehaviour
{
    //Fields assigned on launch
    public float poisonTimeVariable;
    public float smokeTimeVariable;
    public float dmgPerSecVariable;
    public float secToDestroy = 4f;
    public GameObject projectileOwner;

    //Misc Variable
    public GameObject smokeEffect;
    private GameObject instantiatedObject;
    private float secVariable;
    private float invisVariable;

    // Start is called before the first frame update
    private void Start()
    {
        secVariable = secToDestroy;
        invisVariable = poisonTimeVariable;
    }

    // Update is called once per frame
    void Update()
    {
        if (secVariable <= 0)
        {
            CreateSmokeField();
        }
        else
        {
            secVariable -= Time.deltaTime;
        }
    }

    private void CreateSmokeField()
    {
        instantiatedObject = Instantiate(smokeEffect, transform.position, Quaternion.identity);
        //instantiatedObject.transform.position = new Vector3(instantiatedObject.transform.position.x, projectileOwner.transform.position.y, instantiatedObject.transform.position.z);
        CCAttack_SmokeField smokeScript = instantiatedObject.GetComponent<CCAttack_SmokeField>();
        if (smokeScript != null)
        {
            smokeScript.poisonDmg = dmgPerSecVariable;
            smokeScript.poisonTime = poisonTimeVariable;
            smokeScript.smokeTime = smokeTimeVariable;
            smokeScript.immuneCharacter = projectileOwner;
        }
        else
        {
            Debug.Log("FUUUUUk");
        }
        Destroy(gameObject);
    }
}
