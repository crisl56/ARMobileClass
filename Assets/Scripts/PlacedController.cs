using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlacedController : MonoBehaviour
{
    [SerializeField] private ARController controller;
    [SerializeField] private Button[] prefabButtons;

    [SerializeField] private GameObject actionPanel;
    [SerializeField] private Button deleteButton;
    [SerializeField] private Button rotateLeftButton;
    [SerializeField] private Button rotateRightButton;
    [SerializeField] private Button deleteAllButton;

    [SerializeField] private GameObject materialPanel;
    [SerializeField] private Button[] materialButtons;

    private void Start()
    {
        SetupPrefabButton();
        SetupActionButtons();
        SetupMaterialButtons();

        actionPanel.SetActive(true);

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        controller.OnObjectSelected += OnObjectSelected;
        controller.OnSelectionCleared += OnSelectionCleared;
        controller.OnPlacedObjectsChanged += RefreshStatus;
    }

    private void SetupPrefabButton()
    {
        for (int i = 0; i < prefabButtons.Length; i++)
        {
            // we need to save i as index or else all buttons with end up doing the last one.
            int index = i;
            prefabButtons[i].onClick.AddListener(() =>
            {
                controller.SetSelectedPrefab(index);
            });
        }
    }

    private void SetupActionButtons()
    {
        deleteButton.onClick.AddListener(() => controller.DeleteSelected());
        rotateLeftButton.onClick.AddListener(() => controller.RotateSelected(-1f));
        rotateRightButton.onClick.AddListener(() => controller.RotateSelected(1f));
        deleteAllButton.onClick.AddListener(() => controller.ClearAll());
    }

    private void SetupMaterialButtons()
    {
        for (int i = 0; i < materialButtons.Length; i++)
        {
            materialButtons[i].onClick.AddListener(() =>
            {
                controller.SetSelectedMaterial(i);
            });
        }
    }

    private void OnObjectSelected(PlacedObject obj)
    {
        actionPanel.SetActive(true);
        materialPanel.SetActive(true);

        RefreshStatus();
    }

    private void OnSelectionCleared()
    {
        actionPanel.SetActive(false);
        materialPanel.SetActive(false);
        RefreshStatus();
    }

    private void RefreshStatus()
    {

    }

    private void OnDestroy()
    {
        controller.OnObjectSelected -= OnObjectSelected;
        controller.OnSelectionCleared -= OnSelectionCleared;
        controller.OnPlacedObjectsChanged -= RefreshStatus;
    }
}