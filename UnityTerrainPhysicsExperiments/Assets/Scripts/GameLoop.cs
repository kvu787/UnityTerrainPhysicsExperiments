using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Object position:
/// Although the world is 3D, the movement input is 2D.
/// This means that the player can only tell an object to move in the X or Y directions using a gamepad stick or WASD.
/// The game systems will set the Z position of each object based on the object's XY position, the object's dimensions, and the terrain.
///
/// Object rotation:
/// The front facing vector of the object should never point straight up.
/// </summary>
public class GameLoop : MonoBehaviour {
    [SerializeField]
    [Range(0f, 100f)]
    private float CameraPivotTranslationSpeed = 20f;

    [SerializeField]
    [Range(0f, 100f)]
    private float CameraZoomSpeed = 100f;

    [SerializeField]
    private float CameraPivotYawSpeed = 180f;

    [SerializeField]
    private float CameraPivotPitchSpeed = 100f;

    public GameObject Car;
    public GameObject CameraPivot;
    public Camera Camera;
    public Collider TerrainCollider;

    private Vector3 CameraPivotPosition = new(-3, 0, 2.5f);
    private float CameraPivotYaw = 133;
    private float CameraPivotPitch = 65;
    private float CameraPositionZ = -22;

    void Awake() {
        Debug.Log($"GameLoop Awake on object='{this.gameObject.name}' in scene='{this.gameObject.scene.name}'");
        QualitySettings.maxQueuedFrames = 0;
        QualitySettings.vSyncCount = 1;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        if (!SceneManager.GetSceneByName("UiScene").isLoaded) {
            SceneManager.LoadScene("UiScene", LoadSceneMode.Additive);
            Debug.Log("Finished calling 'SceneManager.LoadScene(\"UIScene\", LoadSceneMode.Additive);'");
        }
    }

    // Update is called once per frame
    void Update() {
        // TODO
        // Unity loads scenes asynchronously.
        // Theoretically, loading a scene can take a long time.
        // So, I should add a loading indicator or screen.
        if (!SceneManager.GetSceneByName("UiScene").isLoaded) {
            Debug.Log("Update: UiScene not loaded yet");
            return;
        }

        if (Gamepad.current != null) {
            Gamepad gamepad = Gamepad.current;
            Vector2 leftStick = gamepad.leftStick.ReadValue();
            Vector2 rightStick = gamepad.rightStick.ReadValue();
            float leftTrigger = gamepad.leftTrigger.ReadValue();
            float rightTrigger = gamepad.rightTrigger.ReadValue();

            // Change camera parameters based on gamepad input
            this.CameraPivotYaw += Time.deltaTime * this.CameraPivotYawSpeed * rightStick.x;
            this.CameraPivotPitch += Time.deltaTime * this.CameraPivotPitchSpeed * rightStick.y;
            this.CameraPivotPosition += Time.deltaTime * this.CameraPivotTranslationSpeed * (Quaternion.Euler(0f, this.CameraPivotYaw, 0f) * new Vector3(leftStick.x, 0, leftStick.y));
            this.CameraPositionZ += Time.deltaTime * this.CameraZoomSpeed * (rightTrigger - leftTrigger);

            // Set camera transforms
            this.Camera.transform.localPosition = new Vector3(0, 0, this.CameraPositionZ);
            this.CameraPivot.transform.SetPositionAndRotation(this.CameraPivotPosition, Quaternion.Euler(this.CameraPivotPitch, this.CameraPivotYaw, 0));
        }

        // Change car transform based on camera transform and terrain collision
        Vector3 newCarPosition = this.Car.transform.position;
        bool isHit = TryGetScreenCenterHitPoint(this.Camera, this.TerrainCollider, out Vector3 hitPoint, out Vector3 hitNormal);
        if (isHit) {
            newCarPosition = hitPoint;
        }
        float carYawOffset = -135f;
        Quaternion newCarRotation = Quaternion.AngleAxis(this.CameraPivotYaw + carYawOffset, Vector3.up);
        if (isHit) {
            newCarRotation = Quaternion.FromToRotation(Vector3.up, hitNormal) * newCarRotation;
        }
        this.Car.transform.SetPositionAndRotation(newCarPosition, newCarRotation);
    }

    // hitPoint is in world space and hitNormal is normalized
    private static bool TryGetScreenCenterHitPoint(Camera camera, Collider collider, out Vector3 hitPoint, out Vector3 hitNormal) {
        hitPoint = default;
        hitNormal = default;
        if (camera == null || collider == null) {
            return false;
        }
        Vector3 screenCenter = new(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        Ray ray = camera.ScreenPointToRay(screenCenter);
        if (collider.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity)) {
            hitPoint = raycastHit.point;
            hitNormal = raycastHit.normal.normalized;
            return true;
        }
        return false;
    }

    // hitPoint is in world space and normalized
    private static bool TryGetMouseHitPoint(Camera camera, Collider collider, out Vector3 hitPoint, out Vector3 hitNormal) {
        hitPoint = default;
        hitNormal = default;
        if (camera == null || collider == null) {
            return false;
        }
        Vector3 mousePosition = new(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), 0);
        Ray ray = camera.ScreenPointToRay(mousePosition);
        if (collider.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity)) {
            hitPoint = raycastHit.point;
            hitNormal = raycastHit.normal.normalized;
            return true;
        }
        return false;
    }

    // hitPoint is in world space and normalized
    private static bool TryGetMouseHitPointAny(Camera camera, Collider collider, out Vector3 hitPoint, out Vector3 hitNormal) {
        hitPoint = default;
        hitNormal = default;
        if (camera == null || collider == null) {
            return false;
        }
        Vector3 mousePosition = new(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), 0);
        Ray ray = camera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity)) {
            hitPoint = raycastHit.point;
            hitNormal = raycastHit.normal.normalized;
            return true;
        }
        return false;
    }
}
