using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensoryManager : MonoBehaviour
{
    public GameObject soundObjects;
    public SimpleStateMachine npc;

    private List <SoundObject> registeredSoundObjects = new List<SoundObject>();
    private void Awake()
    {;
           
            RegisterAllSounds();
    }
    public void RegisterAllSounds()
    {
        foreach (Transform child in soundObjects.transform)
        {
            SoundObject soundObject = child.GetComponent<SoundObject>();
            if (soundObject != null)
            {
                Debug.Log($"Registering sound object: {soundObject.name}");
                soundObject.OnSoundTrigger.AddListener(npc.SoundTrigger);
                registeredSoundObjects.Add(soundObject);
            }
        }
    }
}
