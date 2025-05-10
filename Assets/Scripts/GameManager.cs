using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Text coinsText;
    [SerializeField] private Text levelComplexityText;
    [SerializeField] private Text timerText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Animator bowAnimator;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private GameObject basketPrefab;
    [SerializeField] private Vector2 ballSpawnPoint;
    [SerializeField] private Transform[] basketSpawnPoints;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Transform starsContainer;
    [SerializeField] private Sprite starSprite;
    [SerializeField] private Sprite emptyStarSprite;
    [SerializeField] private Image gameOverFrame;
    [SerializeField] private Sprite winFrame;
    [SerializeField] private Sprite loseFrame;
    [SerializeField] private Sprite tryAgain;
    [SerializeField] private Sprite nextLevel;
    [SerializeField] private Text gameOverScoreText;
    [SerializeField] private Button gameOverButton;

    public int levelsCount = 20;
    public float minThrowForce = 5f;
    public float maxThrowForce = 15f;

    private List<LevelData> levelDataList = new List<LevelData>();

    private int timerTime;
    private int maxScore;
    private int selectedLevel;
    private int selectedComplexity;

    private LevelData selectedLevelData = null;

    public int currentScore { get; set; }
    public int shotsInTarget { get; set; }
    public int misses { get; set; }

    private Rigidbody2D ballRb = null;
    private GameObject currentBasket;

    private int remainingTime;
    private bool isGameOver = false;
    private bool canThrow = true;

    private Vector3 mouseStartPos;  // Начальная позиция мыши при нажатии
    private Vector3 mouseEndPos;    // Конечная позиция мыши при отпускании
    private Vector2 force;

    private void Start()
    {
        shotsInTarget = 0;
        misses = 0;

        levelDataList = LevelDataManager.LoadLevels();

        selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        selectedComplexity = PlayerPrefs.GetInt("SelectedComplexity", 1);

        levelComplexityText.text = PlayerPrefs.GetInt("SelectedComplexity", 1) == 1 ? "Easy" : PlayerPrefs.GetInt("SelectedComplexity", 1) == 2 ? "Medium" : "Hard";

        Debug.Log($"Selected Level {selectedLevel}");

        selectedLevelData = levelDataList.Find(data => selectedLevel == data.levelIndex);
        timerTime = selectedLevelData.timer;
        maxScore = selectedLevelData.score;

        if(selectedComplexity == 2) //medium
        {
            maxScore = Mathf.FloorToInt(maxScore * 1.5f);
            timerTime = Mathf.FloorToInt(timerTime * 0.75f);
        }
        else if(selectedComplexity == 3) //hard
        {
            maxScore *= 2;
            timerTime = Mathf.FloorToInt(timerTime * 0.5f);
        }

        Debug.Log($"Selected Level: {selectedLevel}. Selected Level Complexity: {selectedComplexity}");

        gameOverPanel.SetActive(false);

        UpdateText();

        SpawnBallAndBasket();

        remainingTime = timerTime;
        StartCoroutine(Timer());
    }

    public void SpawnBallAndBasket()
    {
        if(ballRb != null)
        {
            Destroy(ballRb.gameObject);
            ballRb = null;
        }

        if(currentBasket != null)
        {
            Destroy(currentBasket);
            currentBasket = null;
        }

        canThrow = true;

        currentBasket = Instantiate(basketPrefab, basketSpawnPoints[Random.Range(0, basketSpawnPoints.Length)].position, Quaternion.identity);
    }

    private void Update()
    {
        if (isGameOver)
            return;

        // Начало нажатия левой кнопки мыши
        if (Input.GetMouseButtonDown(0) && canThrow)
        {
            mouseStartPos = Input.mousePosition; // Запоминаем начальную позицию
            mouseStartPos.z = 0; // Устанавливаем z в 0 для 2D
        }

        // Отпускание левой кнопки мыши
        if (Input.GetMouseButtonUp(0) && canThrow)
        {
            mouseEndPos = Input.mousePosition; // Запоминаем конечную позицию
            mouseEndPos.z = 0; // Устанавливаем z в 0 для 2D

            // Вычисляем дельту (разницу) между начальной и конечной позицией
            Vector2 delta = (mouseEndPos - mouseStartPos) * -0.05f; // Изменяем направление силы
            force = delta;

            // Ограничиваем силу броска минимальным и максимальным значениями
            float forceMagnitude = Mathf.Clamp(force.magnitude, minThrowForce, maxThrowForce);
            force = force.normalized * forceMagnitude;

            canThrow = false;

            AnimateBow();
        }
    }

    private void AnimateBow()
    {
        bowAnimator.SetTrigger("Shoot");
    }

    public void ThrowBall()
    {
        GameObject newBall = Instantiate(ballPrefab, new Vector3(ballSpawnPoint.x, ballSpawnPoint.y, 0), Quaternion.identity);
        ballRb = newBall.GetComponent<Rigidbody2D>();

        if (ballRb != null)
        {
            ballRb.velocity = Vector2.zero;

            ballRb.AddForce(force, ForceMode2D.Impulse);
        }
    }

    public void IncreaseScore()
    {
        currentScore++;

        if(currentScore >= maxScore)
        {
            SoundManager.Instance.PlayClip(SoundManager.Instance.winSound);
            GameOver(true);
        }
        else
        {
            SoundManager.Instance.PlayClip(SoundManager.Instance.scoreSound);
        }

        UpdateText();
    }

    private void UpdateText()
    {
        coinsText.text = PlayerPrefs.GetInt("coins", 0).ToString();
        scoreText.text = $"{currentScore}/{maxScore}";
        timerText.text = $"{remainingTime}";
    }

    private IEnumerator Timer()
    {
        while (remainingTime > 0 && !isGameOver)
        {
            yield return new WaitForSeconds(1);
            remainingTime -= 1;
            UpdateText();
        }

        if (remainingTime <= 0)
        {
            GameOver(false);
        }
    }

    private void GameOver(bool isWin)
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);

        int stars = 0;

        // Рассчитываем процент промахов
        int totalShots = shotsInTarget + misses;
        if (totalShots > 0)
        {
            float missPercentage = (float)misses / totalShots * 100;

            if (missPercentage == 0)
            {
                stars = 3; // Все шары попали в цель
            }
            else if (missPercentage <= 25)
            {
                stars = 2; // Промахов не больше 10%
            }
            else
            {
                stars = 1;
            }
        }

        if (isWin)
        {
            gameOverFrame.sprite = winFrame;
            int coins = PlayerPrefs.GetInt("coins", 0);
            coins += 30;
            PlayerPrefs.SetInt("coins", coins);
            PlayerPrefs.Save();
            UpdateText();
            gameOverButton.image.sprite = nextLevel;
            gameOverScoreText.gameObject.SetActive(false);
            gameOverButton.onClick.RemoveAllListeners();
            int nextLevelNumber = selectedLevel + 1;
            Debug.Log($"Next Level_{nextLevel}");
            LevelData nextLevelData = levelDataList.Find(data => data.levelIndex == nextLevelNumber);
            selectedLevelData.isCompleted = true;
            LevelDataManager.SaveLevels(levelDataList);

            if (nextLevelData == null)
            {
                nextLevelNumber = 1;
            }
            if (!nextLevelData.isUnlocked)
            {
                gameOverButton.image.sprite = tryAgain;
            }
            else
            {
                gameOverButton.image.sprite = nextLevel;
            }

            gameOverButton.onClick.AddListener(() =>
            {
                if (!nextLevelData.isUnlocked)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                else
                {
                    PlayerPrefs.SetInt("SelectedLevel", nextLevelNumber);
                    PlayerPrefs.Save();
                }

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });
            Debug.Log("Win!");
        }
        else
        {
            SoundManager.Instance.PlayClip(SoundManager.Instance.gameoverSound);
            stars = 0;
            gameOverFrame.sprite = loseFrame;
            gameOverScoreText.text = $"{currentScore}/{maxScore}";
            gameOverButton.image.sprite = tryAgain;
            gameOverButton.onClick.RemoveAllListeners();
            gameOverButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });
            Debug.Log("Game Over!");
        }

        // Обновляем визуализацию звёзд
        for (int i = 0; i < starsContainer.childCount; i++)
        {
            if (i < stars)
            {
                starsContainer.GetChild(i).GetComponent<Image>().sprite = starSprite;
            }
            else
            {
                starsContainer.GetChild(i).GetComponent<Image>().sprite = emptyStarSprite;
            }
        }
    }
}
