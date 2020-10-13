using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_AI_Control : MonoBehaviour
{   
    public enum AI_states
    {
        Idle,
        Move,
        Attack
    }
    public AI_states AI_state;
    [Header("Nav Agent")]
    private NavMeshAgent nav_agent;
    [SerializeField] private bool can_move;
    [SerializeField] private bool can_lockon;
    [SerializeField] private float destination_tolerance;
    private GameObject target;
    [Space]
    [Header("Animation")]
    private Animator enemy_animator;
    //Attack duration is managed by animation
    [Space]
    [Header("Attack")]
    [SerializeField] private int attack_damage;
    [SerializeField] private Vector2 attack_cooldown_Range;
    private IEnumerator attack_coroutine;
    public bool Attack_charging;//Used for player to detect enemy charging state.
    private bool attacking;

    // Start is called before the first frame update
    void Start()
    {
        nav_agent = GetComponent<NavMeshAgent>();
        enemy_animator = GetComponent<Animator>();
        //attack_coroutine = AttackCoroutine();
        //StartCoroutine(attack_coroutine);
        target = GameObject.Find("Player");
        nav_agent.SetDestination(target.transform.position);
        //AI_state = AI_states.Idle;
        AI_state = AI_states.Move;
    }

    // Update is called once per frame
    void Update()
    {
        switch(AI_state)
        {
            case AI_states.Idle:
            return;

            case AI_states.Move:
            MoveAgent();
            return;

            case AI_states.Attack:
            return;
        }
    }

    void MoveAgent()
    {
        nav_agent.SetDestination(target.transform.position);
        //Debug.Log(nav_agent.remainingDistance);
        //Debug.Log(nav_agent.stoppingDistance);
        if(nav_agent.remainingDistance - nav_agent.stoppingDistance <　destination_tolerance)
        {
            Debug.Log("stop");
            StartCoroutine(AttackCoroutine());
        }
    }

    private IEnumerator AttackCoroutine()
    {
        //while(true)
        {
            Debug.Log("Attack");
            AI_state = AI_states.Attack;
            nav_agent.enabled = false;
            ChargeAttack();
            yield return new WaitForSeconds(Random.Range(attack_cooldown_Range.x,attack_cooldown_Range.y));
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
        AI_state = AI_states.Move;
        nav_agent.enabled = true;
        nav_agent.SetDestination(target.transform.position);
        Debug.Log("EndAttack");
    }
}
