using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree
{
    public Tree leftTree;
    public Tree rightTree;
    public Tree parTree;
    public RectInt size;
    public RectInt roomSize;
    public Vector2Int center
    {
        get {return new Vector2Int(roomSize.x+roomSize.width/2, roomSize.y + roomSize.height/ 2); }
    }
    public Tree(RectInt size)
    {
        this.size = size;
    }
}