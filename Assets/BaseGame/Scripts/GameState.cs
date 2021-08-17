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

    private void Awake() {
        Instance = this;
    }

    public void Start() {
        currentState = State.Default;
        pathfinding = new Pathfinding(10, 10);
        Test3DGrid testGrid = new Test3DGrid(10, 3, 10);
        CameraHandler.Instance.SetLookLocation(pathfinding.GetGrid().GetGridObject(0, 0).GetCenter());
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
