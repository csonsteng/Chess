using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class PieceDefinition
{
	public GameObject prefab;
	public override string ToString() => GetType().ToString();
	protected abstract List<Move> PotentialMoves(Piece piece);
	public abstract string ID { get; }
	public abstract int Value { get; }
	public virtual void OnMoveFinished(Piece piece) { }

	public List<Move> LegalMoves(Piece piece) => PotentialMoves(piece);

	protected IEnumerable<Move> CheckExtendedDirection(Piece piece, Vector2Int direction) {
		var currentLocation = piece.Square.Location;
		while (true) {
			currentLocation += direction;
			//break if no square found
			if (!Board.TryGetSquare(currentLocation, out var currentSquare)) {
				break;
			}
			//break if occupied
			if (currentSquare.TryGetOccupant(out var occupant)) {
				if (!occupant.IsAlly(piece)) {
					yield return new Move(piece, currentSquare); //if it's not an ally though it is a valid play
				}
				break;
			}
			yield return new Move(piece, currentSquare);
		}
	}
}
