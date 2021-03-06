using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Topdown_Locomotion : MonoBehaviour {
    [Header ("Basic")]
    public bool Can_control;
    public float Move_speed;
    public float Rotate_speed;
    [Space]
    [Header ("Move")]
    private CharacterController player_chara_controller;
    private Camera main_cam;
    private Vector3 cam_forward;
    private Vector3 cam_right;
    private Vector3 move_input;
    private Vector3 move_vector;

    [Space]
    [Header ("Animation")]
    private Animator player_animator;

    private void Awake () {
        player_chara_controller = GetComponent<CharacterController> ();
        player_animator = GetComponentInChildren<Animator> ();
        main_cam = Camera.main;
    }

    // Start is called before the first frame update
    void Start () {
        //Initialize
        Can_control = true;
        //Assume camera doesn't rotate in topdown mode
        cam_forward = main_cam.transform.forward;
        cam_forward.y = 0;
        cam_forward.Normalize ();
        cam_right = main_cam.transform.right;
        cam_right.y = 0;
        cam_right.Normalize ();

    }

    void LateUpdate () {
        move_input.x = Input.GetAxis ("Horizontal");
        move_input.z = Input.GetAxis ("Vertical");
        if (Can_control) {
            MoveCharacter (move_input);
            RotateCharacter ();
        }
    }

    void MoveCharacter (Vector3 input) {
        move_vector = input.x * cam_right + input.z * cam_forward;
        Vector3.ClampMagnitude (move_vector, 1.0f);
        //Update Animation
        player_animator.SetFloat ("Move_speed", move_input.magnitude);
        //Move Character
        move_vector *= Move_speed * Time.deltaTime;
        player_chara_controller.Move (move_vector);
    }

    void RotateCharacter () {
        //Rotate to move input direction
        if (move_input.magnitude > 0.1f) {
            this.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.LookRotation (move_input), Rotate_speed * Time.deltaTime);
        }
    }
}