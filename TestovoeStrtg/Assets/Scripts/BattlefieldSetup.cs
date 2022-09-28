using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlefieldSetup : MonoBehaviour
{
    [Header ("Ground Setup")]
    [SerializeField] private GameObject Ground;
    [SerializeField] private float xScale = 100f;
    [SerializeField] private float yScale = 100f;
    [SerializeField] private float zScale = 100f;

    [Header("Unit Setup")]
    [SerializeField] private GameObject UnitPrefab;
    [SerializeField] private Transform UnitSpawn;
    [SerializeField] private int UnitCount = 1;

    [Header("Enemy Setup")]
    [SerializeField] private GameObject EnemyPrefab;
    [SerializeField] private Transform EnemySpawn;
    [SerializeField] private int EnemyCount = 1;


    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(xScale, yScale, zScale);
        for (int i = 1; i <= UnitCount; i++) {
            Instantiate(UnitPrefab, UnitSpawn.position, UnitSpawn.rotation);
        }
        for (int i = 1; i <= EnemyCount; i++) {
            Instantiate(EnemyPrefab, EnemySpawn.position, EnemySpawn.rotation);
        }
        
    }
}
