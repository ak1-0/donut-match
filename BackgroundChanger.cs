using UnityEngine;
using UnityEngine.UI;

public class BackgroundChanger : MonoBehaviour
{
    public Camera mainCamera;
    public Button button;
    public Sprite sunSprite;
    public Sprite moonSprite;
    public Color whiteColor;
    public Color grayColor;

    public GameObject timerPrefabWhite; // Ссылка на префаб с белым фоном
    public GameObject timerPrefabGray;  // Ссылка на префаб с серым фоном

    private bool isWhite;

    void Start()
    {
        // Загрузка сохраненного состояния
        isWhite = PlayerPrefs.GetInt("IsWhite", 1) == 1;
        ChangeBackground();
    }

    public void OnButtonClick()
    {
        // Переключение фона и сохранение состояния
        isWhite = !isWhite;
        PlayerPrefs.SetInt("IsWhite", isWhite ? 1 : 0);
        ChangeBackground();
    }

    private void ChangeBackground()
    {
        // Изменение фона и спрайта кнопки
        mainCamera.backgroundColor = isWhite ? whiteColor : grayColor;
        button.image.sprite = isWhite ? moonSprite : sunSprite;

        // Активация нужного префаба таймера
        ActivateTimerPrefab();
    }

    private void ActivateTimerPrefab()
    {
        // Деактивация обоих префабов
        timerPrefabWhite.SetActive(false);
        timerPrefabGray.SetActive(false);

        // Активация нужного префаба
        if (isWhite)
        {
            timerPrefabWhite.SetActive(true);
        }
        else
        {
            timerPrefabGray.SetActive(true);
        }
    }
}