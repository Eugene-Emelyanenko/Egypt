using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private Image levelIcon;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite completedSprite;
    [SerializeField] private Sprite LockedSprite;
    [SerializeField] private Text levelNumberText;
    [SerializeField] private Text priceText;
    [SerializeField] private GameObject isCompletedObject;
    public Button button;
    public LevelData levelData;

    public void SetUp(LevelData data)
    {
        levelData = data;

        levelNumberText.text = data.levelIndex.ToString();
        priceText.text = data.price.ToString();

        if(data.isUnlocked)
        {
            levelIcon.sprite = defaultSprite;
            priceText.gameObject.SetActive(false);
            levelNumberText.gameObject.SetActive(true);
            isCompletedObject.SetActive(false);
            if (data.isCompleted)
            {
                levelIcon.sprite = completedSprite;
                levelNumberText.gameObject.SetActive(false);
                isCompletedObject.SetActive(true);
            }
        }
        else
        {
            levelIcon.sprite = LockedSprite;
            priceText.gameObject.SetActive(true);
            levelNumberText.gameObject.SetActive(false);
            isCompletedObject.SetActive(false);
        }
    }
}
