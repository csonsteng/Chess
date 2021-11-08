public struct CheckState {
	private bool black;
	private bool white;
	public CheckState(bool blackInCheck, bool whiteInCheck) {
		black = blackInCheck;
		white = whiteInCheck;
	}

	public bool Check => black || white;
	public bool BlackChecked => black;
	public bool WhiteChecked => white;
}
