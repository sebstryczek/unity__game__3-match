using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject prefabTile;
    [SerializeField] private GameObject prefabItem;
    [SerializeField] private List<TileType> tileTypes;

    [SerializeField] private int rowsCount = 8;
    [SerializeField] private int columnsCount = 6;

    Transform[,] board;
    Tile[,] tiles = null;
    Tile selected = null;

    private void Awake()
    {
        Vector2[] arr = new Vector2[2];
        arr[0] = new Vector2(0, 0);
        arr[1] = new Vector2(0, 0);
        Debug.Log(arr.Distinct().ToArray().Length);
        return;
        this.tiles = new Tile[rowsCount, columnsCount];

        float verticalOffset = (this.rowsCount - 1.0f) / 2.0f;
        float horizontalOffset = (this.columnsCount - 1.0f) / 2.0f;

        for (int rowIndex = 0; rowIndex < this.rowsCount; rowIndex++)
        {
            for (int colIndex = 0; colIndex < this.columnsCount; colIndex++)
            {
                Vector3 pos = new Vector3(colIndex - horizontalOffset, rowIndex - verticalOffset, 0);

                GameObject tileGo = Instantiate(this.prefabTile, pos, Quaternion.identity, this.transform);

                GameObject itemGo = Instantiate(this.prefabItem, pos, Quaternion.identity, this.transform);
                itemGo.name = rowIndex + " " + colIndex;

                List<TileType> excludeTypes = new List<TileType>();
                if (rowIndex > 0) excludeTypes.Add(this.tiles[rowIndex - 1, colIndex].type);
                if (colIndex > 0) excludeTypes.Add(this.tiles[rowIndex, colIndex - 1].type);

                TileType type = this.GetRandomType(excludeTypes);

                Tile tile = itemGo.GetComponent<Tile>();
                tile.SetType(type);

                this.tiles[rowIndex, colIndex] = tile;
            }
        }
    }

    private TileType GetRandomType(List<TileType> excludeTypes)
    {
        List<TileType> range = this.tileTypes.FindAll( x => !excludeTypes.Contains(x));
        int rand = Random.Range(0, range.Count);

        return range[rand];
    }

    private bool isDragging = false;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            this.isDragging = true;
            this.OnMouseDown();
        }

        if (this.isDragging)
        {
            this.OnMouseDrag();
        }

        if (Input.GetMouseButtonUp(0))
        {
            this.isDragging = false;
            this.OnMouseUp();
        }
    }

    private void OnMouseDown()
    {
        Tile hittedTile = this.FindTileAtMousePosition();
        if (hittedTile == null) return;

        if (this.selected == null)
        {
            //first object
            this.selected = hittedTile;
            this.selected.Select();
        }
        else
        {
            this.selected.Deselect();
            //second object
            if (this.AreNeighbors(this.selected, hittedTile))
            {
                // replace
                this.Replace(this.selected, hittedTile);
                this.Collect();
                this.selected = null;
            }
            else
            {
                //change object
                this.selected = hittedTile;
                this.selected.Select();
            }
        }
    }

    private void OnMouseDrag()
    {
        if (this.selected)
        {
            Tile hittedTile = this.FindTileAtMousePosition();
            if (hittedTile == null || hittedTile == this.selected) return;

            Vector2 startPos = this.selected.transform.position;
            Vector2 endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float angle = UtilsVector2.GetAngleBetween(Vector2.up, (endPos - startPos).normalized);

            Tile elo = this.FindNeighborTitle(this.selected, angle);

            this.selected.Deselect();

            if (elo)
            {
                this.Replace(this.selected, elo);//swap
                this.Collect();
            }
            this.selected = null;
        }
    }

    private void OnMouseUp()
    {
    }

    private Tile FindTileAtMousePosition()
    {
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);
        if (hit.collider)
        {
            return hit.collider.gameObject.GetComponent<Tile>();
        }
        return null;
    }

    private Tile FindNeighborTitle(Tile root, float angle)
    {
        UtilsArray2D.Element elem = UtilsArray2D.FindElement<Tile>(this.tiles, this.selected);

        if (angle > 320 || angle > 0 && angle < 40)
        {
            return this.tiles[elem.Row + 1, elem.Column];
        }

        if (angle > 50 && angle < 130)
        {
            return this.tiles[elem.Row, elem.Column + 1];
        }

        if (angle > 140 && angle < 220)
        {
            return this.tiles[elem.Row - 1, elem.Column];
        }

        if (angle > 230 && angle < 310)
        {
            return this.tiles[elem.Row, elem.Column - 1];
        }

        return null;
    }

    private bool AreNeighbors(Tile first, Tile second)
    {
        UtilsArray2D.Element firstElem = UtilsArray2D.FindElement<Tile>(this.tiles, first);
        UtilsArray2D.Element secondElem = UtilsArray2D.FindElement<Tile>(this.tiles, second);

        if (firstElem.Row == secondElem.Row)
        {
            return firstElem.Column == secondElem.Column - 1 || firstElem.Column == secondElem.Column + 1;
        }

        if (firstElem.Column == secondElem.Column)
        {
            return firstElem.Row == secondElem.Row - 1 || firstElem.Row == secondElem.Row + 1;
        }

        return false;
    }

    private void Replace(Tile first, Tile second)
    {
        UtilsArray2D.Element firstElem = UtilsArray2D.FindElement<Tile>(this.tiles, first);
        UtilsArray2D.Element secondElem = UtilsArray2D.FindElement<Tile>(this.tiles, second);
        this.tiles[firstElem.Row, firstElem.Column] = second;
        this.tiles[secondElem.Row, secondElem.Column] = first;

        Vector2 firstPos = first.transform.position;
        first.transform.position = second.transform.position;
        second.transform.position = firstPos;
    }

    private void Collect()
    {
        int rowCount = this.tiles.GetLength(0);
        int colCount = this.tiles.GetLength(1);

        List<Tile> matches = new List<Tile>(); 
        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            for (int colIndex = 0; colIndex < colCount; colIndex++)
            {
                Tile t = this.tiles[rowIndex, colIndex];
                TileType type = t.type;
                UtilsArray2D.Element e = UtilsArray2D.FindElement<Tile>(this.tiles, t);

                if (t.type == null) continue;

                if (colIndex > 0 && colIndex < colCount - 1)
                {
                    Tile prev = this.tiles[e.Row, e.Column - 1];
                    Tile next = this.tiles[e.Row, e.Column + 1];

                    if (prev.type == t.type && t.type == next.type)
                    {
                        if (!matches.Contains(prev)) matches.Add(prev);
                        if (!matches.Contains(t)) matches.Add(t);
                        if (!matches.Contains(next)) matches.Add(next);
                    }
                }

                if (rowIndex > 0 && rowIndex < rowCount - 1)
                {
                    Tile prev = this.tiles[e.Row - 1, e.Column];
                    Tile next = this.tiles[e.Row + 1, e.Column];

                    if (prev.type == t.type && t.type == next.type)
                    {
                        if (!matches.Contains(prev)) matches.Add(prev);
                        if (!matches.Contains(t)) matches.Add(t);
                        if (!matches.Contains(next)) matches.Add(next);
                    }
                }
            }
        }

        matches.ForEach(x => x.Clear());
        if (matches.Count > 0)
        {
            Renumber();
        }
        else
        {
            this.Recreate();
        }
    }

    private void Renumber()
    {
        int rowCount = this.tiles.GetLength(0);
        int colCount = this.tiles.GetLength(1);
        for (int colIndex = 0; colIndex < colCount; colIndex++)
        {
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                Tile t = this.tiles[rowIndex, colIndex];
                if (t.type == null && rowIndex < rowCount - 1)
                {
                    int nextIndex = rowIndex + 1;
                    while(nextIndex < rowCount && this.tiles[nextIndex, colIndex].type == null)
                    {
                        nextIndex++;
                    }
                    if (nextIndex < rowCount) this.Replace(this.tiles[rowIndex, colIndex], this.tiles[nextIndex, colIndex]);

                    //this.Replace(this.tiles[rowIndex, colIndex], this.tiles[nextIndex, colIndex]);
                    /*
                    if (rowIndex < rowCount - 1)
                    {
                        int nextIndex = rowIndex;
                        while(this.tiles[nextIndex, colIndex].type == null && nextIndex < rowCount)
                        {
                            nextIndex++;
                        }

                        if (nextIndex != rowIndex)
                        {
                            this.Replace(this.tiles[rowIndex, colIndex], this.tiles[nextIndex, colIndex]);
                            //rowIndex = nextIndex;
                        }
                    }
                    */
                }
            }
        }

        this.Collect();
    }

    private void Recreate()
    {
        int rowCount = this.tiles.GetLength(0);
        int colCount = this.tiles.GetLength(1);
        for (int colIndex = 0; colIndex < colCount; colIndex++)
        {
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                Tile t = this.tiles[rowIndex, colIndex];
                if (t.type == null)
                {

                    List<TileType> excludeTypes = new List<TileType>();
                    if (rowIndex > 0) excludeTypes.Add(this.tiles[rowIndex - 1, colIndex].type);
                    if (colIndex > 0) excludeTypes.Add(this.tiles[rowIndex, colIndex - 1].type);
                    if (rowIndex < rowCount - 1) excludeTypes.Add(this.tiles[rowIndex + 1, colIndex].type);
                    if (colIndex < colCount - 1) excludeTypes.Add(this.tiles[rowIndex, colIndex + 1].type);
                    excludeTypes = excludeTypes.Distinct().ToList();
                    excludeTypes.Remove(null);

                    TileType type = this.GetRandomType(excludeTypes);
                    t.SetType(type);
                }
            }
        }
    }
}
