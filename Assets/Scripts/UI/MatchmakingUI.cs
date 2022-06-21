using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchmakingUI : MonoBehaviour
{
    private Canvas canvas;

    void Start() {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
    }

    public void StartMatchmaking() {
        if (canvas.enabled == false) {
            canvas.enabled = true;
        }
    }
}
