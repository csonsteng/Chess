using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceList : ScriptableObject
{
	public Pawn pawn;
	public Rook rook;
	public Knight knight;
	public Bishop bishop;
	public Queen queen;
	public King king;


	public IEnumerable<PieceDefinition> Pieces() {
		yield return pawn;
		yield return rook;
		yield return knight;
		yield return bishop;
		yield return queen;
		yield return king;
	}

	public bool TryGetPiece(char id, out PieceDefinition pieceDefinition) {
		foreach(var definition in Pieces()) {
			if(definition.ID.ToUpper() == id.ToString().ToUpper()) {
				pieceDefinition = definition;
				return true;
			}
		}
		pieceDefinition = null;
		return false;

	}
}
