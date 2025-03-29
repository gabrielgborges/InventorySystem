using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPhysicsComponent : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _speed;

    public void MoveForward()
    {
        //enabled = true;
        _rigidbody.velocity = transform.forward * _speed;
    }

    private void FixedUpdate()
    {
        MoveForward();
    }

    public void StopMovement()
    {
        _rigidbody.velocity = Vector3.zero;
        //enabled = 0
    }
}
