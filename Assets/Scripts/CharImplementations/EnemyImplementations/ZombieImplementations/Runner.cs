using CharImplementations;
using CharImplementations.EnemyImplementations;
using UnityCommon.Runtime.Extensions;
using UnityEngine;

public class Runner : Zombie
{
    private int HASH_RUNNING = Animator.StringToHash("Running");
    private int HASH_EXPLODE = Animator.StringToHash("Explode");

    private Collider[] m_OverlapSphereResults = new Collider[1];

    private bool m_FirstDamage;

    private void Start()
    {
        AnimController.SetInt(HASH_ZOMBIE_ID, 2);
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void RiseFromTheDead()
    {
        base.RiseFromTheDead();
        
        var rand = Random.Range(0, 2);
        SetMovementTarget(rand == 0 ? ZombieTarget.Player : ZombieTarget.Door);
    }
    
    public override void StartAttack()
    {
        base.StartAttack();
        Attack();
    }

    public override void Attack()
    {
        AnimController.Trigger(HASH_EXPLODE);

        var count = Physics.OverlapSphereNonAlloc(transform.position, ZombieData.AttackRange, m_OverlapSphereResults,
            LayerMask.GetMask("Player"));

        if (count > 0)
        {
            PlayerExtensions.GetPlayer().GetDamage(ZombieData.DamagePerSecond);
        }

        gameObject.Destroy();
    }

    public override void StopAttack()
    {
        base.StopAttack();
    }

    public override void GetDamage(float damage)
    {
        base.GetDamage(damage);

        if (m_FirstDamage)
            return;

        m_FirstDamage = true;
        
        var moveSpeed = MoveSpeed * 2f;
        NavMeshAgent.speed = moveSpeed;
        SetMovementTarget(ZombieTarget.Player);
        
        AnimController.Trigger(HASH_RUNNING);
    }
}