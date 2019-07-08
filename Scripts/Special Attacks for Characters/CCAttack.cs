using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCAttack : MonoBehaviour
{
    [SerializeField]
    protected PlayerManager attackingPlayer;

    //Modify Variables
    protected float dmgMultiplyer = 1;

    //Time Parametrs
    //public float coolDown;

    //Sequence Control Variables
    public bool isActivated;

    public virtual void UseWeaponCC()
    {
        isActivated = true;
        Debug.Log("CC Attack");
    }

    public void ChangeMultiplayer(float newDmg)
    {
        if(newDmg<=0)
        {
            newDmg = 1f;
        }
        dmgMultiplyer = newDmg;
    }
}
