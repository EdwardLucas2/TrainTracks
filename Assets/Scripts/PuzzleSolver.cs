using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
//using Unity.Jobs;
//using Unity.Mathematics;
using Unity.Burst;
using System;

public class PuzzleSolver : MonoBehaviour
{
    private int[] columns, rows;
    private int finY, finX, mapSize, midX, midY;
    private List<List<Point>> hierachy= new List<List<Point>>();
    public PuzzleGraphicsGenerator completedPuzzleGraphicsGen;

    public void LoadPuzzle(int[] puzzleToLoad)
    {
        //Process puzzle data and save to vars
        mapSize = (puzzleToLoad.Length - 6)/2;

        columns = new int[mapSize];
        rows = new int[mapSize];

        //First mapsize entries are columns
        for(int i = 0; i < columns.Length; i++)
        {
            columns[i] = puzzleToLoad[i];
        }
        //Next mapsize entries are rows
        for(int z = 0; z < rows.Length; z++)
        {
            rows[z] = puzzleToLoad[z + mapSize];
        }

        //Next 2 entries are village A coords
        //Create First Point
        Point startPoint = new Point();
        startPoint.xPos = puzzleToLoad[(mapSize * 2) + 0];
        startPoint.yPos = puzzleToLoad[(mapSize * 2) + 1];
        startPoint.ancestors = new Point[0];

        //Next 2 entries are mid point coords, if don't exist are set to village A coords
        midX = puzzleToLoad[(mapSize * 2) + 2];
        midY = puzzleToLoad[(mapSize * 2) + 3];

        //Last 2 entries are village B coords
        finX = puzzleToLoad[(mapSize * 2) + 4];
        finY = puzzleToLoad[(mapSize * 2) + 5];
        
        StartCoroutine("FindNextPoints", startPoint);
    }

    private int[,] LoadExistanceInstance(Point originPoint)
    {
        Profiler.BeginSample("Load Existance Instance");

        int trackNum = 1;

        //Create array for this point instance
        int[,] map = new int[mapSize, mapSize];

        //Populate array with past tracks
        for(int i = 0; i < originPoint.ancestors.Length; i++)
        {
            map[originPoint.ancestors[i].xPos, originPoint.ancestors[i].yPos] = trackNum;
            trackNum++;
        }

        //Populate array with current point
        map[originPoint.xPos, originPoint.yPos] = trackNum;
        

        Profiler.EndSample();
        return map;
    }

    IEnumerator FindNextPoints(Point point) 
    {
        yield return new WaitForSeconds(0f);

        Profiler.BeginSample("Find Next Points");

        Debug.Log("Solving Puzzle...");
        int[,] mapAtInstance = LoadExistanceInstance(point);

        int xPos = point.xPos;
        int yPos = point.yPos;
        int currentHierachyLevel = point.ancestors.Length;

        //Check Right
        if (xPos < mapSize - 1)
        {
            if (mapAtInstance[xPos + 1, yPos] == 0)
            {
                //Right is possibility, create new point
                CreatePoint(xPos + 1, yPos, point);
            }
        }
        //Check Left
        if (xPos > 0)
        {
            if (mapAtInstance[xPos - 1, yPos] == 0)
            {
                //Left is possibility, create new point
                CreatePoint(xPos - 1, yPos, point);
            }
        }
        //Check Up
        if (yPos < mapSize - 1)
        {
            if (mapAtInstance[xPos, yPos + 1] == 0)
            {
                //Up is possibility, create new point
                CreatePoint(xPos, yPos + 1, point);
            }
        }
        //Check Down
        if (yPos > 0)
        {
            if (mapAtInstance[xPos, yPos - 1] == 0)
            {
                //Down is possibility, create new point
                CreatePoint(xPos, yPos - 1, point);
            }
        }

        Profiler.EndSample();
    }

    private void CreatePoint(int xPos, int yPos, Point parent)
    {
        Profiler.BeginSample("Create Point");

        int hierachyLevelToSpawn = parent.ancestors.Length + 1;

        //Create point to add
        Point point = new Point();
        point.xPos = xPos;
        point.yPos = yPos;
        point.ancestors = new Point[parent.ancestors.Length + 1];
        for(int i = 0; i < parent.ancestors.Length; i++)
        {
            point.ancestors[i] = parent.ancestors[i];
        }

        point.ancestors[parent.ancestors.Length] = parent;

        //Check if current hierachy level exists
        if (hierachy.Count <= hierachyLevelToSpawn)
        {
            //It doesn't exist, create it
            //Create hierachy level to add to hierachy
            List<Point> hierachyLevel = new List<Point>();
            hierachyLevel.Add(point);
            hierachy.Add(hierachyLevel);
        }
        else
        {
            //It exists, add it to hierachy
            hierachy[hierachyLevelToSpawn].Add(point);
        }

        //Check point for violation, completion or continuation
        int a = CheckPointForCompletionOrViolation(point);
        if (a > 0)
        {
            //either complete or has future
            if(a == 2)
            {
                //Complete!
                Debug.Log("Completed Puzzle!");
                //Randomly generated puzzle, no midpoint, show completion
                completedPuzzleGraphicsGen.InitializePuzzle(mapSize);
                completedPuzzleGraphicsGen.UpdateMap(LoadExistanceInstance(point));
            }
            else
            {
                //Not complete, continue
                StartCoroutine("FindNextPoints", point);
            }
        }

        Profiler.EndSample();
    }

    private int CheckPointForCompletionOrViolation(Point point)
    {
        Profiler.BeginSample("Check Point For Completion Or Violation");

        //0 = Violation, 1 = Continuation, 2 = Completion

        int[,] mapInstance = LoadExistanceInstance(point);
        bool completed = true;

        //Check columns
        for (int x = 0; x < mapSize; x++)
        {
            int numInCol = 0;

            for (int y = 0; y < mapSize; y++)
            {
                if (mapInstance[x, y] > 0)
                {
                    numInCol++;
                }
            }

            if (numInCol > columns[x])
            {
                Profiler.EndSample();
                return 0;
            }
            else if(numInCol != columns[x])
            {
                completed = false;
            }
        }

        //Check rows
        for (int y = 0; y < mapSize; y++)
        {
            int numInRow = 0;

            for (int x = 0; x < mapSize; x++)
            {
                if (mapInstance[x, y] > 0)
                {
                    numInRow++;
                }
            }

            if (numInRow > rows[y])
            {
                Profiler.EndSample();
                return 0;
            }
            else if(numInRow != rows[y])
            {
                completed = false;
            }
        }

        //Got here without returning, not in violation
        if (completed && point.xPos == finX && point.yPos == finY)
        {
            //At end, check if gone through mid point
            if(mapInstance[midX, midY] > 0)
            {
                //Gone through, complete
                Profiler.EndSample();
                print("Completition!");
                return 2;
            }
            else
            {
                print("Doesn't go through midpoint");
                Profiler.EndSample();
                return 0;
            }
        }
        else
        {
            Profiler.EndSample();
            return 1;
        }
    }

    private void PrintPuzzle(int[,] data)
    {
        string array = "";

        for (int y = data.GetLength(1) - 1; y > -1; y--)
        {
            for (int x = 0; x < data.GetLength(0); x++)
            {
                array += data[x, y];
                array += " ";
            }

            array += "\n";
        }

        Debug.Log(array);
    }

    public struct Point
    {
        public int xPos, yPos;
        //Ancestors does not include this point
        public Point[] ancestors;
    }
}
