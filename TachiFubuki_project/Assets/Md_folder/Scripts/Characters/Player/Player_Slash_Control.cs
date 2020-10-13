using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

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
    private Topdown_Locomotion topdown_locomotion;
    private ThirdPerson_Locomotion thirdperson_locomotion;
    [Space]
    [Header("Aim")]
    private Camera main_cam;
    [SerializeField]private CinemachineVirtualCamera CM_topdown_cam;
    [SerializeField]private CinemachineFreeLook CM_aiming_cam;
    [SerializeField]private CinemachineFreeLook CM_slashing_cam;
    [SerializeField]private float slash_time_scale;
    public GameObject Target;//Will change to enemy script
    [SerializeField]private Material target_material = null;
    private int aim_layerMask;
    private RaycastHit aim_hit;
    [SerializeField]private float aim_cast_radius = 0;
    [SerializeField]private float aim_cast_distance = 0;
    [SerializeField]private float aim_transition_duration = 0;
    private WaitForSecondsRealtime aim_wait;
    [Space]
    [Header("Dash")]
    [SerializeField]private float dash_offset = 0;
    [SerializeField]private float dash_deflected_offset = 0;
    [SerializeField]private float dash_duration = 0;
    [Space]
    [Header("Slash")]
    [SerializeField]private int slash_damage = 0;
    [SerializeField]private float slash_input_threshold = 0;
    [SerializeField]private float slash_wait_duration = 0;
    private WaitForSecondsRealtime slash_wait;
    // Start is called before the first frame update
    void Start()
    {
        topdown_locomotion = GetComponent<Topdown_Locomotion>();
        thirdperson_locomotion = GetComponent<ThirdPerson_Locomotion>();
        aim_layerMask = 1 << 6;
        main_cam = Camera.main;
        aim_wait = new WaitForSecondsRealtime(aim_transition_duration);
        slash_wait = new WaitForSecondsRealtime(slash_wait_duration);
    }

    // Update is called once per frame
    void Update()
    {
        switch(Slash_state)
        {
            case Slash_states.Move:
                AlineAimCam();
                if(Input.GetButtonDown("Attack"))
                {
                    EnterAiming();
                }
            break;

            case Slash_states.Aim:
                HighlightTarget();
                if(Input.GetButtonUp("Attack"))
                {
                    EnterSlashing();
                }
            break;

            case Slash_states.Slash:
                HighlightTarget();
                CalculateSlashInput();

                //FOR DEBUG ONLY
                if(Input.GetButtonDown("Cancel"))
                {
                    if(Target!= null){
                    //Reset Target
                    target_material.DisableKeyword("_EMISSION");
                    target_material=null;
                    Target = null;
                    }

                    //Reset Camera to topdown
                    thirdperson_locomotion.enabled=false;
                    thirdperson_locomotion.Can_control = false;
                    Slash_state = Slash_states.Move;
                    Time.timeScale = 1.0f;
                    CM_slashing_cam.gameObject.SetActive(false);
                    CM_aiming_cam.gameObject.SetActive(false);
                    CM_topdown_cam.gameObject.SetActive(true);
                    topdown_locomotion.enabled=true;
                }
            break;
        }
        
    }

    void AlineAimCam()
    {
        CM_aiming_cam.m_XAxis.Value= this.gameObject.transform.rotation.eulerAngles.y;
    }

    void EnterAiming()
    {
        Slash_state = Slash_states.Aim;
        //player_locomotion.Move_speed = aim_move_speed;
        CM_aiming_cam.gameObject.SetActive(true);
        CM_topdown_cam.gameObject.SetActive(false);
        topdown_locomotion.enabled=false;
        //thirdperson_locomotion.enabled=true;
        StartCoroutine(AimTransition());
    }

        IEnumerator AimTransition()
    {
        yield return aim_wait;
        thirdperson_locomotion.enabled = true;
        thirdperson_locomotion.Can_control = true;
    }
    
    void HighlightTarget()
    {
        Physics.SphereCast(main_cam.transform.position, aim_cast_radius, main_cam.transform.forward, out aim_hit, aim_cast_distance, aim_layerMask);
        //Physics.Raycast(main_cam.transform.position, main_cam.transform.forward, out aim_hit, aim_cast_distance, aim_layerMask);
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
    }

    void EnterSlashing()
    {
        Time.timeScale = slash_time_scale;
        CM_aiming_cam.gameObject.SetActive(false);
        CM_slashing_cam.gameObject.SetActive(true);
        Slash_state = Slash_states.Slash;
    }

    void CalculateSlashInput()
    {
        //Vector2 slash_input_origin;
        Vector2 slash_input_direction;
        slash_input_direction = Input.GetAxis("Mouse X")*Vector2.right + Input.GetAxis("Mouse Y")*Vector2.up;
        Debug.Log(slash_input_direction);
        if(slash_input_direction.magnitude>slash_input_threshold)
        {
            StartCoroutine(UnleashSlash());
        }
    }

    IEnumerator UnleashSlash()
    {
        Vector3 target_vector;
        Vector3 dash_vector;
        Enemy_AI_Control enemy_script;
        thirdperson_locomotion.enabled=false;
        thirdperson_locomotion.Can_control = false;
        Slash_state = Slash_states.Move;
        Time.timeScale = 1.0f;
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
                    Target.SendMessage("ApplyDamage",slash_damage);
                    dash_vector *= dash_offset;
                }
                else
                {
                    dash_vector *= dash_deflected_offset;
                }
                Debug.Log("Dash");
            }
            this.transform.DOMove(Target.transform.position + dash_vector, dash_duration);

            yield return slash_wait;

            //Reset Target
            target_material.DisableKeyword("_EMISSION");
            target_material=null;
            Target = null;

            //Reset Camera to topdown
            CM_slashing_cam.gameObject.SetActive(false);
            CM_aiming_cam.gameObject.SetActive(false);
            CM_topdown_cam.gameObject.SetActive(true);
            topdown_locomotion.enabled=true;
            thirdperson_locomotion.enabled=false;
        }
    }

    /*
    IEnumerator DashTransition()
    {
        yield return aim_wait;
        thirdperson_locomotion.enabled=true;
    }
    */
}
