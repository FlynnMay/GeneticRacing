using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RaceStartUI : MonoBehaviour
{
    TMP_Text textMesh;
    CanvasGroup canvasGroup;

    private void Awake()
    {
        textMesh = GetComponentInChildren<TMP_Text>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        GameManager.RaceManager.OnCountdownValueChanged.AddListener(UpdateText);
    }

    private void OnDisable()
    {
        GameManager.RaceManager.OnCountdownValueChanged.RemoveListener(UpdateText);
    }

    public void UpdateText(float cooldownTime)
    {
        textMesh.text = cooldownTime.ToString("0");
        canvasGroup.alpha = cooldownTime > 0 ? 1 : 0;
    }
}
