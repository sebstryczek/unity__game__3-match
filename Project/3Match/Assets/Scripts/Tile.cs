using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField] private GameObject border;
    [SerializeField] private SpriteRenderer image;

    public int Row { get; private set; }
    public int Column { get; private set; }

    public TileType type;

    public void SetType(TileType type)
    {
        this.type = type;

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
    }

    public void Clear()
    {
        this.SetType(null);
    }
    
    private void Awake()
    {
        this.border.SetActive(false);
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
