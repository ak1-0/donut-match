using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private int score = 0;
    private int highScore = 0;

    public Text scoreTextPrefab;
    private Text scoreText;
    public string bonusDonutTag;

    public GameObject resultsPanel;
    public Text resultsText;
    public Button nextLevelButton;

    public float animationDuration = 0.2f; // Продолжительность анимации
    public float scaleMultiplier = 1f;   // Множитель масштаба

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Инициализация UI компонентов
        scoreText = GameObject.Find("ScoreText")?.GetComponent<Text>();
        resultsPanel = GameObject.Find("ResultsPanel");
        resultsText = resultsPanel?.transform.Find("ResultsText")?.GetComponent<Text>();
        nextLevelButton = resultsPanel?.transform.Find("NextLevelButton")?.GetComponent<Button>();
        Button restartButton = resultsPanel?.transform.Find("RestartButton")?.GetComponent<Button>();

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(OnNextLevelButtonClicked);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartButtonClicked);
        }

        if (resultsPanel != null)
        {
            resultsPanel.SetActive(false);
        }

        // Инициализация игрового поля
        UpdateScoreText();

    }

    private void OnRestartButtonClicked()
    {
        resultsPanel.SetActive(false);
        GameManager.Instance.RestartGame();
    }

    private void CheckLevelGoal()
    {
        int currentLevel = GameManager.Instance.currentLevel;
        int levelGoal = GetLevelGoal(currentLevel);

        if (score >= levelGoal)
        {
            ShowResultsPanel();
        }
    }

    private int GetLevelGoal(int level)
    {
        switch (level)
        {
            case 0: return 50;
            case 1: return 150;
            case 2: return 200;
            case 3: return 400;
            case 4: return 600;
            case 5: return 800;
            case 6: return 1000;
            case 7: return 1200;
            default: return 0;
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("Текущий счет: " + score);
        UpdateScoreText();

        CheckLevelGoal();

        if (score > highScore)
        {
            highScore = score;
            Debug.Log("Новый рекорд: " + highScore);
            YandexGame.NewLeaderboardScores("Record", highScore);
        }

        // Сохранение текущего счета
        YandexGame.savesData.score = score;
        YandexGame.SaveProgress();
    }

    public int GetScore()
    {
        return score;
    }

    public void SetScore(int newScore)
    {
        score = newScore;
        UpdateScoreText();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    private void ShowResultsPanel()
    {
        if (resultsText != null)
        {
            resultsText.text = score.ToString();
        }

        if (resultsPanel != null)
        {
            resultsPanel.SetActive(true);
        }

        StartCoroutine(AnimateResultsText());
    }

    private IEnumerator AnimateResultsText()
    {
        if (resultsText == null)
        {
            yield break;
        }

        Vector3 originalScale = resultsText.transform.localScale;
        Vector3 targetScale = originalScale * scaleMultiplier;

        Color[] colors = new Color[] { Color.yellow };
        int colorIndex = 0;

        float time = 0f;
        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(1f, scaleMultiplier, time / animationDuration);
            resultsText.transform.localScale = originalScale * scale;

            resultsText.color = Color.Lerp(resultsText.color, colors[colorIndex], Time.deltaTime * 5f);

            yield return null;
        }

        time = 0f;
        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(scaleMultiplier, 1f, time / animationDuration);
            resultsText.transform.localScale = originalScale * scale;

            resultsText.color = Color.Lerp(resultsText.color, colors[colorIndex], Time.deltaTime * 5f);

            yield return null;
        }

        resultsText.transform.localScale = originalScale;
        resultsText.color = Color.white;
    }

    private void OnNextLevelButtonClicked()
    {
        resultsPanel.SetActive(false);
        GameManager.Instance.NextLevel();
    }
}