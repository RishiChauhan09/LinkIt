using UnityEngine;

public class Tile : MonoBehaviour{

    public TilesColor color;
    public Vector2Int gridPosition;

}

public enum TilesColor {
    Red,
    Blue,
    Yellow,
    Green,
    Purple
}