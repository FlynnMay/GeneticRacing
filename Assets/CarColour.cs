using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarColour : MonoBehaviour
{
    [SerializeField] Material[] materials;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] bool useRandom;
    int index = 0;

    private void Awake()
    {
        if (useRandom)
            UseRandomMaterial();
    }

    private void UseRandomMaterial()
    {
        index = Random.Range(0, materials.Length);
        meshRenderer.material = materials[index];
    }

    public int GetIndex()
    {
        return index;
    }

    public void SetColourFromIndex(int colourIndex)
    {
        Mathf.Clamp(colourIndex, 0, materials.Length);
        meshRenderer.material = materials[colourIndex];
    }
}
