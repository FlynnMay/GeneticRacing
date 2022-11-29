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
        playButton.interactable = DNAExporter.Deserialise().Count > 0;
        TMPro.TMP_Text text = playButton.GetComponentInChildren<TMPro.TMP_Text>();
        text.color = new Color(text.color.r, text.color.g, text.color.b, playButton.interactable ? 1.0f : 0.3f);

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
