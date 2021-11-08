using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ClickHandlerHandler : MonoBehaviour, IPointerClickHandler {
	public delegate void PieceClicked(Piece piece);
	public delegate void SquareClicked(Square piece);
	public PieceClicked OnPieceClick;
	public SquareClicked OnSquareClick;
	public void OnPointerClick(PointerEventData eventData) {
		throw new System.NotImplementedException();
	}

	void Update() {
		var mouse = Mouse.current;
		if (mouse == null) {
			return;
		}
		if (!mouse.leftButton.wasPressedThisFrame) {
			return;
		}
		if (EventSystem.current.IsPointerOverGameObject()) {
			return;
		}
		var camera = Camera.main;
		Ray ray = camera.ScreenPointToRay(mouse.position.ReadValue());
		if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore)) {
			var square = hit.transform.GetComponent<Square>();
			if(square != null) {
				OnSquareClick(square);
				return;
			}
			var piece = hit.transform.GetComponentInParent<Piece>();
			if(piece != null) {
				OnPieceClick(piece);
			}
		}
	}
}
