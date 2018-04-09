using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private GameObject border;

    [SerializeField]
    private SpriteRenderer element;

    public TileType type;

    private void Awake()
    {
        this.border.SetActive(false);
    }

    public void SetType(TileType type)
    {
        this.type = type;
        this.element.material.color = type.color;
    }

    public void Select()
    {
        this.border.SetActive(true);
    }

    public void Deselect()
    {
        this.border.SetActive(false);
    }

    public void Clear()
    {
        this.type = null;
        this.element.material.color = Color.white;
    }
}
