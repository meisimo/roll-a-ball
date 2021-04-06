using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorDoorController : MonoBehaviour
{
    public AudioClip openDoorSound;
    public GameObject leftDoor;
    public GameObject rightDoor;
    private AudioSource audioSource;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    private void FreeXRotation(GameObject door)
    {
        door.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player"))
        {
            FreeXRotation(leftDoor);
            FreeXRotation(rightDoor);
            audioSource.PlayOneShot(openDoorSound, 1f);
        }
    }

}
