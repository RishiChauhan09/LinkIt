using DG.Tweening;
using UnityEngine;

public class TileVisual : MonoBehaviour {

    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Start() {
        GameManager.Instance.board.OnBoardInitialized += OnBoardInitialized;
    }

    private void OnBoardInitialized(object sender, System.EventArgs e) {
        Tile tile = GetComponent<Tile>();
        float cellSize = GameManager.Instance.board.gridSystem.GetCellSize();

        transform.DOMove(GameManager.Instance.board.gridSystem.GetWorldPosition(tile.gridPosition.x, tile.gridPosition.y), 1f);

        UpdateTileVisual(tile);
    }

    public void UpdateTileVisual(Tile tile) {
        spriteRenderer.sprite = GameManager.Instance.allAssets.normalColors[(int)tile.color];
    }

    public void SetSelected() {
        spriteRenderer.sprite = GameManager.Instance.allAssets.highlightedColors[(int)GetComponent<Tile>().color];
    }

    public void UnsetSelected() {
        spriteRenderer.sprite = GameManager.Instance.allAssets.normalColors[(int)GetComponent<Tile>().color];
    }
}