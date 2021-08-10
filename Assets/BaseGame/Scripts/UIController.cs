using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private GameState state;
    private Text buildModeText;
    // Start is called before the first frame update
    void Start()
    {
        state = GameState.Instance;
        buildModeText = this.transform.Find("Build Mode").Find("Text").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        buildModeText.text = "Current mode: " + state.currentState.ToString();
    }
}
