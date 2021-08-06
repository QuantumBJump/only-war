using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }
    public enum State {
        Default,
        Building,
    }
    
    public event EventHandler OnStateChanged;
    public State currentState;

    public void Start() {
        Instance = this;

        currentState = State.Default;
    }
}
