using UnityEngine;
using UnityEngine.UIElements;

public class StrikeEffect : MonoBehaviour
{
    private static StrikeEffect _instance;
    public static StrikeEffect Instance => _instance;

    private VisualElement[] _cells;
    private Color _strikeColor = new Color(1f, 0.84f, 0f);

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    public void Initialize(VisualElement[] cells)
    {
        _cells = cells;
    }

    public void PlayStrikeAnimation(int[] winningLine)
    {
        if (winningLine == null || _cells == null)
            return;

        StartCoroutine(AnimateStrikeLine(winningLine));
    }

    private System.Collections.IEnumerator AnimateStrikeLine(int[] line)
    {
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float pulse = Mathf.Sin(t * Mathf.PI * 4) * 0.3f + 0.5f;

            foreach (int i in line)
            {
                if (i >= 0 && i < _cells.Length)
                {
                    var color = _strikeColor;
                    color.a = pulse;
                    _cells[i].style.backgroundColor = new StyleColor(color);
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < 3; i++)
        {
            int cellIndex = line[i];
            if (cellIndex >= 0 && cellIndex < _cells.Length)
            {
                _cells[cellIndex].style.backgroundColor = new StyleColor(new Color(1f, 0.84f, 0f, 0.6f));
            }
        }
    }

    public void ClearStrikeEffect()
    {
        if (_cells == null) return;

        foreach (var cell in _cells)
        {
            cell.style.backgroundColor = new StyleColor(Color.clear);
        }
    }
}