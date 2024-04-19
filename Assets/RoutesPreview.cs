using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutesPreview : MonoBehaviour
{
    LineRenderer lineRenderer;
    private void Awake() {
        lineRenderer = GetComponent<LineRenderer>();
    }

}
