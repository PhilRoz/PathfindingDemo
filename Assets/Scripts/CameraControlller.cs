using UnityEngine;

public class CameraControlller : MonoBehaviour
{
    public bool clampZoom;
    Vector3 lastMousePos;

    const float minZoomValue = 5f;
    const float maxZoomValue = 50f;

    GameMap map;
    EditMode editMode; 
    Collider lastHitTile;

    private void Start()
    {
        map = FindFirstObjectByType<GameMap>();
        editMode = FindFirstObjectByType<EditMode>();
    }

    void Update()
    {
        Click();

        CameraDrag();
        CameraZoom();

        if (editMode.currentState == EditMode.State.Play)
        TryRunPathfinding();
    }

    void Click()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != lastHitTile || Input.GetMouseButtonDown(0))
                {
                    lastHitTile = hit.collider;

                    TileDrawer tileDrawer = hit.collider.GetComponent<TileDrawer>();
                    if (tileDrawer != null)
                    {
                        editMode.Click(tileDrawer);
                    }
                }
            }
        }
    }

    void CameraDrag()
    {
        if (Input.GetMouseButtonDown(2))
        {
            lastMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButton(2))
        {
            Vector3 currentMousePos = Input.mousePosition;
            Vector3 mouseDelta = currentMousePos - lastMousePos;

            float distanceFromCamera = transform.position.y;
            float worldHeight = 2f * distanceFromCamera * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float worldUnitsPerPixel = worldHeight / Screen.height;

            Vector3 worldMovement = new Vector3(
                -mouseDelta.x * worldUnitsPerPixel,
                0,
                -mouseDelta.y * worldUnitsPerPixel
            );
            transform.Translate(worldMovement);
            lastMousePos = currentMousePos;
        }
    }
    void CameraZoom()
    {
        transform.Translate(Vector3.down * Input.mouseScrollDelta.y * (transform.position.y / 10));
        if (clampZoom)
        {
            Vector3 pos = transform.position;
            pos.y = Mathf.Clamp(pos.y, minZoomValue, maxZoomValue);
            transform.position = pos;
        }
    }


    void TryRunPathfinding()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider == lastHitTile) { return; }
            lastHitTile = hit.collider;

            TileDrawer tileDrawer = hit.collider.GetComponent<TileDrawer>();
            if (tileDrawer != null)
            {
                map.FindPath(tileDrawer.transform.position);
            }
        }
    }
}
