using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    private Vector3 rotationalVec;
    private float rotationalSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rotationalVec = new Vector3(Random.Range(0,45), Random.Range(0,45), Random.Range(0,45));
        rotationalSpeed = Random.Range(1.0f, 5.5f);
    }

    // Update is called once per frame
    void Update()
    {
      transform.Rotate( rotationalVec * Time.deltaTime * rotationalSpeed);
    }
}
