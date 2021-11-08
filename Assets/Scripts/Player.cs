using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

[System.Serializable]
public class Player {
	public Piece.Color color;

	private Brain brain;
	
	public Piece king;
	public List<Piece> queens = new List<Piece>();
	public List<Piece> rooks = new List<Piece>();
	public List<Piece> bishops = new List<Piece>();
	public List<Piece> knights = new List<Piece>();
	public List<Piece> pawns = new List<Piece>();

	public bool IsComputer => brain != null;

	public Player(Piece.Color color) {
		this.color = color;
		brain = null;
	}

	public Player(Piece.Color color, Brain brain) {
		this.color = color;
		this.brain = brain;
	}

	public async UniTask ComputerTurn() {
		if (!IsComputer) {
			throw new Exception("Cannot take turn without a brain :P");
		}

		await brain.TakeTurn(this);
	}

	public Player Copy() {
		var copy = new Player(color);
		foreach(var piece in GetPieces()) {
			copy.AddPiece(piece);
		}
		return copy;
	}

	public IEnumerable<Piece> GetPieces() {
		yield return king; 
		foreach (var piece in queens) {
			yield return piece;
		}
		foreach (var piece in rooks) {
			yield return piece;
		}
		foreach (var piece in bishops) {
			yield return piece;
		}
		foreach (var piece in knights) {
			yield return piece;
		}
		foreach (var piece in pawns) {
			yield return piece;
		}
	}

	public void AddPiece(Piece piece) {
		if(piece.GetColor() != color) {
			return;
		}
		if(piece.Type == "k") {
			king = piece;
		} else {
			FindList(piece.Type).Add(piece);
		}
	}

	public void RemovePiece(Piece piece) {
		if (piece.GetColor() != color) {
			return;
		}
		if (piece.Type == "k") {
			king = null;
		} else {
			FindList(piece.Type).Remove(piece);
		}
	}

	private List<Piece> FindList(string type){
		switch (type) {
			case "q":
				return queens;
			case "r":
				return rooks;
			case "b":
				return bishops;
			case "n":
				return knights;
			case "p":
				return pawns;
			default:
				throw new Exception("Unmanaged piece type");
		}
	}
}
