using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapGenerator : MonoBehaviour {
    
    Tree map = new Tree();


    public Text currentNodeText;
    public Text leftChildText;
    public Text rightChildText;

    public Text evenText;

    private GameObject currentTile;
    public GameObject player;
    private GridController currentTileController;

    //public Text leftChildLeftChildText;

    //public Text text;

	// Use this for initialization
	void Start () {
        createMap();
        setText();
        //map.printTree();
        //text.text = "Hello World";     
        //currentNodeText.text = "Current Node ID: " + map.getCurrentNodeId();
    }

    // Update is called once per frame
    void Update () {
        
        
        if ((Input.GetKeyDown(KeyCode.L)))//||(Input.GetKeyDown(KeyCode.LeftArrow)))
        {
            moveLeft();
            //map.traverseLeft();
            //setText();
            //currentNodeText.text = "Current Node ID: " + map.getCurrentNodeId();
            //currentNodeText.text = "Current Node ID: " + map.getCurrentNodeId();
        }
        if ((Input.GetKeyDown(KeyCode.R))) //|| (Input.GetKeyDown(KeyCode.RightArrow)))
        {
            moveRight();
            //map.traverseRight();
            //setText();
        }
        if ((Input.GetKeyDown(KeyCode.B)))//||(Input.GetKeyDown(KeyCode.Space)))
        {
            moveBack();
            //map.traverseBack();
            //setText();
        }
    }
    public void moveLeft()
    {
        map.traverseLeft();
        setText();
    }
    public void moveRight()
    {
        map.traverseRight();
        setText();
    }
    public void moveBack()
    {
        map.traverseBack();
        setText();
    }

    void setText()
    {
        currentNodeText.text = "Current Node ID: " + map.getCurrentNodeId();

        evenText.text = "Current Node Event: "+map.getNodeEvent();

        leftChildText.text = "Left Child ID: " + map.getLeftChildId();
        rightChildText.text = "Right Child ID: " + map.getRightChildId();
        //leftChildLeftChildText.text = "Left Child's Left Child ID: " + map.getLeftChildLeftChildId();
    }

    public void createMap()
    {
        map.insert(50, "Fight");
        map.insert(10, "Fight");
        map.insert(60, "Challenging Fight");

        map.insert(5, "Treasure Chest");
        map.insert(20, "Merchant Room");

        map.insert(3, "Challenging Fight");
        map.insert(7, "Boss");

        map.insert(59, "Ambush Fight");
        map.insert(61, "Trap");  
    }

    public int getLeftChild()
    {
        return map.getLeftChildId();
    }
    public int getRightChild()
    {
        return map.getRightChildId();
    }
    public int getParent()
    {
        return map.getParentExists();
    }

    public class Tree
    {   public btNode c;
        public btNode pred;

        Stack predStack = new Stack();

        public int howmany;

        public Tree()
        {
            howmany = 0;
            btNode c;
            //btNode pred;
        }

        public Tree(int i)
        {
            howmany = 0;
            btNode c;
            //btNode pred;
        }

        public void printNode(btNode t)
        {

            if (t != null)
            {

                //printNode(t.left);
                print("How Many Nodes: " + howmany);
                //currentNodeText.text = "How Many Nodes: " + howmany +"\nCurrent Node: "+t.info;
                print("Current Node: "+t.info);
                //leftChildText.text = "Left Child: "+t.left.info;
                print("Left Child: "+t.left.info);
                //rightChildText.text = "Right Child: " + t.right.info;
                print("Right Child: " + t.right.info);
                //printNode(t.right);
            }
        }
        public void printTree()
        {
            printNode(c);
            
        }

//GETTERS
        public int getCurrentNodeId()
        {
            if(c != null)
            {
                return c.info;
            }
            else { return 0; }
            
        }
        public int getLeftChildId()
        {
            if (c.left != null)
            {
                return c.left.info;
            }
            else { return 0; }
        }
        public int getRightChildId()
        {
            if (c.right != null)
            {
                return c.right.info;
            }
            else { return 0; }
        }
        public int getLeftChildLeftChildId()
        {
            if (c.left.left != null)
            {
                return c.left.left.info;
            }
            else
            {
                return 0;
            }
        }
        public string getNodeEvent()
        {
            return c.randomEvent;
        }
        public int getParentExists()
        {
            return predStack.Count;
        }
//TRAVERSAL METHODS
        public void traverseLeft()
        {
            btNode p = c;
            if (p.left != null)
            {
                c = c.left;
                predStack.Push(p);
            }
        }
        public void traverseRight()
        {
            btNode p = c;
            if (p.right != null)
            {
                c = c.right;
                predStack.Push(p);
            }
        }
        public void traverseBack()
        {
            if (predStack.Count != 0)
            {
                c = (btNode)predStack.Pop();
            }
        }

        public void insert(int i, string r)
        {
            btNode p = c;
            btNode pred = null;
            while ((p != null) && (p.info != i))
            {
                pred = p;
                if (i < p.info)
                {
                    p = p.left;
                }
                else
                {
                    p = p.right;
                }
            }
            if (p == null)
            {
                howmany++;
                p = new btNode(null, i, r, null);
                if (pred != null)
                {
                    if (i < pred.info)
                    {
                        pred.left = p;
                    }
                    else
                    {
                        pred.right = p;
                    }
                }
                else
                {
                    c = p;
                }
            }
        }
                
        public class btNode
        {
            public int info;
            public btNode left;
            public btNode right;
            public string randomEvent;

            public btNode()
            {
                left = null;
                info = 0;
                randomEvent = null;
                right = null;
            }
            public btNode(btNode lt, int i, string r, btNode rt)
            {
                left = lt;
                info = i;
                randomEvent = r;
                right = rt;
            }

        }


    }

}
