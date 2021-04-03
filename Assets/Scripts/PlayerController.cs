using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  private Rigidbody rg;
  public float speedFactor;
  public float frictionFactor;
  public LevelManager levelManager;
  // TODO: Implement
  // public GameObject plane;

  private bool movementAllowed;

  // Start is called before the first frame update
  void Start()
  {
    rg = GetComponent<Rigidbody>();
    movementAllowed = false;
  }

  // Update is called before de phycics callculations
  private void FixedUpdate()
  {
    if(movementAllowed)
    {
      float moveHorizontal = Input.GetAxis("Horizontal");
      float moveVertical   = Input.GetAxis("Vertical");

      Vector3 apliedForce = new Vector3(moveHorizontal * speedFactor , 0.0f, moveVertical * speedFactor );
      Vector3 netForce    = new Vector3(apliedForce.x - frictionFactor * rg.velocity.x, 0.0f, apliedForce.z - frictionFactor * rg.velocity.z);
      // Vector3 netForceP   = Quaternion.FromToRotation(Vector3.up, plane.transform.up) * netForce;
      Vector3 netForceP   = netForce;
      rg.AddForce(netForceP);
    }
  }

  private void OnCollisionEnter(Collision other) {
    if (other.gameObject.CompareTag("Gound"))
    {
      movementAllowed = true;
      if (levelManager.HasRamps())
      {
        levelManager.RemoveRamps();
      }
    }
  }

  private void OnTriggerEnter(Collider other) {
    if (other.gameObject.CompareTag("Collectable"))
    {
      levelManager.Collected(other.gameObject);
    } else if (other.gameObject.CompareTag("FinalPoint")) {
      finalReached();
    }
  }

  private void finalReached() {
    levelManager.FinalReached();
  }

}
