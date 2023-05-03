using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boss : EnemyBase
{
    private int destroyedBuildings = 0;
    private bool isEnded = false;
    private bool usingSkill = false;
    private float skillTerm = 3f;
    private float skillTimer = 0f;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private ResourceTypeSO stoneSO;
    [SerializeField]
    private ResourceTypeSO bossStoneSO;
    [SerializeField]
    private BoxCollider2D attackArray;
    private List<Building> targetBuildings = new List<Building>();
    public static Boss Create(Vector3 posision)
    {
        Transform bossTransform = Instantiate(GameAssets.Instance.pfBoss, posision, Quaternion.identity);
        Boss boss = bossTransform.GetComponent<Boss>();
        return boss;
    }
    private void Awake()
    {
        enemyRigidbody2D = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        skillTimer += Time.deltaTime;
        HandleMovement();
        HandleTargetting();
        VisualFlip();
        Skill();
    }
    protected override void VisualFlip()
    {
        if (!usingSkill)
            base.VisualFlip();
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
    protected override void HealthSystem_OnDamaged(object sender, EventArgs e)
    {
        base.HealthSystem_OnDamaged(sender, e);
    }
    protected override void HealthSystem_OnDied(object sender, EventArgs e)
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.EnemyDie);
        CinemachineShake.Instance.ShakeCamera(15f, 1f);
        ChromaticAberrationEffect.Instance.SetWeight(.5f);
        Instantiate(GameAssets.Instance.pfEnemyDieParticles, transform.position, Quaternion.identity);
        Instantiate(GameAssets.Instance.pfBossResource, transform.position, Quaternion.identity);
        ResourceManager.Instance.AddResource(stoneSO, 500);
        ResourceManager.Instance.AddResource(bossStoneSO, 1);
        Destroy(gameObject);
    }
    protected override void HandleTargetting()
    {
        if (usingSkill) return;
        if (destroyedBuildings > 2 + 3 * DifficultyManager.instance.gameInfo.difficulty)
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;

            if (!isEnded)
            {
                LookingForEndPlace();
                isEnded = true;
            }
        }
        else
            base.HandleTargetting();
    }
    //protected override void LookForTargets()
    //{
    //    float targetMaxRadius = 999f;
    //    Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(transform.position, targetMaxRadius);

    //    foreach (Collider2D collider2D in collider2DArray)
    //    {
    //        Building building = collider2D.GetComponent<Building>();
    //        if (building != null)
    //        {
    //            if (targetTransform == null)
    //            {
    //                targetTransform = building.transform;
    //            }
    //            else
    //            {
    //                if (Random.Range(0, 10) == 2) //그냥 랜덤 히히
    //                {
    //                    targetTransform = building.transform;
    //                }
    //            }
    //        }
    //    }

    //    if (targetTransform == null)
    //    {
    //        if (BuildingManager.Instance.GetHqBuilding() != null)
    //        {
    //            targetTransform = BuildingManager.Instance.GetHqBuilding().transform;
    //        }

    //    }
    //}
    private void LookingForEndPlace()
    {
        targetTransform.position = EnemyWaveManager.Instance.GetSpawnPosition();
    }
    private void Skill()
    {
        var targets = Physics2D.OverlapBoxAll(transform.position, attackArray.size, 0f);
        targetBuildings.Clear();
        foreach (var target in targets)
        {
            Building building = target.GetComponent<Building>();
            if (building != null)
            {
                targetBuildings.Add(building);
                building.onBuildingDied -= BuildingDied;
                building.onBuildingDied += BuildingDied;
            }
        }

        if (targetBuildings.Count > 0 && usingSkill == false && skillTimer >= skillTerm)
        {
            usingSkill = true;
            skillTimer = 0f;
            animator.SetTrigger("skill");
        }
    }
    private void BuildingDied(object sender, System.EventArgs e)
    {
        destroyedBuildings++;
    }
    public void AttackForTargets()
    {
        foreach (var target in targetBuildings)
        {
            target.gameObject.GetComponent<HealthSystem>().Damage(20);
        }
        usingSkill = false;
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position, attackArray.size);
    }
}
