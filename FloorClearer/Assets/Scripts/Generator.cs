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

    //The localScale for each room prefab
    public float roomSize;

    //the offset from 0 to the player's feet
    private float floorThickness;

    public int levelSeed;

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

        unfilledRooms = new List<int>();

        //Add all rooms to the unfilledRoomsList
        for (int index = 0; index < mapSizeX * mapSizeZ; index++)
        {
            unfilledRooms.Add(index);
        }

        //floorThickness = floor.transform.localScale.y;
        floorThickness = .5f;

        StartCoroutine(GenerateFloor());
    }


    /**
     * Implemented in an IEnumerator to easily see the algorithm in play when creating the floor
     * 
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

        SetUpStartRoom();
        
        yield return 0;
    }



    /**
     * Start Room Setup:
     * 
     * 1. Pick number between 0 and mapSizeX*mapSizeZ. This is the room number for the start room.
     * 2. Find the valid passages that the start room may have from the room position
     * 3. Randomly pick a prefab and validate that the prefab has the same number (or fewer) directions than validStartRoomDirections.
     * 4. Once we have a valid room prefab, ensure that no room directions are invalid. If they are, rotate the room and update the directions until they are valid.
     * 5. Instantiate the start room.
     *    
     * 3. Assign each passage a room number to the room that it leads to.
     * 4. Add random start keys to Possible Keys List (extraneous or main). (Contains the possible keys that could have come across so far).
     * 5. Add start room to Edge Room List. (List holds the latest spawned rooms that haven't had attachements added to it)
     * 
     * */
    private void SetUpStartRoom()
    {
        GameObject newRoom = null;
        GameObject startRoom = null;
        List<Direction> passageDirections = null;
        int startRoomIndex = Random.Range(0, mapSizeX * mapSizeZ);

        List<Direction> validStartRoomDirections = ValidDirections(startRoomIndex);
        
        int startRoomPrefabIndex = 0;
        
        bool validRotationExists = false;
        bool prefabInvalid = true;
        bool rotationInvalid = true;

        while(prefabInvalid)
        {
            startRoomPrefabIndex = Random.Range(0, 8);
            Debug.Log("prefabindex:" + startRoomPrefabIndex);
            startRoom = roomPrefabs[startRoomPrefabIndex];
            if(newRoom != null)
            {
                Destroy(newRoom);
            }
            newRoom = Instantiate(startRoom, FindRoomPosition(startRoomIndex), startRoom.transform.rotation);
            passageDirections = newRoom.GetComponent<Room>().passageDirections;

            validRotationExists = CheckIfValidRotationExists(passageDirections, validStartRoomDirections);

            if (validRotationExists)
            {
                prefabInvalid = false;
                Debug.Log("Prefab used: " + newRoom + " at location: " + startRoomIndex);
            }
        }
        
        while (rotationInvalid)
        {
            rotationInvalid = !CheckIfCorrectOrientation(newRoom.GetComponent<Room>().passageDirections, validStartRoomDirections);
            
            if (rotationInvalid)
            {
                Debug.Log("Gotta rotate room");
                RotateRoom(newRoom);
            }
        }

        //TODO
        //Room is instantiated, now to update the lists, add keys
        //!!!! Add the position to the filled rooms list.... Figure out everything I need to update later. Tired.
       
    }

    /**
     * Check to see if there is some valid rotation for the room prefab.
     * -Ensures that there are no issues when the start room is at the corner of the map
     * -Changes the directions of the passages for the room prefab in order to determine if there is a valid orientation.
     * */
    private bool CheckIfValidRotationExists(List<Direction> passageDirections, List<Direction> validStartRoomDirections)
    {
        if(validStartRoomDirections.Count < passageDirections.Count)
        {
            return false;
        }

        for(int orientation = 0; orientation < 4; orientation++)
        {
            if (CheckIfCorrectOrientation(passageDirections, validStartRoomDirections))
            {
                return true;
            }

            //update the passage directions with the next orientation (rotated 90 degrees)
            passageDirections = ChangeDirections(passageDirections);
        }

        //Even though the count of passages for the room prefab is not larger than the possible valid directions, no correct orientation exists for this prefab.
        return false;
    }

    /**
     * Returns false if the potential room passages don't contain all prefab directions.
     * Checks all entries in the list unless the passage direction is invalid.
     * */
    private bool CheckIfCorrectOrientation(List<Direction> passageDirections, List<Direction> validStartRoomDirections)
    {
        foreach (var passageDirection in passageDirections)
        {
            if (!validStartRoomDirections.Contains(passageDirection))
            {
                return false;
            }
        }
        return true;
    }

    /**
     * Rotates the prefab by 90 degrees along the Y axis and calls another function to update the passage directions
     * */
    private void RotateRoom(GameObject startRoom)
    {
        startRoom.transform.Rotate(0, 90, 0);
        startRoom.GetComponent<Room>().passageDirections = ChangeDirections(startRoom.GetComponent<Room>().passageDirections);
    }
    
    /**
     * Returns a copy of the inputed list with the directions rotated one direction clockwise. 
     * */
    private List<Direction> ChangeDirections(List<Direction> directionsToChange)
    {
        //Update the directions. Each 90 degree rotation is rotates the directions one direction "clockwise".
        List<Direction> newDirections = new List<Direction>();

        if (directionsToChange.Contains(Direction.North))
        {
            newDirections.Add(Direction.East);
        }

        if (directionsToChange.Contains(Direction.East))
        {
            newDirections.Add(Direction.South);
        }

        if (directionsToChange.Contains(Direction.South))
        {
            newDirections.Add(Direction.West);
        }

        if (directionsToChange.Contains(Direction.West))
        {
            newDirections.Add(Direction.North);
        }
        return newDirections;
    }

    /**
     * Takes in a room instance and an input direction and returns the index of the room that the passage leads to
     * */
    private int GetRoomIndexFromDirection(Room room, Direction passageDirection)
    {
        int currentRoomNumber = room.GetRoomNumber();
        int roomIndex = -1;

        if(passageDirection.Equals(Direction.South))
        {
            roomIndex = currentRoomNumber + mapSizeX;
        }

        if (passageDirection.Equals(Direction.North))
        {
            roomIndex = currentRoomNumber - mapSizeX;
        }

        if (passageDirection.Equals(Direction.East))
        {
            roomIndex = currentRoomNumber + 1;
        }

        if (passageDirection.Equals(Direction.West))
        {
            roomIndex = currentRoomNumber - 1;
        }
        return roomIndex;
    }





    /**
     * Returns the position of the room in world space given the index for the room
     * */
    private Vector3 FindRoomPosition(int index)
    {
        float width = 6f;

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
    public Key GenerateUniqueKey(bool main)
    {
        Key uniqueKey = new Key();
        //TODO
        //ensure that it is unique. Set extraneous or main.
        return uniqueKey;
    }
}
