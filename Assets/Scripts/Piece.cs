using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class Piece:MonoBehaviour
{
	public readonly List<Square> moves = new List<Square>();
	private PieceDefinition definition;
	public string Type => definition.ID;
	public int Value => definition.Value;
	private Color color;

	public Square Square { get; private set; }

	public enum Color {
		White,
		Black
	}

	public string Name => definition.GetType().Name;
	public bool IsWhitePiece => color == Color.White;
	public void Configure(PieceDefinition definition, Color color, Square square) {
		this.definition = definition;
		this.color = color;
		Square = square;
		square.Occupy(this);
	}

	public List<Move> GetAvailableMoves(bool accountForCheck = true) {
		if (definition == null) {
			return null;
		}
		List<Move> availableMoves = new List<Move>();
		foreach (var move in definition.LegalMoves(this)) {
			//add the move if we aren't accounting for check, or if we're not in check
			//we need to avoid stack overflow
			if (!accountForCheck || !move.WillPutSelfInCheck()) {
				availableMoves.Add(move);
			}
		}
		return availableMoves;
	}


	public virtual void Capture() {
		Pieces.Capture(this);
		Destroy(gameObject);
	}

	public Color GetColor() => color;

	protected void SetColor(Color color) => this.color = color;

	public bool IsAlly(Piece otherPiece) => color == otherPiece.color;

	public async UniTask Move(Square destination) {
		ProcessEnPassantState(destination);
		Square.Deoccupy();
		var tween = gameObject.transform.DOMove(destination.gameObject.transform.position, 0.5f);
		await tween.AsyncWaitForCompletion();

		await ProcessCastling(destination);

		destination.Occupy(this);
		moves.Add(destination);
		Square = destination;
	}

	private async UniTask ProcessCastling(Square destination) {
		if (moves.Count > 0) {
			//if we've already moved before then we've already resolved this
			return;
		}

		if (definition.GetType() == typeof(King)) {
			Pieces.KingMoved(this);
			//if we only moved 1 file or less it isn't a castle
			if(Mathf.Abs(destination.File - Square.File) < 2) {
				return;
			}
			if(destination.File > Square.File) {
				if(Board.TryGetSquare(Square.Rank, 7, out var rookSquare)) {
					if(rookSquare.TryGetOccupant(out var rook)) {
						if (Board.TryGetSquare(Square.Rank, 5, out var rookDestination)) {
							await rook.Move(rookDestination);
						}
					}
				}
			} else {
				if (Board.TryGetSquare(Square.Rank, 0, out var rookSquare)) {
					if (rookSquare.TryGetOccupant(out var rook)) {
						if (Board.TryGetSquare(Square.Rank, 3, out var rookDestination)) {
							await rook .Move(rookDestination);
						}
					}
				}
			}


			return;
		}

		if (definition.GetType() != typeof(Rook)) {
			return;
		}

		if(Square.File == 0) {
			if(color == Color.Black) {
				Pieces.blackCanQueenSideCastle = false;
			} else {
				Pieces.whiteCanQueenSideCastle = false;
			}
			return;
		}
		if (color == Color.Black) {
			Pieces.blackCanKingSideCastle = false;
		} else {
			Pieces.whiteCanKingSideCastle = false;
		}
	}

	private void ProcessEnPassantState(Square destination) {
		if (definition.GetType() != typeof(Pawn)) {
			Pieces.SetEnPassantPawn(null);
			return;
		}

		if (Mathf.Abs(Square.Rank - destination.Rank) == 2) {
			Pieces.SetEnPassantPawn(this);
			return;
		}

		//if there is an en passant pawn
		if (Pieces.TryGetEnPassantPawn(out var pawn)) {
			//if we are next to en passant and moving to the same rank as the en passant
			if (Square.Rank == pawn.Square.Rank && destination.File == pawn.Square.File) {
				pawn.Square.Deoccupy();
				pawn.Capture();
			}

		}


		Pieces.SetEnPassantPawn(null);
	}

	public void ChangeSquare(Square square) {
		Square = square;
	}

}
