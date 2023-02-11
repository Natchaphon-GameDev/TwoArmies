
using System;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Transform playerCameraTransform = default;
    [SerializeField] private float speed = default;
    [SerializeField] private float screenBorderThickness = default;
    [SerializeField] private Vector2 screenXLimits = default;
    [SerializeField] private Vector2 screenZLimits = default;


    private Controller controls;
    private Vector2 previousInput;

    public override void OnStartAuthority()
    {
        playerCameraTransform.gameObject.SetActive(true);

        controls = new Controller();
        controls.Player.MoveCamara.performed += SetPreviousInput;
        controls.Player.MoveCamara.canceled += SetPreviousInput;

        controls.Enable();
    }

    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority || !Application.isFocused) {return;}
        
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        var pos = playerCameraTransform.position;

        if (previousInput == Vector2.zero)
        {
            var cursorMovement = Vector3.zero;

            var cursorPosition = Mouse.current.position.ReadValue();

            if (cursorPosition.y >= Screen.height - screenBorderThickness)
            {
                cursorMovement.z++;
            }
            else if (cursorPosition.y <= screenBorderThickness)
            {
                cursorMovement.z--;
            }
            
            if (cursorPosition.x >= Screen.width - screenBorderThickness)
            {
                cursorMovement.x++;
            }
            else if (cursorPosition.x <= screenBorderThickness)
            {
                cursorMovement.x--;
            }

            pos += cursorMovement.normalized * speed * Time.deltaTime;
        }
        else
        {
            pos += new Vector3(previousInput.x, 0f, previousInput.y) * speed * Time.deltaTime;
        }

        pos.x = Mathf.Clamp(pos.x, screenXLimits.x, screenXLimits.y);
        pos.z = Mathf.Clamp(pos.z, screenZLimits.x, screenZLimits.y);

        playerCameraTransform.position = pos;
    }

    private void SetPreviousInput(InputAction.CallbackContext ctx)
    {
        previousInput = ctx.ReadValue<Vector2>();
    }
}