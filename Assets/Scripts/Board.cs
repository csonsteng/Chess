using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Board 
{
	//public static Dictionary<string, Square> squareDict = new Dictionary<string, Square>();

	private static Square[,] squares;

	public static readonly string[] files = { "A", "B", "C", "D", "E", "F", "G", "H" };
	public static readonly int size = 8;

	//public static bool TryGetSquare(string id, out Square square) => squareDict.TryGetValue(id, out square);
	public static bool TryGetSquare(int rank, int file, out Square square) {
		var max = Mathf.Max(rank, file);
		var min = Mathf.Min(rank, file);
		if(max > size -1 || min < 0) {
			square = null;
			return false;
		}
		square = squares[rank, file];
		return true;
	}

	public static bool TryGetSquare(Vector2Int location, out Square square) => TryGetSquare(location.x, location.y, out square);

	private static string SquareID(int rank, int file) {
		if(file < 0 || file >= files.Length) {
			return "-1";
		}
		var fileName = files[file];
		return $"{fileName}{rank + 1}";
	}

	public static void Generate(GameObject squareTemplate, VisualAppearance appearance) {
		squares = new Square[size, size];
		//squareDict.Clear();
		squareTemplate.SetActive(false);
		bool isBlackSquare = true;
		for (var rank = 0; rank < size; rank++) {
			for (var file = 0; file < size; file++) {
				var squareObject = Object.Instantiate(squareTemplate, squareTemplate.transform.parent);
				squareObject.SetActive(true);

				var renderer = squareObject.GetComponent<MeshRenderer>();
				if (isBlackSquare) {
					renderer.material = appearance.blackSquareMaterial;
				} else {
					renderer.material = appearance.whiteSquareMaterial;
				}

				squareObject.transform.position = new Vector3(file, rank, 0f);

				var id = SquareID(rank, file);
				squareObject.name = id;

				var square = squareObject.AddComponent<Square>();
				square.Configure(id, rank, file, isBlackSquare);
				//squareDict.Add(id, square);
				squares[rank, file] = square;
				isBlackSquare = !isBlackSquare;
			}
			isBlackSquare = !isBlackSquare;
		}
	}
}
