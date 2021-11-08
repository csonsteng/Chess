using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class King : PieceDefinition {
	public override string ID => "k";
	public override int Value => 99;
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
			if(Board.TryGetSquare(piece.Square.Location + direction, out var square)) {
				if(square.TryGetOccupant(out var occupant)) {
					if (occupant.IsAlly(piece)) {
						continue;
					}
				}
				moves.Add(new Move(piece, square));
			}
		}
		if(piece.moves.Count != 0) {
			return moves;
		}
		var castles = Pieces.CanCastle(piece.GetColor());

		if (castles.CanKingSideCastle) {
			var clearMoves = CheckExtendedDirection(piece, new Vector2Int(0, 1));

			if (clearMoves.Count() == 2) {
				var clearPath = true;
				foreach (var move in clearMoves) {
					if (move.WillPutSelfInCheck()) {
						clearPath = false;
						break;
					}
				}
				//can't castle through check.
				if (clearPath) {
					moves.Add(clearMoves.Last());
				}
			}

		}
		if (castles.CanQueenSideCastle) {
			var clearMoves = CheckExtendedDirection(piece, new Vector2Int(0, -1));
			if (clearMoves.Count() == 3) {
				var clearPath = true;
				foreach (var move in clearMoves) {
					if (move.WillPutSelfInCheck()) {
						clearPath = false;
						break;
					}
				}
				//can't castle through check.
				if (clearPath) {
					moves.Add(clearMoves.ElementAt(1));
				}
			}

		}

		return moves;
	}


}
