using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interfaces: MonoBehaviour
{
    
}
public interface IDamagable
{
    float HealthAmount { get; set; }
    float MaxHealthAmount { get; set; }

    bool DealDamage(PlayerManager otherplayer, float damageAmount);
    void DeathAction();
}
public interface IHealable
{
    float HealthAmount { get; set; }
    float MaxHealthAmount { get; set; }

    void Heal(float healingAmount);
}
public interface IMoveable
{
    void Push(Vector3 dirrecton, float force);
}
public interface IEffectable
{
    bool StunEffectActivated { get; set; }

    void StunEffect(float time);
}
