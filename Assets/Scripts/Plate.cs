using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : SoundObject
{
    public Material PlateMaterialPressed;
    MeshRenderer meshRenderer;
    // Start is called before the first frame update

    public override void Awake()
    {
        base.Awake();
        meshRenderer = GetComponent<MeshRenderer>();
    }
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        meshRenderer.material = PlateMaterialPressed;
    }
    public void OnTriggerExit(Collider other)
    {
        meshRenderer.material = PlateMaterialPressed;
    }

}
