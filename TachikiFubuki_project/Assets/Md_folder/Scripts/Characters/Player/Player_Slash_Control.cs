using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player_Slash_Control : MonoBehaviour
{
    public enum Slash_states
    {
        Move,
        Aim,
        Slash,
    }
    [Header("Basic")]
    public Slash_states Slash_state;
    [Space]
    [Header("Aim")]
    public GameObject Target;//Will change to enemy script
    [SerializeField]private Material target_material = null;
    private int aim_layerMask;
    private RaycastHit aim_hit;
    [SerializeField]private float aim_distance = 0;
    [Space]
    [Header("Dash")]
    [SerializeField]private float Dash_offset = 0;
    [SerializeField]private float Dash_deflected_offset = 0;
    [SerializeField]private float Dash_duration = 0;
    [Space]
    [Header("Slash")]
    [SerializeField]private int Slash_damage = 0;
    // Start is called before the first frame update
    void Start()
    {
        aim_layerMask = 1 << 6;
    }

    // Update is called once per frame
    void Update()
    {
        switch(Slash_state)
        {
            case Slash_states.Move:
                if(Input.GetButtonDown("Attack"))
                {
                    EnterAiming();
                }
            break;
            case Slash_states.Aim:
                Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out aim_hit, aim_distance, aim_layerMask);
                if(aim_hit.transform != null)
                {
                    if(Target != aim_hit.transform.gameObject)
                    {
                        if(target_material!=null)
                            target_material.DisableKeyword("_EMISSION");
                        Target = aim_hit.transform.gameObject;
                        target_material = Target.GetComponentInChildren<MeshRenderer>().material;
                        target_material.EnableKeyword("_EMISSION");
                    }
                }
                else
                {
                    if(Target != null)
                    {
                        target_material.DisableKeyword("_EMISSION");
                        target_material=null;
                    }
                    Target = null;
                }
                if(Input.GetButtonUp("Attack"))
                {
                    //DEBUG Only
                    UnleashSlash();
                    //EnterSlashing();
                }
            break;
            case Slash_states.Slash:
                if(Input.GetButtonUp("Attack"))
                {
                    UnleashSlash();
                }
            break;
        }
    }

    void EnterAiming()
    {
        Slash_state = Slash_states.Aim;
    }
    void EnterSlashing()
    {
        Slash_state = Slash_states.Slash;
    }
    void UnleashSlash()
    {
        Vector3 target_vector;
        Vector3 dash_vector;
        Enemy_AI_Control enemy_script;
        Slash_state = Slash_states.Move;
        if(Target!=null)
        {
            //Calculate target direction vector
            target_vector = Target.transform.position - this.transform.position;

            //Calculate dash vector
            dash_vector = target_vector;
            dash_vector.y=0;
            dash_vector.Normalize();


            //Apply Damage
            if(Target.tag == "Enemy")
            {
                enemy_script = Target.GetComponent<Enemy_AI_Control>();
                if(enemy_script.Attack_charging)
                {
                    Target.SendMessage("ApplyDamage",Slash_damage);
                    dash_vector *= Dash_offset;
                }
                else
                {
                    dash_vector *= Dash_deflected_offset;
                }
            }
            this.transform.DOMove(Target.transform.position + dash_vector, Dash_duration);


            //Reset Target
            target_material.DisableKeyword("_EMISSION");
            target_material=null;
            Target = null;
        }
    }
}
