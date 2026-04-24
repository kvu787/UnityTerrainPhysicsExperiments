using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Object position:
/// Although the world is 3D, the movement input is 2D.
/// This means that the player can only tell an object to move in the X or Y directions using a gamepad stick or WASD.
/// The game systems will set the Z position of each object based on the object's XY position, the object's dimensions, and the terrain.
///
/// Object orientation:
/// The front facing vector of the object should never point straight up.
/// </summary>
public class GameLoop : MonoBehaviour {
    public GameObject Car;
    public GameObject CameraPivot;
    public Camera MainCamera;
    public Collider TerrainCollider;

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

        Gamepad gamepad = Gamepad.current;

        if (gamepad == null) {
            return;
        }

        Vector2 leftStick = gamepad.leftStick.ReadValue();
        Vector3 cameraPivotTranslation = new(leftStick.x, 0, leftStick.y);
        cameraPivotTranslation = Quaternion.Euler(0f, this.CameraPivot.transform.eulerAngles.y, 0f) * cameraPivotTranslation;
        float moveSpeed = 20f;
        this.CameraPivot.transform.position += moveSpeed * Time.deltaTime * cameraPivotTranslation;

        Vector2 rightStick = gamepad.rightStick.ReadValue();
        float rotateSpeedDegreesPerSecond = 180f;
        float yawDelta = rightStick.x * rotateSpeedDegreesPerSecond * Time.deltaTime;
        this.CameraPivot.transform.Rotate(0, yawDelta, 0, Space.World);

        if (TryGetScreenCenterHitPoint(this.MainCamera, this.TerrainCollider, out Vector3 hitPoint, out Vector3 hitNormal)) {
            float offset = -135f;
            Quaternion spin = Quaternion.AngleAxis(this.CameraPivot.transform.eulerAngles.y + offset, Vector3.up);
            Quaternion align = Quaternion.FromToRotation(Vector3.up, hitNormal);
            Quaternion carOrientation = align * spin;

            this.Car.transform.SetPositionAndRotation(hitPoint, carOrientation);
        }
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
