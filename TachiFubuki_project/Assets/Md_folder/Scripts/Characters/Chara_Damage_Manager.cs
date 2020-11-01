using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chara_Damage_Manager : MonoBehaviour
{
    public int HP;
    private int max_hp;
    // Start is called before the first frame update
    void Start()
    {
        max_hp = HP;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ApplyDamage(int value)
    {
        HP -= value;
        Debug.Log(HP);
        if(HP<=0)
        {
            Dead();
        }
    }

    private void Dead()
    {
        Destroy(this.gameObject);
    }
}
