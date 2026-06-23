using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class CharacterSpawner : MonoBehaviour
{
    [SerializeField] private GameObject characterPrefab;

    [Header("References")]
    [SerializeField] private Button actionButton;
    [SerializeField] private Joystick joystickRef;


    private ARRaycastManager _raycastManager;
    private GameObject _spawnedCharacter;
    private static List<ARRaycastHit> Hits = new();


    private void Awake()
    {
        _raycastManager = GetComponent<ARRaycastManager>();
    }

    private void Update()
    {
        SpawnCharacterCheck();
    }

    private void SpawnCharacterCheck()
    {
        if (_spawnedCharacter != null) return;

        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;

        if (_raycastManager.Raycast(touch.position, Hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = Hits[0].pose;

            SpawnCharacter(hitPose.position, hitPose.rotation);
        }
    }

    public void SpawnCharacterBtn()
    {
        if (_spawnedCharacter != null) return;

        Transform cam = Camera.main.transform;
        Vector3 spawnPos = cam.position + cam.forward * 1.5f;

        SpawnCharacter(spawnPos, Quaternion.identity);
    }

    private void SpawnCharacter(Vector3 Position, Quaternion Rotation)
    {
        _spawnedCharacter = Instantiate(characterPrefab, Position, Rotation);

        ARCharacterController controller = _spawnedCharacter.GetComponent<ARCharacterController>();
        controller.SetInputs(actionButton, joystickRef);
    }
}
