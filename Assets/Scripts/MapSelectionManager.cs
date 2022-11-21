using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSelectionManager : MonoBehaviour
{
    [SerializeField] GameObject mapParent;
    [SerializeField] SelectableMap mapPrefab;
    [SerializeField] MapContainer mapContainer;

    private void Awake()
    {
        for (int i = 0; i < mapContainer.Maps.Count; i++)
        {
            Map map = mapContainer.Maps[i];
            Instantiate(mapPrefab, mapParent.transform).SetMap(map);
        }
    }
}
