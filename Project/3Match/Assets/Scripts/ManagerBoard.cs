using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ManagerBoard : Singleton<ManagerBoard>
{
    [SerializeField] private GameObject prefabEmpty;
    [SerializeField] private GameObject prefabTile;
    [SerializeField] private TileType[] tileTypes;
    [SerializeField] private int rowsCount = 8;
    [SerializeField] private int columnsCount = 5;

    private Board board = null;
    private BoardTile selectedTile = null;
    
    public TileType GetRandomType(TileType[] forbiddenTypes = null)
    {
        forbiddenTypes = forbiddenTypes ?? new TileType[0];
        TileType[] range = this.tileTypes.Where(x => !forbiddenTypes.Contains(x)).ToArray();
        int rand = Random.Range(0, range.Length);

        return range[rand];
    }

    private void Awake()
    {
        this.board = new Board(this.rowsCount, this.columnsCount);
        
        float verticalOffset = (this.rowsCount - 1.0f) / 2.0f;
        float horizontalOffset = (this.columnsCount - 1.0f) / 2.0f;

        GameObject boardBackground = new GameObject("Background");

        for (int rowIndex = 0; rowIndex < this.rowsCount; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < this.columnsCount; columnIndex++)
            {
                Vector3 pos = new Vector3(columnIndex - horizontalOffset, rowIndex - verticalOffset, 0);

                GameObject empty = Instantiate(this.prefabEmpty, pos, Quaternion.identity, boardBackground.transform);

                GameObject tileGo = Instantiate(this.prefabTile, pos, Quaternion.identity);
                BoardTile tile = tileGo.GetComponent<BoardTile>();
                tile.SetCoords(rowIndex, columnIndex);
                
                this.board.AddTile(tile);
            }
        }

        this.board.FillEmptyTiles();
    }

    public void OnMouseDown()
    {
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        BoardTile hittedTile = this.GetTileAtPosition(mouseWorldPosition);

        if (hittedTile == null) return;

        if (this.selectedTile == null)
        {
            this.selectedTile = hittedTile;
            this.selectedTile.Select();
        }
        else
        {
            this.selectedTile.Deselect();
            
            if (this.selectedTile.IsNeighbor(hittedTile))
            {
                this.board.SwapTiles(this.selectedTile, hittedTile);

                BoardTile[] matches = this.board.GetMatches();
                if (matches.Length > 0)
                {
                    this.board.CollectMatches(matches);
                }
                else
                {
                    this.board.SwapTiles(this.selectedTile, hittedTile);
                }

                this.selectedTile = null;
            }
            else
            {
                this.selectedTile = hittedTile;
                this.selectedTile.Select();
            }
        }
    }

    public void OnMouseDrag()
    {
        if (this.selectedTile)
        {
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            BoardTile hittedTile = this.GetTileAtPosition(mouseWorldPosition);

            if (hittedTile == null || hittedTile == this.selectedTile) return;

            this.selectedTile.Deselect();

            Vector2 startPos = this.selectedTile.transform.position;
            Vector2 endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float angle = UtilsVector2.GetAngleBetween(Vector2.up, (endPos - startPos).normalized);

            BoardTile neighborTile = this.board.FindNeighborTitle(this.selectedTile, angle);

            if (neighborTile != null)
            {
                this.board.SwapTiles(this.selectedTile, hittedTile);
                
                BoardTile[] matches = this.board.GetMatches();
                if (matches.Length > 0)
                {
                    this.board.CollectMatches(matches);
                }
                else
                {
                    this.board.SwapTiles(this.selectedTile, hittedTile);
                }
            }

            this.selectedTile = null;
        }
    }
    
    private BoardTile GetTileAtPosition(Vector2 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);
        
        if (hit.collider)
        {
            return hit.collider.gameObject.GetComponent<BoardTile>();
        }

        return null;
    }
}
