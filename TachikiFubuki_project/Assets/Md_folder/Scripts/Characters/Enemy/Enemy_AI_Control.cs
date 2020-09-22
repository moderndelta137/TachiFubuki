using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AI_Control : MonoBehaviour
{
    [Header("Animation")]
    private Animator enemy_animator;
    //Attack duration is managed by animation
    [Space]
    [Header("Attack")]
    public int Attack_damage;
    public Vector2 Attack_cooldown_Range;
    private IEnumerator attack_coroutine;
    public bool Attack_charging;
    private bool attacking;

    // Start is called before the first frame update
    void Start()
    {
        enemy_animator = GetComponent<Animator>();
        attack_coroutine = AttackCoroutine();
        StartCoroutine(attack_coroutine);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator AttackCoroutine()
    {
        while(true)
        {
            ChargeAttack();
            yield return new WaitForSeconds(Random.Range(Attack_cooldown_Range.x,Attack_cooldown_Range.y));
        }
    }

    private void ChargeAttack()
    {
        
        attacking=true;
        Attack_charging=true;
        enemy_animator.SetTrigger("Attack");
        Debug.Log("ChargeAttack");
    }

    //Animation Event
    public void UnleashAttack()
    {
        Attack_charging=false;
        Debug.Log("UnleashAttack");
    }

    //Animation Event
    public void EndAttack()
    {
        attacking=false;
        Debug.Log("EndAttack");
    }
}
