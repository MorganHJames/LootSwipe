//\===========================================================================================
//\ File: PrimsMazeGenerator.cs
//\ Author: Morgan James
//\ Brief: Spawns a maze made from predetermined prefabs using prim's algorithm.
//\===========================================================================================

using System.Collections.Generic;
using UnityEngine;

public class PrimsMazeGenerator : MonoBehaviour
{
	private Vector2 m_v2GridWorldSize = new Vector2(10, 10);//How large the maze is.

	[HideInInspector]
	public Node[,] m_nGrid;//A 2d array of nodes.

	[HideInInspector]
	public int m_iGridSizeX, m_iGridSizeY;//How many nodes there are in each direction.

	private float m_fNodeRadius = 0.5f;//How large each node's radius is.
	private float m_fNodeDiameter;//How large each node's diameter is.
	private List<GameObject> m_lMazePieces;//All of the spawned game objects.
	private Camera m_cMainCamera;//The camera.
	private float m_fScreenOffset = 1.0f;//How far the move the camera up after fitting the maze to the screen to account for the player and the height of the nodes.
	private Vector3 m_v3WorldBottomLeft;//The bottom left of the maze.

	[HideInInspector]
	public List<Node> m_lSetNodes;//All of the nodes that are set (have a walkable path).

	private List<List<Node>> m_llAdjacentSetNodes;//A list of all adjacent nodes for every node.

	[SerializeField]
	private bool m_bGizmos = false;//True to see node outlines and maze outline.

	[SerializeField]
	private GameObject m_gPlayerPrefab;//The player prefab that will spawn.
	[SerializeField]
	private GameObject m_gCrate;//The crate prefab that will be placed on all set nodes.
	[SerializeField]
	private GameObject m_gIsland;//The island that will spawn at the end of the maze.
	[SerializeField]
	private GameObject m_gShipWreck;//The ship wreck that will spawn at the start of the maze with the player atop of it.

	//Simple Singleton pattern.
	public static PrimsMazeGenerator instance;//The Singleton instance.

	private void Awake()
	{
		//Make sure there is only one maze generator.
		if (instance == null)
			instance = this;
	}

	private void OnEnable()
	{
		m_cMainCamera = Camera.main;//Set the camera.
		m_lMazePieces = new List<GameObject>();//Create a new list of maze pieces.
		m_fNodeDiameter = m_fNodeRadius * 2;//Set the node diameter.
		Generate();//Generate a new maze.
	}

	private void Update()
	{
		//If the screen changes size readjust the camera to fit the maze within it.
		if (m_cMainCamera.WorldToViewportPoint(m_v3WorldBottomLeft).x < 0.0f 
			|| m_cMainCamera.WorldToViewportPoint(m_v3WorldBottomLeft).y < 0.0f
			|| m_cMainCamera.WorldToViewportPoint(m_v3WorldBottomLeft).x > 0.01f
			|| m_cMainCamera.WorldToViewportPoint(m_v3WorldBottomLeft).y > 0.01f)
		{
			MoveCamera();
		}
	}

	public void Generate()
	{
		DestroyMazePieces();//Destroy all of the previous maze pieces.

		m_lMazePieces = new List<GameObject>();//Create a new list of maze pieces.

		m_iGridSizeX = Mathf.RoundToInt(m_v2GridWorldSize.x / m_fNodeDiameter);//Set how many nodes there are horizontally across the screen.
		m_iGridSizeY = Mathf.RoundToInt(m_v2GridWorldSize.y / m_fNodeDiameter);//Set how many nodes there are vertically across the screen.

		//Get the position of the bottom left node.
		m_v3WorldBottomLeft = transform.position - Vector3.right * m_v2GridWorldSize.x / 2 - Vector3.forward * m_v2GridWorldSize.y / 2;

		CreateGrid();//Create a grid.
		SetStartingNode(m_nGrid[Random.Range(0, m_iGridSizeX), Random.Range(0, m_iGridSizeY)]);//Set the starting node.
		FindNext();//Create the maze.

		MoveCamera();//Fix the camera.
	}

	private void CreateGrid()
	{
		m_nGrid = new Node[m_iGridSizeX, m_iGridSizeY];//Creates a new node grid.

		//For every point on the grid.
		for (int x = 0; x < m_iGridSizeX; ++x)
		{
			for (int y = 0; y < m_iGridSizeY; ++y)
			{
				//Find out the world point of that point.
				Vector3 v3WorldPoint = m_v3WorldBottomLeft + Vector3.right * (x * m_fNodeDiameter + m_fNodeRadius) + Vector3.forward * (y * m_fNodeDiameter + m_fNodeRadius);

				//Create the node with that world point and a random weight.
				m_nGrid[x, y] = new Node(v3WorldPoint, RandomDigit());
			}
		}
		SetAdjacentNodes();//Set all of the adjacent nodes for each node.
	}

	private void SetAdjacentNodes()
	{
		//Go through all the nodes.
		for (int x = 0; x < m_iGridSizeX; ++x)
		{
			for (int y = 0; y < m_iGridSizeY; ++y)
			{
				m_nGrid[x, y].m_lAdjacentNodes = new List<Node>();

				//If there is a Node to the left add it to the adjacent list.
				if (x - 1 >= 0)
				{
					m_nGrid[x, y].m_lAdjacentNodes.Add(m_nGrid[x - 1, y]);
				}

				//If there is a Node to the right add it to the adjacent list.
				if (x + 1 < m_iGridSizeX)
				{
					m_nGrid[x, y].m_lAdjacentNodes.Add(m_nGrid[x + 1, y]);
				}

				//If there is a Node behind add it to the adjacent list.
				if (y - 1 >= 0)
				{
					m_nGrid[x, y].m_lAdjacentNodes.Add(m_nGrid[x, y - 1]);
				}

				//If there is a Node in front add it to the adjacent list.
				if (y + 1 < m_iGridSizeY)
				{
					m_nGrid[x, y].m_lAdjacentNodes.Add(m_nGrid[x, y + 1]);
				}

				//Order the list
				m_nGrid[x, y].m_lAdjacentNodes.Sort(SortByLowestWeight);
			}
		}
	}

	//The sorting method for nodes based on weight.
	private int SortByLowestWeight(Node a_nNodeA, Node a_nNodeB)
	{
		int a = a_nNodeA.m_iWeight;//Set a to be equal to nodeA's weight.
		int b = a_nNodeB.m_iWeight;//Set a to be equal to nodeB's weight.

		return a.CompareTo(b);//Compare a and b against one another.
	}

	private int RandomDigit()
	{
		return Random.Range(0, 9);//Return a random digit.
	}

	private void SetStartingNode(Node a_nStartingNode)
	{
		//Create a new list for the set nodes.
		m_lSetNodes = new List<Node>();

		//Create a new list of lists for the set adjacent nodes.
		m_llAdjacentSetNodes = new List<List<Node>>();

		for (int i = 0; i < 10; ++i)
		{
			m_llAdjacentSetNodes.Add(new List<Node>());//Initialize the adjacent set nodes.
		}

		AddToSetNodes(a_nStartingNode);//Add the starting node to the set nodes.

		a_nStartingNode.m_bIsStart = true;//Indicate that the node is the starting node.
	}

	private void AddToSetNodes(Node a_nNodeToSet)
	{
		m_lSetNodes.Add(a_nNodeToSet);//Add the node to the set nodes list.

		//Set the adjacent set nodes for that node.
		foreach(Node adjacentNode in a_nNodeToSet.m_lAdjacentNodes)
		{
			adjacentNode.m_iAdjacentNodesOpened++;

			if (!m_lSetNodes.Contains(adjacentNode) && !(m_llAdjacentSetNodes[adjacentNode.m_iWeight].Contains(adjacentNode)))
			{
				m_llAdjacentSetNodes[adjacentNode.m_iWeight].Add(adjacentNode);
			}
		}
	}

	void FindNext()
	{
		Node nextNode; 

		do
		{
			bool empty = true;

			int lowestList = 0;

			for (int i = 0; i < 10; ++i)
			{
				lowestList = i;

				if (m_llAdjacentSetNodes[i].Count > 0)
				{
					empty = false;

					break;
				}
			}

			if (empty)
			{
				CancelInvoke("FindNext");//Early out.

				//Set the end node. 
				m_lSetNodes[m_lSetNodes.Count - 1].m_bIsEnd = true;

				foreach (Node node in m_nGrid)
				{
					if (m_lSetNodes.Contains(node))
					{
						//If it's the starting node spawn a ship wreck.
						if (node.m_bIsStart)
						{
							//Spawn ship.
							GameObject ship = Instantiate(m_gShipWreck);
							ship.transform.position = new Vector3(node.m_v3WorldPosition.x, node.m_v3WorldPosition.y, node.m_v3WorldPosition.z);
							ship.transform.parent = this.transform;
							ship.name = "Ship(Start)";
							m_lMazePieces.Add(ship);

							//Spawn player.
							GameObject player = Instantiate(m_gPlayerPrefab);
							player.transform.position = new Vector3(node.m_v3WorldPosition.x, node.m_v3WorldPosition.y + 1, node.m_v3WorldPosition.z);
							player.transform.parent = this.transform;
							player.name = "Player";
							m_lMazePieces.Add(player);

							//Set the current node that the player is on.
							player.GetComponent<Player>().m_nCurrentNode = node;
							player.GetComponent<Player>().m_fTimeRemaining = m_iGridSizeX * m_iGridSizeY;
						}
						//If its the end node spawn an island with a treasure chest.
						else if (node.m_bIsEnd)
						{
							GameObject island = Instantiate(m_gIsland);
							island.transform.position = new Vector3(node.m_v3WorldPosition.x, node.m_v3WorldPosition.y, node.m_v3WorldPosition.z);
							island.transform.parent = this.transform;
							island.name = "Island(End)";
							m_lMazePieces.Add(island);
						}
						//Else spawn a crate.
						else
						{
							GameObject crate = Instantiate(m_gCrate);
							crate.transform.position = new Vector3(node.m_v3WorldPosition.x, node.m_v3WorldPosition.y, node.m_v3WorldPosition.z);
							crate.transform.parent = this.transform;
							crate.name = "Crate(End)" + crate.transform.position.x + ", " + crate.transform.position.z;
							m_lMazePieces.Add(crate);
						}
					}
				}

				return;
			}

			nextNode = m_llAdjacentSetNodes[lowestList][0];
			
			m_llAdjacentSetNodes[lowestList].Remove(nextNode);

		} while (nextNode.m_iAdjacentNodesOpened >= 2);

		AddToSetNodes(nextNode);//Add the node to the set nodes.

		Invoke("FindNext", 0);//Continue.
	}

	public void DestroyMazePieces()
	{
		//If the maze pieces array is not empty.
		if (m_lMazePieces.Count != 0)
		{
			foreach (GameObject piece in m_lMazePieces)
			{
				Destroy(piece);//Destroy each maze piece.
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (m_bGizmos)
		{
			//Draw the maze outline.
			Gizmos.DrawWireCube(transform.position, new Vector3(m_v2GridWorldSize.x, 0, m_v2GridWorldSize.y));

			if (m_nGrid != null)
			{
				foreach (Node node in m_nGrid)
				{
					//Draw each maze node.
					Gizmos.DrawCube(node.m_v3WorldPosition, Vector3.one * m_fNodeDiameter);
				}
			}
		}
	}

	private void MoveCamera()
	{
		//Reset camera.
		m_cMainCamera.transform.position = new Vector3(0, 0.01f, 0);

		//Make sure the left and right fit on the screen.
		while (m_cMainCamera.WorldToViewportPoint(m_v3WorldBottomLeft).x < 0.0f )
		{
			m_cMainCamera.transform.position = new Vector3(m_cMainCamera.transform.position.x, m_cMainCamera.transform.position.y + 0.01f, m_cMainCamera.transform.position.z);
		}

		//Make sure the top and bottom bit in the screen.
		while (m_cMainCamera.WorldToViewportPoint(m_v3WorldBottomLeft).y < 0.0f)
		{
			m_cMainCamera.transform.position = new Vector3(m_cMainCamera.transform.position.x, m_cMainCamera.transform.position.y + 0.01f, m_cMainCamera.transform.position.z);
		}
		
		//Apply Offset.
		m_cMainCamera.transform.position = new Vector3(m_cMainCamera.transform.position.x, m_cMainCamera.transform.position.y + m_fScreenOffset, m_cMainCamera.transform.position.z);
		
	}

	//Allow the sliders to effect the grid size.
	public void SetGridWidth(float a_fGridWidth)
	{
		m_v2GridWorldSize.x = a_fGridWidth;
	}

	//Allow the sliders to effect the grid size.
	public void SetGridHeight(float a_fGridHeight)
	{
		m_v2GridWorldSize.y = a_fGridHeight;
	}
}