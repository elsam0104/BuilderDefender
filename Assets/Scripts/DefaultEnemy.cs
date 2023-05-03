using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{ 
    [SerializeField]
    protected Transform visualTransform;

    protected Transform targetTransform;
    protected Rigidbody2D enemyRigidbody2D;
    protected HealthSystem healthSystem;

    protected float lookForTargetTimer;
    protected float lookForTargetTimerMax = .2f;
    [SerializeField]
    protected float moveSpeed = 5f;

    private void Start()
    {
        EnemyStart();
    }
    protected virtual void EnemyStart()
    {
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.OnDied += HealthSystem_OnDied;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;

        HandleMovement();
        HandleTargetting();
        VisualFlip();
    }
    protected virtual void HealthSystem_OnDamaged(object sender, System.EventArgs e)
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.EnemyHit);
        CinemachineShake.Instance.ShakeCamera(5f, .1f);
        ChromaticAberrationEffect.Instance.SetWeight(.5f);
    }

    protected virtual void HealthSystem_OnDied(object sender, System.EventArgs e)
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.EnemyDie);
        CinemachineShake.Instance.ShakeCamera(7f, .15f);
        ChromaticAberrationEffect.Instance.SetWeight(.5f);
        Instantiate(GameAssets.Instance.pfEnemyDieParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    protected virtual void VisualFlip()
    {
        if (targetTransform == null) return;
        Vector2 crossProduct = Vector3.Cross(transform.forward, targetTransform.position - transform.position);
        //print(crossProduct.y);
        if (crossProduct.y > 0)
        {
            visualTransform.localScale = new Vector3(visualTransform.localScale.x * -1, visualTransform.localScale.y, visualTransform.localScale.z);
        }
        else
        {
            visualTransform.localScale = new Vector3(Mathf.Abs(visualTransform.localScale.x), visualTransform.localScale.y, visualTransform.localScale.z);
        }
    }
    protected void HandleMovement()
    {
        if (targetTransform != null)
        {
            Vector3 moveDir = (targetTransform.position - transform.position).normalized;
            enemyRigidbody2D.velocity = moveDir * moveSpeed * DifficultyManager.instance.gameInfo.difficulty;
        }
        else
        {
            enemyRigidbody2D.velocity = Vector2.zero;
        }
    }

    protected virtual void HandleTargetting()
    {
        lookForTargetTimer -= Time.deltaTime;


        if (lookForTargetTimer < 0)
        {
            lookForTargetTimer += lookForTargetTimerMax;
            LookForTargets();
        }
    }


    protected virtual void LookForTargets()
    {
        float targetMaxRadius = 20f;
        Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(transform.position, targetMaxRadius);

        foreach (Collider2D collider2D in collider2DArray)
        {
            Building building = collider2D.GetComponent<Building>();
            if (building != null)
            {
                if (targetTransform == null)
                {
                    targetTransform = building.transform;
                }
                else
                {
                    if (Vector3.Distance(transform.position, building.transform.position) <
                        Vector3.Distance(transform.position, targetTransform.position))
                    {
                        targetTransform = building.transform;
                    }
                }
            }
        }

        if (targetTransform == null)
        {
            if (BuildingManager.Instance.GetHqBuilding() != null)
            {
                targetTransform = BuildingManager.Instance.GetHqBuilding().transform;
            }

        }
    }
}

public class DefaultEnemy : EnemyBase
{
    public static DefaultEnemy Create(Vector3 position)
    {
        Transform enemyTransform = Instantiate(GameAssets.Instance.pfEnemy, position, Quaternion.identity);
        DefaultEnemy enemy = enemyTransform.GetComponent<DefaultEnemy>();
        return enemy;
    }

    private void Awake()
    {
        enemyRigidbody2D = GetComponent<Rigidbody2D>();
    }

    protected override void EnemyStart()
    {
        if (BuildingManager.Instance.GetHqBuilding() != null)
        {
            targetTransform = BuildingManager.Instance.GetHqBuilding().transform;
        }
        lookForTargetTimer = Random.Range(0f, lookForTargetTimerMax);


        base.EnemyStart();
    }
    private void Update()
    {
        HandleMovement();
        HandleTargetting();
        //VisualFlip();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Building building = collision.gameObject.GetComponent<Building>();
        if (building != null)
        {
            HealthSystem healthSystem = building.GetComponent<HealthSystem>();
            healthSystem.Damage(10);
            this.healthSystem.Damage(999);
        }
    }


   
}
