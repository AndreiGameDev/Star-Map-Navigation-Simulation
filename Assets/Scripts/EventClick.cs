using UnityEngine;
using UnityEngine.EventSystems;

public class EventClick : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    RouteDefiner routeDefiner;
    Star star;
    private void Start() {
        routeDefiner = RouteDefiner.Instance;
        star = GetComponent<Star>();
    }
    public void OnPointerClick(PointerEventData eventData) {
        routeDefiner.EventClickAction(star);
    }
    public void OnPointerEnter(PointerEventData eventData) {
    }

    public void OnPointerExit(PointerEventData eventData) {
    }
}
