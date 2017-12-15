using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    /**
     * All dimension data for rooms and the map
     */

    //Width of the map
    public int mapSizeX;

    ////Height of the map (Some day I may try to implement the floor with multiple levels!
    //public int MapSizeY;

    //Length of the map
    public int mapSizeZ;

    //The locaclScale for each room prefab
    public Vector3 roomSize;

    //the offset from 0 to the player's feet
    private float floorThickness;

    /**
     * All GameObjects associated with the game. This includes the main and extraneous keys and rooms + room prefabs
     */
    /** The possible room types that must be generated:
     * Index:     Room Type:
     * 0            Dead end
     * 1            Single door(left)
     * 2            Single door(top)
     * 3            Single door(right)
     * 4            Double door(left, top)
     * 5            Double door(top, right)
     * 6            Double door(left, right)
     * 7            Triple door(left, top and right)
     */
    public GameObject[] roomPrefabs;

    //Used to find the thickness of the floor
    //public GameObject floor;

    //A list with the currently created rooms
    private List<Room> createdRooms;

    //A list with the current edge rooms
    private List<Room> edgeRooms;

    //An array to hold the potential indices of the floor
    private List<int> potentialRoomHotspotIndices;

    //holds the current room when generating the floor
    private Room currentRoom;



    void Start()
    {
        roomSize = roomPrefabs[0].gameObject.transform.localScale;
        //floorThickness = floor.transform.localScale.y;
        floorThickness = .5f;

        if (mapSizeX < 3 || mapSizeZ < 3)
        {
            Debug.Log("Enter a different Size for the map.. Both sizes must be larger than 3.");
            return;
        }

        StartCoroutine(GenerateFloor());
    }


    /**
     * 
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
     */
    IEnumerator GenerateFloor()
    {
        //First, start by putting all possible hotspots into a data structure to keep track of if we have created specific room indices
        for (int index = 0; index < (mapSizeX * mapSizeZ); index++)
        {
            potentialRoomHotspotIndices.Add(index);
        }

        /*
         * Begin Start room portion
         * 
         */
        //TODO:  CheckRoomValidity(); 





        yield return 0;
    }

    /**
     * Generate Random Key
     */
    public Key GenerateUniqueKey()
    {
        Key uniqueKey = new Key();
        //TODO
        //ensure that it is unique. Set extraneous or main.
        return uniqueKey;
    }

    /**
     * 
     * Need the four neighbors to each room to determine the proper room prefabs to place down. Checking for open squares.
     * 
     */
    private Room GetRoomNeighbors(Room currentRoom)
    {
        //TODO
        return currentRoom;
    }

    private bool CheckRoomValidity(int roomIndex)
    {
        //TODO

        //Each time we have an index, we get the adjacent rooms.
        //make sure left room is NOT up a row in the last spot to the right (use modulo) Do for each possible adjacent room.

        return true;
    }




}
