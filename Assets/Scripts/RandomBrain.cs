using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;

public class RandomBrain : Brain {
	public override async UniTask TakeTurn(Player player) {

		List<Move> potentialMoves = new List<Move>();

		var highestValue = -100;
		bool foundCheckMate = false;
		foreach (var piece in player.GetPieces().ToArray()) {
			foreach (var potentialMove in piece.GetAvailableMoves().ToArray()) {
				var startingValue = potentialMove.Value();
				potentialMove.Spoof();
				if (Pieces.IsCheckMate(player)) {
					potentialMoves.Clear();
					potentialMoves.Add(potentialMove);
					foundCheckMate = true;
					break;
				}
				var checks = Pieces.GetCheckState();
				if(player.color == Piece.Color.White && checks.BlackChecked) {
					potentialMove.causesCheck = true;
				}
				if (player.color == Piece.Color.Black && checks.WhiteChecked) {
					potentialMove.causesCheck = true;
				}
				var worstValue = 0;
				foreach (var opposing in Pieces.OpposingPlayer(player).GetPieces().ToArray()) {
					foreach (var secondMove in opposing.GetAvailableMoves().ToArray()) {
						var value = secondMove.Value();
						if (value > worstValue) {
							worstValue = value;
						}
					}
				}
				potentialMove.UnSpoof();
				potentialMove.deepValue = startingValue - worstValue;
				if(potentialMove.deepValue >= highestValue) {
					potentialMoves.Add(potentialMove);
					highestValue = potentialMove.deepValue;
				}
			}
			if (foundCheckMate) {
				break;
			}
		}
		bool anyChecks = false;
		if(highestValue > 0) {
			foreach(var potentialMove in potentialMoves.ToArray()) {
				if (potentialMove.deepValue < highestValue) {
					potentialMoves.Remove(potentialMove);
				} else if(potentialMove.causesCheck) {
					anyChecks = true;
				}
			}
		}

		if (anyChecks) {
			foreach(var potentialMove in potentialMoves.ToArray()) {
				if (!potentialMove.causesCheck) {
					potentialMoves.Remove(potentialMove);
				}
			}
		}
		var roll = Random.Range(0, potentialMoves.Count);
		var move = potentialMoves[roll];
		await move.Invoke();
	}


}
