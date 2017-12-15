using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    enum Direction { North, East, South, West};
    
    /**
     * 
     * Each Room contains:
     *  -Room Number
     *  -Room Position
     *  -Keys obtained on entering
     *  -List of passages
     *  -Keys in the corresponding order for those passages (NULL if no key for a particular passageway)
     *  -Extraneous or main identifier
     */
    private int roomNumber;
    private Vector3 roomPosition;
    private List<Key> keysOnEnter;
    private List<Direction> passageDirections;
    private List<Key> correspondingKeys;
    private bool mainPath;

    public void SetRoomStats()
    {

    }



}
