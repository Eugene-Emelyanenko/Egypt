using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Settings : MonoBehaviour
{
    public Sprite[] offon;
    public Image[] sound_offon;
    public Image[] music_offon;
    public GameObject[] bg_checks;
    void Start()
    {
        var music = PlayerPrefs.GetInt("music", 1);
        var sound = PlayerPrefs.GetInt("sound", 1);
        var bg = PlayerPrefs.GetInt("bg", 3);
        if (music == 0)
        {
            music_offon[0].sprite = offon[1];
            music_offon[1].sprite = offon[0];
            SoundManager.Instance.TurnOffMusic();
        }
        else
        {
            music_offon[0].sprite = offon[0];
            music_offon[1].sprite = offon[1];
            SoundManager.Instance.TurnOffMusic();
        }
        if (sound == 0)
        {
            sound_offon[0].sprite = offon[1];
            sound_offon[1].sprite = offon[0];
            SoundManager.Instance.TurnOffSfx();
        }
        else
        {
            sound_offon[0].sprite = offon[0];
            sound_offon[1].sprite = offon[1];
            SoundManager.Instance.TurnOnSfx();
        }
        foreach (GameObject bg_check in bg_checks)
        {
            bg_check.SetActive(false);
        }
        bg_checks[bg].SetActive(true);
    }

    public void SetMusic(int val)
    {
        PlayerPrefs.SetInt("music", val);
        SceneManager.LoadScene("Settings");
    }
    public void SetSound(int val)
    {
        PlayerPrefs.SetInt("sound", val);
        SceneManager.LoadScene("Settings");
    }
    public void SetBG(int val)
    {
        PlayerPrefs.SetInt("bg", val);
        SceneManager.LoadScene("Settings");
    }
}