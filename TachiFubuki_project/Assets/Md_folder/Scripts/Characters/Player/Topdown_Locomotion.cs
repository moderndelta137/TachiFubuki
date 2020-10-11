using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Topdown_Locomotion : MonoBehaviour
{
    [Header("Basic")]
    public bool Can_control;
    public float Move_speed;
    public float Rotate_speed;
    //Movement
    private CharacterController player_chara_controller;
    private Camera main_cam;
    private Vector3 cam_forward;
    private Vector3 cam_right;
    private Vector3 move_input;
    private Vector3 move_vector;
    //Rotation
    public bool Mouse_control;
    [SerializeField]private GameObject mouse_cursor_prefab = null;
    public GameObject mouse_cursor;
    private Plane mouse_plane;
    private Ray mouse_ray;
    private float mouse_plane_distance;
    private Vector3 mouse_world_position;
    private Vector3 mouse_direction_vector;
    private Vector3 rotate_input;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize
        player_chara_controller = GetComponent<CharacterController>();
        main_cam = Camera.main;
        Can_control = true;
        Mouse_control = true;
        //Assume camera doesn't rotate in topdown mode
        cam_forward = main_cam.transform.forward;
        cam_forward.y = 0;
        cam_forward.Normalize();
        cam_right = main_cam.transform.right;
        cam_right.y = 0;
        cam_right.Normalize();

        InitiateMouseControl();
    }

    void InitiateMouseControl()
    {
        //mouse_cursor=Instantiate(mouse_cursor_prefab,Vector3.zero,Quaternion.identity);
        mouse_plane = new Plane(this.transform.up, this.transform.position);
        //mouse_cursor.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
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
        move_vector = input.x * cam_right + input.y * cam_forward;
        Vector3.ClampMagnitude(move_vector,1.0f);
        move_vector *= Move_speed * Time.deltaTime;
        player_chara_controller.Move(move_vector);
    }

    void RotateCharacter()
    {
        if(Mouse_control)
        {
                mouse_ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (mouse_plane.Raycast(mouse_ray, out mouse_plane_distance))
                {
                    mouse_world_position = mouse_ray.GetPoint(mouse_plane_distance);
                    mouse_cursor.transform.position = mouse_world_position;
                    mouse_direction_vector = mouse_world_position - this.transform.position;
                    rotate_input = mouse_direction_vector.normalized-this.transform.forward;
                    if(rotate_input.magnitude>0.1f)
                        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(mouse_direction_vector), Rotate_speed*Time.deltaTime);
                }
        }
        else
        {
            
        }
    }
}
