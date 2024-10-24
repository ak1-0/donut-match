using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellManager : MonoBehaviour
{
    public Transform spawnPoint;
    public float cellSize = 1f;

    public static CellManager Instance { get; private set; }

    private GameObject[,] grid;
    private GameObject[] donutPrefabs;

    private void Awake()
    {
        Instance = this;
    }

    public void CreateGrid(int rows, int cols, GameObject[] donutPrefabs)
    {
        this.donutPrefabs = donutPrefabs; // Сохраняем массив префабов

        // Очищаем старую сетку, если она существует
        if (grid != null)
        {
            ClearGrid();
        }

        grid = new GameObject[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Vector3 spawnPosition = spawnPoint.position + new Vector3(j * cellSize, -i * cellSize, 0f);
                GameObject donut = Instantiate(donutPrefabs[Random.Range(0, donutPrefabs.Length)], spawnPosition, Quaternion.identity, transform);
                grid[i, j] = donut;

                DonutController donutController = donut.GetComponent<DonutController>();
                if (donutController != null)
                {
                    donutController.SetGridPosition(i, j);
                }
            }
        }
    }

    public void ClearGrid()
    {
        if (grid != null)
        {
            foreach (GameObject donut in grid)
            {
                if (donut != null)
                {
                    Destroy(donut);
                }
            }
            grid = new GameObject[grid.GetLength(0), grid.GetLength(1)]; // Очищаем сетку, не присваивая null
        }
    }

    public void DestroyDonuts(GameObject[] donuts)
    {
        foreach (GameObject donut in donuts)
        {
            if (donut != null)
            {
                Vector2Int gridPos = new Vector2Int(donut.GetComponent<DonutController>().GridX, donut.GetComponent<DonutController>().GridY);
                grid[gridPos.x, gridPos.y] = null;
                Destroy(donut);
            }
        }

        StartCoroutine(FillEmptyCellsCoroutine());
    }

    private IEnumerator FillEmptyCellsCoroutine()
    {
        yield return new WaitForSeconds(0.2f); // Небольшая задержка перед началом заполнения пустых ячеек

        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        // Перемещаем существующие пончики вниз
        for (int j = 0; j < cols; j++)
        {
            int emptyCount = 0;

            for (int i = rows - 1; i >= 0; i--)
            {
                if (grid[i, j] == null)
                {
                    emptyCount++;
                }
                else if (emptyCount > 0)
                {
                    grid[i + emptyCount, j] = grid[i, j];
                    grid[i, j] = null;
                    grid[i + emptyCount, j].GetComponent<DonutController>().SetGridPosition(i + emptyCount, j);
                    StartCoroutine(MoveDonutToPosition(grid[i + emptyCount, j], spawnPoint.position + new Vector3(j * cellSize, -(i + emptyCount) * cellSize, 0f)));
                }
            }
        }

        // Создаём новые пончики для оставшихся пустых ячеек
        for (int i = rows - 1; i >= 0; i--)
        {
            List<IEnumerator> coroutines = new List<IEnumerator>();

            for (int j = 0; j < cols; j++)
            {
                if (grid[i, j] == null)
                {
                    GameObject newDonut = Instantiate(donutPrefabs[Random.Range(0, donutPrefabs.Length)], spawnPoint.position + new Vector3(j * cellSize, cellSize * rows, 0f), Quaternion.identity, transform);
                    grid[i, j] = newDonut;
                    grid[i, j].GetComponent<DonutController>().SetGridPosition(i, j);
                    coroutines.Add(MoveDonutToPosition(newDonut, spawnPoint.position + new Vector3(j * cellSize, -i * cellSize, 0f)));
                }
            }

            // Запуск корутин одновременно и ожидание их завершения
            foreach (var coroutine in coroutines)
            {
                StartCoroutine(coroutine);
            }

            // Ожидание завершения всех корутин
            foreach (var coroutine in coroutines)
            {
                yield return coroutine;
            }
        }
    }

    private IEnumerator MoveDonutToPosition(GameObject donut, Vector3 targetPosition)
    {
        if (donut == null) yield break; // Если donut равен null, прекратить выполнение корутины

        float elapsedTime = 0f;
        float duration = 0.5f; // Продолжительность анимации перемещения
        Vector3 startingPosition = donut.transform.position;

        while (elapsedTime < duration)
        {
            if (donut == null) yield break; // Если donut равен null, прекратить выполнение корутины

            donut.transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (donut != null) // Если donut не равен null, установить его позицию
        {
            donut.transform.position = targetPosition;
        }
    }
}