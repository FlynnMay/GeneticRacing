using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectionManager : MonoBehaviour
{
    [SerializeField] GameObject mapParent;
    [SerializeField] SelectableMap mapPrefab;
    [SerializeField] MapContainer mapContainer;
    [SerializeField] Button playButton;
    [SerializeField] Button trainButton;
    [SerializeField] TrainingSender trainSender;

    private void Awake()
    {
        for (int i = 0; i < mapContainer.Maps.Count; i++)
        {
            Map map = mapContainer.Maps[i];
            Instantiate(mapPrefab, mapParent.transform).SetMap(map);
        }
    }

    private void OnEnable()
    {
        playButton.onClick.AddListener(OnPlay);
        trainButton.onClick.AddListener(OnTrain);
    }
    
    private void OnDisable()
    {
        playButton.onClick.RemoveListener(OnPlay);
        trainButton.onClick.RemoveListener(OnTrain);
    }

    private void OnPlay()
    {
        mapParent.SetActive(true);
        trainSender.SetTraining(false);
    }

    private void OnTrain()
    {
        mapParent.SetActive(true);
        trainSender.SetTraining(true);
    }
}
