using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RaceManager : MonoBehaviour
{
    [SerializeField] bool carsCanMoveBeforeRaceStart = false;
    [SerializeField] float startCountdown = 3.0f;
    public UnityEvent<float> OnCountdownValueChanged;
    public UnityEvent<Car> OnRaceFinished;
    public float StartCountdown
    {
        get => startCountdown;
        set
        {
            startCountdown = value;
            OnCountdownValueChanged?.Invoke(value);
        }
    }
    float countdownReset;
    List<Car> cars = new List<Car>();
    Car[] finishedPositions;
    int finishedRacers = 0;

    private void Awake()
    {
        cars = FindObjectsOfType<Car>().ToList();
        finishedPositions = new Car[cars.Count];

        foreach (Car car in cars)
            car.CanMove = carsCanMoveBeforeRaceStart;

        countdownReset = startCountdown;

        StartRace();
    }

    public void StartRace()
    {
        StartCoroutine(StartRoutine());
    }

    IEnumerator StartRoutine()
    {
        StartCountdown = countdownReset;
        while (startCountdown > 0.0f)
        {
            StartCountdown -= Time.deltaTime;
            yield return null;
        }

        foreach (Car car in cars)
            car.CanMove = true;
    }

    public void CarFinishedRace(Car finsishedCar)
    {
        if (GameManager.Instance.IsTraining)
            return;

        finishedPositions[finishedRacers] = finsishedCar;
        finishedRacers++;

        if (finishedRacers >= cars.Count)
            OnRaceFinished?.Invoke(finishedPositions.First());
    }
}
