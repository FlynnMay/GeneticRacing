using Evo;
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
        countdownReset = startCountdown;

        StartRace();
    }

    public void SetupCars()
    {
        if (GameManager.Instance.IsTraining)
        {
            cars = FindObjectsOfType<Car>().ToList();
        }
        else
        {
            GameObject player = Instantiate(playerPrefab);
            player.transform.position = spawnPositions[0].position;
            cars.Add(player.GetComponent<Car>());

            List<AICarInstance> options = DNAExporter.Deserialise();
            AICarController aiCarPrefab = aiPrefab.GetComponent<AICarController>();
            for (int i = 1; i < spawnPositions.Length; i++)
            {
                AICarController aiCar = Instantiate(aiCarPrefab);
                Transform t = spawnPositions[i];
                aiCar.SetPositionAndRotation(t.position, t.rotation);
                aiCar.FindAgent();
                aiCar.FromInstance(options[Random.Range(0, options.Count)]);
                cars.Add(aiCar);
            }
        }

        finishedPositions = new Car[cars.Count];

        foreach (Car car in cars)
            car.CanMove = carsCanMoveBeforeRaceStart;
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
