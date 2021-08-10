using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUnit : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private Vector3 velocityVector;


    // Update is called once per frame
    void Update()
    {
        transform.position += velocityVector * moveSpeed * Time.deltaTime;
    }

    public void SetVelocity(Vector3 velocityVector) {
        this.velocityVector = velocityVector;
    }
}
