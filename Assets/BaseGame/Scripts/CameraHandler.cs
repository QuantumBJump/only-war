using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{

    public static CameraHandler Instance { get; private set; }
    private const float cameraSpeed = 50f;
    private const float verticalSpeed = 2f;
    private const float MIN_Y = 10f;
    private const float MAX_Y = 30f;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }
    public void MoveLeft() {
        this.transform.position += new Vector3(-1, 0, 0) * cameraSpeed * Time.deltaTime;
    }

    public void MoveRight() {
        this.transform.position += new Vector3(1, 0, 0) * cameraSpeed * Time.deltaTime;
    }
    public void MoveForward() {
        this.transform.position += new Vector3(0, 0, 1) * cameraSpeed * Time.deltaTime;
    }
    public void MoveBackward() {
        this.transform.position += new Vector3(0, 0, -1) * cameraSpeed * Time.deltaTime;
    }

    public void MoveUp() {
        this.transform.position += new Vector3(0, 1, 0) * verticalSpeed;
        Vector3 pos = this.transform.position;
        if (pos.y > MAX_Y) {
            float difference = MAX_Y - pos.y;
            this.transform.position += new Vector3(0, difference, 0);
        }
    }

    public void MoveDown() {
        this.transform.position += new Vector3(0, -1, 0) * verticalSpeed;
        Vector3 pos = this.transform.position;
        if (pos.y < MIN_Y) {
            float difference = MIN_Y - pos.y;
            this.transform.position += new Vector3(0, difference, 0);
        }
    }
}
