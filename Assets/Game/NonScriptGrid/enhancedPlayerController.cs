using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enhancedPlayerController : MonoBehaviour
{

    public GameObject currentTile;
    private mapController map;
    public GameObject mapObject;

    private GameObject bottomDoor, leftDoor, rightDoor, topDoor;

    public Button upButton;
    public Button leftButton;
    public Button rightButton;
    public Button downButton;

    private TileController currentTileController;
    private TileController topTileController;
    private TileController leftTileController;
    private TileController rightTileController;
    private TileController bottomTileController;

    //List<TileController[]> roomConfigs = new List<TileController[]>();
    Stack<TileController[]> roomConfigs = new Stack<TileController[]>();

    // Use this for initialization
    void Start()
    {
        currentTile = GameObject.Find("startTile");
        transform.position = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, -1);

        bottomDoor = GameObject.Find("bottomDoor");
        leftDoor = GameObject.Find("leftDoor");
        rightDoor = GameObject.Find("rightDoor");
        topDoor = GameObject.Find("topDoor");

        Button lbutton = leftButton.GetComponent<Button>();
        lbutton.onClick.AddListener(moveLeft);

        Button rbutton = rightButton.GetComponent<Button>();
        rbutton.onClick.AddListener(moveRight);

        Button ubutton = upButton.GetComponent<Button>();
        ubutton.onClick.AddListener(moveUp);

        Button dbutton = downButton.GetComponent<Button>();
        dbutton.onClick.AddListener(moveDown);

        checkTiles();
        initDoors();
        setDoorways();
    }

    // Update is called once per frame
    void Update()
    {

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
        transform.position = new Vector3(destinationTilePosition.x, destinationTilePosition.y, -1);
    }
    private void checkTiles()
    {
        currentTileController = currentTile.GetComponent<TileController>();

        checkDoors(currentTileController);

        if (currentTileController.topTile != null)
        {
            topTileController = currentTileController.topTile.GetComponent<TileController>();
        }
        else { topTileController = null; }

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
        map = mapObject.GetComponent<mapController>();
        string tileName = currentTile.transform.name;
        if (currentTile.leftDoor)
        {
            map.moveLeft();
            //direction = "left";
            direction = getDirection(tileName);
            resetRoom();
            //tileName = currentTile.transform.name;
            //print("ENTERING LEFT DOORWAY");
        }
        if (currentTile.rightDoor)
        {
            map.moveRight();
            direction = getDirection(tileName);
            resetRoom();
            //print("ENTERING RIGHT DOORWAY");
        }
        if (currentTile.backDoor)
        {
            map.moveBack();
            
            //direction = "back";

            direction = getDirection(tileName);
            //traverseBack();
            //direction = "back";
            resetRoom();
            //print("ENTERING BACK DOORWAY");
        }
    }

    public string getDirection(string name)
    {
        string movementDirection = "";
        if (string.Equals(name, "leftDoor")) { movementDirection = "left"; }
        else if (string.Equals(name, "rightDoor")) { movementDirection = "right"; }
        else if (string.Equals(name, "topDoor")) { movementDirection = "up"; }
        else if (string.Equals(name, "bottomDoor")) { movementDirection = "down"; }
        return movementDirection;
    }

    public void resetRoom()
    {       
        //setDoorways();

        currentTile = GameObject.Find("startTile");
        
        if (string.Equals(direction, "left"))
        {
            currentTile = GameObject.Find("rightEntrance");
            backDoorTileController = rightDoor.GetComponent<TileController>();
            leftDoorTileController = bottomDoor.GetComponent<TileController>();
            rightDoorTileController = topDoor.GetComponent<TileController>();
            topDoorTileController = leftDoor.GetComponent<TileController>();
        }
        if (string.Equals(direction, "right"))
        {
            currentTile = GameObject.Find("leftEntrance");
            backDoorTileController = leftDoor.GetComponent<TileController>();
            rightDoorTileController = bottomDoor.GetComponent<TileController>();
            leftDoorTileController = topDoor.GetComponent<TileController>();
            topDoorTileController = rightDoor.GetComponent<TileController>();
        }
        if (string.Equals(direction, "down"))
        {
            currentTile = GameObject.Find("topEntrance");
            backDoorTileController = topDoor.GetComponent<TileController>();
            rightDoorTileController = leftDoor.GetComponent<TileController>();
            leftDoorTileController = rightDoor.GetComponent<TileController>();
            topDoorTileController = bottomDoor.GetComponent<TileController>();
        }
        if (string.Equals(direction, "up"))
        {
            currentTile = GameObject.Find("bottomEntrance");
            backDoorTileController = bottomDoor.GetComponent<TileController>();
            rightDoorTileController = rightDoor.GetComponent<TileController>();
            leftDoorTileController = leftDoor.GetComponent<TileController>();
            topDoorTileController = topDoor.GetComponent<TileController>();
        }
        if (string.Equals(direction, "back"))
        {
            TileController[] currentDoorConfig = new TileController[4];
            currentDoorConfig = roomConfigs.Pop();
            print(currentDoorConfig);
            leftDoorTileController = currentDoorConfig[0];
            rightDoorTileController = currentDoorConfig[1];
            backDoorTileController = currentDoorConfig[2];
            topDoorTileController = currentDoorConfig[3];
        }

        backDoorTileController.backDoor = true;
        backDoorTileController.rightDoor = false;
        backDoorTileController.leftDoor = false;

        leftDoorTileController.backDoor = false;
        leftDoorTileController.rightDoor = false;
        leftDoorTileController.leftDoor = true;

        rightDoorTileController.backDoor = false;
        rightDoorTileController.rightDoor = true;
        rightDoorTileController.leftDoor = false;

        topDoorTileController.backDoor = false;
        topDoorTileController.rightDoor = false;
        topDoorTileController.leftDoor = false;

        
        //setDoorways();

        transform.position = new Vector3(currentTile.transform.position.x, currentTile.transform.position.y, -1);
        checkTiles();
    }

    TileController leftDoorTileController;
    TileController rightDoorTileController;
    TileController backDoorTileController;
    TileController topDoorTileController;
    string direction = "";
    public void initDoors()
    {
        TileController[] currentdoorConfig = new TileController[4];
        leftDoorTileController = leftDoor.GetComponent<TileController>();
        //currentdoorConfig[0] = leftDoorTileController;
        rightDoorTileController = rightDoor.GetComponent<TileController>();
        //currentdoorConfig[1] = rightDoorTileController;
        backDoorTileController = bottomDoor.GetComponent<TileController>();
        //currentdoorConfig[2] = backDoorTileController;
        topDoorTileController = topDoor.GetComponent<TileController>();
        //currentdoorConfig[3] = topDoorTileController;
        roomConfigs.Push(currentdoorConfig);
    }
    public void setDoorways()
    {

        print("inside SetDoorways");

        GameObject doorMapObject = GameObject.Find("mapObject");
        mapController doorMap = mapObject.GetComponent<mapController>();

        Color defaultColor = new Color(1, 1, 1, 1);

        Renderer leftTileRenderer = leftDoorTileController.GetComponent<Renderer>();
        if (map.getLeftChild() != 0)
        {
            leftDoorTileController.leftDoor = true;
            leftTileRenderer.material.color = Color.green;
        }
        else
        {
            leftDoorTileController.leftDoor = false;
            leftTileRenderer.material.color = defaultColor;
        }

        Renderer rightTileRenderer = rightDoorTileController.GetComponent<Renderer>();
        if (map.getRightChild() != 0)
        {
            rightDoorTileController.rightDoor = true;
            rightTileRenderer.material.color = Color.green;
        }
        else
        {
            rightDoorTileController.rightDoor = false;
            rightTileRenderer.material.color = defaultColor;
        }

        Renderer backTileRenderer = backDoorTileController.GetComponent<Renderer>();
        if (map.getParent() != 0)
        {
            backDoorTileController.backDoor = true;
            backTileRenderer.material.color = Color.green;
        }
        else
        {
            backDoorTileController.backDoor = false;
            backTileRenderer.material.color = defaultColor;
        }
    }

    public void arrangeDoors(string direction)
    {

    }
}
