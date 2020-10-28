using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class Player_Slash_Control : MonoBehaviour {
    public enum Slash_states {
        Move,
        Aim,
        Slash,
    }

    [Header ("Basic")]
    public Slash_states Slash_state;
    private Topdown_Locomotion topdown_locomotion;
    private ThirdPerson_Locomotion thirdperson_locomotion;
    [Space]
    [Header ("Aim")]
    private Camera main_cam;
    [SerializeField] private CinemachineVirtualCamera CM_topdown_cam = null;
    //[SerializeField]private CinemachineTargetGroup CM_player_target_group = null;
    [SerializeField] private CinemachineFreeLook CM_aiming_cam = null;
    [SerializeField] private CinemachineFreeLook CM_slashing_cam = null;
    [SerializeField] private float slash_time_scale = 1;
    public GameObject Target; //Will change to enemy script
    [SerializeField] private Material target_material = null;
    private int aim_layerMask;
    private RaycastHit aim_hit;
    [SerializeField] private float aim_cast_radius = 0;
    [SerializeField] private float aim_cast_distance = 0;
    [SerializeField] private float aim_transition_duration = 0;
    private WaitForSecondsRealtime aim_wait;
    [Space]
    [Header ("Dash")]
    [SerializeField] private float dash_offset = 0;
    [SerializeField] private float dash_deflected_offset = 0;
    [SerializeField] private float dash_duration = 0;
    [Space]
    [Header ("Slash")]
    [SerializeField] private int slash_damage = 0;
    [SerializeField] private float slash_input_threshold = 0;
    [SerializeField] private float slash_wait_duration = 0;
    private WaitForSecondsRealtime slash_wait;
    //Use a pitched boxcastall to collide all the enemy parts in the slash trajectory
    public GameObject[] Target_bodyparts;
    private int bodyparts_layerMask;
    private RaycastHit[] bodyparts_hits;
    [SerializeField] private float bodyparts_cast_offset = 0;
    [SerializeField] private Vector3 bodyparts_cast_size = Vector3.zero;
    [SerializeField] private float bodyparts_cast_pitch = 0;
    public GameObject DEBUG_BOXCAST_BOX;
    [Space]
    [Header ("Animation")]
    private Animator player_animator;
    // Start is called before the first frame update
    void Start () {
        topdown_locomotion = GetComponent<Topdown_Locomotion> ();
        thirdperson_locomotion = GetComponent<ThirdPerson_Locomotion> ();
        player_animator = GetComponentInChildren<Animator> ();
        //CM_player_target_group.m_Targets[1].target = topdown_locomotion.mouse_cursor.transform;
        aim_layerMask = 1 << 6;
        bodyparts_layerMask = 1 << 7; //DEBUG using enemy layer for now
        main_cam = Camera.main;
        aim_wait = new WaitForSecondsRealtime (aim_transition_duration);
        slash_wait = new WaitForSecondsRealtime (slash_wait_duration);
    }

    // Update is called once per frame
    void Update () {
        switch (Slash_state) {
            case Slash_states.Move:
                AlineAimCam ();
                if (Input.GetButtonDown ("Attack")) {
                    EnterAiming ();
                }
                break;

            case Slash_states.Aim:
                HighlightTarget ();
                if (Input.GetButtonUp ("Attack")) {
                    EnterSlashing ();
                }
                break;

            case Slash_states.Slash:
                HighlightTarget ();
                CalculateSlashInput ();

                //FOR DEBUG ONLY
                if (Input.GetButtonDown ("Cancel")) {
                    if (Target != null) {
                        //Reset Target
                        //target_material.DisableKeyword("_EMISSION");
                        target_material = null;
                        Target = null;
                    }

                    //Reset Camera to topdown
                    thirdperson_locomotion.enabled = false;
                    thirdperson_locomotion.Can_control = false;
                    Slash_state = Slash_states.Move;
                    Time.timeScale = 1.0f;
                    CM_slashing_cam.gameObject.SetActive (false);
                    CM_aiming_cam.gameObject.SetActive (false);
                    CM_topdown_cam.gameObject.SetActive (true);
                    topdown_locomotion.enabled = true;
                    player_animator.SetTrigger ("Draw");
                }
                break;
        }

    }

    void AlineAimCam () {
        CM_aiming_cam.m_XAxis.Value = this.gameObject.transform.rotation.eulerAngles.y;
    }

    void EnterAiming () {
        Slash_state = Slash_states.Aim;
        CM_aiming_cam.gameObject.SetActive (true);
        CM_aiming_cam.m_YAxis.Value = 0.35f;
        CM_topdown_cam.gameObject.SetActive (false);
        topdown_locomotion.enabled = false;
        player_animator.SetTrigger ("Sheath");
        StartCoroutine (AimTransition ());
    }

    IEnumerator AimTransition () {
        yield return aim_wait;
        thirdperson_locomotion.enabled = true;
        thirdperson_locomotion.Can_control = true;
    }

    void HighlightTarget () {
        Physics.SphereCast (this.transform.position, aim_cast_radius, main_cam.transform.forward, out aim_hit, aim_cast_distance, aim_layerMask);
        if (aim_hit.transform != null) {
            if (Target != aim_hit.transform.gameObject) {
                //To be removed!!!!
                if (target_material != null)
                    target_material.DisableKeyword ("_EMISSION");
                Target = aim_hit.transform.gameObject;
                target_material = Target.GetComponentInChildren<SkinnedMeshRenderer> ().material;
                target_material.EnableKeyword ("_EMISSION");
            }
        } else {
            if (Target != null) {
                target_material.DisableKeyword ("_EMISSION");
                target_material = null;
            }
            Target = null;
        }
    }

    void EnterSlashing () {
        Time.timeScale = slash_time_scale;
        CM_aiming_cam.gameObject.SetActive (false);
        CM_slashing_cam.gameObject.SetActive (true);
        Slash_state = Slash_states.Slash;
    }

    void CalculateSlashInput () {
        //Vector2 slash_input_origin;
        Vector2 slash_input_direction;
        slash_input_direction = Input.GetAxis ("Mouse X") * Vector2.right + Input.GetAxis ("Mouse Y") * Vector2.up;
        //Unleash slash
        if (slash_input_direction.magnitude > slash_input_threshold) {
            StartCoroutine (UnleashSlash (slash_input_direction));
        }
    }

    IEnumerator UnleashSlash (Vector2 input_direction) {
        Vector3 target_vector;
        Vector3 dash_vector;
        Enemy_AI_Control enemy_script;
        bool deflected;
        float slash_direction;
        deflected = false;
        thirdperson_locomotion.enabled = false;
        thirdperson_locomotion.Can_control = false;
        Slash_state = Slash_states.Move;
        Time.timeScale = 0.1f;

        //Calculate unleash direction
        slash_direction = Vector2.SignedAngle (Vector2.up, input_direction);
        player_animator.SetFloat ("Slash_direction", slash_direction / 180f); //Degree to normalized


        //Boxcast to check enenmy bodyparts in the slash trajectory
        bodyparts_hits = Physics.BoxCastAll (main_cam.transform.position + main_cam.transform.forward * bodyparts_cast_offset, bodyparts_cast_size, main_cam.transform.forward, Quaternion.AngleAxis (slash_direction, main_cam.transform.forward), aim_cast_distance, bodyparts_layerMask);
        DEBUG_BOXCAST_BOX.transform.position = main_cam.transform.position + main_cam.transform.forward * bodyparts_cast_offset;
        DEBUG_BOXCAST_BOX.transform.localScale = bodyparts_cast_size;
        DEBUG_BOXCAST_BOX.transform.rotation =  Quaternion.AngleAxis (slash_direction, main_cam.transform.forward);
        foreach(RaycastHit part in bodyparts_hits )
        {
            //part.collider.GetComponentInChildren<MeshRenderer>().material.EnableKeyword ("_EMISSION");
            //Debug.Log(part.collider.gameObject.name);
        }
        

        if (Target != null) {
            //Calculate target direction vector
            target_vector = Target.transform.position - this.transform.position;

            //Calculate dash vector
            dash_vector = target_vector;
            dash_vector.y = 0;
            dash_vector.Normalize ();

            CM_slashing_cam.Follow = null;
            //Dash in front of the enemy
            Tween myTween = this.transform.DOMove (Target.transform.position + dash_vector*dash_deflected_offset, dash_duration).SetUpdate(true);
            yield return myTween.WaitForCompletion();

            //Play slash animation
            player_animator.SetTrigger ("Unleash");

            //Apply Damage
            if (Target.tag == "Enemy") {
                enemy_script = Target.GetComponent<Enemy_AI_Control> ();
                if (enemy_script.Attack_charging) {
                    //Target.SendMessage ("ApplyDamage", slash_damage);
                    foreach(RaycastHit part in bodyparts_hits )
                    {
                        Debug.Log(part.collider.gameObject.name);
                        if(!part.collider.gameObject.GetComponent<Enemy_Part_Damage_Manager>().isSteel)
                        {
                            part.collider.gameObject.SendMessage ("ApplyDamage", slash_damage);
                            Debug.Log("Damage");
                        }
                        else
                        {
                            deflected = true;
                            Debug.Log("Steel");
                            break;
                        }
                        //part.collider.GetComponentInChildren<MeshRenderer>().material.EnableKeyword ("_EMISSION");
                    }
                } else {
                    deflected = true;
                }
            }
            if(!deflected)
            {
                //Not deflected
                dash_vector.Normalize ();
                dash_vector *= dash_offset;
                myTween = this.transform.DOMove (Target.transform.position + dash_vector, dash_duration).SetUpdate(true);
                yield return myTween.WaitForCompletion();
            }
            else
            {
                //Deflected
                player_animator.SetTrigger ("Deflected");
            }


            yield return slash_wait;
            CM_slashing_cam.Follow = this.gameObject.transform;
            Time.timeScale = 1f;

            //Reset Target
            //target_material.DisableKeyword("_EMISSION");
            target_material = null;
            Target = null;

            if (Input.GetButton ("Attack")) 
            {
                EnterAiming ();
            } 
            else 
            {
                CM_slashing_cam.gameObject.SetActive (false);
                CM_aiming_cam.gameObject.SetActive (false);
                CM_topdown_cam.gameObject.SetActive (true);
                topdown_locomotion.enabled = true;
                thirdperson_locomotion.enabled = false;
                player_animator.SetTrigger ("Draw");
            }
            //Reset Camera to topdown

        }
    }

}