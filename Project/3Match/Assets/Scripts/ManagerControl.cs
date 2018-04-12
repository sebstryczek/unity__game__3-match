using UnityEngine;

public class ManagerControl : Singleton<ManagerControl>
{
    private bool isDragging = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            this.isDragging = true;
            ManagerBoard.Instance.OnMouseDown();
        }

        if (this.isDragging)
        {
            ManagerBoard.Instance.OnMouseDrag();
        }

        if (Input.GetMouseButtonUp(0))
        {
            this.isDragging = false;
        }
    }
}
