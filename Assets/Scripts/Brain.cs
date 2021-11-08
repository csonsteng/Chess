using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public abstract class Brain
{
	public abstract UniTask TakeTurn(Player team);
}
