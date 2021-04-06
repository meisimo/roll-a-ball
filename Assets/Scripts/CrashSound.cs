using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrashSound : MonoBehaviour
{
  public AudioClip crashSound;

  private AudioSource source;

  private void Awake() {
    source = GetComponent<AudioSource>();
  }

  private void OnCollisionEnter(Collision other) {
    source.PlayOneShot(crashSound, 0.5f);
  }
}
