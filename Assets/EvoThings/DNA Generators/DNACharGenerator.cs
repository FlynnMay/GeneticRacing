using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Evo;

[CreateAssetMenu(fileName = "CharFromValidChars", menuName = "EvoCustom/ValueGenerators/CharFromValidChars")]
public class DNACharGenerator : DNAValueGenerator<char>
{
    public string validChars = "";
    public override char GetValue()
    {
        return validChars[Random.Range(0, validChars.Length)];
    }
}
