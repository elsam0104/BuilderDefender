using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager instance;
    public GameInfoSO gameInfo;

    private void Awake()
    {
        instance = this;
    }

    public void IncraseDifficulty()
    {
        if (gameInfo.difficulty > 1) return;
        gameInfo.difficulty += 0.5f;
    }
    public void DecraseDifficulty()
    {
        if (gameInfo.difficulty <= 0.5f) return;
        gameInfo.difficulty -= 0.5f;
    }
}
