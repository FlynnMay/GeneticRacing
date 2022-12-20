using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AIInformationWriter : MonoBehaviour
{
    public event Action<bool> onActiveStateChanged;
    [SerializeField] Image previewImage;
    [SerializeField] Image usingImage;
    [SerializeField] Button button;
    [SerializeField] TMP_Text label;
    [SerializeField] TMP_Text desc;
    [SerializeField] Sprite[] sprites;
    bool activeAI = false;

    private void OnEnable()
    {
        button.onClick.AddListener(ButtonPressed);
        onActiveStateChanged += UsingIconToggleDisplay;
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(ButtonPressed);
    }

    public void Write(AICarInstance ai)
    {
        label.text = ai.name;
        string genesString = "";
        for (int i = 0; i < ai.genes.Length; i++)
        {
            float gene = ai.genes[i];
            bool islast = i == ai.genes.Length - 1;
            genesString += gene.ToString() + ((!islast) ? ", " : "");
        }
        desc.text = $"Generations: {ai.generations} \n" +
            $"Genes: {genesString}";
        previewImage.sprite = sprites[ai.colourIndex];
    }

    private void UsingIconToggleDisplay(bool isUsing)
    {
        usingImage.color = isUsing ? Color.green : Color.white;
    }

    private void ButtonPressed()
    {
        activeAI = !activeAI;

        onActiveStateChanged?.Invoke(activeAI);
    }
}
