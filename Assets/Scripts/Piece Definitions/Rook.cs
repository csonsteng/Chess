using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rook : PieceDefinition {

	public override string ID => "r";
	public override int Value => 5;
	protected override List<Move> PotentialMoves(Piece piece) {
		var moves = new List<Move>();
		var directions = new List<Vector2Int>() {
			new Vector2Int(1, 0),
			new Vector2Int(-1, 0),
			new Vector2Int(0, 1),
			new Vector2Int(0, -1),
		};
		foreach(var direction in directions) {
			moves.AddRange(CheckExtendedDirection(piece, direction));
		}
		return moves;
	}


}
