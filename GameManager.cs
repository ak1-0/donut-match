using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

public class GameManager : MonoBehaviour
{
    public SceneFader sceneFader;
    public static GameManager Instance { get; private set; }
    public Image bonusDonutImage;
    public GameObject[] allDonutPrefabs;
    public Vector2Int[] levelGridSizes; // Массив размеров сетки для уровней
    public int currentLevel = 0;
    private int savedScore;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Сохраняем GameManager между сценами
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Подписываемся на событие загрузки сцены
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Инициализируем уровень, если это первый уровень
        if (currentLevel == 0)
        {
            InitializeLevel();
        }
        else
        {
            sceneFader.FadeTo(currentLevel); // Плавный переход к следующему уровню
        }
    }

    private void InitializeLevel()
    {
        SetBonusDonut();
        CreateLevelGrid();
    }

    private void SetBonusDonut()
    {
        ScoreManager.Instance.bonusDonutTag = allDonutPrefabs[Random.Range(0, allDonutPrefabs.Length)].tag;
        SetBonusDonutSprite();
    }

    private void CreateLevelGrid()
    {
        Vector2Int gridSize;
        gridSize = GetGridSizeForLevel(currentLevel);
        CellManager.Instance.CreateGrid(gridSize.x, gridSize.y, allDonutPrefabs);
    }

    private Vector2Int GetGridSizeForLevel(int level)
    {
        if (level == 0)
        {
            return new Vector2Int(3, 3);
        }
        else if (level == 1)
        {
            return new Vector2Int(4, 4);
        }
        else if (level == 2)
        {
            return new Vector2Int(5, 5);
        }
        else if (level == 3)
        {
            return new Vector2Int(6, 5);
        }
        else
        {
            return new Vector2Int(7, 5);
        }
    }

    private void SetBonusDonutSprite()
    {
        GameObject[] donutsOnScene = GameObject.FindGameObjectsWithTag(ScoreManager.Instance.bonusDonutTag);
        if (donutsOnScene.Length > 0 && bonusDonutImage != null)
        {
            bonusDonutImage.sprite = donutsOnScene[0].GetComponent<SpriteRenderer>().sprite;
        }
    }

    public void NextLevel()
    {
        currentLevel++;
        // Сохраняем прогресс перед переходом на следующий уровень
        sceneFader.FadeTo(currentLevel);
    }

    public void RestartLevel()
    {
        RestartLevelImmediately();
    }

    void RestartLevelImmediately()
    {
        savedScore = ScoreManager.Instance.GetScore();
        CellManager.Instance.ClearGrid();
        InitializeLevel();
        ScoreManager.Instance.SetScore(savedScore);
    }


    public void RestartGame()
    {
        currentLevel = 0;
        ScoreManager.Instance.ResetScore();
        CellManager.Instance.ClearGrid();
        SaveProgress();
        sceneFader.FadeTo(0);
    }

    private void SaveProgress()
    {
        YandexGame.savesData.currentLevel = currentLevel;
        YandexGame.SaveProgress();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Сцена загружена: " + scene.name);
        // Инициализируем уровень только если он не тот же, что и текущий
        if (scene.buildIndex == currentLevel && scene.isLoaded)
        {
            InitializeLevel(); // Инициализируем только если загружается новый уровень
        }
    }

    private void OnDestroy()
    {
        // Отписываемся от события при уничтожении GameManager
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}