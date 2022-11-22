using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAllCars : MonoBehaviour
{
    [SerializeField] float radius = 2.0f;
    Dictionary<Transform, Car> carsDict;
    Cinemachine.CinemachineTargetGroup cinemachineTargetGroup;
    List<Car> overrideCars = new List<Car>();

    private void Start()
    {
        Car[] cars = GameManager.RaceManager.GetCars().ToArray();
        carsDict = new Dictionary<Transform, Car>();
        cinemachineTargetGroup = GetComponent<Cinemachine.CinemachineTargetGroup>();
        for (int i = 0; i < cars.Length; i++)
        {
            carsDict.Add(cars[i].transform, cars[i]);
            cinemachineTargetGroup.AddMember(cars[i].transform, 1.0f, radius);
        }
    }

    private void Update()
    {
        for (int i = 0; i < cinemachineTargetGroup.m_Targets.Length; i++)
        {
            Cinemachine.CinemachineTargetGroup.Target target = cinemachineTargetGroup.m_Targets[i];
            Car car = carsDict[target.target];
            target.weight = car.KeepInView ? 1 : 0;
            target.radius = radius;

            //if (!car.gameObject.activeInHierarchy)
            //    target.weight = 0;

            if (overrideCars.Contains(car))
                target.weight = 1;
            else
                target.weight = 0;
            
            cinemachineTargetGroup.m_Targets[i] = target;
        }
    }

    public void SetOverrideCars(List<Car> carsToView)
    {
        overrideCars = carsToView;
    }

    public void ClearOverride()
    {
        overrideCars.Clear();
    }
}
