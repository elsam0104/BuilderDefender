using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndingUI : MonoBehaviour
{
    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private TMP_Text endScoreText;

    private void Start()
    {
        panel.SetActive(false);
    }
    public void TurnOn()
    {
        Time.timeScale = 0f;
        panel.SetActive(true);
        endScoreText.text = $"Score : {DifficultyManager.instance.gameInfo.difficulty * EnemyWaveManager.Instance.GetWaveNumber() * 100}";
    }
    public void TurnMain()
    {
        Time.timeScale = 1f;
        GameSceneManager.Load(GameSceneManager.Scene.MainMenuScene);
    }
}
