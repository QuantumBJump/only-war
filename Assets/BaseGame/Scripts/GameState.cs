using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    private Pathfinding pathfinding;
    public PlacedObject selected;
    public enum State {
        Default,
        Selection,
        Selected,
        Building,
    }
    [SerializeField] public WallTypeSO[] wallTypes;

    
    public event EventHandler OnStateChanged;
    public State currentState;

    public int y_level;

    private void Awake() {
        Instance = this;
        currentState = State.Default;
    }

    public void Start() {
        Test3DGrid testGrid = Test3DGrid.Instance;
        pathfinding = new Pathfinding(testGrid.GetGrid());
        CameraHandler.Instance.SetLookLocation(pathfinding.GetGrid().GetGridObject(0, 0, 0).GetPathNode().GetCenter());
        OnStateChanged += StateChanged_Deselect;
    }

    public void SelectObject(PlacedObject selected) {
        this.selected = selected;
        Debug.Log("Now selected: " + this.selected);
        currentState = State.Selected;
    }

    private void StateChanged_Deselect(object sender, System.EventArgs e) {
        if (currentState != State.Selected) {
            selected = null;
        }
    }

    public void TriggerStateUpdate() {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public Pathfinding GetPathfinder() {
        return pathfinding;
    }
}
