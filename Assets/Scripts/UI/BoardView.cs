using UnityEngine;
using UnityEngine.UI;

public class BoardView : MonoBehaviour
{
    private Button[] _cells = new Button[9];
    private Image[] _cellImages = new Image[9];
    private RectTransform _boardContainer;
    private bool _boardCreated;

    public bool IsCreated => _boardCreated;
    public Button[] Cells => _cells;

    public void CreateBoard(GameObject gamePanel)
    {
        if (_boardCreated) return;
        if (gamePanel == null) return;

        _boardContainer = EnsureBoardContainer(gamePanel);
        ConfigureGrid();

        for (int i = 0; i < 9; i++)
            CreateCell(i);

        _boardCreated = true;
    }

    public void ClearBoard()
    {
        for (int i = 0; i < 9; i++)
        {
            if (_cellImages[i] != null)
            {
                _cellImages[i].sprite = null;
                _cellImages[i].color = Color.clear;
            }
            if (_cells[i] != null)
                _cells[i].interactable = true;
        }
    }

    public void PlaceMark(int index, bool isPlayer1, int themeIndex)
    {
        var theme = ThemeManager.Instance;
        if (theme == null) return;

        Sprite markSprite = isPlayer1 ? theme.GetXSprite(themeIndex) : theme.GetOSprite(themeIndex);

        if (markSprite == null)
            Debug.LogError($"[BoardView] Sprite is null! isP1: {isPlayer1}, theme: {themeIndex}. Assign sprites in Inspector.");

        _cellImages[index].sprite = markSprite;
        _cellImages[index].color = markSprite != null ? Color.white : Color.red;
        _cells[index].interactable = false;
    }

    public void RegisterCellClicks(System.Action<int> onCellClick)
    {
        for (int i = 0; i < 9; i++)
        {
            if (_cells[i] == null) continue;
            int idx = i;
            _cells[i].onClick.RemoveAllListeners();
            _cells[i].onClick.AddListener(() => onCellClick(idx));
        }
    }

    private RectTransform EnsureBoardContainer(GameObject gamePanel)
    {
        if (_boardContainer != null) return _boardContainer;

        var go = new GameObject("BoardContainer");
        go.transform.SetParent(gamePanel.transform, false);
        _boardContainer = go.AddComponent<RectTransform>();

        _boardContainer.anchorMin = new Vector2(0.5f, 0.55f);
        _boardContainer.anchorMax = new Vector2(0.5f, 0.55f);
        _boardContainer.pivot = new Vector2(0.5f, 0.5f);
        _boardContainer.anchoredPosition = Vector2.zero;
        _boardContainer.sizeDelta = new Vector2(330, 330);

        return _boardContainer;
    }

    private void ConfigureGrid()
    {
        var grid = _boardContainer.GetComponent<GridLayoutGroup>();
        if (grid == null) grid = _boardContainer.gameObject.AddComponent<GridLayoutGroup>();

        grid.cellSize = new Vector2(100, 100);
        grid.spacing = new Vector2(10, 10);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 3;
        grid.childAlignment = TextAnchor.MiddleCenter;
    }

    private void CreateCell(int i)
    {
        var cellGO = new GameObject($"Cell{i}", typeof(RectTransform));
        cellGO.transform.SetParent(_boardContainer);
        cellGO.transform.localScale = Vector3.one;

        var rect = cellGO.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var bgImage = cellGO.AddComponent<Image>();
        bgImage.color = new Color(1, 1, 1, 0.2f);

        var btn = cellGO.AddComponent<Button>();
        btn.targetGraphic = bgImage;
        ColorBlock cb = btn.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = new Color(1, 1, 1, 1f);
        cb.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        cb.disabledColor = Color.white;
        btn.colors = cb;
        btn.transition = Selectable.Transition.ColorTint;

        var markGO = new GameObject("CellMark", typeof(RectTransform));
        markGO.transform.SetParent(cellGO.transform);
        markGO.transform.localScale = Vector3.one;

        var markRect = markGO.GetComponent<RectTransform>();
        markRect.anchorMin = new Vector2(0.15f, 0.15f);
        markRect.anchorMax = new Vector2(0.85f, 0.85f);
        markRect.offsetMin = Vector2.zero;
        markRect.offsetMax = Vector2.zero;

        var markImage = markGO.AddComponent<Image>();
        markImage.color = Color.clear;
        markImage.preserveAspect = true;
        markImage.raycastTarget = false;

        _cells[i] = btn;
        _cellImages[i] = markImage;
    }
}
