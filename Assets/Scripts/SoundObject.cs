using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoundObject : MonoBehaviour
{

    public UnityEvent <Vector3> OnSoundTrigger = new UnityEvent<Vector3>();

    AudioSource audioSource;

    public virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null)
        {
            audioSource.Play();
            OnSoundTrigger.Invoke(transform.position);
        }
        }
    }