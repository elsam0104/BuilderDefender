using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionUI : MonoBehaviour
{
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private MusicManager musicManager;
    private TextMeshProUGUI soundVolumeText;
    private TextMeshProUGUI musicVolumeText;
    private TextMeshProUGUI difficultyText;

    private void Awake()
    {
        soundVolumeText = transform.Find("soundVolumeText").GetComponent<TextMeshProUGUI>();
        musicVolumeText = transform.Find("musicVolumeText").GetComponent<TextMeshProUGUI>();
        difficultyText = transform.Find("difficultyVolumeText").GetComponent<TextMeshProUGUI>();

        transform.Find("soundIncreaseBtn").GetComponent<Button>().onClick.AddListener(() =>{
            soundManager.IncreaseVolume();
            UpdateText();

        });
        transform.Find("soundDecreaseBtn").GetComponent<Button>().onClick.AddListener(() => {
            soundManager.DecreaseVolume();
            UpdateText();
        });
        transform.Find("musicIncreaseBtn").GetComponent<Button>().onClick.AddListener(() => {
            musicManager.IncreaseVolume();
            UpdateText();
        });
        transform.Find("musicDecreaseBtn").GetComponent<Button>().onClick.AddListener(() => {
            musicManager.DecreaseVolume();
            UpdateText();
        });
        transform.Find("mainMenuBtn").GetComponent<Button>().onClick.AddListener(() => {
            Time.timeScale = 1f;
            GameSceneManager.Load(GameSceneManager.Scene.MainMenuScene);
        });
        transform.Find("edgeScrollingToggle").GetComponent<Toggle>().onValueChanged.AddListener((bool set) =>
        {
            CameraHandler.Instance.SetEdgeScrolling(set);
        });
        transform.Find("difficultyIncreaseBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            DifficultyManager.instance.IncraseDifficulty();
            EnemyWaveManager.Instance.UpdateSpawnTime();
            UpdateText();
        });
        transform.Find("difficultyDecreaseBtn").GetComponent<Button>().onClick.AddListener(() =>
        {
            DifficultyManager.instance.DecraseDifficulty();
            EnemyWaveManager.Instance.UpdateSpawnTime();
            UpdateText();
        });

    }
    private void Start()
    {
        UpdateText();
        gameObject.SetActive(false);

        transform.Find("edgeScrollingToggle").GetComponent<Toggle>().SetIsOnWithoutNotify(CameraHandler.Instance.GetEdgeScrolling());
    }

    private void UpdateText()
    {
        soundVolumeText.SetText(Mathf.RoundToInt(soundManager.GetVolume() * 10).ToString());
        musicVolumeText.SetText(Mathf.RoundToInt(musicManager.GetVolume() * 10).ToString());
        difficultyText.SetText(DifficultySet(DifficultyManager.instance.gameInfo.difficulty));
    }

    private static string DifficultySet(float difficulty) => difficulty switch
    {
        >= 1.4f => "HARD",
        > 0.5f and <= 1.4f => "NORMAL",
        <= 0.5f => "EASY",
    };
    public void ToggleVisible()
    {
        gameObject.SetActive(!gameObject.activeSelf);

        if (gameObject.activeSelf)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
