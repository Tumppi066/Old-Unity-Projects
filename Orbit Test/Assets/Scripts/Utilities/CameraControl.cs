using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraControl : MonoBehaviour
{
    public float zoomSpeed;
    public float movementSpeed;
    public GameObject currentlyFocusedObject = null;
    [Header("Set by script, don't touch")]
    public Camera camera;
    private GameObject previousGameObject = null;
    private bool wasNull = false;
    private Universe Universe;
    private void Start()
    {
        Universe = FindObjectOfType<Universe>();
        camera = GetComponent<Camera>();
    }
    public void SetMovementSpeed(float speed)
    {
        movementSpeed = speed;
    }
    public void SetScrollSpeed(float speed)
    {
        zoomSpeed = speed;
    }
    private void Update()
    {
        
        gameObject.transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0)*Time.deltaTime*movementSpeed*(camera.orthographicSize/5);


        if(currentlyFocusedObject != null)
        {
            Tween tween = gameObject.transform.DOMove(new Vector3(currentlyFocusedObject.transform.position.x, currentlyFocusedObject.transform.position.y, -10), Time.fixedDeltaTime);
            tween.SetEase(Ease.Linear);
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                FindObjectOfType<PlanetManagement>().targetedBody = null;
                currentlyFocusedObject = null;
            }
            if (!wasNull || currentlyFocusedObject != previousGameObject)
            {
                DOTween.Clear();
                try
                {
                    camera.DOOrthoSize(currentlyFocusedObject.GetComponent<BodyInformation>().radius * 3, 0.5f);
                    Universe.Print("Set focus to " + currentlyFocusedObject.name, gameObject);
                }
                catch { }             
            }
            previousGameObject = currentlyFocusedObject;
            wasNull = true;
        }
        else
        {
            wasNull = false;
        }

        if(Input.mouseScrollDelta.y != 0)
        {
            camera.DOOrthoSize(camera.orthographicSize + -Input.mouseScrollDelta.y * Time.deltaTime * zoomSpeed * Mathf.Clamp01(camera.orthographicSize / 10), 0.25f);
        }
        if(camera.orthographicSize < 0)
        {
            DOTween.Clear();
            camera.orthographicSize = 0.1f;
        }
    }
}
