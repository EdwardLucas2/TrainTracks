using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGraphicsGenerator : MonoBehaviour
{
    private int mapSize;
    private GameObject[,] spriteArray;
    public GameObject background, hor, ver, ltb, btr, ttl, ttr, parentObj;
    public bool isOffset;
    private float xOffset = 0;

    public void InitializePuzzle(int size)
    {
        mapSize = size;
        if (isOffset)
            xOffset = mapSize * 1.02f + 1;


        //Clear Sprite Array And Old Tiles
        if (spriteArray != null)
        {
            foreach (GameObject sprite in spriteArray)
            {
                Destroy(sprite);
            }
        }

        spriteArray = new GameObject[size, size];

        for(int x = 0; x < size; x++)
        {
            for(int y = 0; y < size; y++)
            {
                GameObject newTile = GameObject.Instantiate(background, new Vector3(x + xOffset, y, 0), Quaternion.identity, parentObj.transform);
                spriteArray[x, y] = newTile;
                newTile.name = "Tile X: " + x + " Y: " + y;
            }
        }
    }

    public void UpdateMap(int[,] newMapArray)
    {
        for (int x = 0; x < newMapArray.GetLength(0); x++)
        {
            for (int y = 0; y < newMapArray.GetLength(1); y++)
            {
                if (newMapArray[x, y] > 0)
                {
                    int trackType = GetTrackType(newMapArray[x, y], newMapArray);
                    Destroy(spriteArray[x, y]);

                    if (trackType == 1)
                    {
                        spriteArray[x, y] = GameObject.Instantiate(btr, new Vector3(x + xOffset, y, 0), Quaternion.identity, parentObj.transform);
                    }
                    else if(trackType == 2)
                    {
                        spriteArray[x, y] = GameObject.Instantiate(ttr, new Vector3(x + xOffset, y, 0), Quaternion.identity, parentObj.transform);
                    }
                    else if (trackType == 3)
                    {
                        spriteArray[x, y] = GameObject.Instantiate(ttl, new Vector3(x + xOffset, y, 0), Quaternion.identity, parentObj.transform);
                    }
                    else if (trackType == 4)
                    {
                        spriteArray[x, y] = GameObject.Instantiate(ltb, new Vector3(x + xOffset, y, 0), Quaternion.identity, parentObj.transform);
                    }
                    else if (trackType == 5)
                    {
                        spriteArray[x, y] = GameObject.Instantiate(hor, new Vector3(x + xOffset, y, 0), Quaternion.identity, parentObj.transform);
                    }
                    else if (trackType == 6)
                    {
                        spriteArray[x, y] = GameObject.Instantiate(ver, new Vector3(x + xOffset, y, 0), Quaternion.identity, parentObj.transform);
                    }
                }
            }
        }
    }

    private void PrintMap(int[,] map)
    {
        string lineToPrint = "";
        for (int y = map.GetLength(1) - 1; y > -1; y--)
        {
            lineToPrint += "\n";
            for (int x = 0; x < map.GetLength(0); x++)
            {
                if (map[x, y] < 10)
                {
                    lineToPrint += "  ";
                }
                else
                {
                    lineToPrint += " ";
                }

                lineToPrint += map[x, y];
            }
        }

        Debug.Log(lineToPrint);
    }

    private int GetTrackType(int trackNum, int[,] mapArray)
    {
        int prevX = 0, Xpos = 0, postX = 0, prevY = 0, Ypos = 0, postY = 0, numTracks = 0;

        //Find Variables
        for(int x = 0; x < mapArray.GetLength(0); x++)
        {
            for(int y = 0; y < mapArray.GetLength(1); y++)
            {
                if (mapArray[x, y] > 0)
                {
                    numTracks++;
                }
                if (mapArray[x, y] == trackNum)
                {
                    Xpos = x;
                    Ypos = y;
                }
                else if(mapArray[x, y] == trackNum - 1)
                {
                    prevX = x;
                    prevY = y;
                }
                else if (mapArray[x, y] == trackNum + 1)
                {
                    postX = x;
                    postY = y;
                }
            }
        }

        //Calculate
        //1 = rtb, 2 = rtt, 3 = ltt, 4 = ltb, 5 = horizontal, 6 = vertical
        #region Calculate And Return Track Type

        //For Starting and ending tracks
        if (trackNum == 1)
        {
            //Starting track, can be either, hor, ltt, ltb
            if(postX > Xpos)
            {
                return 5;
            }
            else if(postY > Ypos)
            {
                //left to top
                return 3;
            }
            else if(postY < Ypos)
            {
                //left to bottom
                return 4;
            }
            else
            {
                Debug.Log("Can't find track type");
                return 4;
            }
        }
        else if(trackNum == numTracks)
        {
            //Ending Track, must point towards edge of map
            if(Xpos == 0)
            {
                //On left of map, point left
                if(Ypos < prevY)
                {
                    //top to left
                    return 3;
                }
                else if(Ypos == prevY)
                {
                    //left to left (hor)
                    return 5;
                }
                else
                {
                    //bottom to left
                    return 4;
                }
            }
            else if(Xpos == mapSize - 1)
            {
                //On right of map, point right
                if (Ypos < prevY)
                {
                    //top to right
                    return 2;
                }
                else if (Ypos == prevY)
                {
                    //right to right (hor)
                    return 5;
                }
                else
                {
                    //bottom to right
                    return 1;
                }
            }
            else if(Ypos == 0)
            {
                //On bottom of map, point down
                if (Xpos < prevX)
                {
                    //right to bottom
                    return 1;
                }
                else if (Xpos == prevX)
                {
                    //down to down (vert)
                    return 6;
                }
                else
                {
                    //left to bottom
                    return 4;
                }
            }
            else if (Ypos == mapSize - 1)
            {
                //On top of map, point up
                if (Xpos < prevX)
                {
                    //right to top
                    return 2;
                }
                else if (Xpos == prevX)
                {
                    //up to up (vert)
                    return 6;
                }
                else
                {
                    //left to top
                    return 3;
                }
            }
            else
            {
                Debug.Log("Regenerating");
                return 0;
            }
        }

        //For Middle Tracks
        if (prevX == postX - 2 || prevX == postX + 2)
        {
            //Horizontal
            return 5;
        }
        else if(prevY == postY - 2 || prevY == postY + 2)
        {
            //Vertical
            return 6;
        }
        else if (Ypos > prevY)
        {
            //Going Up
            if(postX > prevX)
            {
                //bottom to right
                return 1;
            }
            else
            {
                //bottom to left
                return 4;
            }
        }
        else if(Ypos < prevY)
        {
            //Turning From Down To Left Or Right
            if (postX > prevX)
            {
                //top to right
                return 2;
            }
            else
            {
                //top to left
                return 3;
            }
        }
        else if(Ypos == prevY)
        {
            //Turning from left or right to up or down
            if(Xpos > prevX)
            {
                //Turning from left to up or down
                if(postY > Ypos)
                {
                    //Turning from left to up
                    return 3;
                }
                else
                {
                    //Turning from left to down
                    return 4;
                }
            }
            else
            {
                //Turning from right to up or down
                if (postY > Ypos)
                {
                    //Turning from right to up
                    return 2;
                }
                else
                {
                    //Turning from right to down
                    return 1;
                }
            }
        }
        else
        {
            Debug.LogError("Can't find track type, regenerating");
            //matrixGen.Welcome();
            return 0;
        }

        #endregion
    }
}
