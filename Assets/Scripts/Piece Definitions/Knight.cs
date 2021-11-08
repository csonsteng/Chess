using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Knight : PieceDefinition {
	public override string ID => "n";

	public override int Value => 3;
	protected override List<Move> PotentialMoves(Piece piece) {
		var moves = new List<Move>();
		var directions = new List<Vector2Int>() {
			new Vector2Int(1, 2),
			new Vector2Int(-1, 2),
			new Vector2Int(1, -2),
			new Vector2Int(-1, -2),
			new Vector2Int(2, 1),
			new Vector2Int(-2, 1),
			new Vector2Int(2, -1),
			new Vector2Int(-2, -1),
		};
		foreach (var direction in directions) {
			if (Board.TryGetSquare(piece.Square.Location + direction, out var square)) {
				if (square.TryGetOccupant(out var occupant)) {
					if (occupant.IsAlly(piece)) {
						continue;
					}
				}
				moves.Add(new Move(piece, square));
			}
		}
		return moves;
	}


}
