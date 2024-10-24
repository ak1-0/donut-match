using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance { get; private set; }
    public Image img;
    public AnimationCurve curve;

    void Start()
    {
        if (img == null)
        {
            img = GameObject.Find("FadeImage").GetComponent<Image>();
        }
        StartCoroutine(FadeIn());
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnLevelWasLoaded(int level)
    {
        if (img == null)
        {
            img = GameObject.Find("FadeImage").GetComponent<Image>();
        }
    }

    public void FadeTo(int sceneIndex)
    {
        Debug.Log("Загрузка сцены с индексом: " + sceneIndex);  // Отладка
        StartCoroutine(FadeOut(sceneIndex));
    }

    IEnumerator FadeIn()
    {
        if (img == null)
        {
            yield break;
        }

        img.enabled = true;

        float t = 1f;

        while (t > 0f)
        {
            t -= Time.deltaTime;
            float a = curve.Evaluate(t);
            if (img != null)
            {
                img.color = new Color(0f, 0f, 0f, a);
            }
            yield return null;
        }

        if (img != null)
        {
            img.enabled = false;
        }
    }

    IEnumerator FadeOut(int sceneIndex)
    {
        if (img == null)
        {
            yield break;
        }

        img.enabled = true;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;
            float a = curve.Evaluate(t);
            if (img != null)
            {
                img.color = new Color(0f, 0f, 0f, a);
            }
            yield return null;
        }

        // Проверьте, что загружается правильная сцена
        Debug.Log("Попытка загрузки сцены с индексом: " + sceneIndex);

        // Если текущая сцена совпадает с загружаемой, просто вызовите FadeIn и вернитесь
        if (sceneIndex == SceneManager.GetActiveScene().buildIndex)
        {
            StartCoroutine(FadeIn());
            yield break;
        }

        SceneManager.LoadScene(sceneIndex);

        // После загрузки новой сцены нужно найти новый объект FadeImage и продолжить затемнение
        if (img == null)
        {
            img = GameObject.Find("FadeImage").GetComponent<Image>();
        }
        StartCoroutine(FadeIn());
    }
}