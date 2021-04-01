﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  private Rigidbody rg;
  public float speedFactor;
  public float frictionFactor;
  public int timePerCollectableReward;
  // TODO: Implement
  // public GameObject plane;
  public Timer timer;


  private int remainingCollectables;

  // Start is called before the first frame update
  void Start()
  {
    remainingCollectables = 12;
    rg = GetComponent<Rigidbody>();
  }

  // Update is called before de phycics callculations
  private void FixedUpdate()
  {
    float moveHorizontal = Input.GetAxis("Horizontal");
    float moveVertical   = Input.GetAxis("Vertical");

    Vector3 apliedForce = new Vector3(moveHorizontal * speedFactor , 0.0f, moveVertical * speedFactor );
    Vector3 netForce    = new Vector3(apliedForce.x - frictionFactor * rg.velocity.x, 0.0f, apliedForce.z - frictionFactor * rg.velocity.z);
    // Vector3 netForceP   = Quaternion.FromToRotation(Vector3.up, plane.transform.up) * netForce;
    Vector3 netForceP   = netForce;
    rg.AddForce(netForceP);
  }

  private void OnTriggerEnter(Collider other) {
    if (other.gameObject.CompareTag("Collectable")){
      other.gameObject.SetActive(false);
      remainingCollectables--;

      timer.IncreaseTimeOffset(-timePerCollectableReward);

      if (remainingCollectables == 0) {
        Debug.Log("LEVEL PASSED");
      }
    } else {

    }
  }
}