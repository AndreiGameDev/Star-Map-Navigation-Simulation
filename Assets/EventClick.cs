using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventClick : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    RouteDefiner routeDefiner;
    Star star;
    private void Awake() {
        routeDefiner = RouteDefiner.Instance;
        star = GetComponent<Star>();
    }
    public void OnPointerClick(PointerEventData eventData) {
        routeDefiner.EventClickAction(star);
        Debug.Log(gameObject.name + " Has been clicked.");
    }
    public void OnPointerEnter(PointerEventData eventData) {
        Debug.Log("Hovering " + gameObject.name + ".");
    }

    public void OnPointerExit(PointerEventData eventData) {
        Debug.Log("Stopped hovering " + gameObject.name + ".");
    }
}
