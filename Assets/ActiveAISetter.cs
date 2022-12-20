using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActiveAISetter : MonoBehaviour
{
    public event Action onAnyActiveStateChanged;
    [SerializeField] GameObject contentField;
    [SerializeField] GameObject warningObject;
    [SerializeField] AIInformationWriter aiInformationPrefab;
    Dictionary<AICarInstance, bool> activeAI;
    List<AICarInstance> allAI;

    void Awake()
    {
        activeAI = new Dictionary<AICarInstance, bool>();
        allAI = DNAExporter.Deserialise();

        if (allAI.Count <= 0)
        {
            AICarInstance demo1 = new AICarInstance("Demo AI", new float[] {
                53.66473388671875f,
                2.0492255687713625f,
                4.754626750946045f
            }, 12, 0);

            demo1.Serialise();
            allAI.Add(demo1);
        }

        warningObject.SetActive(allAI.Count <= 0);

        for (int i = 0; i < allAI.Count; i++)
        {
            AICarInstance ai = allAI[i];
            activeAI.Add(ai, false);
            AIInformationWriter writer = Instantiate(aiInformationPrefab, contentField.transform);
            writer.Write(ai);
            writer.onActiveStateChanged += (active) => { activeAI[ai] = active; };
            writer.onActiveStateChanged += _ => ActiveStateChanged();
        }
    }

    public void SetValidAiIndicies()
    {
        List<AICarInstance> validCars = activeAI.Where(x => x.Value).Select(x => x.Key).ToList();
        PlayerPrefs.SetInt(GRPrefKeys.GRIndiciesCount, validCars.Count);

        for (int i = 0; i < validCars.Count; i++)
        {
            PlayerPrefs.SetInt((GRPrefKeys.GRIndicies + i).ToString(), allAI.IndexOf(validCars[i]));
        }
    }

    public bool AnyActive()
    {
        return activeAI.Any(x => x.Value);
    }

    private void ActiveStateChanged()
    {
        onAnyActiveStateChanged?.Invoke();
    }
}
