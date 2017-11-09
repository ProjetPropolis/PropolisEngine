using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseUiController : MonoBehaviour {

    public string mouseState = "default";
    public Texture2D cursorTextureCreate,cursorTextureDelete;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    void OnGUI() {

        if (mouseState != "default")
        {
            if (mouseState == "create")
            {
                Cursor.SetCursor(cursorTextureCreate, hotSpot, cursorMode);
            }

            if (mouseState == "delete")
            {
                Cursor.SetCursor(cursorTextureDelete, hotSpot, cursorMode);
            }

        }

        Cursor.SetCursor(null, Vector2.zero, cursorMode);

        if(Input.GetMouseButtonDown(0))
        {
            mouseState = "default";
        }
    }

    public void setState(string fromUi)
    {
        mouseState = fromUi;
    }
}
