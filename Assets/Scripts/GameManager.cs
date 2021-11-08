using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject squareTemplate;
    public Transform pieceHolder;
    public PieceList pieces;
    public VisualAppearance appearance;


    private static string startingFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";


    private Player turn;
    private Status status;
    private Piece selected;
    private List<Move> availableMoves;
    private enum Status {
        TurnStart,
        PieceSelected,
        Processing,
        GameOver,
	}

    // Start is called before the first frame update
    void Start()
    {
        availableMoves = new List<Move>();
        var clickHandler = FindObjectOfType<ClickHandlerHandler>();
        clickHandler.OnPieceClick += PieceClicked;
        clickHandler.OnSquareClick += SquareClicked;
        Board.Generate(squareTemplate, appearance);
        Pieces.Generate(startingFEN, pieces, pieceHolder, appearance);
        turn = Pieces.White;
        StartTurn();
    }

    private void PieceClicked(Piece piece) {
        if(status != Status.Processing) {
            if (piece.GetColor() == turn.color) {
                SelectPiece(piece);
                return;
            }
        }

        if (status == Status.PieceSelected) {
            if (piece.GetColor() != turn.color) {
                SquareClicked(piece.Square);
            }
        }

	}

    private void SelectPiece(Piece piece) {
        if(status != Status.PieceSelected && status != Status.TurnStart) {
            return;
		}
        status = Status.Processing;
        selected = piece;
        availableMoves = piece.GetAvailableMoves();
        status = Status.PieceSelected;
    }

    private void SquareClicked(Square square) {
        if (status != Status.PieceSelected) {
            return;
		}
		if (!availableMoves.Contains(new Move(selected, square))) {
            return;
		}
        MoveSelected(square);
    }

    private async void MoveSelected(Square square) {
        status = Status.Processing;
        await selected.Move(square);
        NextTurn();
    }

    private void NextTurn() {
        Pieces.TurnComplete();
        SwitchActivePlayer();
        StartTurn();
	}

    private async void StartTurn() {
        if (Pieces.IsCheckMate(turn)) {
            Debug.Log("Checkmate");
            status = Status.GameOver;
            return;
        }

        if (Pieces.checkState.Check) {
            Debug.Log("Check");
        }

        if (turn.IsComputer) {
            await turn.ComputerTurn();
            NextTurn();
        } else {
            status = Status.TurnStart;
        }
	}

    private void SwitchActivePlayer() {
        turn = Pieces.OpposingPlayer(turn);
    }

}
