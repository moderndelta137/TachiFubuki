using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Slash_Control : MonoBehaviour
{
    [Header("Basic")]
    public bool Attack_charging;
    [Space]
    [Header("Aim")]
    public GameObject Target;//Will change to enemy script
    public Material target_material;
    private int aim_layerMask;
    private RaycastHit aim_hit;
    [SerializeField]private float aim_distance;
    [Space]
    [Header("Dash")]
    public float Dash_offset;
    public float Dash_duration;

    // Start is called before the first frame update
    void Start()
    {
        aim_layerMask = 1 << 6;
    }

    // Update is called once per frame
    void Update()
    {
        if(!Attack_charging)
        {
            if(Input.GetButtonDown("Attack"))
            {
                StartCharge();
            }
        }

        if(Attack_charging)
        {   
            Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out aim_hit, aim_distance, aim_layerMask);
            if(aim_hit.transform != null)
            {
                if(Target != aim_hit.transform.gameObject)
                {
                    if(target_material!=null)
                        target_material.DisableKeyword("_EMISSION");
                    Target = aim_hit.transform.gameObject;
                    target_material = Target.GetComponent<MeshRenderer>().material;
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
                ReleaseCharge();
            }
        }
    }

    void StartCharge()
    {
        Attack_charging = true;
    }

    void ReleaseCharge()
    {
        Vector3 target_vector;
        Vector3 dash_vector;
        Attack_charging = false;
        if(Target!=null)
        {
            //Calculate target direction vector
            target_vector = Target.transform.position - this.transform.position;

            //Calculate dash vector
            dash_vector = target_vector;
            dash_vector.y=0;
            dash_vector.Normalize();
            this.transform.DOMove(Target.transform.position + dash_vector*Dash_offset, Dash_duration);
            target_material.DisableKeyword("_EMISSION");
            target_material=null;
            Target = null;
        }
    }
}
