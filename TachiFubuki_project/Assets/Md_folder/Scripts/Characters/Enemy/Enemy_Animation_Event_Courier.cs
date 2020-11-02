using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Animation_Event_Courier : MonoBehaviour
{
    private Enemy_AI_Control enemy_ai_control;

    // Start is called before the first frame update
    private void Awake () 
    {
        enemy_ai_control = this.GetComponentInParent<Enemy_AI_Control>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void UnleashAttack()
    {
        enemy_ai_control.UnleashAttack();
    }
    public void EndAttack()
    {
        enemy_ai_control.EndAttack();
    }

}
