using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potal : MonoBehaviour
{
    private EndingUI endingUI;
    private bool canEnd = false;
    private float firstWaveTimer = 0;
    private float timer = 0;
    private void Start()
    {
        endingUI = FindObjectOfType<EndingUI>();
        firstWaveTimer = EnemyWaveManager.Instance.GetNextWaveSpawnTimer();
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > firstWaveTimer)
        {
            canEnd = true;
        }
    }
    public void Click()
    {
        if (!canEnd) return;
        endingUI.TurnOn();
    }

}
