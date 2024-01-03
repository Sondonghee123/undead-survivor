using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public float count;
    public float speed;

    float timer;
    Player player;

    void Awake()
    {
        player = GameManager.Instance.player;
    }
    

    void Update()
    {
        if (!GameManager.Instance.isLive)
        {
            return;
        }

        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            default:
                timer += Time.deltaTime;

                if (timer > speed)
                {
                    timer = 0f;
                    Fire();
                }
                break;
        }
    }

    public void LevelUp(float damage, int count)
    {
        this.damage = damage * Character.Damage;
        this.count += count;

        if (id == 0)
            Batch();

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }


    public void Init(ItemData data)
    {
        name = "Weapon " + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        id = data.itemId;
        damage = data.baseDamage * Character.Damage;
        count = data.baseCount + Character.Count;

        for (int index=0; index < GameManager.Instance.pool.prefabs.Length; index++)
        {
            if (data.projectile  == GameManager.Instance.pool.prefabs[index])
            {
                prefabId = index;
                break;
            }
        }

        switch (id)
        {
            case 0:
                speed = 150 * Character.WeaponSpeed;
                Batch();
                break;
            case 1:
                speed = 0.5f * Character.WeaponRate;
                break;
            case 2:
                speed = 150 * Character.WeaponSpeed;
                Batch();
                break;
            case 3:
                speed = 150 * Character.WeaponSpeed;
                Batch();
                break;
        }

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    void Batch()
    {
        for (int index = 0; index < count; index++)
        {
            Transform bullet;
             
            if (index < transform.childCount)
            {
                bullet = transform.GetChild(index);
            }
            else
            {
                bullet = GameManager.Instance.pool.Get(prefabId).transform;
                bullet.parent = transform;
            }
            
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.5f, Space.World);
            bullet.GetComponent<Bullet>().Init(damage, -1, Vector3.zero);  // per가 -1이면 무한 관통임
        }
    }

    void Fire()
    {
        if (!player.scanner.nearestTarget)
            return;
            

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        Transform bullet = GameManager.Instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Init(damage, (int)count, dir);

        AudioManager.Instance.PlaySfx(AudioManager.sfx.Range);
    }
}
