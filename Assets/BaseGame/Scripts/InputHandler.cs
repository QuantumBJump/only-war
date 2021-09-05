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

    private void Start() {
        gridBuilder = GridBuildingSystem.Instance;
    }

    private void Update() {
        // Check states
        if (Input.GetKeyDown(KeyCode.B)) {
            Debug.Log("Game state: Building");
            GameState.Instance.currentState = GameState.State.Building;
            GameState.Instance.TriggerStateUpdate();
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            Debug.Log("Game state: Selection");
            GameState.Instance.currentState = GameState.State.Selection;
            GameState.Instance.TriggerStateUpdate();
        }

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
        // Rotate camera
        if (Input.GetKeyDown(KeyCode.Q)) {
            CameraHandler.Instance.RotateLeft();
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            CameraHandler.Instance.RotateRight();
        }

        // Building Mode
        if (GameState.Instance.currentState == GameState.State.Building) {

            if (Input.GetKeyDown(KeyCode.Tab)) {
                GridBuildingSystem.Instance.NextPlacementMode();
            }
            // Leave building mode
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Debug.Log("Game mode: Default");
                GameState.Instance.currentState = GameState.State.Default;
                GameState.Instance.TriggerStateUpdate();
            }

            // Rotate building
            if (Input.GetKeyDown(KeyCode.R)) {
                gridBuilder.RotateObject();
            }

            // Choose building to place
            if (Input.GetKeyDown(KeyCode.Alpha1)) { gridBuilder.SelectObject(1); }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { gridBuilder.SelectObject(2); }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { gridBuilder.SelectObject(3); }
            if (Input.GetKeyDown(KeyCode.Alpha4)) { gridBuilder.SelectObject(4); }
            if (Input.GetKeyDown(KeyCode.Alpha5)) { gridBuilder.SelectObject(5); }

            if (Input.GetMouseButtonDown(0)) { gridBuilder.PlaceObject(); }
            if (Input.GetMouseButtonDown(1)) { gridBuilder.DemolishBuilding(); }
        }

        if (GameState.Instance.currentState == GameState.State.Selection) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Debug.Log("Game mode: Default");
                GameState.Instance.currentState = GameState.State.Default;
                GameState.Instance.TriggerStateUpdate();
            }

            if (Input.GetMouseButtonDown(0)) {
                IPlaceable selectObject = gridBuilder.GetPlacedObject();
                if (selectObject == null) { return; }
                if (selectObject.GetType() == typeof(PlacedObject)) {
                    GameState.Instance.SelectObject((PlacedObject)selectObject);
                }
            }
        }

        if (GameState.Instance.currentState == GameState.State.Selected) {
            if (GameState.Instance.selected == null) {
                GameState.Instance.currentState = GameState.State.Selection;
            } else {
                PlacedObject selected = GameState.Instance.selected;
                if (Input.GetKeyDown(KeyCode.Escape)) {
                    Debug.Log("Game mode: Default");
                    GameState.Instance.currentState = GameState.State.Default;
                    GameState.Instance.TriggerStateUpdate();
                }

                if (Input.GetMouseButtonDown(0)) {
                    Vector3 mousePos = Utils.GetMouseWorldPositionWithCollider(Utils.floorLayerMask);
                    Pathfinding.Instance.GetGrid().GetXYZ(mousePos, out int endX, out int endY, out int endZ);
                    selected.GetOrigin(out int startX, out int startZ);
                    List<PathNode> path = Pathfinding.Instance.FindPath(startX, 0, startZ, endX, endY, endZ);
                    if (path != null) {
                        foreach (PathNode node in path) {
                            Debug.Log("Next node: (" + node.x + "," + node.y + "," + node.z + ")");
                        }
                        for (int i = 0; i < path.Count-1; i++) {
                            Debug.DrawLine(
                                path[i].GetCenter(),
                                path[i+1].GetCenter(),
                                Color.blue,
                                3f
                            );
                        }
                    }
                }
            }
        }

    }
}
