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
    
    public event EventHandler OnStateChanged;
    public State currentState;

    public void Start() {
        Instance = this;

        currentState = State.Default;
        pathfinding = new Pathfinding(10, 10);
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
}
