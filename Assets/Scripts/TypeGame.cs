using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TypeGame : MonoBehaviour
{
    [SerializeField] private Text coinsText;
    [SerializeField] private Text levelComplexityText;
    [SerializeField] private Button buyButton;
    [SerializeField] private GameObject buyMenu;
    [SerializeField] private GameObject notEnough;
    [SerializeField] private Text buyMenuAmountText;
    public int meduimComplexityPrice = 50;
    public int hardComplexityPrice = 100;

    private string complexityKey = "SelectedComplexity";

    private void Start()
    {
        levelComplexityText.text = PlayerPrefs.GetInt(complexityKey, 1) == 1 ? "Easy" : PlayerPrefs.GetInt(complexityKey, 1) == 2 ? "Medium" : "Hard";
    }

    public void EasyGame()
    {
        PlayerPrefs.SetInt(complexityKey, 1);
        SceneManager.LoadScene("Levels");
    }

    public void SelectComplexity(int levelComplexity)
    {
        buyMenu.SetActive(true);
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => BuyComplexity(levelComplexity));
        buyMenuAmountText.text = levelComplexity == 2 ? meduimComplexityPrice.ToString() : hardComplexityPrice.ToString();
    }

    private void BuyComplexity(int levelComplexity)
    {
        int coins = PlayerPrefs.GetInt("coins", 0);
        if (coins >= meduimComplexityPrice)
        {
            SoundManager.Instance.PlayClip(SoundManager.Instance.buySound);
            coins -= meduimComplexityPrice;
            PlayerPrefs.SetInt("coins", coins);
            PlayerPrefs.Save();
            UpdateCoinsText();
            PlayerPrefs.SetInt(complexityKey, 2);
            SceneManager.LoadScene("Levels");
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
        UpdateCoinsText();
    }

    private void UpdateCoinsText()
    {
        coinsText.text = PlayerPrefs.GetInt("coins", 0).ToString();
    }
}
