using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Random = System.Random;

public class ARController : MonoBehaviour
{
    [SerializeField] private ARRaycastManager arRaycastManager;
    [SerializeField] private Camera arCameraManager;

    [SerializeField] private GameObject[] placeablePrefabs;
    [SerializeField] private GameObject pointerPrefab;

    [SerializeField] private Material[] swappableMaterials;

    [SerializeField] private float rotationStep = 15f;
    [SerializeField] private float scaleStep = 0.1f;
    [SerializeField] private float minScale = 0.05f;
    [SerializeField] private float maxScale = 3.0f;

    private int _selectedPrefabIndex;
    private PlacedObject _selectedObject;

    private readonly List<ARRaycastHit> arHits = new List<ARRaycastHit>();
    private readonly List<PlacedObject> _placedObjects = new List<PlacedObject>();

    public PlacedObject SelectedObject => _selectedObject;
    public int PlacedCount => _placedObjects.Count;
    public Material[] SwappableMaterials => swappableMaterials;

    public event System.Action<PlacedObject> OnObjectSelected;
    public event System.Action OnSelectionCleared;
    public event System.Action OnPlacedObjectsChanged;

    private void Update()
    {
        UpdatePointer();

        if (TryGetPointerPlaced(out Vector2 screenPos) && !IsPointerOverUI())
        {
            HandleInput(screenPos);
        }
    }

    private void UpdatePointer()
    {
        Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        bool valid = arRaycastManager.Raycast(screenCenter, arHits, TrackableType.PlaneWithinPolygon);

        pointerPrefab.SetActive(valid);

        if (valid)
        {
            Pose pose = arHits[0].pose;
            pointerPrefab.transform.SetPositionAndRotation(pose.position, pose.rotation);
        }
    }

    private void HandleInput(Vector2 screenPos)
    {
        // whenever the screen points to ray.
        Ray ray = arCameraManager.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            PlacedObject placed = hit.collider.GetComponentInParent<PlacedObject>();
            if (placed != null)
            {
                SelectObject(placed);
                return;
            }
        }

        if (arRaycastManager.Raycast(screenPos, arHits, TrackableType.PlaneWithinPolygon))
        {
            ClearSelection();
            PlaceObject(arHits[0].pose);
        }
    }

    private void PlaceObject(Pose pose)
    {
        GameObject go = Instantiate(placeablePrefabs[_selectedPrefabIndex], pose.position, pose.rotation);

        PlacedObject placed = go.GetComponent<PlacedObject>();
        if (placed == null)
            placed = go.AddComponent<PlacedObject>();

        _placedObjects.Add(placed);
        OnPlacedObjectsChanged?.Invoke();
    }

    public void ClearSelection()
    {
        if (_selectedObject != null)
        {
            _selectedObject.SetSelected(false);
            _selectedObject = null;
            OnSelectionCleared?.Invoke();
        }
    }

    private void SelectObject(PlacedObject placed)
    {
        if (_selectedObject == placed) return;

        ClearSelection();

        _selectedObject = placed;
        _selectedObject.SetSelected(true);
        OnObjectSelected.Invoke(_selectedObject);
    }

    public void ClearAll()
    {
        foreach (PlacedObject obj in _placedObjects)
            if (obj is not null)
                Destroy(obj.gameObject);

        _placedObjects.Clear();
        _selectedObject = null;
        OnSelectionCleared?.Invoke();
        OnPlacedObjectsChanged?.Invoke();
    }

    private bool TryGetPointerPlaced(out Vector2 position)
    {
        Pointer pointer = Pointer.current;
        if (pointer != null && pointer.press.wasPressedThisFrame)
        {
            position = pointer.position.ReadValue();
            return true;
        }

        position = default;
        return false;
    }

    public void RotateSelected(float direction)
    {
        if (_selectedObject is not null)
            _selectedObject.transform.Rotate(Vector3.up, direction * rotationStep);
    }

    public void SetSelectedMaterial(int materialIndex)
    {
        if (_selectedObject is not null && materialIndex >= 0 && materialIndex < swappableMaterials.Length)
        {
            _selectedObject.SetMaterial(swappableMaterials[materialIndex]);
        }
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    public void SetSelectedPrefab(int index)
    {
        Debug.Log(index);
        _selectedPrefabIndex = Mathf.Clamp(index, 0, placeablePrefabs.Length - 1);
    }

    public void DeleteSelected()
    {
        if (_selectedObject == null) return;

        _placedObjects.Remove(_selectedObject);
        Destroy(_selectedObject.gameObject);
        _selectedObject = null;
        OnSelectionCleared?.Invoke();
        OnPlacedObjectsChanged?.Invoke();
    }
}
