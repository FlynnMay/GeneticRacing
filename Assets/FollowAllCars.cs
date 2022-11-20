using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAllCars : MonoBehaviour
{
    [SerializeField] float radius = 2.0f;
    Dictionary<Transform, Car> carsDict;
    Cinemachine.CinemachineTargetGroup cinemachineTargetGroup;
    private void Awake()
    {
        Car[] cars = FindObjectsOfType<Car>();
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

            if (!car.gameObject.activeInHierarchy)
                target.weight = 0;
            
            cinemachineTargetGroup.m_Targets[i] = target;
        }
    }
}
