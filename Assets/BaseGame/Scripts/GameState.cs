using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    public PlacedObject selected;
    public enum State {
        Default,
        Selection,
        Building,
    }
    
    public event EventHandler OnStateChanged;
    public State currentState;

    public void Start() {
        Instance = this;

        currentState = State.Default;
    }

    public void SelectObject(PlacedObject selected) {
        this.selected = selected;
        Debug.Log("Now selected: " + this.selected);
    }
}
