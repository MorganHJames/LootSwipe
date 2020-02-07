//\===========================================================================================
//\ File: Node.cs
//\ Author: Morgan James
//\ Brief: The maze is made up of nodes and so this class is used to save information about each node.
//\===========================================================================================

using UnityEngine;
using System.Collections.Generic;

public class Node
{
	public Vector3 m_v3WorldPosition;//The position of the node in world space.
	public int m_iWeight;//How important this node is.
	public List<Node> m_lAdjacentNodes;//Contains all of the node adjacent to this node.
	public int m_iAdjacentNodesOpened;//How many adjacent nodes have been check through.
	public bool m_bIsStart = false;//True if the node is the starting node.
	public bool m_bIsEnd = false;//True if the node is the final node.

	//Creates a node at the desired location and with the desired weight of importance.
	public Node(Vector3 a_v3WorldPosition, int a_iWeight)
	{
		m_v3WorldPosition = a_v3WorldPosition;//Sets the position of the node.
		m_iWeight = a_iWeight;//Sets the weight of the node.
	}
}
