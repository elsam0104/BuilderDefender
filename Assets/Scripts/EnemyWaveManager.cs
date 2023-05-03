using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class EnemyWaveManager : MonoBehaviour
{
    public static EnemyWaveManager Instance { get; private set; }

    private enum State
    {
        WaitingToSpawnNextWave,
        SpawningWave,
        SpawinigBoss,
    }

    public event EventHandler OnWaveNumberChanged;

    [SerializeField] private List<Transform> spawnPositionTransformList;
    [SerializeField] private Transform nextWaveSpawnPositionTransform;
    private State state;
    private int waveNumber;
    private float nextWaveSpawnTimer;
    private float nextEnemySpawnTimer;
    private int remainingEnemySpawnAmount;
    private Vector3 spawnPosition;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        state = State.WaitingToSpawnNextWave;
        spawnPosition = spawnPositionTransformList[UnityEngine.Random.Range(0, spawnPositionTransformList.Count)].position;
        nextWaveSpawnPositionTransform.position = spawnPosition;
        nextWaveSpawnTimer = 3f;
    }

    private void Update()
    {
        switch (state)
        {
            case State.WaitingToSpawnNextWave:
                nextWaveSpawnTimer -= Time.deltaTime;
                if (nextWaveSpawnTimer < 0f)
                {
                    if (waveNumber == 30)
                    {
                        SpawnWave();
                        state = State.SpawinigBoss;
                    }
                    else
                    {
                        SpawnWave();
                        state = State.SpawningWave;
                    }
                }
                break;
            case State.SpawningWave:
                if (remainingEnemySpawnAmount > 0)
                {
                    nextEnemySpawnTimer -= Time.deltaTime;
                    if (nextEnemySpawnTimer < 0f)
                    {
                        nextEnemySpawnTimer = UnityEngine.Random.Range(0f, .2f);
                        DefaultEnemy.Create(spawnPosition + UtilClass.GetRandomDir() * UnityEngine.Random.Range(0f, 10f));
                        remainingEnemySpawnAmount--;
                        if (remainingEnemySpawnAmount <= 0)
                        {
                            if (waveNumber+1 % 14 == 0)
                            {
                                state = State.SpawinigBoss;
                            }
                            state = State.WaitingToSpawnNextWave;
                            spawnPosition = spawnPositionTransformList[UnityEngine.Random.Range(0, spawnPositionTransformList.Count)].position;
                            nextWaveSpawnPositionTransform.position = spawnPosition;
                            nextWaveSpawnTimer = 10f;
                        }
                    }
                }
                break;
            case State.SpawinigBoss:
                Boss.Create(nextWaveSpawnPositionTransform.position);
                state = State.WaitingToSpawnNextWave;
                spawnPosition = spawnPositionTransformList[UnityEngine.Random.Range(0, spawnPositionTransformList.Count)].position;
                nextWaveSpawnPositionTransform.position = spawnPosition;
                nextWaveSpawnTimer = 10f;
                break;
        }
    }

    private void SpawnWave()
    {
        remainingEnemySpawnAmount = Mathf.RoundToInt(3 + 1.5f * waveNumber * DifficultyManager.instance.gameInfo.difficulty);
        waveNumber++;
        OnWaveNumberChanged?.Invoke(this, EventArgs.Empty);
    }
    public void UpdateSpawnTime()
    {
        nextWaveSpawnTimer = 3f * (4f - DifficultyManager.instance.gameInfo.difficulty);
    }
    public int GetWaveNumber()
    {
        return waveNumber;
    }

    public float GetNextWaveSpawnTimer()
    {
        return nextWaveSpawnTimer;
    }

    public Vector3 GetSpawnPosition()
    {
        return spawnPosition;
    }
}
