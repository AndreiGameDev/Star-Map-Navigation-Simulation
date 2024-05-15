using UnityEngine;
using UnityEngine.EventSystems;
/// <summary>
/// Click event for stars
/// </summary>
public class EventClick : MonoBehaviour, IPointerClickHandler {
    RouteDefiner routeDefiner;
    Star star;
    private void Start() {
        routeDefiner = RouteDefiner.Instance;
        star = GetComponent<Star>();
    }
    public void OnPointerClick(PointerEventData eventData) {
        routeDefiner.EventClickAction(star);
    }
}
