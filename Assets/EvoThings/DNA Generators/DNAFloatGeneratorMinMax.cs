using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Evo;

[CreateAssetMenu(fileName = "MinMaxFloat", menuName = "EvoCustom/ValueGenerators/MinMaxFloat")]
public class DNAFloatGeneratorMinMax : DNAValueGenerator<object>
{
    [SerializeField] float min = 0.0f;
    [SerializeField] float max = 10.0f;
    public override object GetValue()
    {
        return Random.Range(min, max);
    }
}
