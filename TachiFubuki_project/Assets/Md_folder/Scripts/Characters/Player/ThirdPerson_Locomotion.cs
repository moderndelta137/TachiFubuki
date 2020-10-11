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
    private Vector3 chara_forward;
    private Vector3 chara_right;
    private Vector3 move_input;
    private Vector3 move_vector;

    
    // Start is called before the first frame update
    void Start()
    {
        player_chara_controller = GetComponent<CharacterController>();
        main_cam = Camera.main;
        Can_control = true;
    }

    // Update is called once per frame
    private void FixedUpdate() {
        move_input.x = Input.GetAxis("Horizontal");
        move_input.y = Input.GetAxis("Vertical");
        if(Can_control)
        {
            MoveCharacter(move_input);
            RotateCharacter();
        }
    }

    void MoveCharacter(Vector3 input)
    {
        chara_forward = this.transform.forward;
        chara_forward.y = 0;
        chara_right = this.transform.right;
        chara_right.y = 0;
        move_vector = input.x * chara_right + input.y * chara_forward;
        Vector3.ClampMagnitude(move_vector,1.0f);
        move_vector *= Move_speed * Time.deltaTime;
        player_chara_controller.Move(move_vector);
    }

    void RotateCharacter()
    {
        Vector3 cam_forward_flattened;
        cam_forward_flattened = main_cam.transform.forward;
        cam_forward_flattened.y = 0;
        this.transform.rotation = Quaternion.LookRotation(cam_forward_flattened,Vector3.up);

    }
}
