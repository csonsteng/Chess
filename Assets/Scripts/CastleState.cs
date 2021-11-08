using System;

public class CastleState: Tuple<bool, bool> {

	public bool CanKingSideCastle => Item1;
	public bool CanQueenSideCastle => Item2;

	public CastleState(bool canKingSideCastle, bool canQueenSideCastle) : base(canKingSideCastle, canKingSideCastle) {

	}
}
