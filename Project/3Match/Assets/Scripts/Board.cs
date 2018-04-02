using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject prefabTile;

    private int lines = 8;
    private int columns = 6;

    private void Start()
    {
        float horizontalOffset = (this.columns - 1.0f) / 2.0f;
        float verticalOffset = (this.lines - 1.0f) / 2.0f;
        for (int lineIndex = 0; lineIndex < this.lines; lineIndex++)
        {
            for (int columnIndex = 0; columnIndex < this.columns; columnIndex++)
            {
                Vector3 pos = new Vector3(columnIndex - horizontalOffset, lineIndex - verticalOffset, 0);
                GameObject tile = Instantiate(this.prefabTile, pos, Quaternion.identity);
                tile.transform.SetParent(this.transform);
            }
        }
    }
    
    private void Update()
    {
        
    }
}
