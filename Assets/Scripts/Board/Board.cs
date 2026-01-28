using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    [SerializeField] private Vector2Int boardRoot;
    [SerializeField] private Transform tileParent;

    [SerializeField] private GameObject tilePrefab;
    [HideInInspector] public GridSystem<Tile> gridSystem;
    [SerializeField] private Transform yTileSpawnPosition;

    [SerializeField] private float timeToCompleteGravity = .5f;

    public Queue<Tile> destroyeddTilesPool;

    public event EventHandler OnBoardInitialized;

    public event EventHandler<OnTilesDestroyedEventArgs> OnTilesDestroyed;
    public class OnTilesDestroyedEventArgs : EventArgs {
        public int numberOfTilesDestroyed;
    }


    private void Awake() {
        destroyeddTilesPool = new Queue<Tile>();
        CreateBoard();
    }

    private void CreateBoard() {
        Vector2 originPosition = new Vector2(transform.position.x + boardRoot.x, transform.position.y + boardRoot.y);
        gridSystem = new GridSystem<Tile>(width, height, cellSize, originPosition);
        StartCoroutine(InitializeBoard());
    }

    private IEnumerator InitializeBoard() {
        for(int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++) {
                GameObject tileGO = Instantiate(tilePrefab, gridSystem.GetWorldPosition(i, height - 1), Quaternion.identity, tileParent);
                Tile tile = tileGO.GetComponent<Tile>();
                gridSystem.SetGridObject(i, j, tile);
                tile.color = (TilesColor)UnityEngine.Random.Range(0, Enum.GetValues(typeof(TilesColor)).Length);
                tile.gridPosition.x = i;
                tile.gridPosition.y = j;
            }
        }

        yield return new WaitForEndOfFrame();

        OnBoardInitialized?.Invoke(this, EventArgs.Empty);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;

        Vector3 centerPos = new Vector3(transform.position.x + boardRoot.x + width * .5f * cellSize, transform.position.y + boardRoot.y + height * .5f * cellSize, 0f);
        Gizmos.DrawWireCube(centerPos, new Vector3(width * cellSize, height * cellSize, 0f));
    }

    public void ApplyGravity() {
        int completingCount = 0;
        for(int i = 0; i < width; i++) {
            int emptyY = -1;
            int stopPos = -1;

            // checking how many are empty and moving tiles down
            for(int j = 0; j < height; j++) {
                gridSystem.TryGetGridObject(i, j, out Tile tile);

                if(tile == null && emptyY == -1) {
                    emptyY = j;
                } else if(tile != null && emptyY != -1) {
                    // applying gravity to this object
                    Vector3 pos = gridSystem.GetWorldPosition(i, emptyY);

                    tile.transform.DOMove(pos, timeToCompleteGravity).SetEase(Ease.InQuad).SetUpdate(true);
                    gridSystem.ChangePosition(new Vector2Int(i, j), new Vector2Int(i, emptyY));
                    emptyY++;
                }
            }

            stopPos = emptyY;

            if(emptyY != -1) {
                while(emptyY != height) {
                    Tile t = destroyeddTilesPool.Dequeue();
                    Vector3 finalPos = gridSystem.GetWorldPosition(i, emptyY);

                    t.transform.position = new Vector3(finalPos.x, yTileSpawnPosition.position.y + cellSize * (emptyY - stopPos));
                    t.transform.localScale = Vector3.one;

                    completingCount++;
                    Tween tween = t.transform.DOMove(finalPos, timeToCompleteGravity).SetEase(Ease.InQuad).SetUpdate(true);

                    tween.OnComplete(() => {
                        completingCount--;
                        if(completingCount == 0)
                            OnGravityComplete();
                    });

                    t.color = (TilesColor)UnityEngine.Random.Range(0, Enum.GetValues(typeof(TilesColor)).Length);
                    t.GetComponent<TileVisual>().UpdateTileVisual(t);
                    gridSystem.SetGridObject(i, emptyY, t);
                    emptyY++;
                }
            }
        }
    }

    /*private void SpawnTiles() {
        int completingCount = 0;
        for(int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++) {
                gridSystem.TryGetGridObject(i, j, out Tile tile);
                if(tile != null)
                    continue;

                Tile t = destroyeddTilesPool.Dequeue();
                Vector3 finalPos = gridSystem.GetWorldPosition(i, j);

                t.transform.position = new Vector3(finalPos.x, yTileSpawnPosition.position.y);
                t.transform.localScale = Vector3.one;

                completingCount++;
                t.transform.DOMove(finalPos, timeToCompleteGravity).SetEase(Ease.InQuad).SetUpdate(true)
                    .OnComplete(() => {
                        completingCount--;
                        if(completingCount == 0)
                            OnGravityComplete();
                    });
                t.color = (TilesColor)UnityEngine.Random.Range(0, Enum.GetValues(typeof(TilesColor)).Length);
                t.GetComponent<TileVisual>().UpdateTileVisual(t);
                gridSystem.SetGridObject(i, j, t);
            }
        }
    }*/

    private void OnGravityComplete() {
        GameInputs.Instance.enabled = true;
        Time.timeScale = 1f;
    }

    public void InvokeOnTilesDestroyedEvent(int numberOfTiles) {
        OnTilesDestroyed?.Invoke(this, new OnTilesDestroyedEventArgs { numberOfTilesDestroyed = numberOfTiles });
    }

}