using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Part_Damage_Manager : MonoBehaviour
{
    public bool isSteel;
    public bool isWeakness;
    private Enemy_Damage_Manager parent_damage_manager;
    // Start is called before the first frame update
    void Start()
    {
        parent_damage_manager = GetComponentInParent<Enemy_Damage_Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ApplyDamage(int value)
    {
        if(isSteel)
        {

        }
        else
        {
            if(isWeakness)
            {
                parent_damage_manager.SendMessage ("ApplyDamage", 999);
            }
            else
            {
                parent_damage_manager.SendMessage ("ApplyDamage", value);
            }
        }
    }
}
