using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Evo;

[CreateAssetMenu(fileName = "MinMaxInt", menuName = "EvoCustom/ValueGenerators/MinMaxInt")]
public class DNAIntGenerator : DNAValueGenerator<object>
{
    public int max = 6;
    public int min = 0;
    public override object GetValue()
    {
        return Random.Range(min, max + 1);
    }
}
