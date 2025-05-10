using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

[Serializable]
public class LevelData
{
    public int levelIndex;
    public int timer;
    public int score;
    public int price;
    public bool isUnlocked;
    public bool isCompleted;

    public LevelData(int levelIndex, int timer, int score, int price, bool isUnlocked, bool isCompleted)
    {
        this.levelIndex = levelIndex;
        this.timer = timer;
        this.score = score;
        this.price = price;
        this.isUnlocked = isUnlocked;
        this.isCompleted = isCompleted;
    }
}

public static class LevelDataManager
{
    public static readonly string levelsDataKey = "LevelsData";

    public static List<LevelData> LoadLevels()
    {
        if (PlayerPrefs.HasKey(levelsDataKey))
        {
            string jsonData = PlayerPrefs.GetString(levelsDataKey);

            LevelDataWrapper levelDataWrapper = JsonUtility.FromJson<LevelDataWrapper>(jsonData);

            return levelDataWrapper.levelDataList;
        }

        return new List<LevelData>();
    }

    public static void SaveLevels(List<LevelData> levelDataList)
    {
        LevelDataWrapper levelDataWrapper = new LevelDataWrapper(levelDataList);

        string jsonData = JsonUtility.ToJson(levelDataWrapper);

        PlayerPrefs.SetString(levelsDataKey, jsonData);

        PlayerPrefs.Save();
    }
}

[Serializable]
public class LevelDataWrapper
{
    public List<LevelData> levelDataList = new List<LevelData>();

    public LevelDataWrapper(List<LevelData> levelDataList)
    {
        this.levelDataList = levelDataList;
    }
}

public class Levels : MonoBehaviour
{
    [SerializeField] private Transform levelsContainer;
    [SerializeField] private GameObject levelPrefab;
    [SerializeField] private Text levelComplexityText;
    [SerializeField] private Text coinsText;
    [SerializeField] private GameObject buyMenu;
    [SerializeField] private Text priceText;
    [SerializeField] private GameObject notEnough;

    public int levelsCount = 20;

    private List<LevelData> levelDataList = new List<LevelData>();
    private LevelData selectedLevel = null;

    private void Start()
    {
        levelComplexityText.text = PlayerPrefs.GetInt("SelectedComplexity", 1) == 1 ? "Easy" : PlayerPrefs.GetInt("SelectedComplexity", 1) == 2 ? "Medium" : "Hard";
        levelDataList = LevelDataManager.LoadLevels();
        if (levelDataList.Count == 0)
            GenerateDefaultData();

        DisplayLevels();
    }

    private void GenerateDefaultData()
    {
        levelDataList = new List<LevelData>();

        int initialTimer = 25;
        int timerIncreaser = 10;
        int initialScore = 5;
        int scoreIncreaser = 2;

        int initialLevelPrice = 10;
        int levelPriceIncreaser = 10;
        int currentPrice = initialLevelPrice;

        for (int i = 0; i < levelsCount; i++)
        {
            int timer = initialTimer + i * timerIncreaser;
            int score = initialScore + i * scoreIncreaser;

            if (i > 0 && i % 4 == 0)
            {
                currentPrice += levelPriceIncreaser;
            }

            levelDataList.Add(new LevelData(i + 1, timer, score, currentPrice, i == 0, false));
            Debug.Log($"Created Level_{i + 1}. Timer: {timer}. Score: {score}. Price: {currentPrice}. IsUnclocked: {i == 0}. IsCompleted: {false}");
        }

        LevelDataManager.SaveLevels(levelDataList);
    }

    private void DisplayLevels()
    {
        foreach (Transform t in levelsContainer)
        {
            Destroy(t.gameObject);
        }

        foreach (LevelData data in levelDataList)
        {
            GameObject levelObject = Instantiate(levelPrefab, levelsContainer);
            levelObject.name = $"Level_{data.levelIndex}";
            LevelUI levelUI = levelObject.GetComponent<LevelUI>();
            levelUI.SetUp(data);
            levelUI.button.onClick.RemoveAllListeners();
            levelUI.button.onClick.AddListener(() => 
            {
                if(data.isUnlocked)
                {
                    PlayerPrefs.SetInt("SelectedLevel", data.levelIndex);
                    PlayerPrefs.Save();
                    SceneManager.LoadScene("Game");
                }
                else
                {
                    priceText.text = data.price.ToString();
                    buyMenu.SetActive(true);
                    selectedLevel = data;
                }                
            });
        }
    }

    public void BuyLevel()
    {
        var coins = PlayerPrefs.GetInt("coins", 0);
        int price = selectedLevel.price;
        if (coins >= price)
        {
            SoundManager.Instance.PlayClip(SoundManager.Instance.buySound);
            coins -= price;
            selectedLevel.isUnlocked = true;
            PlayerPrefs.SetInt("coins", coins);
            PlayerPrefs.Save();
            buyMenu.SetActive(false);
            UpdateCoinsUI();
            LevelDataManager.SaveLevels(levelDataList);
            DisplayLevels();
        }
        else
        {
            notEnough.SetActive(true);
        }
    }
    public void IncreaseCoins()
    {
        var coins = PlayerPrefs.GetInt("coins", 0);
        coins += 20;
        PlayerPrefs.SetInt("coins", coins);
        PlayerPrefs.Save();
        UpdateCoinsUI();
    }
    void UpdateCoinsUI()
    {
        var coins = PlayerPrefs.GetInt("coins", 0);
        coinsText.text = coins.ToString();
    }
}
