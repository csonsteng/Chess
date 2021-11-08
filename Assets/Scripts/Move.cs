using Cysharp.Threading.Tasks;
public class Move {
	public Piece piece;
	public Square square;
	private int value = -1;

	public int deepValue = 0;
	public bool causesCheck = false;

	public override int GetHashCode() {
		return piece.GetHashCode()+square.GetHashCode();
	}

	public override bool Equals(object obj) {
		if(obj.GetType() != typeof(Move)) {
			return false;
		}
		var other = (Move)obj;
		if(other.piece == piece && other.square == square) {
			return true;
		}
		return false;
	}

	public Move(Piece piece, Square square) {
		this.piece = piece;
		this.square = square;
	}

	public async UniTask Invoke() {
		await piece.Move(square);
	}

	public int Value() {
		if(value > -1) {
			return value;
		}
		if (square.TryGetOccupant(out var occupant)) {
			value = occupant.Value;
		} else {
			value = 0;
		}
		return value;
	}

	public bool WillPutSelfInCheck() {
		Spoof();
		var checkState = Pieces.GetCheckState();
		UnSpoof();

		if (piece.GetColor() == Piece.Color.White && checkState.WhiteChecked) {
			return true;
		}
		if (piece.GetColor() == Piece.Color.Black && checkState.BlackChecked) {
			return true;
		}
		return false;
	}

	private Piece captured;
	private Square lastPosition;
	public void Spoof() {
		//Temporarily move piece and "capture" existing
		if (square.TryGetOccupant(out captured)) {
			Pieces.Capture(captured);
		}

		lastPosition = piece.Square;
		lastPosition.ChangeOccupancy(null);
		piece.ChangeSquare(square);
		piece.moves.Add(square);
		square.ChangeOccupancy(piece);
	}

	public void UnSpoof() {
		piece.ChangeSquare(lastPosition);
		piece.moves.Remove(square);

		lastPosition.ChangeOccupancy(piece);
		square.ChangeOccupancy(captured);
		if (captured != null) {
			if (captured.IsWhitePiece) {
				Pieces.White.AddPiece(captured);
			} else {
				Pieces.Black.AddPiece(captured);
			}
		}
	}

}

