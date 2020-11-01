using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Weapon_Hitbox : Weapon_Hitbox
{
    //private Player_Slash_Control slash_control_script;
    // Start is called before the first frame update
    void Start()
    {
        //slash_control_script = this.GetComponentInParent<Player_Slash_Control>();
    }

    public override void OnHit(Collider other)
    {
        bool isSteel;
        base.OnHit(other);
        //Debug.Log(other.name);
        isSteel = bodypart_script.isSteel;
        //Debug.Log(isSteel);
        this.transform.root.SendMessage("ReduceSharpness",isSteel);
        if(isSteel)
        {
            //this.gameObject.SetActive(false);
        }

    }
}
