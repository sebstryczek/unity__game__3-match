using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board
{
    public int RowsCount { get; private set; }
    public int ColumnsCount { get; private set; }
    private BoardTile[,] tiles = null;

    public Board(int rowsCount, int columnsCount)
    {
        this.RowsCount = rowsCount;
        this.ColumnsCount = columnsCount;
        this.tiles = new BoardTile[this.RowsCount, this.ColumnsCount];
    }

    public void AddTile(BoardTile tile)
    {
        this.tiles[tile.Row, tile.Column] = tile;
    }

    public BoardTile GetTile(int rowIndex, int columnIndex)
    {
        if (rowIndex < 0 || columnIndex < 0 || rowIndex >= RowsCount || columnIndex >= ColumnsCount)
        {
             return null;
        }
        
        return this.tiles[rowIndex, columnIndex];
    }

    public void SwapTiles(BoardTile t1, BoardTile t2)
    {
        int t1Row = t1.Row;
        int t1Column = t1.Column;
        t1.SetCoords(t2.Row, t2.Column);
        t2.SetCoords(t1Row, t1Column);

        this.tiles[t1.Row, t1.Column] = t1;
        this.tiles[t2.Row, t2.Column] = t2;
        
        Vector2 firstPos = t1.transform.position;
        t1.transform.position = t2.transform.position;
        t2.transform.position = firstPos;
    }

    public TileType[] GetForbiddenTypes(int rowIndex, int columnIndex)
    {
        List<TileType> result = new List<TileType>();
        
        BoardTile bottom1 = this.GetTile(rowIndex - 1, columnIndex);
        BoardTile bottom2 = this.GetTile(rowIndex - 2, columnIndex);
        if (bottom1 && bottom2 && bottom1.HasType && bottom1.IsTheSameType(bottom2))
        {
            if (!result.Contains(bottom1.Type)) result.Add(bottom1.Type);
        }
        
        BoardTile left1 = this.GetTile(rowIndex, columnIndex - 1);
        BoardTile left2 = this.GetTile(rowIndex, columnIndex - 2);
        if (left1 && left2 && left1.HasType && left1.IsTheSameType(left2))
        {
            if (!result.Contains(left1.Type)) result.Add(left1.Type);
        }
        
        BoardTile top1 = this.GetTile(rowIndex + 1, columnIndex);
        BoardTile top2 = this.GetTile(rowIndex + 2, columnIndex);
        if (top1 && top2 && top1.HasType && top1.IsTheSameType(top2))
        {
            if (!result.Contains(top1.Type)) result.Add(top1.Type);
        }
        
        BoardTile right1 = this.GetTile(rowIndex, columnIndex + 1);
        BoardTile right2 = this.GetTile(rowIndex, columnIndex + 2);
        if (right1 && right2 && right1.HasType && right1.IsTheSameType(right2))
        {
            if (!result.Contains(right1.Type)) result.Add(right1.Type);
        }

        return result.ToArray();
    }

    public BoardTile FindNeighborTitle(BoardTile root, float angle)
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

    public BoardTile[] GetMatches()
    {
        List<BoardTile> result = new List<BoardTile>(); 
        for (int rowIndex = 0; rowIndex < this.RowsCount; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < this.ColumnsCount; columnIndex++)
            {
                BoardTile tile = this.GetTile(rowIndex, columnIndex);

                if (!tile.HasType) continue;

                if (columnIndex > 0 && columnIndex < this.ColumnsCount - 1)
                {
                    BoardTile prevTile = this.GetTile(rowIndex, columnIndex - 1);
                    BoardTile nextTile = this.GetTile(rowIndex, columnIndex + 1);

                    if (tile.IsTheSameType(prevTile) && tile.IsTheSameType(nextTile))
                    {
                        if (!result.Contains(prevTile)) result.Add(prevTile);
                        if (!result.Contains(tile)) result.Add(tile);
                        if (!result.Contains(nextTile)) result.Add(nextTile);
                    }
                }

                if (rowIndex > 0 && rowIndex < this.RowsCount - 1)
                {
                    BoardTile prevTile = this.GetTile(rowIndex - 1, columnIndex);
                    BoardTile nextTile = this.GetTile(rowIndex + 1, columnIndex);

                    if (tile.IsTheSameType(prevTile) && tile.IsTheSameType(nextTile))
                    {
                        if (!result.Contains(prevTile)) result.Add(prevTile);
                        if (!result.Contains(tile)) result.Add(tile);
                        if (!result.Contains(nextTile)) result.Add(nextTile);
                    }
                }
            }
        }

        return result.ToArray();
    }
    
    public void CollectMatches(BoardTile[] matches)
    {
        ManagerState.Instance.AddPoints(matches.Length);
        System.Array.ForEach(matches, x => x.SetType(null));
        this.ReorderTiles();
    }

    private void ReorderTiles()
    {
        for (int i = 0; i < this.RowsCount; i++)
        {
            for (int j = 0; j < this.ColumnsCount; j++)
            {
                BoardTile tile = this.GetTile(i, j);
                
                if (!tile.HasType && tile.Row < this.RowsCount - 1)
                {
                    int nextRow = tile.Row + 1;
                    BoardTile nextTile = this.GetTile(nextRow, tile.Column);

                    while(nextRow < this.RowsCount && !nextTile.HasType)
                    {
                        nextRow++;
                        nextTile = this.GetTile(nextRow, tile.Column);
                    }

                    if (nextRow < this.RowsCount)
                    {
                        this.SwapTiles(tile, nextTile);
                    }
                }
            }
        }

        BoardTile[] matches = this.GetMatches();

        if (matches.Length > 0)
        {
            this.CollectMatches(matches);
        }
        else
        {
            this.FillEmptyTiles();
        }
    }

    public void FillEmptyTiles()
    {
        for (int i = 0; i < this.RowsCount; i++)
        {
            for (int j = 0; j < this.ColumnsCount; j++)
            {
                BoardTile tile = this.GetTile(i, j);

                if (!tile.HasType)
                {
                    TileType[] forbiddenTypes = this.GetForbiddenTypes(tile.Row, tile.Column);
                    TileType type = ManagerBoard.Instance.GetRandomType(forbiddenTypes);
                    tile.SetType(type);
                }
            }
        }
    }
}
