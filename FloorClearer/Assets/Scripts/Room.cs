using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    /**
     * 
     * Each Room contains:
     *  -Room Number
     *  -Room Position
     *  -Keys obtained on entering
     *  -List of passages
     *  -Keys to unlock the room
     *  -Start room identifier
     *  -End room identifier
     *  -Extraneous or main identifier
     * */
    private int roomNumber;
    private Vector3 roomPosition;
    private List<Key> keysOnEnter;
    private List<Generator.Direction> passageDirections;
    private Key keyToUnlock;
    private bool start;
    private bool end;
    private bool mainPath;

    public void SetRoomStats()
    {
        //SetRoomNumber();
        //SetRoomPosition();
        //SetKeysOnEnter();
        //SetStart();
        //SetEnd();
    }

    public void LockRoom(Key lockingKey)
    {
        this.keyToUnlock = lockingKey;
    }

    public List<Generator.Direction> GetPassageDirections()
    {
        return passageDirections;
    }

    public void SetPassageDirections(List<Generator.Direction> newDirections)
    {
        this.passageDirections = newDirections;
    }

    public bool GetMainPath()
    {
        return mainPath;
    }

    public void SetMainPath(bool mainPath)
    {
        this.mainPath = mainPath;
    }

    public int GetRoomNumber()
    {
        return roomNumber;
    }

    public bool GetStart()
    {
        return start;
    }

    public void SetRoomNumber(int number)
    {
        this.roomNumber = number;
    }

    public void SetRoomPosition(Vector3 position)
    {
        this.roomPosition = position;
    }

    public void SetKeysOnEnter(List<Key> keys)
    {
        this.keysOnEnter = keys;
    }

    public void SetKeyToUnlock(Key unlockKey)
    {
        this.keyToUnlock = unlockKey;
    }

    public void SetStart(bool start)
    {
        this.start = start;
    }

    public void SetEnd(bool end)
    {
        this.end = end;
    }
}
