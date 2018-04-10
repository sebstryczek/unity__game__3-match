using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject prefabEmpty;
    [SerializeField] private GameObject prefabTile;
    [SerializeField] private TileType[] tileTypes;
    [SerializeField] private int rowsCount = 8;
    [SerializeField] private int columnsCount = 5;

    [SerializeField] private Text uiTextScore;
    private int score;

    private Board board = null;
    private bool isDragging = false;
    private Tile selected = null;

    private void Awake()
    {
        this.board = new Board(this.rowsCount, this.columnsCount);
        
        float verticalOffset = (this.rowsCount - 1.0f) / 2.0f;
        float horizontalOffset = (this.columnsCount - 1.0f) / 2.0f;

        GameObject bg = new GameObject("Background");

        for (int rowIndex = 0; rowIndex < this.rowsCount; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < this.columnsCount; columnIndex++)
            {
                Vector3 pos = new Vector3(columnIndex - horizontalOffset, rowIndex - verticalOffset, 0);

                GameObject empty = Instantiate(this.prefabEmpty, pos, Quaternion.identity, bg.transform);

                GameObject tileGo = Instantiate(this.prefabTile, pos, Quaternion.identity, this.transform);
                tileGo.name = rowIndex + " " + columnIndex;

                Tile tile = tileGo.GetComponent<Tile>();
                tile.SetCoords(rowIndex, columnIndex);
                
                this.board.AddTile(tile);
            }
        }

        this.board.FillEmptyTiles();
    }

    public TileType GetRandomType(Tile tile)
    {
        TileType[] exclude = this.board.GetForbiddenTypes(tile.Row, tile.Column);
        TileType[] range = this.tileTypes.Where(x => !exclude.Contains(x)).ToArray();
        int rand = Random.Range(0, range.Length);

        return range[rand];
    }

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
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Tile hittedTile = this.board.GetTileAtPosition(mouseWorldPosition);
        if (hittedTile == null) return;

        if (this.selected == null)
        {
            this.selected = hittedTile;
            this.selected.Select();
        }
        else
        {
            this.selected.Deselect();
            
            if (this.board.AreNeighbors(this.selected, hittedTile))
            {
                this.board.SwapTiles(this.selected, hittedTile);
                this.board.CollectMatches();
                this.selected = null;
            }
            else
            {
                this.selected = hittedTile;
                this.selected.Select();
            }
        }
    }

    private void OnMouseDrag()
    {
        if (this.selected)
        {
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Tile hittedTile = this.board.GetTileAtPosition(mouseWorldPosition);

            if (hittedTile == null || hittedTile == this.selected) return;

            this.selected.Deselect();

            Vector2 startPos = this.selected.transform.position;
            Vector2 endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float angle = UtilsVector2.GetAngleBetween(Vector2.up, (endPos - startPos).normalized);

            Tile neighbor = this.board.FindNeighborTitle(this.selected, angle);

            if (neighbor)
            {
                this.board.SwapTiles(this.selected, neighbor);
                this.board.CollectMatches();
            }

            this.selected = null;
        }
    }

    private void OnMouseUp()
    {
    }

    public void AddPoints(int points)
    {
        this.score += points;
        this.uiTextScore.text = this.score.ToString();
    }
}