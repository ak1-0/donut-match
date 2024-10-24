using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutController : MonoBehaviour
{
    //public GameObject effectPrefab;

    private int gridX;
    private int gridY;

    public int GridX { get { return gridX; } }
    public int GridY { get { return gridY; } }

    private static List<DonutController> selectedDonuts = new List<DonutController>();

    private bool isSelected = false;

    public void SetGridPosition(int x, int y)
    {
        gridX = x;
        gridY = y;
    }

    private void OnMouseDown()
    {
        // Начинаем процесс соединения
        selectedDonuts.Clear();
        AddToSelection();
    }

    private void OnMouseEnter()
    {
        // Если зажата кнопка мыши, продолжаем соединение
        if (Input.GetMouseButton(0))
        {
            AddToSelection();
        }
    }

    private void OnMouseUp()
    {
        // Проверка на совпадение цветов и удаление пончиков
        if (selectedDonuts.Count >= 2)
        {
            List<GameObject> matchingDonuts = new List<GameObject>();
            foreach (DonutController donut in selectedDonuts)
            {
                matchingDonuts.Add(donut.gameObject);

            }

            CellManager.Instance.DestroyDonuts(matchingDonuts.ToArray());

            int scoreMultiplier = gameObject.tag.Equals(ScoreManager.Instance.bonusDonutTag) ? 50 : 10;

            ScoreManager.Instance.AddScore(selectedDonuts.Count * scoreMultiplier);
        }

        // Очистка выбранных пончиков после выполнения действий
        DeselectAll();
    }

    private void AddToSelection()
    {
        if (!selectedDonuts.Contains(this))
        {
            if (selectedDonuts.Count == 0 || IsAdjacentToLastSelected())
            {
                selectedDonuts.Add(this);
                Select();
            }
        }
    }

    private bool IsAdjacentToLastSelected()
    {
        if (selectedDonuts.Count == 0) return false;

        DonutController lastSelected = selectedDonuts[selectedDonuts.Count - 1];
        if (lastSelected.tag != this.tag) return false;

        int deltaX = Mathf.Abs(lastSelected.GridX - this.GridX);
        int deltaY = Mathf.Abs(lastSelected.GridY - this.GridY);

        // Проверка на соседство по вертикали, горизонтали и диагонали
        bool isAdjacent = (deltaX <= 1 && deltaY <= 1);

        return isAdjacent;
    }

    private void Select()
    {
        isSelected = true;
        // Визуальная индикация выделения, например, изменение цвета
        GetComponent<SpriteRenderer>().color = Color.gray;
    }

    private void Deselect()
    {
        isSelected = false;
        // Сброс визуальной индикации
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    private static void DeselectAll()
    {
        foreach (DonutController donut in selectedDonuts)
        {
            donut.Deselect();
        }
        selectedDonuts.Clear();
    }

    private void OnDestroy()
    {
        selectedDonuts.Remove(this);
    }
}