using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCurser : MonoBehaviour
{
    /// <summary>
    /// Das Image, das genutzt wird um mit dem Curser 
    /// Einheiten auszuwählen.
    /// </summary>
    public Image selectionImage;

    /// <summary>
    /// Das Rect, des Images
    /// </summary>
    private RectTransform rectTransform;

    /// <summary>
    /// Die Größe des Canvas
    /// </summary>
    public RectTransform canvas;

    /// <summary>
    /// dDie genutzte Camera.
    /// </summary>
    private Camera _camera;

    /// <summary>
    /// Liste der gerade ausgewählten Gameobjekte.
    /// </summary>
    public List<GameObject> selectedGameObjects = new List<GameObject>();

    /// <summary>
    /// Wird die Mouse gerade gerückt und gezogen?
    /// </summary>
    private bool isDrag;

    /// <summary>
    /// Ist gerade etwas ausgewählt?
    /// </summary>
    private bool onSelecting;

    /// <summary>
    /// Letzt Mouseposition
    /// </summary>
    private Vector3 lastMousePosition = Vector3.zero;

    /// <summary>
    /// Mouseposition zu Beginn des Ziehens.
    /// </summary>
    private Vector2 startMousePosition;

    /// <summary>
    /// Liste der Objekte, die die Kamera erfasst.
    /// </summary>
    private List<GameObject> objectsInCameraView = new List<GameObject>();

    /// <summary>
    /// Array, welches die vier Kantens des selectionImages erfasst.
    /// </summary>
    private Vector3[] cc = new Vector3[4];

    /// <summary>
    /// Ray, um die Treffer auf einzelnen Objekten zu registrieren.
    /// </summary>
    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        rectTransform = selectionImage.GetComponent<RectTransform>();
        selectionImage.gameObject.SetActive(false);
        lastMousePosition = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        bool deselect = Input.GetKey(KeyCode.LeftShift);

        // Wenn linke Mousetaste gedrückt wir.
        if (Input.GetMouseButtonDown(0))
        {
            startMousePosition = Input.mousePosition;

            startMousePosition.x = startMousePosition.x - (Screen.width / 2);
            startMousePosition.y = startMousePosition.y - (Screen.height / 2);

            rectTransform.anchoredPosition = startMousePosition;
            rectTransform.sizeDelta = Vector2.zero;
            lastMousePosition = Input.mousePosition;
        }

        if (IsMouseDraged(2))
        {
            if (!selectionImage.gameObject.activeInHierarchy)
            {
                selectionImage.gameObject.SetActive(true);
            }

            Vector2 currentMousePosition = Input.mousePosition;
            currentMousePosition.x = currentMousePosition.x - (Screen.width / 2);
            currentMousePosition.y = currentMousePosition.y - (Screen.height / 2);
            Vector2 size = currentMousePosition - startMousePosition;

            size.x = Mathf.Abs(size.x);
            size.y = Mathf.Abs(size.y);

            rectTransform.sizeDelta = size;

            Vector2 center = (startMousePosition + currentMousePosition) * 0.5f;
            rectTransform.anchoredPosition = center;

            if (onSelecting)
            {
                deselect = false;
            }

            GetGameObjectInSelection(deselect);
        }
        else
        {
            if (selectionImage.gameObject.activeInHierarchy)
            {
                selectionImage.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Methode zu selektieren einzelner GameObjects
    /// </summary>
    /// <param name="delete">befinden wir uns im unselect Modus</param>
    public void SingleSelect(bool delete = false)
    {

    }

    /// <summary>
    /// Methode zur Bestimmung, ob die Maus gedrückt und gezogen wird.
    /// </summary>
    /// <param name="min">Mindestbewegung der Mouse</param>
    /// <returns></returns>
    public bool IsMouseDraged(float min = 0f)
    {
        bool isMouseDown;
        bool currentMouseDown = Input.GetMouseButton(0);
        Vector3 currentMousePosition = Input.mousePosition;

        bool positionChanged = Vector3.Distance(currentMousePosition,lastMousePosition) >= min;

        if (currentMouseDown)
        {
            isMouseDown = true;
            lastMousePosition = currentMousePosition;
            if (positionChanged)
            {
                isDrag = true;
            }
        }
        else
        {
            isDrag = false;
            isMouseDown = false;
        }

        return isMouseDown && isDrag;
    }

    /// <summary>
    /// Liste der selektierten Gameobjekte erstellen.
    /// </summary>
    /// <param name="deleting">befinden wir uns im Modus unselect?</param>
    public void GetGameObjectInSelection(bool deleting = false)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(_camera);

        objectsInCameraView.Clear();

        foreach(GameObject gameObject in FindObjectsOfType<GameObject>())
        {
            Collider collider = gameObject.GetComponent<Collider>();
            if(collider != null && GeometryUtility.TestPlanesAABB(planes, collider.bounds))
            {
                if (!objectsInCameraView.Contains(gameObject))
                {
                    objectsInCameraView.Add(gameObject);
                }
            }
        }

        for(int i = 0; i < objectsInCameraView.Count; i++)
        {
            Vector3 sp = _camera.WorldToScreenPoint(objectsInCameraView[i].transform.position);

            rectTransform.GetWorldCorners(cc);

            if(sp.x >= cc[0].x && sp.x <= cc[2].x
                && sp.y >= cc[0].y && sp.y <= cc[1].y)
            {
                if (deleting)
                {
                    if (selectedGameObjects.Contains(objectsInCameraView[i]))
                    {
                        objectsInCameraView[i].GetComponent<IHuman>().isSelected = false;
                        selectedGameObjects.Remove(objectsInCameraView[i]);
                    }
                }
                else
                {
                    if (!selectedGameObjects.Contains(objectsInCameraView[i]))
                    {
                        objectsInCameraView[i].GetComponent<IHuman>().isSelected = true;
                        selectedGameObjects.Add(objectsInCameraView[i]);
                    }
                }
            }
            else
            {
                if(deleting || onSelecting)
                {
                    continue;
                }

                if(selectedGameObjects.Contains(objectsInCameraView[i])
                    && objectsInCameraView[i].GetComponent<IHuman>())
                {
                    objectsInCameraView[i].GetComponent<IHuman>().isSelected = false;
                    selectedGameObjects.Remove(objectsInCameraView[i]);
                }
            }
        }
    }
}
