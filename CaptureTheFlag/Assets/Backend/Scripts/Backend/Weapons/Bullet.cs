﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour 
{
    public IShootable bulletOwner;

    public AudioClip bulletSound;

    public GameObject bulletCasingObject;

    public int poolCasingIndex = 1;
    public int poolIndex = 0;
    public int poolBulletHoleIndex = 2;
    public int poolBloodIndex = 3;

    public int damage;

    public float speed;

    private Rigidbody _rigidbody;

    public int teamALayer = 14, teamBLayer = 15;

    public Vector2 randomness;

    private PoolObject _poolObject;

    void Awake()
    {
        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody>();

        if (!_poolObject)
            _poolObject = GetComponent<PoolObject>();
    }

    void Start()
    {
        if (bulletOwner == null)
            return;

        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x + Random.Range(randomness.x, randomness.y), transform.localEulerAngles.y + Random.Range(randomness.x, randomness.y), transform.localEulerAngles.z);
        
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        transform.position = bulletOwner.GetMuzzle().position;
        transform.rotation = bulletOwner.GetMuzzle().rotation;
    }

    void Update()
    {
        if (bulletOwner.GetOwner().GetTeam() == Team.Type.A)
        {
            gameObject.layer = teamALayer;
        }
        else if (bulletOwner.GetOwner().GetTeam() == Team.Type.B)
        {
            gameObject.layer = teamBLayer;
        }

        _rigidbody.velocity = (transform.forward * speed);
    }

    void OnCollisionEnter(Collision collision)
    {
        IAgent agent = collision.gameObject.GetComponent<IAgent>();

        if (agent != null)
        {
            // Hit soldier
            PoolManager.instance.SetPath(poolBloodIndex);
            PoolObject bloodImpact = PoolManager.instance.Create(null);
            bloodImpact.transform.rotation = Quaternion.FromToRotation(-Vector3.back, collision.contacts[0].normal);
            bloodImpact.transform.position = collision.contacts[0].point + transform.up * -0.04f;

            agent.Damage(damage, bulletOwner.GetOwner().GetLocation());
        }
        else
        {
            // Hit other       
            PoolManager.instance.SetPath(poolBulletHoleIndex);
            PoolObject bulletHole = PoolManager.instance.Create(null);
            bulletHole.transform.rotation = Quaternion.FromToRotation(-Vector3.back, collision.contacts[0].normal);
            bulletHole.transform.position = collision.contacts[0].point + transform.up * -0.04f;
        }

        _poolObject.Hide();
    }
}