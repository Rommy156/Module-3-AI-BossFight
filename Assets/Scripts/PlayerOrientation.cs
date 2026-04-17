using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerOrientation : MonoBehaviour
{
    public Transform target;
    [SerializeField]Vector3 offset;
    [SerializeField] Vector2 clampAxis = new Vector2(60, 60);

    [SerializeField] float followSmoothing = 5;
    [SerializeField] float rotateSmoothing = 5;
    [SerializeField] float sensitivity = 1;

    public InputActionReference LockOn;

    float rotX, rotY;
    Transform cameraTransform;
    bool cursorLocked = false;

    public float rotationSpeed = 10f;

    private Vector2 moveInput;

    void Update()
    {
        Vector3 targetPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, followSmoothing * Time.deltaTime);

        CameraTargetRotation();

        if (LockOn != null && LockOn.action.WasPressedThisFrame())
        {
            if (cursorLocked)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
    void CameraTargetRotation()
    {
        Vector2 mouseAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        rotX += (mouseAxis.x * sensitivity) * Time.deltaTime;
        rotY -= (mouseAxis.y * sensitivity) * Time.deltaTime;

        rotY = Mathf.Clamp(rotY, clampAxis.x, clampAxis.y);

        Quaternion localRotation = Quaternion.Euler(rotY, rotX, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, localRotation,  Time.deltaTime * rotateSmoothing );
    }
    
    
}