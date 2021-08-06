using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private enum State {
        Default,
        Selection,
        Build,
    }

    private State currentState;

    private GridBuildingSystem gridBuilder;
    private GridMap<GridBuildingSystem.GridObject> grid;


    private void Start() {
        gridBuilder = GridBuildingSystem.Instance;
    }

    private void Update() {
        // Check states

        // Camera control
        // Movement
        if (Input.GetKey(KeyCode.A)) {
            CameraHandler.Instance.MoveLeft();
        }
        if (Input.GetKey(KeyCode.D)) {
            CameraHandler.Instance.MoveRight();
        }
        if (Input.GetKey(KeyCode.W)) {
            CameraHandler.Instance.MoveForward();
        }
        if (Input.GetKey(KeyCode.S)) {
            CameraHandler.Instance.MoveBackward();
        }
        // Vertical
        if (Input.GetAxis("Mouse ScrollWheel") > 0f ) {
            CameraHandler.Instance.MoveDown();
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f ) {
            CameraHandler.Instance.MoveUp();
        }

        // Building Mode
        if (GameState.Instance.currentState == GameState.State.Building) {
            // Leave building mode
            if (Input.GetKeyDown(KeyCode.Escape)) {
                GameState.Instance.currentState = GameState.State.Selection;
            }

            // Rotate building
            if (Input.GetKeyDown(KeyCode.R)) {
                gridBuilder.RotateBuilding();
            }

            // Choose building to place
            if (Input.GetKeyDown(KeyCode.Alpha1)) { gridBuilder.SelectBuilding(1); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { gridBuilder.SelectBuilding(2); }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { gridBuilder.SelectBuilding(3); }
            if (Input.GetKeyDown(KeyCode.Alpha4)) { gridBuilder.SelectBuilding(4); }

            if (Input.GetMouseButtonDown(0)) { gridBuilder.PlaceBuilding(); }
            if (Input.GetMouseButtonDown(1)) { gridBuilder.DemolishBuilding(); }
        }

        if (GameState.Instance.currentState == GameState.State.Selection) {
            if (Input.GetMouseButtonDown(0)) {
                PlacedObject selectObject = gridBuilder.GetPlacedObject();
                GameState.Instance.SelectObject(selectObject);
            }
        }

        // State changes
        if (Input.GetKeyDown(KeyCode.B)) {
            Debug.Log("Game state: Building");
            GameState.Instance.currentState = GameState.State.Building;
        }

    }
}
