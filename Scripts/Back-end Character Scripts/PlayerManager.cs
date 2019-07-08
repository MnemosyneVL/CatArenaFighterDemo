using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour, IDamagable, IHealable, IMoveable
{
    public int killsNeeded = 5;
    public float deathSeconds = 2f;

    // For IHealable interface
    [SerializeField] float healthVariable;
    [SerializeField] float maxHealthVariable;
    [SerializeField] Image healthFlash;
    [SerializeField] Image healthBg;
    [SerializeField] Image healthBar;
    [SerializeField] GameObject flashBar;
    [SerializeField] DamageUI dmgUI;

    //For healthbar
    private bool hasFlashed;
    private float healthShrinkTime;


    // For IMoveable interface
    public CharacterController charComponent;
    float mass = 3.0f; // defines the character mass
    Vector3 impact = Vector3.zero;

    // To keep track of scores
    private int hasDied = 0, hasKilled = 0;

    //BasicMovement script for Stun effect and ability to take control over character
    private BasicMovement charMovement;


    // Needed for being off
    public bool death = false, canHurt = true;
    [SerializeField] private float invicibleTime = 5f;
    [SerializeField] public GameObject looks;

    [SerializeField] private HitAnimation hitAnimation;

    private Animator animator;
    [SerializeField] private float timeDeathAnimation = 3.5f;
    [SerializeField] private float timeDamageAnimation = 1f;

    [SerializeField] private GameObject ghost;

    //Audio
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void SetAnimator(Animator anim)
    {
        animator = anim;
    }

    // For IDamagable and IHealable interfaces
    public float HealthAmount
    {
        get { return healthVariable; }
        set {
            dmgUI.ShowDamage(value - healthVariable);

            healthVariable = value;
            UpdateHealthBar();
        }
    }
    public float MaxHealthAmount
    {
        get { return maxHealthVariable; }
        set { maxHealthVariable = value; }
    }

    public void UpdateHealthBar()
    {
        if (healthBar == null) return;

        if (maxHealthVariable == HealthAmount)
            healthBar.fillAmount = 1;
        else if (0 >= HealthAmount)
            healthBar.fillAmount = 0;
        else
            healthBar.fillAmount = HealthAmount / maxHealthVariable;
    }

    public int HasDied
    {
        get { return hasDied; }
        private set { hasDied++; }
    }

    public int HasKilled
    {
        get { return hasKilled; }
        private set { hasKilled++; }
    }

    // For IDamagable interface
    public bool DealDamage(PlayerManager otherplayer, float damageAmount)
    {
        if (!canHurt) return false;

        //Damage audio
        //audioManager.Play("Character_GetDamage");//     -------------------------------- HERE

        HealthAmount -= damageAmount;

        if (animator != null)
            StartCoroutine(Animation(damageAmount));

        if (HealthAmount <= 0)
        {
            if(otherplayer == null)
            {
                hasKilled--;
            }
            DeathAction();
            return true;
        }
        return false;
    }

    // If animation available
    public IEnumerator Animation(float dmg)
    {
        bool flash = false;
        float time = timeDamageAnimation / 3;

        if(hitAnimation != null)
            hitAnimation.EmissionOn();

        if (dmg > 35)
            animator.SetTrigger("ExtraDamage");
        else
            animator.SetTrigger("Damage");

        // charMovement.attack = true;

        // apparantly for() does not work in IEnumerator

        // off
        flashBar.SetActive(flash);
        flash = !flash;
        yield return new WaitForSeconds(time);

        // on
        flash = OfOnn(flashBar, flash);
        yield return new WaitForSeconds(time);

        // off
        flash = OfOnn(flashBar, flash);
        yield return new WaitForSeconds(time);

        // on
        OfOnn(flashBar, flash);

        if (hitAnimation != null)
            hitAnimation.EmissionOff();

        charMovement.attack = false;
    }

    public bool OfOnn(GameObject obj, bool flash)
    {
        obj.SetActive(flash);
        return !flash;
    }

    //For continuous damage substractions: poison/ being in trap box
    public void ContinuousDamage(PlayerManager attackingPlayer, float damageAmount, float time)
    {
        StartCoroutine(SubstractHealth(attackingPlayer,damageAmount,time));
    }

    public void Immobile(bool useStunAnimation, float time)
    {
        StartCoroutine(StunCharacter(useStunAnimation, time));
    }

    public void DeathAction()
    {
        //Death audio
        audioManager.Play("Character_Death");

        StartCoroutine(BeingDeath());
    }

    public IEnumerator BeingDeath()
    {
        healthBar.fillAmount = 0;
        death = true;
        canHurt = false;

        if (animator != null)
        {
            animator.SetTrigger("Death");

            yield return new WaitForSeconds(timeDeathAnimation/2);
            // Show ghost
            // Show Ghost
            if (ghost != null)
            {
                ghost.SetActive(true);

                Animator anim = ghost.GetComponent<Animator>();
                anim.SetTrigger("Rise");
            }
            yield return new WaitForSeconds(timeDeathAnimation / 2);
        }
        else
        {
            // Show Ghost
            if (ghost != null)
            {
                ghost.SetActive(true);

                Animator anim = ghost.GetComponent<Animator>();
                anim.SetTrigger("Rise");
            }
        }

        looks.SetActive(false);

        HasDied = 1;

        yield return new WaitForSeconds(deathSeconds);

        charMovement.MoveToSpawnPoint();

        if (ghost != null)
            ghost.SetActive(false);

        if (looks != null)
            looks.SetActive(true);

        HealthAmount = maxHealthVariable;
        healthFlash.enabled = true;
        death = false;
        StartCoroutine(CanHurt());
        animator.SetBool("Game", true);
    }

    IEnumerator CanHurt()
    {
        yield return new WaitForSeconds(invicibleTime);
        healthFlash.enabled = false;
        canHurt = true;
    }

    IEnumerator SubstractHealth(PlayerManager attackingPlayer, float damage, float time)
    {
        while(time>0)
        {
            time -= Time.deltaTime;
            if(DealDamage(attackingPlayer, damage * Time.deltaTime))
            {
                attackingPlayer.KilledCharacter();
            }
            yield return null;
        }
    }
    IEnumerator StunCharacter(bool playAnimation, float time)
    {
        bool stunned = false;
        while (time > 0)
        {
            if(!stunned)
            {
                charMovement.canMove = false;
                if(playAnimation)
                {
                    Debug.Log("Stun animation");
                }
                stunned = true;
                
            }
            else
            {
                time -= Time.deltaTime;
                yield return null;
            }
           
        }
        charMovement.canMove = true;
    }
    // For IHealable interface
    public void Heal(float healingAmount)
    {
        if (HealthAmount + healingAmount > maxHealthVariable)
        {
            HealthAmount = maxHealthVariable;
        }
        else
        {
            HealthAmount += healingAmount;
        }
    }

    // Change rigidbody to character controller
    // For IMoveable interface
    public void Push(Vector3 pushDirrection, float pushForce)
    {
        if (death) return;

        pushDirrection.y = 0;
        pushDirrection.Normalize();
        if (pushDirrection.y < 0) pushDirrection.y = -pushDirrection.y; // reflect down force on the ground
        impact += pushDirrection.normalized * pushForce / mass;
    }

    // Start is called before the first frame update
    void Start()
    {

        healthFlash.enabled = false;
        // Rigidbody to Character controller
        charComponent = gameObject.GetComponent<CharacterController>();
        HealthAmount = maxHealthVariable;
        charMovement = gameObject.GetComponent<BasicMovement>();

        if (ghost != null)
            ghost.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // apply the impact force:
        if (impact.magnitude > 0.2) charComponent.Move(impact * Time.deltaTime);
        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);

        if(healthBar.fillAmount >= healthBg.fillAmount)
        {
            healthBg.fillAmount = healthBar.fillAmount;
            hasFlashed = false;
            healthShrinkTime = 0.5f;
        }
        else if(healthBar.fillAmount < healthBg.fillAmount)
        {
            if (hasFlashed)
            {
                healthBg.fillAmount -= Time.deltaTime * 0.5f;
            }
            else
            {
                if (healthShrinkTime > 0)
                {
                    healthShrinkTime -= Time.deltaTime;
                }
                else
                {
                    hasFlashed = true;
                }
            }
        }

        // Check if needed kills
        if (HasKilled >= killsNeeded)
        {
            Debug.Log("Won");
            //Victory audio
            audioManager.Play("General_Victory");

            GameObject level = GameObject.Find("LevelManager");
            if (level != null)
            {
                LevelManager manager = level.GetComponent<LevelManager>();
                manager.EndLevel();
            }
        }
    }

    public void KilledCharacter()
    {
        HasKilled = 1;

        if(HasKilled >= killsNeeded)
        {
            Debug.Log("Won");
            //Victory audio
            audioManager.Play("General_Victory");

            GameObject level = GameObject.Find("LevelManager");
            if(level != null)
            {
                LevelManager manager = level.GetComponent<LevelManager>();
                manager.EndLevel();
            }
        }
    }
}
