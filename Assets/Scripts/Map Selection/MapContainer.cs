using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Maps/Container", fileName = "New Map Container")]
public class MapContainer : ScriptableObject
{
    [SerializeField] List<Map> maps = new List<Map>();
    public List<Map> Maps { get => maps; }
}
