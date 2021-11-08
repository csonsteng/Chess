using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Queen : PieceDefinition {
	public override string ID => "q";

	public override int Value => 9;
	protected override List<Move> PotentialMoves(Piece piece) {
		var moves = new List<Move>();
		var directions = new List<Vector2Int>() {
			new Vector2Int(1, 1),
			new Vector2Int(-1, 1),
			new Vector2Int(1, -1),
			new Vector2Int(-1, -1),
			new Vector2Int(1, 0),
			new Vector2Int(-1, 0),
			new Vector2Int(0, 1),
			new Vector2Int(0, -1),
		};
		foreach (var direction in directions) {
			moves.AddRange(CheckExtendedDirection(piece, direction));
		}
		return moves;
	}


}
