using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class AISpectator : MonoBehaviour
{
    [SerializeField] FollowAllCars followCars;
    [SerializeField] TMPro.TMP_InputField inputField;
    [SerializeField] TMPro.TMP_InputField nameInputField;
    int aiIndex = -1;
    Car car;

    private void OnEnable()
    {
        inputField.onEndEdit.AddListener(SetFromInputField);
        UpdateInputField();
    }

    private void OnDisable()
    {
    }

    private void Start()
    {
        SetViewedAI();
    }

    private void UpdateInputField()
    {
        inputField.text = aiIndex.ToString();
    }

    public void SetViewedAI()
    {
        UpdateInputField();
        SetForceRender(false);

        if (aiIndex == -1)
        {
            car = null;
            followCars.ClearOverride();
            return;
        }

        car = GameManager.RaceManager.GetCars()[aiIndex];
        SetForceRender(true);
        followCars.SetOverrideCars(new List<Car> { car });
    }

    private void SetForceRender(bool shouldForce)
    {
        AICarController ai = car as AICarController;
        if (ai != null)
            ai.forceRender = shouldForce;
    }

    public void Save()
    {
        AICarController carToSave = (car as AICarController);
        carToSave.name = nameInputField.text;
        
        if (carToSave == null)
            return;

        carToSave.Serialise();
    }

    public void Next()
    {
        aiIndex++;

        if (aiIndex >= GameManager.RaceManager.GetCars().Count)
            aiIndex = -1;

        SetViewedAI();
    }

    public void Prev()
    {
        aiIndex--;

        if (aiIndex < -1)
            aiIndex = GameManager.RaceManager.GetCars().Count - 1;

        SetViewedAI();
    }

    void SetFromInputField(string txt)
    {
        aiIndex = int.Parse(txt);

        if (aiIndex >= GameManager.RaceManager.GetCars().Count)
            aiIndex = -1;

        if (aiIndex < -1)
            aiIndex = GameManager.RaceManager.GetCars().Count - 1;

        SetViewedAI();
    }

    public void Best()
    {
        List<Car> cars = GameManager.RaceManager.GetCars();
        Car best = cars.Where(c => (c as AICarController) != null).Cast<AICarController>().FirstOrDefault(c => c.agent.IsKing);
        aiIndex = cars.IndexOf(best);
        SetViewedAI();
    }
}
