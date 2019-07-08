using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCAttack_Flashbang : MonoBehaviour
{
    //Fields assigned on launch
    public float stunTimeVariable;
    public float flashTimeVariable;
    public float dmgVariable;
    public float secToDestroy = 0.1f;
    public GameObject projectileOwner;

    //Misc Variable
    public GameObject flashEffect;
    private GameObject instantiatedObject;
    private float secVariable;

    // Start is called before the first frame update
    private void Start()
    {
        secVariable = secToDestroy;
    }

    // Update is called once per frame
    void Update()
    {
        if (secVariable <= 0)
        {
            CreateFlashField();
        }
        else
        {
            secVariable -= Time.deltaTime;
        }
    }

    private void CreateFlashField()
    {
        instantiatedObject = Instantiate(flashEffect, transform.position, Quaternion.identity);
        //instantiatedObject.transform.position = new Vector3(instantiatedObject.transform.position.x, projectileOwner.transform.position.y, instantiatedObject.transform.position.z);
        CCAttack_FlashField smokeScript = instantiatedObject.GetComponent<CCAttack_FlashField>();
        smokeScript.flashDmg = dmgVariable;
        smokeScript.stunTime = stunTimeVariable;
        smokeScript.flashTime = flashTimeVariable;
        smokeScript.immuneCharacter = projectileOwner;

        Destroy(gameObject);
    }
}
