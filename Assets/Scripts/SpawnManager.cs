using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    public static float time;
    [SerializeField] private GameObject ennemy;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    private void Update() {
        time += Time.deltaTime;
        Spawn();
    }

    private void Spawn() {
        if(time < 3f) return;
        time = 0;
        int random = Random.Range(0, spawnPoints.Count);
        Instantiate(ennemy, spawnPoints[random]);
    }
}
