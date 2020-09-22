using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Spawner : MonoBehaviour
{
    public GameObject Enemy_prefab;
    public Vector2 Spawn_zone;
    public float Spawn_cooldown;
    private WaitForSeconds spawn_waitforseconds;
    // Start is called before the first frame update
    void Start()
    {
        spawn_waitforseconds = new WaitForSeconds(Spawn_cooldown);
        StartCoroutine(SpawnEnemy());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnEnemy()
    {
        while(true)
        {
            GameObject new_spawned;
            Vector3 spawn_position = Vector3.zero;
            Quaternion spawn_rotation = Quaternion.identity;
            spawn_position = Vector3.zero;
            spawn_position.x = Random.Range(-Spawn_zone.x,Spawn_zone.x);
            spawn_position.z = Random.Range(-Spawn_zone.y,Spawn_zone.y);
            spawn_rotation= Quaternion.Euler(0,Random.Range(0,360f),0);
            new_spawned = Instantiate(Enemy_prefab,spawn_position, spawn_rotation);
            yield return spawn_waitforseconds;
        }
    }
}
