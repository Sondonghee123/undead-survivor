using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public int per;

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, int per, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;

        if (per > -1)
        {
            rigid.velocity = dir * 15f;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy") || per == -1)
            return;

        per--;

        if (per == -1)
        {
            rigid.velocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }
    void Update()
    {
        Dead();
    }

    void Dead()
    {
        Transform target = GameManager.Instance.player.transform;
        Vector3 targetPos = target.position;
        float dir = Vector3.Distance(targetPos, transform.position);
        if (dir > 20f)
            this.gameObject.SetActive(false);
    }
}
