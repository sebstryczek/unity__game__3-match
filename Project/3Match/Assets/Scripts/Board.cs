using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board
{
    public int RowsCount { get; private set; }
    public int ColumnsCount { get; private set; }

    private GameObject go = null;
    private Tile[] tiles = null;

    public Board(int rowsCount, int columnsCount)
    {
        this.go = new GameObject("Board");
        this.RowsCount = rowsCount;
        this.ColumnsCount = columnsCount;
        this.tiles = new Tile[this.RowsCount * this.ColumnsCount];
    }

    public void AddTile(Tile tile)
    {
        this.tiles[tile.Row * this.ColumnsCount + tile.Column] = tile;
    }

    public Tile GetTile(int rowIndex, int columnIndex)
    {
        return System.Array.Find(this.tiles, x => x && x.Row == rowIndex && x.Column == columnIndex);
    }

    public Tile GetTileAtPosition(Vector2 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);
        
        if (hit.collider)
        {
            return hit.collider.gameObject.GetComponent<Tile>();
        }

        return null;
    }

    public bool AreNeighbors(Tile t1, Tile t2)
    {
        if (t1.Row == t2.Row)
        {
            return t1.Column == t2.Column - 1 || t1.Column == t2.Column + 1;
        }

        if (t1.Column == t2.Column)
        {
            return t1.Row == t2.Row - 1 || t1.Row == t2.Row + 1;
        }

        return false;
    }

    public void SwapTiles(Tile t1, Tile t2)
    {
        int t1Row = t1.Row;
        int t1Column = t1.Column;
        t1.SetCoords(t2.Row, t2.Column);
        t2.SetCoords(t1Row, t1Column);

        Vector2 firstPos = t1.transform.position;
        t1.transform.position = t2.transform.position;
        t2.transform.position = firstPos;
    }

    public TileType[] GetForbiddenTypes(int rowIndex, int columnIndex)
    {
        List<TileType> result = new List<TileType>();
        
        Tile bottom1 = this.GetTile(rowIndex - 1, columnIndex);
        Tile bottom2 = this.GetTile(rowIndex - 2, columnIndex);
        if (bottom1 != null && bottom2 != null && bottom1.type == bottom2.type && !result.Contains(bottom1.type))
        {
            result.Add(bottom1.type);
        }
        
        Tile left1 = this.GetTile(rowIndex, columnIndex - 1);
        Tile left2 = this.GetTile(rowIndex, columnIndex - 2);
        if (left1 != null && left2 != null && left1.type == left2.type && !result.Contains(left1.type))
        {
            result.Add(left1.type);
        }
        
        Tile top1 = this.GetTile(rowIndex + 1, columnIndex);
        Tile top2 = this.GetTile(rowIndex + 2, columnIndex);
        if (top1 != null && top2 != null && top1.type == top2.type && !result.Contains(top1.type))
        {
            result.Add(top1.type);
        }
        
        Tile right1 = this.GetTile(rowIndex, columnIndex + 1);
        Tile right2 = this.GetTile(rowIndex, columnIndex + 2);
        if (right1 != null && right2 != null && right1.type == right2.type && !result.Contains(right1.type))
        {
            result.Add(right1.type);
        }

        return result.ToArray();
    }

    public Tile FindNeighborTitle(Tile root, float angle)
    {
        if (angle > 320 || angle > 0 && angle < 40)
        {
            return this.GetTile(root.Row + 1, root.Column);
        }

        if (angle > 50 && angle < 130)
        {
            return this.GetTile(root.Row, root.Column + 1);
        }

        if (angle > 140 && angle < 220)
        {
            return this.GetTile(root.Row - 1, root.Column);
        }

        if (angle > 230 && angle < 310)
        {
            return this.GetTile(root.Row, root.Column - 1);
        }

        return null;
    }

    public void Renumber()
    {
        for (int i = 0; i < this.tiles.Length; i++)
        {
            Tile tile = this.tiles[i];
            
            if (tile.type == null && tile.Row < this.RowsCount - 1)
            {
                int nextRow = tile.Row + 1;

                while(nextRow < this.RowsCount && this.GetTile(nextRow, tile.Column).type == null)
                {
                    nextRow++;
                }

                if (nextRow < this.RowsCount)
                {
                    this.SwapTiles(tile, this.GetTile(nextRow, tile.Column));
                }
            }
        }

        this.Collect();
    }

    public void Recreate()
    {
        for (int i = 0; i < this.tiles.Length; i++)
        {
            Tile tile = this.tiles[i];

            if (tile.type == null)
            {
                TileType type = GameManager.Instance.GetRandomType(tile);
                tile.SetType(type);
            }
        }
    }
    
    public void Collect()
    {
        List<Tile> matches = new List<Tile>(); 
        for (int rowIndex = 0; rowIndex < this.RowsCount; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < this.ColumnsCount; columnIndex++)
            {
                Tile tile = this.GetTile(rowIndex, columnIndex);

                if (tile.type == null) continue;

                if (columnIndex > 0 && columnIndex < this.ColumnsCount - 1)
                {
                    Tile prev = this.GetTile(rowIndex, columnIndex - 1);
                    Tile next = this.GetTile(rowIndex, columnIndex + 1);

                    if (prev.type == tile.type && tile.type == next.type)
                    {
                        if (!matches.Contains(prev)) matches.Add(prev);
                        if (!matches.Contains(tile)) matches.Add(tile);
                        if (!matches.Contains(next)) matches.Add(next);
                    }
                }

                if (rowIndex > 0 && rowIndex < this.RowsCount - 1)
                {
                    Tile prev = this.GetTile(rowIndex - 1, columnIndex);
                    Tile next = this.GetTile(rowIndex + 1, columnIndex);

                    if (prev.type == tile.type && tile.type == next.type)
                    {
                        if (!matches.Contains(prev)) matches.Add(prev);
                        if (!matches.Contains(tile)) matches.Add(tile);
                        if (!matches.Contains(next)) matches.Add(next);
                    }
                }
            }
        }
        
        GameManager.Instance.AddPoints(matches.Count);
        
        matches.ForEach(x => x.Clear());

        if (matches.Count > 0)
        {
            this.Renumber();
        }
        else
        {
            this.Recreate();
        }
    }
}
