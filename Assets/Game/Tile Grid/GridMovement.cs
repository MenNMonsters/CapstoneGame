using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    //public enum DIRECTION { UP, DOWN, LEFT, RIGHT}

public class GridMovement : MonoBehaviour {
    
    private bool canMove = true, moving=false;
    public int speed = 5, buttonCooldown = 0;
    //private DIRECTION dir = DIRECTION.DOWN;
    private Vector3 pos;
    public Vector3 upVector = new Vector3(0, 1, 0);

    private GridController gridController;

    private TileController currentTileController;
    private TileController topTileController;
    private TileController leftTileController;
    private TileController rightTileController;
    private TileController bottomTileController;

    private MapGenerator map;
    public GameObject mapObject;

    //public Vector3 currentPosition = new Vector3(0, 0, 0);
    private GameObject startTile;
    private GameObject currentTile;
    private GameObject grid;

    
    public Button upButton;
    public Button leftButton;
    public Button rightButton;
    public Button downButton;
    
    //public GameObject currentNodeText;


    public string occupier = "Player";

	// Use this for initialization
	void Start () {
        //currentTile = GameObject.Find("tile(0,7)");
        grid = GameObject.Find("GridGenerator");
        mapObject = GameObject.Find("MapObject");
        gridController = grid.GetComponent<GridController>();
        currentTile = GameObject.Find("tile("+gridController.numberOfRows/3+"," + (gridController.numberOfTiles/2) + ")");
        //currentTile = startTile;
        //print("startTile position = "+startTile.transform.position.y);
        transform.position = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, -1);
        checkTiles();
        currentTileController.occupied = true;

        Button lbutton = leftButton.GetComponent<Button>();
        lbutton.onClick.AddListener(moveLeft);

        Button rbutton = rightButton.GetComponent<Button>();
        rbutton.onClick.AddListener(moveRight);

        Button ubutton = upButton.GetComponent<Button>();
        ubutton.onClick.AddListener(moveUp);

        Button dbutton = downButton.GetComponent<Button>();
        dbutton.onClick.AddListener(moveDown);
        //() => MyMethod(a)
    }
    
    // Update is called once per frame
    void Update () {
        
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            moveUp();            
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moveLeft();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            moveRight();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            moveDown();
        }
    }
    public void moveUp()
    {
        if (currentTileController.topTile != null && topTileController.occupied == false)
        {
            currentTileController.occupied = false;
            currentTileController.occupiedObject = null;

            movePlayer(currentTileController.topTile.transform.position);
            currentTile = currentTileController.topTile;
            checkTiles();
            occupyTile(currentTileController);
        }
    }
    public void moveLeft()
    {
        if (currentTileController.leftTile != null && leftTileController.occupied == false)
        {
            currentTileController.occupied = false;
            currentTileController.occupiedObject = null;

            movePlayer(currentTileController.leftTile.transform.position);
            currentTile = currentTileController.leftTile;
            checkTiles();
            occupyTile(currentTileController);
        }
    }
    public void moveRight()
    {
        if (currentTileController.rightTile != null && rightTileController.occupied == false)
        {
            currentTileController.occupied = false;
            currentTileController.occupiedObject = null;

            movePlayer(currentTileController.rightTile.transform.position);
            currentTile = currentTileController.rightTile;
            checkTiles();
            occupyTile(currentTileController);
        }
    }
    public void moveDown()
    {
        if (currentTileController.bottomTile != null && bottomTileController.occupied == false)
        {
            currentTileController.occupied = false;
            currentTileController.occupiedObject = null;

            movePlayer(currentTileController.bottomTile.transform.position);
            currentTile = currentTileController.bottomTile;
            checkTiles();
            occupyTile(currentTileController);
        }
    }

    private void movePlayer(Vector3 destinationTilePosition)
    {
        transform.position = new Vector3 (destinationTilePosition.x, destinationTilePosition.y, -1);
    }
    private void checkTiles()
    {
        currentTileController = currentTile.GetComponent<TileController>();

        checkDoors(currentTileController);

        if(currentTileController.topTile != null)
        {
            topTileController = currentTileController.topTile.GetComponent<TileController>();
        }else { topTileController = null; }

        if (currentTileController.rightTile != null)
        {
            rightTileController = currentTileController.rightTile.GetComponent<TileController>();
        }
        else { rightTileController = null; }

        if (currentTileController.leftTile != null)
        {
            leftTileController = currentTileController.leftTile.GetComponent<TileController>();
        }
        else { leftTileController = null; }

        if (currentTileController.bottomTile != null)
        {
            bottomTileController = currentTileController.bottomTile.GetComponent<TileController>();
        }
        else { bottomTileController = null; }
    }

    public void occupyTile(TileController currentTile)
    {
        currentTile.occupied = true;
        currentTile.occupiedObject = "player";
    }

    public void checkDoors(TileController currentTile)
    {
        map = mapObject.GetComponent<MapGenerator>();

        if(currentTile.leftDoor)
        {
            map.moveLeft();
            resetRoom();
            //print("ENTERING LEFT DOORWAY");
        }
        if (currentTile.rightDoor)
        {
            map.moveRight();
            resetRoom();
            //print("ENTERING RIGHT DOORWAY");
        }
        if (currentTile.backDoor)
        {
            map.moveBack();
            resetRoom();
            //print("ENTERING BACK DOORWAY");
        }
    }
    public void resetRoom()
    {
        gridController.setDoorways();

        currentTile = GameObject.Find("tile(" + gridController.numberOfRows / 3 + "," + (gridController.numberOfTiles / 2) + ")");
        transform.position = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, -1);
        checkTiles();
    }
}
