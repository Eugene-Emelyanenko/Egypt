using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;

public class DataLoading : MonoBehaviour
{
    public int music = 0;
    public int sound = 0;
    public int background = 0;
    public int coins = 0; 
    public Sprite[] bgs;
    public Image Bg;
    [CanBeNull] public Text coins_text;
    void Start()
    {
        music = PlayerPrefs.GetInt("music", 1);
        if(music == 1)
            SoundManager.Instance.TurnOnMusic();
        else
            SoundManager.Instance.TurnOffMusic();
        sound = PlayerPrefs.GetInt("sound", 1);
        if (sound == 1)
            SoundManager.Instance.TurnOnSfx();
        else
            SoundManager.Instance.TurnOffSfx();

        background = PlayerPrefs.GetInt("bg", 3);

        coins = PlayerPrefs.GetInt("coins", 0);

        if (coins_text != null)
        {
            coins_text.text = "" + coins;
        }
        Bg.GetComponent<Image>().sprite = bgs[background];
    }
}