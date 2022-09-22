using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator :MonoBehaviour
{
    [SerializeField] Vector2Int mapSize;
    [SerializeField] float minimumDevideSize;
    [SerializeField] float maximumDivideSize;
    [SerializeField] Vector2Int minimumMapSize;
    [SerializeField] private GameObject line;
    [SerializeField] private Transform lineHolder;
    [SerializeField] private GameObject rectangle;
    [SerializeField] private int maximumDepth;
    [SerializeField] Tilemap tileMap;
    [SerializeField] Tile roomTile;
    [SerializeField] Tile wallTile;
    [SerializeField] Tile exceptTile;
    [SerializeField] Camera camera;
    void Start()
    { //OnDrawRectangle(0, 0, mapSize.x, mapSize.y);
        MakeRoom();
    }
    void FillBackground()
    {
        for(int i = 0; i < mapSize.x; i++)
        {
            for(int j = 0; j < mapSize.y; j++)
            {
                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), exceptTile);
            }
        }
    }
    void FillWall()
    {      
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
               if(tileMap.GetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0)) == exceptTile)
                {
    
                    for(int x = -1; x <= 1; x++)
                    {
                        for(int y = -1; y <= 1; y++)
                        {
                            if (x == 0 && y == 0) continue;
                            if(tileMap.GetTile(new Vector3Int(i - mapSize.x / 2+x, j - mapSize.y / 2+y, 0)) == roomTile)
                            {
                                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0) , wallTile);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    void DivideTree(Tree tree,int n)
    {
        if (n == maximumDepth) return;
        int maxLength = Mathf.Max(tree.size.width, tree.size.height);
        int split = Mathf.RoundToInt(Random.Range(maxLength * minimumDevideSize, maxLength * maximumDivideSize));
        if (tree.size.width >= tree.size.height)
        {
            tree.leftTree = new Tree(new RectInt(tree.size.x,tree.size.y,split,tree.size.height));
            tree.rightTree= new Tree(new RectInt(tree.size.x+split, tree.size.y, tree.size.width-split, tree.size.height));
            OnDrawLine(new Vector2(tree.size.x + split, tree.size.y), new Vector2(tree.size.x + split, tree.size.y + tree.size.height));
        }
        else
        {
            tree.leftTree = new Tree(new RectInt(tree.size.x, tree.size.y, tree.size.width,split));
            tree.rightTree = new Tree(new RectInt(tree.size.x, tree.size.y + split, tree.size.width , tree.size.height-split));
            OnDrawLine(new Vector2(tree.size.x , tree.size.y+ split), new Vector2(tree.size.x + tree.size.width, tree.size.y  + split));
        }
        tree.leftTree.parTree = tree;
        tree.rightTree.parTree = tree;
        DivideTree(tree.leftTree, n + 1);
        DivideTree(tree.rightTree, n + 1);
    }
    private RectInt GenerateLeaf(Tree tree,int n)
    {
        RectInt rect;
        if (n == maximumDepth)
        {
           rect = tree.size;
           int width = Mathf.Max(Random.Range(rect.width/2,rect.width-1));
           int height=Mathf.Max(Random.Range(rect.height / 2,rect.height - 1));  
           int x = rect.x + Random.Range(1, rect.width - width);
           int y = rect.y + Random.Range(1, rect.height - height);        
           rect = new RectInt(x, y, width, height);
           FillRoom(rect);
        }
        else
        {
            tree.leftTree.roomSize = GenerateLeaf(tree.leftTree,n+1);
            tree.rightTree.roomSize = GenerateLeaf(tree.rightTree, n + 1);
            rect = tree.leftTree.roomSize;
        }
        return rect;
    }
    private void FillRoom(RectInt rect) {
    for(int i = rect.x; i< rect.x + rect.width; i++)
        {
            for(int j = rect.y; j < rect.y + rect.height; j++)
            {
                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), roomTile);
            }
        }
    }

    private void GenerateRoad(Tree tree,int n)
    {
        if (n == maximumDepth)
            return;
        Vector2Int leftTreeCenter = tree.leftTree.center;
        Vector2Int rightTreeCenter = tree.rightTree.center;
        for(int i=Mathf.Min(leftTreeCenter.x,rightTreeCenter.x);i<=Mathf.Max(leftTreeCenter.x, rightTreeCenter.x); i++)
        {
            tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, leftTreeCenter.y - mapSize.y / 2, 0), roomTile);
        }
        for (int j = Mathf.Min(leftTreeCenter.y, rightTreeCenter.y); j <= Mathf.Max(leftTreeCenter.y, rightTreeCenter.y); j++)
        {
            tileMap.SetTile(new Vector3Int(rightTreeCenter.x - mapSize.x / 2, j - mapSize.y / 2, 0), roomTile);
        }
        GenerateRoad(tree.leftTree, n + 1);
        GenerateRoad(tree.rightTree, n + 1);
    }


    private void OnDrawRectangle(int x, int y, float width, float height) //라인 렌더러를 이용해 사각형을 그리는 메소드
    {
        LineRenderer lineRenderer = Instantiate(rectangle,lineHolder).GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, new Vector2(x, y) - mapSize / 2); //위치를 화면 중앙에 맞춤
        lineRenderer.SetPosition(1, new Vector2(x + width, y) - mapSize / 2);
        lineRenderer.SetPosition(2, new Vector2(x + width, y + height) - mapSize / 2);
        lineRenderer.SetPosition(3, new Vector2(x, y + height) - mapSize / 2);
    }
    private void OnDrawLine(Vector2 from, Vector2 to) //라인 렌더러를 이용해 라인을 그리는 메소드
    {
        LineRenderer lineRenderer = Instantiate(line, lineHolder).GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, from - mapSize / 2);
        lineRenderer.SetPosition(1, to - mapSize / 2);
    }
    public void MakeRoom()
    {
        FillBackground();
        Tree root = new Tree(new RectInt(0, 0, mapSize.x, mapSize.y));
        DivideTree(root, 0);
        GenerateLeaf(root, 0);
        GenerateRoad(root, 0);
        FillWall();
    }
    
}
