using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square: MonoBehaviour
{
    public int Rank { get; private set; }
    public int File { get; private set; }
    
    public string ID { get {
            return id;
		} 
    }
    public Vector2Int Location => GetLocation();
	private Vector2Int GetLocation() {
		return new Vector2Int(Rank, File);
	}

	protected string id;
    protected Color color;
    private Piece occupant;

    public enum Color {
        White,
        Black
	}

    public override string ToString() => ID;

	public void Configure(string id, int rank, int file, bool isBlackSquare) {
        this.id = id;
        Rank = rank;
        File = file;
        if (isBlackSquare) {
            color = Color.Black;
		} else {
            color = Color.White;
		}
        occupant = null;
	}

    public bool TryGetOccupant(out Piece occupant) {
        occupant = this.occupant;
        return occupant != null;
	}

    public void Occupy(Piece newOccupant) {
        if (occupant != null) {
            occupant.Capture();
        }
        occupant = newOccupant;
	}

    public void Deoccupy() {
        occupant = null;
	}

    public void ChangeOccupancy(Piece newOccupant) {
        occupant = newOccupant;
	}

    public int Distance(Square other) {
        var dRank = other.Rank - Rank;
        var dFile = other.File - File;
        return Mathf.Abs(dRank) + Mathf.Abs(dFile);
    }
}
