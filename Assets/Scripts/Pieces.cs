using UnityEngine;
using Object = UnityEngine.Object;

public static class Pieces
{
	private static Player black;
	private static Player white;

	public static Player Black => black;
	public static Player White => white;
	//private static List<Piece> pieces = new List<Piece>();
	//private static Piece whiteKing;
	//private static Piece blackKing;

	private static Piece lastPawn;

	public static bool blackCanQueenSideCastle = true;
	public static bool blackCanKingSideCastle = true;
	public static bool whiteCanQueenSideCastle = true;
	public static bool whiteCanKingSideCastle = true;

	public static CheckState checkState;

	public static void Capture(Piece piece) {
		if(piece.GetColor() == Piece.Color.White) {
			white.RemovePiece(piece);
		} else {
			black.RemovePiece(piece);
		}
	}

	public static bool TryGetEnPassantPawn(out Piece pawn) {
		pawn = lastPawn;
		return pawn != null;
	}

	public static void SetEnPassantPawn(Piece pawn) {
		lastPawn = pawn;
	}

	public static bool InCheck(Piece.Color color) {
		return color == Piece.Color.Black && checkState.BlackChecked || 
			color == Piece.Color.White && checkState.WhiteChecked;
	}

	public static void KingMoved(Piece king) {
		if(king == white.king) {
			whiteCanKingSideCastle = false;
			whiteCanQueenSideCastle = false;
		}
		if (king == black.king) {
			blackCanKingSideCastle = false;
			blackCanQueenSideCastle = false;
		}
	}

	public static CastleState CanCastle(Piece.Color color) {
		CastleState castles = new CastleState(false, false);
		if (InCheck(color)) {
			return castles;
		}

		switch (color) {
			case Piece.Color.Black:
				castles = new CastleState(blackCanKingSideCastle, blackCanQueenSideCastle);
				break;
			case Piece.Color.White:
				castles = new CastleState(whiteCanKingSideCastle, whiteCanQueenSideCastle);
				break;
		}
		return castles;
	}

	public static bool WillPutSelfInCheck(Piece piece, Square potentialMove) {
		//since castling does it's own check, i don't need to spoof "castling"


		//Temporarily move piece and "capture" existing
		if(potentialMove.TryGetOccupant(out var captured)) {
			Capture(captured);
		}

		var lastPosition = piece.Square;
		lastPosition.ChangeOccupancy(null);
		piece.ChangeSquare(potentialMove);
		piece.moves.Add(potentialMove);
		potentialMove.ChangeOccupancy(piece);

		var checkState = GetCheckState();

		//Restore board state before returning results
		piece.ChangeSquare(lastPosition);
		piece.moves.Remove(potentialMove);

		lastPosition.ChangeOccupancy(piece);
		potentialMove.ChangeOccupancy(captured);
		if(captured != null) {
			if (captured.IsWhitePiece) {
				white.AddPiece(captured);
			} else {
				black.AddPiece(captured);
			}
		}

		if (piece.GetColor() == Piece.Color.White && checkState.WhiteChecked) {
			return true;
		}
		if (piece.GetColor() == Piece.Color.Black && checkState.BlackChecked) {
			return true;
		}
		return false;
	}
	public static void TurnComplete() {
		checkState = GetCheckState();
	}
	public static CheckState GetCheckState() {

		if(white == null || white.king == null || white.king.Square == null) {
			throw new System.Exception($"White king something is null {white},{white.king},{white.king.Square}");
		}
		if (black == null || black.king == null || black.king.Square == null) {
			throw new System.Exception($"Black king something is null {black},{black.king},{black.king.Square}");
		}

		//we can ignore kings other than this
		if (white.king.Square.Distance(black.king.Square) <= 1) {
			return new CheckState(true, true);
		}

		bool whiteInCheck = false;
		bool blackInCheck = false;

		foreach (var piece in white.GetPieces()) {
			if(piece == white.king) {
				continue;
			}
			var availableMoves = piece.GetAvailableMoves(false);
			foreach (var move in availableMoves) {
				if (move.square.ID == black.king.Square.ID) {
					blackInCheck = true;
				}
			}
		}

		foreach (var piece in black.GetPieces()) {
			if (piece == black.king) {
				continue;
			}
			var availableMoves = piece.GetAvailableMoves(false);
			foreach (var move in availableMoves) {
				if (move.square.ID == white.king.Square.ID) {
					whiteInCheck = true;
				}
			}
		}
		return new CheckState(blackInCheck, whiteInCheck);
	}

	public static bool IsCheckMate(Player currentTurn) {
		//as long as we have one available move it isn't mate
		foreach(var piece in currentTurn.GetPieces()) {
			if(piece.GetAvailableMoves().Count > 0) {
				return false;
			}
		}
		return true;
	}

	public static Player OpposingPlayer(Player player) {
		if (player == Black) {
			return White;
		}
		if (player == White) {
			return Black;
		}
		throw new System.Exception($"Invalid player: {player}");
	}

	public static void Generate(string FEN, PieceList pieceList, Transform pieceHolder, VisualAppearance appearance) {
		white = new Player(Piece.Color.White);
		black = new Player(Piece.Color.Black, new RandomBrain());


		var rank = 0;
		var file = 0;
		lastPawn = null;    
		blackCanQueenSideCastle = true;
		blackCanKingSideCastle = true;
		whiteCanQueenSideCastle = true;
		whiteCanKingSideCastle = true;
		foreach (var character in FEN) {
			if (character == ' ') {
				break;
			}
			if(character == '/') {
				file = 0;
				rank++;
				continue;
			}
			if (int.TryParse(character.ToString(), out var blanks)) {
				file += blanks;
			} else {
				if (pieceList.TryGetPiece(character, out var piece)) {
					var team = white;
					if(character.ToString() == character.ToString().ToUpper()) {
						team = black;
					}
					if (Board.TryGetSquare(rank, file, out var square)) {
						SpawnPiece(piece, team, square, pieceHolder, appearance);
					}

				}
				file++;
			}
		}

	}

	private static void SpawnPiece(PieceDefinition definition, Player player, Square square, Transform transform, VisualAppearance appearance) {
		var pieceObject = Object.Instantiate(definition.prefab, transform);

		pieceObject.transform.position = square.transform.position;

		var renderers = pieceObject.GetComponentsInChildren<MeshRenderer>();
		foreach (var renderer in renderers) {
			if (player.color == Piece.Color.Black) {
				renderer.material = appearance.blackPieceMaterial;
			} else {
				renderer.material = appearance.whitePieceMaterial;
			}
		}
		var piece = pieceObject.AddComponent<Piece>();
		piece.Configure(definition, player.color, square);
		player.AddPiece(piece);
	}
}
