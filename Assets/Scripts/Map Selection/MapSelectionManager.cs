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
    [SerializeField] ActiveAISetter activeAISetter;
    TMPro.TMP_Text pbText;

    private void Awake()
    {
        pbText = playButton.GetComponentInChildren<TMPro.TMP_Text>();
        SetButtonInteractableColour();

        for (int i = 0; i < mapContainer.Maps.Count; i++)
        {
            Map map = mapContainer.Maps[i];
            Instantiate(mapPrefab, mapParent.transform).SetMap(map);
        }
    }

    private void Start()
    {
        StateChanged();
    }

    private void OnEnable()
    {
        playButton.onClick.AddListener(OnPlay);
        trainButton.onClick.AddListener(OnTrain);
        activeAISetter.onAnyActiveStateChanged += StateChanged;
    }
    
    private void OnDisable()
    {
        playButton.onClick.RemoveListener(OnPlay);
        trainButton.onClick.RemoveListener(OnTrain);
        activeAISetter.onAnyActiveStateChanged -= StateChanged;
    }

    private void OnPlay()
    {
        mapParent.SetActive(true);
        trainSender.SetTraining(false);
        activeAISetter.SetValidAiIndicies();
    }

    private void OnTrain()
    {
        mapParent.SetActive(true);
        trainSender.SetTraining(true);
    }

    public void StateChanged()
    {
        playButton.interactable = activeAISetter.AnyActive();
        SetButtonInteractableColour();
    }

    private void SetButtonInteractableColour()
    {
        pbText.color = new Color(pbText.color.r, pbText.color.g, pbText.color.b, playButton.interactable ? 1.0f : 0.3f);
    }
}
