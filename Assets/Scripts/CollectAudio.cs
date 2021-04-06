using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectAudio : MonoBehaviour
{
  public AudioClip collectSound;

  private AudioSource source;

  private void Awake() {
    source = GetComponent<AudioSource>();
  }

  private void OnTriggerEnter(Collider other)
	{
    source.PlayOneShot(collectSound, 1f);
  }
}
