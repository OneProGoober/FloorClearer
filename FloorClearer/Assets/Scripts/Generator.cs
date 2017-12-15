using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public enum Direction { North, East, South, West };

    /**
     * All dimension data for rooms and the map
     * */

    //Width of the map
    public int mapSizeX;

    ////Height of the map (Some day I may try to implement the floor with multiple levels!
    //public int MapSizeY;

    //Length of the map
    public int mapSizeZ;

    //The locaclScale for each room prefab
    public RectTransform roomSize;

    //the offset from 0 to the player's feet
    private float floorThickness;

    public  int levelSeed;

    /**
     * All GameObjects associated with the game. This includes the main and extraneous keys and rooms + room prefabs
     */
    /** The possible room types that must be generated: (all rooms have an entrace at the bottom)
     * Index:     Room Type:
     * 0            Dead end
     * 1            Single door(left)
     * 2            Single door(top)
     * 3            Single door(right)
     * 4            Double door(left, top)
     * 5            Double door(top, right)
     * 6            Double door(left, right)
     * 7            Triple door(left, top and right)
     * */
    public List<GameObject> roomPrefabs;

    //Used to find the thickness of the floor
    //public GameObject floor;

    //A list with the currently created rooms
    private List<Room> createdRooms;

    //A list with the current edge rooms
    private List<Room> edgeRooms;

    //A list that holds the indices of all vacant rooms
    private List<int> unfilledRooms;

    //holds the current room when generating the floor
    private Room currentRoom;



    void Start()
    {
        if (mapSizeX < 3 || mapSizeZ < 3)
        {
            Debug.Log("Enter a different Size for the map.. Both sizes must be larger than 3.");
            return;
        }
        
        //Initiate the random number generator with a particular seed. Will be used to re-access different levels.
        Random.InitState(levelSeed);

        //Add all rooms to the unfilledRoomsList
        for(int index = 0; index < mapSizeX*mapSizeZ; index++)
        {
            unfilledRooms.Add(index);
        }

        roomSize = roomPrefabs[0].gameObject.GetComponent<RectTransform>();
        //floorThickness = floor.transform.localScale.y;
        floorThickness = .5f;

        StartCoroutine(GenerateFloor());
    }


    /**
     * Implemented in an IEnumerator to easily see the algorithm in play when creating the floor
     * 
     * Algorithm:
     * 
     * Start Room Portion:
     * 1. Pick number between 0 and mapSizeX*mapSizeZ. This is the room number for the start room.
     * 2. Pick number 1-4 to Generate Start room. Number corresponds to the number of passages leaving the start room.
     *    -If number is 3 or 4, ensure that room number is not on the edge of the map or in a corner respectively.
     * 3. Assign each passage a room number to the room that it leads to.
     * 4. Add random start keys to Possible Keys List (extraneous or main). (Contains the possible keys that could have come across so far).
     * 5. Add start room to Edge Room List. (List holds the latest spawned rooms that haven't had attachements added to it)
     * 
     * Repeated Algorithm:
     * 1. For each room in Edge Room List, if key hasn't been added to Possible Keys List (extraneous or main), add them.
     * 2. Pick random room from Edge Room List. Set it to curr
     * 
     * If curr is main:
     * 3. For each passage:
     *    -Randomly generate a number between 1 and 3 and verify that there are at least that many adjacent rooms to fill.
     *    -Generate a second number to determine number of extraneous passages.
     *    -For each generated passage (1-3):
     *      -Use curr->passage[passageNumber] (returns a room) Check room->passages and set each one to a key that is in Possible Keys List
     *    -After each passage accounted for, add room to Edge Room List
     *    -Add keys from that newly spawned room to the Possible Keys List (extraneous or main)
     * 
     * 
     * Else: (all keys are extraneous)
     * 3. For each passage:
     *    -Randomly generate a number between 1 and 3 and verify that there are at least that many adjacent rooms to fill.
     *    -For each generated passage (1-3):
     *      -Use curr->passage[passageNumber] (returns a room) Check room->passages and set each one to a key that is in Possible Keys List(extraneous)
     *    -After each passage accounted for, add room to Edge Room List
     *    -Add keys from that newly spawned room to the Possible Keys List (extraneous)
     * */
    private IEnumerator GenerateFloor()
    {
        /**
        * Start Room Portion:
        * 1. Pick number between 0 and mapSizeX*mapSizeZ. This is the room number for the start room.
        * 2. Pick number 1-4 to Generate Start room. Number corresponds to the number of passages leaving the start room.
        *    -If number is 3 or 4, ensure that room number is not on the edge of the map or in a corner respectively.
        *    
        *    ESSENTIALLY DONE WITH The ABOVE SO FAR..
        *    
        * 3. Assign each passage a room number to the room that it leads to.
        * 4. Add random start keys to Possible Keys List (extraneous or main). (Contains the possible keys that could have come across so far).
        * 5. Add start room to Edge Room List. (List holds the latest spawned rooms that haven't had attachements added to it)
        * */
        GameObject startRoom;
        //1:
        int startRoomIndex = Random.Range(0, mapSizeX*mapSizeZ);

        //2:
        List<Direction> startRoomDirections = ValidDirections(startRoomIndex);
        int startRoomPrefabIndex = 0;

        /** The possible room types that must be generated:
         * Index:     Room Type:
         * 
         *  (ALL ROOMS HAVE BOTTOM ENTRANCE)
         * 0            Dead end
         * 1            Single door(left)
         * 2            Single door(top)
         * 3            Single door(right)
         * 4            Double door(left, top)
         * 5            Double door(top, right)
         * 6            Double door(left, right)
         * 7            Triple door(left, top and right)
         * */
         
        if (startRoomDirections.Count == 4)
        {
            startRoomPrefabIndex = Random.Range(0, 8);
        }

        else if (startRoomDirections.Count == 3)
        {
            startRoomPrefabIndex = Random.Range(0, 7);
        }

        else if (startRoomDirections.Count == 2)
        {
            startRoomPrefabIndex = Random.Range(0, 4);
        }

        else if (startRoomDirections.Count == 1)
        {
            startRoomPrefabIndex = 0;
        }

        else
        {
            Debug.Log("Start room should always have at least one valid direction. This is an error. startRoomDirections.Count: " + startRoomDirections.Count);
        }




        //TODO: make sure that the room is going to only open up in the correct direction. Change the rotation if necessary.
        //make a mapping for each prefab. List of directions corresponding to the prefabs.
        //make a function that checks if prefab rotation matches startRoomDirections. if it doesn't, rotate the prefab and update the direction that the prefab has with that rotation. 





        //Instantiate(Object original, Vector3 position, Quaternion rotation);
        startRoom = Instantiate(roomPrefabs[startRoomPrefabIndex], FindRoomPosition(startRoomIndex), roomPrefabs[startRoomPrefabIndex].transform.rotation);

        yield return 0;
    }

    /**
     * Returns the position of the room in world space given the index for the room
     * */
    private Vector3 FindRoomPosition(int index)
    {
        float width = roomSize.rect.width;

        int row = index / mapSizeX;
        int column = index % mapSizeX;

        Vector3 roomPosition = new Vector3(row*width , 0f, column*width);
        return roomPosition;
    }


    /**
     * Checks if we can place a new room in this location
     * Returns true if valid
     */
        private bool CheckIfValidPlacement(int index)
        {
            if(unfilledRooms.Contains(index))
            {
                return true;
            }
            return false;
        }
    
    /**
     * Returns the directions that are valid for a room placement
     * Checks to ensure that each room is open and that it is directly connected to the original in question.
     * */
    private List<Direction> ValidDirections(int index)
    {
        List<Direction> validDirections = new List<Direction>();
        int left = index - 1;
        int right = index + 1;
        int above = index - mapSizeX;
        int below = index + mapSizeX;

        //Check to make sure that the room is directly to the left
        if(!(left % mapSizeX == (mapSizeX - 1)) && CheckIfValidPlacement(left))
        {
            validDirections.Add(Direction.West);
        }

        //Check to make sure that the room is directly to the right
        if (!(right % mapSizeX == 0) && CheckIfValidPlacement(right))
        {
            validDirections.Add(Direction.East);
        }

        //Check to make sure that the room is directly above
        if (above >= 0 && CheckIfValidPlacement(above))
        {
            validDirections.Add(Direction.North);
        }

        //Check to make sure that the room is directly below
        if (below < mapSizeX*mapSizeZ && CheckIfValidPlacement(below))
        {
            validDirections.Add(Direction.South);
        }

        return validDirections;
    }

    /**
     * Generate Random Key
     * */
    public Key GenerateUniqueKey()
    {
        Key uniqueKey = new Key();
        //TODO
        //ensure that it is unique. Set extraneous or main.
        return uniqueKey;
    }
}
