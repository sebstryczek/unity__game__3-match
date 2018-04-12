using UnityEngine;

public class BoardTile : MonoBehaviour
{
    [SerializeField] private GameObject border;
    [SerializeField] private SpriteRenderer image;

    public int Row { get; private set; }
    public int Column { get; private set; }
    public TileType Type { get; private set; }
    public bool HasType
    {
        get
        {
            return this.Type != null;
        }
    }

    public void SetType(TileType type)
    {
        this.Type = type;

        if (type != null)
        {
            this.image.material.color = type.color;
        }
        else
        {
            this.image.material.color = new Color(0, 0, 0, 0);
        }
    }

    public void SetCoords(int rowIndex, int columnIndex)
    {
        this.Row = rowIndex;
        this.Column = columnIndex;
        this.gameObject.name = string.Format("[{0},{1}]", rowIndex, columnIndex);
    }

    public bool IsTheSameType(BoardTile otherTile)
    {
        return this.Type == otherTile.Type;
    }
    
    public bool IsNeighbor(BoardTile otherTile)
    {
        if (this.Row == otherTile.Row)
        {
            return this.Column == otherTile.Column - 1 || this.Column == otherTile.Column + 1;
        }

        if (this.Column == otherTile.Column)
        {
            return this.Row == otherTile.Row - 1 || this.Row == otherTile.Row + 1;
        }

        return false;
    }

    private void Awake()
    {
        this.Deselect();
    }

    public void Select()
    {
        this.border.SetActive(true);
    }
    
    public void Deselect()
    {
        this.border.SetActive(false);
    }
}
