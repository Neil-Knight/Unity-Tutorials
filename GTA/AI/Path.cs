using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PathDisplayMode { None, Connections, Paths }

public class Path : MonoBehaviour
{
	[HideInInspector]
	public PathDisplayMode DisplayMode = PathDisplayMode.Connections;
	[HideInInspector]
	public int UIStart = 0;
	[HideInInspector]
	public int UIEnd = 0;

	public List<Transform> nodes = new List<Transform>();
}
