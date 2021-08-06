using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    private const float cameraSpeed = 50f;
    private const float verticalSpeed = 2f;
    private const float MIN_Y = 10f;
    private const float MAX_Y = 30f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f ) {
            this.transform.position += new Vector3(0, 1, 0) * verticalSpeed;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f ) {
            this.transform.position += new Vector3(0, -1, 0) * verticalSpeed;
        }
        Vector3 pos = this.transform.position;
        if (pos.y < MIN_Y) {
            float difference = MIN_Y - pos.y;
            this.transform.position += new Vector3(0, difference, 0);
        }
        if (pos.y > MAX_Y) {
            float difference = MAX_Y - pos.y;
            this.transform.position += new Vector3(0, difference, 0);
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            this.transform.position += new Vector3(-1, 0, 0) * cameraSpeed * Time.deltaTime;
        } 
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            this.transform.position += new Vector3(0, 0, 1) * cameraSpeed * Time.deltaTime;
        } 
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            this.transform.position += new Vector3(0, 0, -1) * cameraSpeed * Time.deltaTime;
        } 
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            this.transform.position += new Vector3(1, 0, 0) * cameraSpeed * Time.deltaTime;
        } 
    }
}
