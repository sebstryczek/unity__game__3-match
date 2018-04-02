using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    private Color defaultColor;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.defaultColor = this.spriteRenderer.material.color;
    }

    private void OnMouseEnter()
    {
        this.spriteRenderer.material.color = new Color(1, 0, 0, 1);
    }

    private void OnMouseExit()
    {
        this.spriteRenderer.material.color = this.defaultColor;
    }
}
