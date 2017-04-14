using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour {

    public GameObject tilePrefab;
    public int numberOfTiles;
    public int numberOfRows;
    public float distanceBetweenTiles;
    List<GameObject[]> tileList = new List<GameObject[]>();
    public Vector3 rowVector;

    private MapGenerator map;
    private GameObject mapObject;

	// Use this for initialization
	void Start () {
        float tileSize = tilePrefab.GetComponent<Renderer>().bounds.size.y;
        createTiles();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void createTiles()
    {
        float tileHeight = tilePrefab.GetComponent<Renderer>().bounds.size.y;
        float xOffset = (float)-(numberOfTiles/2.0f);
        float yOffset = tilePrefab.GetComponent<Renderer>().bounds.size.y;
        float columnOffset;
        float rowOffset = 5;
        
        float startingYOffset = -(4);
        
        rowOffset = startingYOffset;

        for (int row = 0; row < numberOfRows; row++)
        {
            columnOffset = xOffset;
            rowOffset += distanceBetweenTiles;

            GameObject[] rowArray = new GameObject[numberOfTiles];

            for (int tilesCreated = 0; tilesCreated < numberOfTiles; tilesCreated++)
            {
                columnOffset += distanceBetweenTiles;
                rowVector = new Vector3(transform.position.x + columnOffset, transform.position.y + rowOffset, 0);
                GameObject newTilePrefab = Instantiate(tilePrefab, rowVector, transform.rotation);
                newTilePrefab.name = "tile(" + row + "," + tilesCreated + ")";
                rowArray[tilesCreated] = newTilePrefab;
            }
            tileList.Add(rowArray);
            
        }
        setReferences(tileList);
        setDoorways();
    }

    public void setReferences(List<GameObject[]> generatedTileList)
    {
        
        print(generatedTileList.Count);
        for (int i = 0; i < generatedTileList.Count; i++)
        {
            for(int j = 0; j < generatedTileList[i].Length; j++)
            {
                TileController tileController = generatedTileList[i][j].GetComponent<TileController>();
                if ((i + 1) < numberOfRows)
                {
                    tileController.topTile = generatedTileList[i+1][j];
                }else
                {
                    tileController.topTile = null;
                }
                if ((i - 1) >= 0)
                {
                    tileController.bottomTile = generatedTileList[i - 1][j];
                }
                if ((j + 1) < numberOfTiles)
                {
                    tileController.rightTile = generatedTileList[i][j + 1];
                }else
                {
                    tileController.rightTile = null;
                }
                if ((j - 1) >= 0)
                {
                    tileController.leftTile = generatedTileList[i][j - 1];
                }else
                {
                    tileController.leftTile = null;
                }
            }            
        }
    }

    public void setDoorways()
    {
        //mapObject = GameObject.Find("MapObject");
        //map = mapObject.GetComponent<MapGenerator>();
        print("INSIDE SETDOORWAYS");
        Color defaultColor = new Color(1,1,1,1);

        TileController leftTileController = tileList[numberOfRows / 2][0].GetComponent<TileController>();
        Renderer leftTileRenderer = leftTileController.GetComponent<Renderer>();
        if (map.getLeftChild()!=0)
        {
            leftTileController.leftDoor = true;
            leftTileRenderer.material.color = Color.green;
        }
        else
        {
            leftTileController.leftDoor = false;
            leftTileRenderer.material.color = defaultColor;
        }

        TileController rightTileController = tileList[numberOfRows / 2][numberOfTiles - 1].GetComponent<TileController>();
        Renderer rightTileRenderer = rightTileController.GetComponent<Renderer>();
        if (map.getRightChild() != 0)
        {
            rightTileController.rightDoor = true;
            rightTileRenderer.material.color = Color.green; 
        }
        else
        {
            rightTileController.rightDoor = false;
            rightTileRenderer.material.color = defaultColor;
        }

        TileController backTileController = tileList[0][numberOfTiles / 2].GetComponent<TileController>();
        Renderer backTileRenderer = backTileController.GetComponent<Renderer>();
        if (map.getParent() != 0)
        {
            backTileController.backDoor = true;
            backTileRenderer.material.color = Color.green;
        }
        else
        {
            backTileController.backDoor = false;
            backTileRenderer.material.color = defaultColor;
        }       
    }
}
