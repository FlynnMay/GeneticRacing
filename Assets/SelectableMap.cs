using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableMap : MonoBehaviour
{
    [SerializeField] Map map;
    [SerializeField] Image previewImage;

    public void SetMap(Map _map)
    {
        map = _map;
        previewImage.sprite = map.Preview; 
    }
}
