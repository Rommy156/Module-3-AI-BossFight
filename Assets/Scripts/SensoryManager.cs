using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensoryManager : MonoBehaviour
{
    public GameObject soundObjects;
    public SimpleStateMachine npc;

    private List <SoundObject> registeredSoundObjects = new List<SoundObject>();
    private void Awake()
    {
            RegisterAllSounds();
    }
    // register all sound objects in the scene to the npc's hearing system
    
    public void RegisterAllSounds()
    {
        foreach (Transform child in soundObjects.transform)
        {
            SoundObject soundObject = child.GetComponent<SoundObject>();
            if (soundObject != null)
            {

                soundObject.OnSoundTrigger.AddListener(npc.SoundTrigger);
                registeredSoundObjects.Add(soundObject);
            }
        }
    }
}
