using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenController : MonoBehaviour
{
    [SerializeField] GeneticTitle winnerGeneticTitle;
    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        Toggle(false);
    }

    public void Gameover(Car car)
    {
        string txt = $"{car.carName} Won!";
        winnerGeneticTitle.title = txt;
        winnerGeneticTitle.Setup();
        Toggle(true);
    }

    void Toggle(bool toggle)
    {
        image.enabled = toggle;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(toggle);
        }
    }
}
