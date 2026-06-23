using System;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class ARCharacterController : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Joystick joystick;
    [SerializeField] private Animator animator;
    private Button actionButton;

    [Header("Settings")] 
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 10f;

    private const string AnimatorSpeedVarName = "Speed";
    private const string AnimatorActionVarName = "Action";

    private void Update()
    {
        MoveCharacter();
    }

    private void MoveCharacter()
    {
        if (joystick == null) return;

        Vector2 input = joystick.Direction;
        Vector3 move = new Vector3(input.x, 0f, input.y);

        if (move.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

            transform.position += move * (moveSpeed * Time.deltaTime);
        }

        animator.SetFloat(AnimatorSpeedVarName, input.magnitude);
    }

    public void TriggerAction()
    {
        animator.SetTrigger(AnimatorActionVarName);
    }

    public void SetInputs(Button actionButton, Joystick joystick)
    {
        this.joystick = joystick;
        this.actionButton = actionButton;

        actionButton.onClick.AddListener(TriggerAction);
    }

    private void OnDestroy()
    {
        if(actionButton != null)
            actionButton.onClick.RemoveListener(TriggerAction);
    }
}
