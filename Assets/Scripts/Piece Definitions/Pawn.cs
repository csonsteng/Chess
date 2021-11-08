using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pawn : PieceDefinition {

	public override string ID => "p";
	public override int Value => 1;

	//MISSING: PAWN PROMOTIONS
	protected override List<Move> PotentialMoves(Piece piece) {
		var current = piece.Square;
		int sign = -1;
		if (piece.IsWhitePiece) {
			sign = 1;
		}

		List<Move> moves = new List<Move>();

		//unoccupied square in front of us
		if (Board.TryGetSquare(current.Rank+sign, current.File, out var square)){
			if(!square.TryGetOccupant(out _)) {
				moves.Add(new Move(piece, square));
			}
		}

		//unoccupied square two in front of us
		//move in front of us has to be eligible, and we can't have moved yet
		if (moves.Count == 1 && piece.moves.Count == 0) {
			if (Board.TryGetSquare(current.Rank+2*sign, current.File, out square)) {
				if (!square.TryGetOccupant(out _)) {
					moves.Add(new Move(piece, square));
				}
			}
		}

		//If the diagonals are occupied by enemies
		if (Board.TryGetSquare(current.Rank+sign, current.File + 1, out square)) {
			if (square.TryGetOccupant(out var occupant)) {
				if (!occupant.IsAlly(piece)) {
					moves.Add(new Move(piece, square));
				}
			}
		}

		if(Board.TryGetSquare(current.Rank+sign, current.File - 1, out square)) {
			if (square.TryGetOccupant(out var occupant)) {
				if (!occupant.IsAlly(piece)) {
					moves.Add(new Move(piece, square));
				}
			}
		}

		//if there is an en passant pawn the space behind them is available
		//the en passant pawn also needs to be next to us
		if(Pieces.TryGetEnPassantPawn(out var pawn)) {
			if (pawn.Square.Rank == current.Rank && Mathf.Abs(pawn.Square.File - current.File) == 1) {
				if (Board.TryGetSquare(pawn.Square.Rank + sign, pawn.Square.File, out square)) {
					moves.Add(new Move(piece, square));
				}
			}
		}
		return moves;

	}

}
