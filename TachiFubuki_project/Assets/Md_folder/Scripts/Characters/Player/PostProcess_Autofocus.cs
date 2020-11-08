using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine.PostFX;
using UnityEngine.Rendering.Universal;

public class PostProcess_Autofocus : MonoBehaviour
{
    public Player_Slash_Control player_script;
    //private GameObject target;
    private Vector3 target_vector;
    private Camera main_camera;
    private CinemachineVolumeSettings post_process_setting;
    private DepthOfField dof_component;
    [SerializeField] private float dof_default = 5;
    [SerializeField] private float dof_scale = 1;
    [SerializeField] private float dof_offset = 0;
    // Start is called before the first frame update
    private void Awake() {
        post_process_setting = this.GetComponent<CinemachineVolumeSettings>();
        post_process_setting.m_Profile.TryGet<DepthOfField>(out dof_component);
        main_camera = Camera.main;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(player_script.Target!=null)
        {
            target_vector = player_script.Target.transform.position-main_camera.transform.position;
            dof_component.focusDistance.value = target_vector.magnitude*dof_scale+dof_offset;
        }
        else
        {
            dof_component.focusDistance.value = dof_default;
        }
    }
}
