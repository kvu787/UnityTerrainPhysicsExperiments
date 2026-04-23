using UnityEngine;

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
    public Camera MainCamera;
    public Collider TerrainCollider;

    void Awake() {
        Debug.Log($"GameLoop Awake on object='{this.gameObject.name}' in scene='{this.gameObject.scene.name}'");
        QualitySettings.maxQueuedFrames = 0;
        QualitySettings.vSyncCount = 1;
    }

    /*
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
    }
    */

    // Update is called once per frame
    void Update() {
        Debug.Log(Input.mousePosition);
        //Console.WriteLine("here");

        if (TryGetMouseHitPoint(this.MainCamera, this.TerrainCollider, out Vector3 hitPoint, out Vector3 hitNormal)) {
            this.Car.transform.SetPositionAndRotation(hitPoint, Quaternion.FromToRotation(Vector3.up, hitNormal.normalized));
        }
    }

    // hitPoint is in world space and normalized
    public static bool TryGetMouseHitPoint(Camera camera, Collider collider, out Vector3 hitPoint, out Vector3 hitNormal) {
        hitPoint = default;
        hitNormal = default;
        if (camera == null || collider == null) {
            return false;
        }
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (collider.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity)) {
            hitPoint = raycastHit.point;
            hitNormal = raycastHit.normal.normalized;
            return true;
        }
        return false;
    }
}
