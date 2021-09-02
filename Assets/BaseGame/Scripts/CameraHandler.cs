using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{

    public static CameraHandler Instance { get; private set; }

    private Vector3 targetPos;

    private float targetRot;

    private int y_level;

    // Directions
    private Vector3 forward;
    private Vector3 right;
    private const float cameraSpeed = 50f;
    private const float verticalSpeed = 5f;
    private const float MIN_Y = 10f;
    private const float MAX_Y = 30f;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        targetPos = this.transform.position;
        UpdateDirections();
        UpdateYLevel();
    }

    void LateUpdate() {
        UpdateDirections();
        this.transform.position = Vector3.Lerp(this.transform.position, targetPos, Time.deltaTime * 15f);
        if (targetRot != 0) {
            float direction = Mathf.Sign(targetRot);
            Vector3 pivot = lookLocation();
            float rotateSpeed = Mathf.Min(360f * Time.deltaTime, Mathf.Abs(targetRot));
            float rotateVelocity = direction * rotateSpeed;
            this.transform.RotateAround(pivot, Vector3.down, rotateVelocity);
            targetPos = this.transform.position;
            targetRot -= rotateVelocity;
        }
    }
    public void MoveLeft() {
        targetPos += right * -1f * cameraSpeed * Time.deltaTime;
    }

    public void MoveRight() {
        targetPos += right * cameraSpeed * Time.deltaTime;
    }
    public void MoveForward() {
        targetPos += forward * cameraSpeed * Time.deltaTime;
    }
    public void MoveBackward() {
        targetPos += forward * -1f * cameraSpeed * Time.deltaTime;
    }

    public void MoveUp() {
        targetPos += new Vector3(0, 1, 0) * verticalSpeed;
        if (targetPos.y > MAX_Y) {
            float difference = MAX_Y - targetPos.y;
            targetPos += new Vector3(0, difference, 0);
        }
        UpdateYLevel();
    }

    public void MoveDown() {
        targetPos += new Vector3(0, -1, 0) * verticalSpeed;
        if (targetPos.y < MIN_Y) {
            float difference = MIN_Y - targetPos.y;
            targetPos += new Vector3(0, difference, 0);
        }
        UpdateYLevel();
    }

    public void RotateRight() {
        targetRot += 45f;
    }

    public void RotateLeft() {
        targetRot -= 45f;
    }

    private void UpdateDirections() {
        Vector3 f = this.transform.forward;
        Vector3 r = this.transform.right;
        f.y = 0f;
        r.y = 0f;
        f.Normalize();
        r.Normalize();
        forward = f;
        right = r;
    }

    public void SetLookLocation(Vector3 target) {
        Vector3 currentOffset = lookLocation() - this.transform.position;
        targetPos = target - currentOffset;
    }

    private Vector3 lookLocation() {
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 999f, Utils.floorLayerMask)) {
            return hitInfo.point;
        }
        return Vector3.zero;
    }

    private void UpdateYLevel() {
        y_level = (int)(this.targetPos.y - 10f + 0.5f);
        if (y_level < 0) {
            y_level = 0;
        }
        if (y_level > 15) { y_level = 15; }
    }

    public int GetYLevel() {
        return this.y_level;
    }

}