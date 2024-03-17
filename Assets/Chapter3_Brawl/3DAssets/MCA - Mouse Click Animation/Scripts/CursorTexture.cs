using UnityEngine;

public class CursorTexture : MonoBehaviour
{
    public Texture2D cursorTexture; // normal cursor
    public Texture2D attackTexture; //attack cursor
    public Texture2D selectTexture; //attack cursor

    private Vector2 mouse;
    private CursorMode cursorMode = CursorMode.Auto;
    private Vector2 hotSpot = Vector2.zero;

    private bool selectActive = false;
    private bool moveActive = false;

    private RaycastHit hit;

    void toggleMouse(Texture2D cursor){ 
        if(cursor == attackTexture)
            Cursor.SetCursor(attackTexture, hotSpot, cursorMode);
        if(cursor == selectTexture)
            Cursor.SetCursor(selectTexture, hotSpot, cursorMode);
        if(cursor == cursorTexture)
            Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    void Update()
    {

        if (Input.GetButtonDown("Fire1"))
            selectActive = true;
        if (Input.GetButtonUp("Fire1"))
            selectActive = false;

        if (Input.GetButtonDown("Fire2"))
            moveActive = true;
        if (Input.GetButtonUp("Fire2"))
            moveActive = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)){
            if (hit.collider.tag == "OutlineHover"){
                toggleMouse(attackTexture);
            }else{
                if (selectActive == true){
                    toggleMouse(selectTexture);
                }else if (moveActive == true){
                    toggleMouse(attackTexture);
                }else{
                    toggleMouse(cursorTexture);
                }
            }
        }else{
            if (selectActive == true){
                toggleMouse(selectTexture);
            }else if(moveActive == true){
                toggleMouse(attackTexture);
            }else{
                toggleMouse(cursorTexture);
            }
        }

    }
}
















