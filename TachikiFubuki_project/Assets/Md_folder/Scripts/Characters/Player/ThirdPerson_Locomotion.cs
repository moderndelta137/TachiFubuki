using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPerson_Locomotion : MonoBehaviour
{
    [Header("Basic")]
    public bool Can_control;
    public float Move_speed;
    public float Rotate_speed;

    //Movement
    private CharacterController player_chara_controller;
    private Camera main_cam;
    
    // Start is called before the first frame update
    void Start()
    {
        player_chara_controller = GetComponent<CharacterController>();
        main_cam = Camera.main;
        Can_control = true;
    }

    // Update is called once per frame
    private void FixedUpdate() {
        RotateCharacter();
    }

    void RotateCharacter()
    {
        Vector3 cam_forward_flattened;
        cam_forward_flattened = main_cam.transform.forward;
        cam_forward_flattened.y = 0;
        this.transform.rotation = Quaternion.LookRotation(cam_forward_flattened,Vector3.up);

    }
}
