using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RaceManager : MonoBehaviour
{
    [SerializeField] bool carsCanMoveBeforeRaceStart = false;
    [SerializeField] float startCountdown = 3.0f;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject aiPrefab;
    [SerializeField] Transform[] spawnPositions;
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

    private void Start()
    {
        if (!GameManager.Instance.IsTraining)
        {
            GameObject player = Instantiate(playerPrefab);
            player.transform.position = spawnPositions[0].position;
        }

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

    public List<Car> GetCars()
    {
        return cars;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            Transform t = spawnPositions[i];
            Gizmos.DrawSphere(t.position, 0.5f);
        }
    }
}
