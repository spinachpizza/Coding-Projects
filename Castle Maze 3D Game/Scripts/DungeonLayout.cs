using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonLayout : MonoBehaviour
{

    private int[,] roomVariations = {
        {0,0,0,0},
        {0,1,0,1},
        {1,0,1,0},
        {1,1,0,0},
        {0,1,1,0},
        {0,0,1,1},
        {1,0,0,1},
        {1,0,0,0},
        {0,1,0,0},
        {0,0,1,0},
        {0,0,0,1},
        {1,1,1,0},
        {0,1,1,1},
        {1,0,1,1},
        {1,1,0,1},
        {1,1,1,1}
    };

    public GameObject[] empty = new GameObject[1];
    public GameObject[] room1 = new GameObject[1];
    public GameObject[] room2 = new GameObject[1];
    public GameObject[] room3 = new GameObject[1];
    public GameObject[] room4 = new GameObject[1];
    public GameObject[] room5 = new GameObject[1];
    public GameObject[] room6 = new GameObject[1];
    public GameObject[] room7 = new GameObject[1];
    public GameObject[] room8 = new GameObject[1];
    public GameObject[] room9 = new GameObject[1];
    public GameObject[] room10 = new GameObject[1];
    public GameObject[] room11 = new GameObject[1];
    public GameObject[] room12 = new GameObject[1];
    public GameObject[] room13 = new GameObject[1];
    public GameObject[] room14 = new GameObject[1];
    public GameObject[] room15 = new GameObject[1];

    public GameObject[] exits = new GameObject[4];


    public GameObject[][] rooms;

    public GameObject ER; //Entrance room

    private int numOfRooms = 0;


    public int[,] matrix;




    public void SetupDungeon()
    {
        rooms = new GameObject[16][] {empty, room1, room2, room3, room4, room5, room6, room7, room8, room9, room10, room11, room12, room13, room14, room15};
        CreateLayoutMatrix();
    }



    private void CreateLayoutMatrix() 
    {
        //Setup layout matrix
        int matrixSize = 49; //11
        int midpoint = matrixSize / 2;  //The entrance room
        matrix = new int[matrixSize,matrixSize];

        //Assign initial rooms 
        matrix[1, midpoint] = 1;
        matrix[1, midpoint - 1] = 1;
        matrix[1, midpoint + 1] = 1;
        matrix[2, midpoint] = 1;


        //Random variable that determines path lengths
        int randomMax = 50; //32

        int nextj = 1;
        int nexti = matrixSize / 2;
        bool newCoords = false;

        int iterations = 7;
        //Iteratively create paths through the matrix 
        for(int count=0; count<iterations; count++)
        {
            //Setup of coords, if new ones found inside the cave then use those
            int j = 1;
            int i = matrixSize / 2;

            //Stores random coords in a previous path for next starting position - allows a path to start halfway in
            if(newCoords)
            {
                j = nextj;
                i = nexti;
            }

            newCoords = false;


            //Random variables
            int lastAxis = Random.Range(0,2);    //0 = horizontal, 1 = vertical
            int lastDirection = Random.Range(0,2);
    
            //Put into the correct format
            if(lastDirection == 0) {
                lastDirection = -1;
            } else {
                lastDirection = 1;
            }



            //int continueChance = 0;
            //Continue while random variable allows it
            for(int x = 0; x < randomMax; x++)
            {
                bool directionFound = false;
                int direction = lastDirection;

                //If last direction was horizontal
                if(lastAxis == 0)
                {

                    int randomNum = Random.Range(0,5);

                    //Horizontal movement
                    if(randomNum < 3 && validIndex(matrix, matrixSize, j, i + direction))
                    {
                        directionFound = true;
                        i = i + direction;
                        lastAxis = 0;
                        
                    } else {
                        randomNum = Random.Range(3,5);
                    }
                    
                    //Vertical movement
                    if(randomNum == 3 && validIndex(matrix, matrixSize, j + direction, i))
                    {
                        directionFound = true;
                        j = j + direction;
                        lastAxis = 1; 

                    //Vertical movement
                    } else if(randomNum == 4 && validIndex(matrix, matrixSize, j - direction, i))
                    {
                        directionFound = true;
                        j = j - direction;
                        lastAxis = 1; 
                        lastDirection = - direction;
                    }  
                    

                //If last direction was vertical
                } else {

                    int randomNum = Random.Range(0,5);

                    //Vertical movement
                    if(randomNum < 3 && validIndex(matrix, matrixSize, j + direction, i))
                    {
                        directionFound = true;
                        j = j + direction;
                        lastAxis = 1;
                    } else {
                        randomNum = Random.Range(3,5);
                    }

                    //Horizontal movement
                    if(randomNum == 3 && validIndex(matrix, matrixSize, j, i + direction))
                    {
                        directionFound = true;
                        i = i + direction;
                        lastAxis = 0; 

                    //Horizontal movement
                    } else if(randomNum == 4 && validIndex(matrix, matrixSize, j, i - direction))
                    {
                        directionFound = true;
                        i = i - direction;
                        lastAxis = 0; 
                        lastDirection = - direction;
                    }  
                }
            

                if(directionFound == true && matrix[j,i] == 0)
                {
                    matrix[j,i] = 1;
                    numOfRooms ++;

                    //Chance to store coords for next path (doesnt apply to first 2 paths)
                    int randomCoordChance = Random.Range(0,25);
                    if(randomCoordChance == 0 && count >= 2)
                    {
                        nextj = j;
                        nexti = i;
                        newCoords = true;
                    }
                }
            }
        }

        CreateLayout(matrix, matrixSize);
    }


    public int[,] GetLayoutMatrix()
    {
        return matrix;
    }


    //Check if an index given is valid and within array bounds
    private bool validIndex(int[,] matrix, int matrixSize, int j, int i) 
    {
        //Checks if index is in bounds
        if((i < matrixSize - 1 && i > 0) && (j < matrixSize - 1 && j > 0))
        {
            //Checks if room is empty - a 0 (so a room can be placed) unless its the default rooms
            int midpoint = matrixSize / 2;
            if((i == midpoint || i == midpoint + 1 || i == midpoint - 1) && (j == 1 || j == 2))
            {
                return true;

            } else if(matrix[j,i] == 0)
            {
                return true;

            } else {
                return false;
            }

        } else {
            return false;
        }
    }


    //Convert binary value array into array of room types
    private void CreateLayout(int[,] matrix, int size)
    {

        int[,] roomMatrix = new int[size,size];

        for(int j = 1; j<size-1; j++)
        {
            for(int i = 1; i<size-1; i++)
            {
                
                //Check which room is suitable
                int[] doors = {0,0,0,0};
                int roomNum = 0;

                //If on a build tile
                if(matrix[j,i] == 1)
                {
                    //Check room above
                    if(matrix[j-1,i] == 1)
                    {
                        doors[0] = 1;
                    }
                    //Check room to right
                    if(matrix[j,i+1] == 1)
                    {
                        doors[1] = 1;
                    }
                    //Check room below
                    if(matrix[j+1,i] == 1)
                    {
                        doors[2] = 1;
                    }
                    //Check room to left
                    if(matrix[j,i-1] == 1)
                    {
                        doors[3] = 1;
                    }
                }

                //Compare with list to find match

                //Iterate through each room type
                for(int c = 0; c<16; c++)
                {
                    int matchCount = 0;
                    //Iterate through each array element
                    for(int index = 0; index<4; index++)
                    {
                        if(doors[index] == roomVariations[c, index])
                        {
                            matchCount ++;
                        }
                    }

                    //Match found (all 4 door optons align)
                    if(matchCount >= 4)
                    {
                        roomNum = c;
                        c = 16;
                    }
                }

                roomMatrix[j,i] = roomNum;



            }
        }
        
        //Assign first room to be the entrance room
        roomMatrix[1,size/2] = 50;

        buildMap(roomMatrix, size);
    }


    //Using the room array place the room tiles on the map
    private void buildMap(int[,] mapLayout, int size)
    {
        float x = 0f;
        float z = 0f;
        float y = 0f;
        int roomNum = 0;
    
        //Traverse layout array and build each element

        for(int j=0; j<size; j++)
        {
            z = 0f;

            for(int i=0; i<size; i++) 
            {
                roomNum = mapLayout[j,i];

                //Add room width
                z += 12f;

                //Special condition for entrance room as its slightly larger
                if(roomNum == 50)
                {
                    //Instantiate(ER, new Vector2(x + 5.4f,y + 1.6f), transform.rotation, this.gameObject.transform); //Place entrance room
                    Instantiate(ER, new Vector3(x,y,z),transform.rotation, this.gameObject.transform); //Place entrance room

                //Else place room and choose a room variation for it
                } else if(roomNum != 0) {

                    int variationNumber;

                    //Find how many variations there are for certain room type
                    if(rooms[roomNum].Length > 1)
                    {
                        variationNumber = ChooseRoomVariation(roomNum);

                    } else {
                        
                        variationNumber = 0;
                    }
                    

                    Instantiate(rooms[roomNum][variationNumber], new Vector3(x,y,z),rooms[roomNum][variationNumber].transform.rotation, this.gameObject.transform); //Place room tile
                }
            }

            //Add room height
            x += 12f;
        }

        AddExit();
    }


    
    public int ChooseRoomVariation(int roomNum)
    {
        //Find how many variations there are for certain room type
        GameObject[] roomVariations = rooms[roomNum];
        int numOfVariations = roomVariations.Length;


        if(roomNum == 1 || roomNum == 2)
        {
            //Only for walkways / corridors
            //Select a random room variation, order of array is the priority
            for(int c = 0; c < numOfVariations; c++)
            {
                int random = Random.Range(0,5);
                if(random != 0)
                {
                    return c;
                }
            }

        } else {
            //Select a random room variation
            int random = Random.Range(0,numOfVariations);
            return random;
        }

        return 0;
    }


    public void ResetLayout() {
        int children = transform.childCount;
        for(int i = 0; i < children - 1; i++)
        {
            Transform child = transform.GetChild(i);
            Destroy(child.gameObject);
            
        }
    }

    private void AddExit()
    {
        int ChildCount = transform.childCount;

        for(int i=ChildCount-1; i >= 0; i--)
        {
            int RandomChance = Random.Range(0,12);
            Transform child = transform.GetChild(i);
            //Check if exit has no been placed yet and reset
            if(i == 0)
            {
                i = ChildCount-1;

            } else {

                
                //Replace room with an exit if its not a 4 door room
                if(RandomChance == 0 && GetRoomNumber(child.name) != 15)
                {
                    //Retrieve room number from name string
                    int number = GetRoomNumber(child.name);

                    GameObject exit = exits[0];

                    //Place suitable exit for room number design
                    if(number == 1 || number == 2)
                    {
                        exit = exits[0];
                
                    }else if(number == 3 || number == 4 || number == 5 || number == 6)
                    {
                        exit = exits[1];

                    }else if(number == 7 || number == 8 || number == 9 || number == 10)
                    {
                        exit = exits[2];

                    }else if(number == 11 || number == 12 || number == 13 || number == 14)
                    {
                        exit = exits[3];
                    }

                    Instantiate(exit, child.position, child.rotation, this.gameObject.transform);
                    Destroy(child.gameObject);
                    i = -1;
                }
            }

        }
    }


    private int GetRoomNumber(string name)
    {
        string substring = name.Substring(2);
        int endIndex;

        //Check if string contains a . (is a variation of a room)
        if(substring.Contains('.'))
        {
            endIndex = substring.IndexOf('.');

        } else {
            endIndex = substring.IndexOf('(');
        }

        substring = substring.Substring(0,endIndex);
        return int.Parse(substring);
    }

    public int getNumRooms() 
    {
        return numOfRooms;
    }
}


