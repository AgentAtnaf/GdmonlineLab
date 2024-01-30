using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Movementscript : NetworkBehaviour 
{
    Rigidbody rb;
   public float speed = 5.0f;
   public float rotationspeed = 10.0f;
   void Start()
   {
      rb = this.GetComponent<Rigidbody>();
   }
   private void FixedUpdate()
   {
        if(IsOwner)
    {
      float translation = Input.GetAxis("Vertical") * speed;
      float rotation = Input.GetAxis("Horizontal") * rotationspeed;
      translation *= Time.deltaTime;
      Quaternion turn = Quaternion.Euler(0f, rotation, 0f);
      rb.MovePosition(rb.position + this.transform.forward * translation);
      rb.MoveRotation(rb.rotation * turn);
    }
   }
}
