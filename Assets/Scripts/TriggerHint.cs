using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHint : MonoBehaviour
{
    public AudioClip openSound;
    public List<ParticleSystem> hints;

    private bool hintsActives = false;
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        SetHintsState(false);
    }

    private void SetHintsState(bool state)
    {
        hintsActives = state;
        if (state)
        {
            foreach(ParticleSystem hint in hints)
            {
                hint.Play();
            }
        }
        else
        {
            foreach(ParticleSystem hint in hints)
            {
                hint.Stop();
            }

        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player"))
        {
            source.PlayOneShot(openSound, 1f);
            SetHintsState(true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.CompareTag("Player"))
        {
            SetHintsState(false);
        }
    }
}
