using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoardLogic : MonoBehaviour {

    private Camera mainCamera;

    public List<Vector2Int> selectedTiles;

    private bool isPointerDownForSelection = false;
    private Vector2Int lastCell;
    private Tile lastCellTile;

    private bool isMobile;

    [SerializeField] private LineRenderer lineRenderer;

    Board board;

    #region Unity Methods
    private void Awake() {
        mainCamera = Camera.main;
    }

    private void Start() {
        isMobile = Application.isMobilePlatform;
        if(isMobile)
            GameInputs.Instance.OnTouch += OnSomethingDown;
        else
            GameInputs.Instance.OnMouseLeftClick += OnSomethingDown;

        board = GameManager.Instance.board;
        lineRenderer.positionCount = 0;
    }


    private void Update() {
        if(!isPointerDownForSelection)
            return;

        if(Time.timeScale == 0)
            return;

        Vector2 position;
        if(!isMobile) {
            position = Mouse.current.position.ReadValue();
        } else {
            position = Touchscreen.current.primaryTouch.position.ReadValue();
        }

        position = mainCamera.ScreenToWorldPoint(position);

        lineRenderer.SetPosition(selectedTiles.Count, position);
        TrySendDrag(position);
    }

    #endregion


    #region Input Methods

    private void OnSomethingDown(object sender, GameInputs.OnInputTrackEventArgs e) {
        if(e.ctx.performed) {
            if(Time.timeScale == 0)
                return;

            lastCell = new Vector2Int(-1, -1);
            lastCellTile = null;

            Vector2 position;
            if(!isMobile) {
                position = Mouse.current.position.ReadValue();
            } else {
                position = Touchscreen.current.primaryTouch.position.ReadValue();
            }

            position = mainCamera.ScreenToWorldPoint(position);
            if(!board.gridSystem.TryGetXY(position, out int x, out int y)) {
                return;
            }

            if(!GameManager.Instance.isGameStarted)
                GameManager.Instance.isGameStarted = true;

            lineRenderer.positionCount = 1;
            isPointerDownForSelection = true;
            TryAddTile(x, y);
        }
        if(e.ctx.canceled) {
            if(!isPointerDownForSelection)
                return;

            lineRenderer.positionCount = 0;
            isPointerDownForSelection = false;
            lastCell = new Vector2Int(-1, -1);
            lastCellTile = null;
            if(selectedTiles.Count <= 2) {
                ClearSelectedTiles();
                return;
            }

            GameInputs.Instance.enabled = false;

            DestroySelectedTiles();
        }
    }

    #endregion


    #region Drag Methods 

    public void TrySendDrag(Vector2 mousePos) {
        if(!board.gridSystem.TryGetXY(mousePos, out int x, out int y)) {
            return;
        }
        Vector2Int cell = new Vector2Int(x, y);

        if(lastCell == cell)
            return;

        if(!IsAdjecent(lastCell, cell))
            return;


        TryAddTile(x, y);
    }

    public void TryAddTile(int x, int y) {
        board.gridSystem.TryGetGridObject(x, y, out Tile t);
        if(t == null)
            return;
        if(lastCellTile != null && !IsSameColor(lastCellTile, t))
            return;

        if(selectedTiles.Contains(new Vector2Int(x, y))) {
            if(selectedTiles[selectedTiles.Count - 2] == new Vector2Int(x, y)) {
                BackTrack(x, y);
            }
            return;
        }

        t.GetComponent<TileVisual>().SetSelected();
        selectedTiles.Add(new Vector2Int(x, y));
        lastCell = new Vector2Int(x, y);
        lastCellTile = t;

        // sound
        AudioManager.Instance.PlayAudio("pop", selectedTiles.Count);

        // visual
        lineRenderer.SetPosition(selectedTiles.Count - 1, t.transform.position);
        lineRenderer.positionCount++;

    }

    private void BackTrack(int backToX, int backToY) {
        lastCellTile.GetComponent<TileVisual>().UnsetSelected();
        selectedTiles.RemoveAt(selectedTiles.Count - 1);
        lastCell = new Vector2Int(backToX, backToY);
        if(board.gridSystem.TryGetGridObject(backToX, backToY, out Tile t)) {
            lastCellTile = t;
        }
        lineRenderer.positionCount--;
    }

    #endregion


    #region After list is ready methods

    private void ClearSelectedTiles() {
        foreach(Vector2Int cell in selectedTiles) {
            board.gridSystem.TryGetGridObject(cell.x, cell.y, out Tile t);
            t.GetComponent<TileVisual>().UnsetSelected();
        }
        selectedTiles.Clear();
    }

    private void DestroySelectedTiles() {
        Sequence seq = DOTween.Sequence().SetUpdate(true);

        board.InvokeOnTilesDestroyedEvent(selectedTiles.Count);
        Time.timeScale = 0f;

        foreach(Vector2Int cell in selectedTiles) {
            board.gridSystem.TryGetGridObject(cell.x, cell.y, out Tile t);
            t.GetComponent<TileVisual>().UnsetSelected();
            seq.Join(t.transform.DOScale(Vector3.zero, .25f).SetEase(Ease.OutBack).SetUpdate(true));

            board.gridSystem.SetGridObject(cell.x, cell.y, null);
            board.destroyeddTilesPool.Enqueue(t);
        }
        seq.OnComplete(() => {
            board.ApplyGravity();
        });
        selectedTiles.Clear();
    }


    #endregion


    #region Other Custom Methods

    public bool IsAdjecent(Vector2 beforeTile, Vector2 afterTile) {
        if(Mathf.Abs(beforeTile.x - afterTile.x) <= 1 && Mathf.Abs(beforeTile.y - afterTile.y) <= 1)
            return true;
        else
            return false;
    }

    public bool IsSameColor(Tile beforeTile, Tile afterTile) {
        if(beforeTile.color == afterTile.color)
            return true;
        else
            return false;
    }

    #endregion

}