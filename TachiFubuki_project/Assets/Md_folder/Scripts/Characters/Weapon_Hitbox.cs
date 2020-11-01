using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Hitbox : MonoBehaviour
{
    public string Target_tag;//The tag used on the root parent of the hitbox you wish to hit. Also make sure the root parent is set to the correct hitbox.
    public int Damage;
    private List<GameObject> Damaged_targets = new List<GameObject>();//Used to log all the individual targets the hitbox hits. This is to pervent the hitbox damage the same target multiple times when it hits multiple bodyparts
    public Bodypart_Damage_Manager bodypart_script;
    private Chara_Damage_Manager parent_script;
    // Start is called before the first frame update
    private void OnEnable() 
    {
        Damaged_targets.Clear();
        Debug.Log("Clean");
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.transform.root.CompareTag(Target_tag))
        {
            bodypart_script = other.GetComponent<Bodypart_Damage_Manager>();
            parent_script = other.GetComponentInParent<Chara_Damage_Manager>();
            if(bodypart_script != null && parent_script != null)
            {
                if(!Damaged_targets.Contains(parent_script.transform.gameObject))//Check if the hitbox already hit the target's other bodyparts.
                {
                    Damaged_targets.Add(parent_script.transform.gameObject);
                    OnHit(other);
                }
            }
            
        }
    }

    public virtual void OnHit(Collider other)
    {
        bodypart_script.SendMessage("ApplyDamage", Damage);
        Debug.Log(other.name);
    }
}
