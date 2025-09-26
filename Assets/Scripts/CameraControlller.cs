using UnityEngine;

public class CameraControlller : MonoBehaviour
{
    public bool clampZoom;
    Vector3 lastMousePos;

    const float minZoomValue = 5f;
    const float maxZoomValue = 50f;

    GameMap map;
    Collider cachedHit;

    private void Start()
    {
        map = FindFirstObjectByType<GameMap>();
    }

    void Update()
    {
        CameraDrag();
        CameraZoom();


        //if (Input.GetKeyDown(KeyCode.Space))
        TryRunPathfinding();
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
            if (hit.collider == cachedHit) { return; }
            cachedHit = hit.collider;

            TileDrawer tileDisplay = hit.collider.GetComponent<TileDrawer>();
            if (tileDisplay != null)
            {
                map.FindPath(tileDisplay.transform.position);
            }
        }
    }
}
