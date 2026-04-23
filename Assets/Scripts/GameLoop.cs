using UnityEngine;

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

        if (TryGetMouseHitPoint(this.MainCamera, this.TerrainCollider, out Vector3 hitPoint)) {
            this.Car.transform.position = hitPoint;
        }
    }

    // hitPoint is in world space
    public static bool TryGetMouseHitPoint(Camera camera, Collider collider, out Vector3 hitPoint) {
        hitPoint = default;
        if (camera == null || collider == null) {
            return false;
        }
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (collider.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) {
            hitPoint = hit.point;
            return true;
        }
        return false;
    }
}
