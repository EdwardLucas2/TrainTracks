using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MatrixGeneratorScript : MonoBehaviour
{
    public int minTrackNum, currentTrackNum = 1, maxTrackNum, rightBoost, leftBoost, upBoost, downBoost;
    public int mapSize;
    private int[,] map;
    public PuzzleGraphicsGenerator graphicsGen;
    //public PuzzleSolverMultiThreaded pSolverMT;
    public PuzzleSolver pSolver;

    private void Start()
    {
        Debug.Log("Generating Puzzle");
        if(mapSize < 6)
        {
            mapSize = 6;
        }

        Welcome();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Welcome();
        }
    }
    
    public void Welcome()
    {
        currentTrackNum = 1;
        map = new int[mapSize, mapSize];

        int y =  Random.Range(0, mapSize);
        map[0, y] = currentTrackNum;
        currentTrackNum++;
        graphicsGen.InitializePuzzle(mapSize);
        FindAdjacentAreas(0, y); 
    }

    public void FindAdjacentAreas(int x, int y)
    {
        //Debug.Log("Centre Track Pos xPos: " + x + " yPos: " + y);

        PossiblePos[] possiblePosArray = new PossiblePos[0];

        if (x < mapSize - 1)
        {
            if (map[x + 1, y] == 0)
            {
                //Track Not There, Possible
                PossiblePos[] oldArray = possiblePosArray;
                PossiblePos[] newArray = new PossiblePos[oldArray.Length + 1];

                for (int i = 0; i < oldArray.Length; i++)
                {
                    newArray[i] = oldArray[i];
                }

                newArray[0].oreintation = 1;
                newArray[0].xPos = x + 1;
                newArray[0].yPos = y;

                possiblePosArray = newArray;
            }
        }
        if (y < mapSize - 1)
        {
            if (map[x, y + 1] == 0)
            {
                //Track Not There, Possible
                PossiblePos[] oldArray = possiblePosArray;
                PossiblePos[] newArray = new PossiblePos[oldArray.Length + 1];

                for (int i = 0; i < oldArray.Length; i++)
                {
                    newArray[i] = oldArray[i];
                }

                newArray[oldArray.Length].oreintation = 4;
                newArray[oldArray.Length].xPos = x;
                newArray[oldArray.Length].yPos = y + 1;

                possiblePosArray = newArray;
            }
        }
        if (x > 0)
        {
            if (map[x - 1, y] == 0)
            {
                //Track Not There, Possible
                PossiblePos[] oldArray = possiblePosArray;
                PossiblePos[] newArray = new PossiblePos[oldArray.Length + 1];

                for (int i = 0; i < oldArray.Length; i++)
                {
                    newArray[i] = oldArray[i];
                }

                newArray[oldArray.Length].oreintation = 3;
                newArray[oldArray.Length].xPos = x - 1;
                newArray[oldArray.Length].yPos = y;

                possiblePosArray = newArray;
            }
        }
        if (y > 0)
        {
            if (map[x, y - 1] == 0)
            {
                //Track Not There, Possible
                PossiblePos[] oldArray = possiblePosArray;
                PossiblePos[] newArray = new PossiblePos[oldArray.Length + 1];

                for (int i = 0; i < oldArray.Length; i++)
                {
                    newArray[i] = oldArray[i];
                }

                newArray[oldArray.Length].oreintation = 2;
                newArray[oldArray.Length].xPos = x;
                newArray[oldArray.Length].yPos = y - 1;

                possiblePosArray = newArray;
            }
        }
        GenerateTrack(possiblePosArray);
    }

    public void GenerateTrack(PossiblePos[] posArray)
    {
        if (posArray.Length < 1)
        {
            //Regenerate Map
            Debug.Log("Regenerating");
            graphicsGen.UpdateMap(map);
            Welcome();
            return;
        }
        else
        {
            int[] edgePos = CheckForEdgePiece(posArray);
            if(currentTrackNum >= minTrackNum && edgePos != null)
            {
                map[edgePos[0], edgePos[1]] = currentTrackNum;
                if (ValidateMap(map))
                {
                    graphicsGen.UpdateMap(map);
                    //SavePuzzle("C:/Users/EPCL/Documents/other/TrainTrackPuzzles/", "testPuzzle.txt",ConvertMapToPuzzle(map));
                    StartCoroutine("WaitToSolve", map);
                }
                else
                {
                    Debug.Log("Regenerating");
                    graphicsGen.UpdateMap(map);
                    Welcome();
                }
                return;
            }
        }

        float basicSplit = 100 / posArray.Length;

        float rightWeight = 0, leftWeight = 0, upWeight = 0, downWeight = 0;

        foreach (PossiblePos pos in posArray)
        {
            //Debug.Log("oreintation: " + pos.oreintation + " xPos: " + pos.xPos + " yPos: " + pos.yPos);

            if (pos.oreintation == 1)
            {
                rightWeight = basicSplit + rightBoost;
            }
            if (pos.oreintation == 3)
            {
                leftWeight = basicSplit + leftBoost;
            }
            if (pos.oreintation == 2)
            {
                downWeight = basicSplit + downBoost;
            }
            if (pos.oreintation == 4)
            {
                upWeight = basicSplit + upBoost;
            }
        }

        int ranNum = Random.Range(0, (int)Mathf.Round(upWeight + downWeight + leftWeight + rightWeight));
        int newX = 0, newY = 0;

        if (ranNum <= rightWeight)
        {
            for (int i = 0; i < posArray.Length; i++)
            {
                if (posArray[i].oreintation == 1)
                {
                    map[posArray[i].xPos, posArray[i].yPos] = currentTrackNum;
                    newX = posArray[i].xPos;
                    newY = posArray[i].yPos;

                    //Check
                }
            }
        }
        else if (ranNum <= rightWeight + leftWeight)
        {
            for (int i = 0; i < posArray.Length; i++)
            {
                if (posArray[i].oreintation == 3)
                {
                    map[posArray[i].xPos, posArray[i].yPos] = currentTrackNum;
                    newX = posArray[i].xPos;
                    newY = posArray[i].yPos;
                }
            }
        }
        else if (ranNum <= rightWeight + leftWeight + upWeight)
        {
            for (int i = 0; i < posArray.Length; i++)
            {
                if (posArray[i].oreintation == 4)
                {
                    map[posArray[i].xPos, posArray[i].yPos] = currentTrackNum;
                    newX = posArray[i].xPos;
                    newY = posArray[i].yPos;
                }
            }
        }
        else if (ranNum <= rightWeight + leftWeight + upWeight + downWeight)
        {
            for (int i = 0; i < posArray.Length; i++)
            {
                if (posArray[i].oreintation == 2)
                {
                    map[posArray[i].xPos, posArray[i].yPos] = currentTrackNum;
                    newX = posArray[i].xPos;
                    newY = posArray[i].yPos;
                }
            }
        }
        else
        {
            //Regenerate Map
            Debug.LogError("weighting error");
            Debug.LogError(rightWeight + leftWeight + upWeight + downWeight);
            Welcome();
            return;
        }

        currentTrackNum++;

        if (currentTrackNum < maxTrackNum)
        {
            FindAdjacentAreas(newX, newY);
        }
        else
        {
            Debug.Log("At Max Track Num");
            graphicsGen.UpdateMap(map);
            return;
        }
    }

    IEnumerator WaitToSolve(int[,] map)
    {
        yield return new WaitForSeconds(0f);
        Debug.Log("solving");

        int[] puzzle = ConvertMapToPuzzle(map);

        /*string t = "";
        for (int i = 0; i < puzzle.Length; i++)
        {
            t += puzzle[i];
            t += " ";
        }
        print(t);
        */
        pSolver.LoadPuzzle(puzzle);
    }

    public int[] CheckForEdgePiece(PossiblePos[] posArrayToCheck)
    {
        foreach(PossiblePos pos in posArrayToCheck)
        {
            if(pos.xPos == 0 || pos.xPos == mapSize - 1 || pos.yPos == 0 || pos.yPos == mapSize - 1)
            {
                int[] array = new int[2];
                array[0] = pos.xPos;
                array[1] = pos.yPos;
                return array;
            }
        }

        return null;
    }

    public bool ValidateMap(int[,] mapToCheck)
    {
        //Find Max Track Num
        int maxTrackNum = 0;
        for(int x = 0; x < mapToCheck.GetLength(0); x++)
        {
            for (int y = 0; y < mapToCheck.GetLength(1); y++)
            {
                if(mapToCheck[x, y] > maxTrackNum)
                {
                    maxTrackNum = mapToCheck[x, y];
                }
            }
        }

        //Check all numbers below max track num exist
        for(int numToFind = 0; numToFind < maxTrackNum; numToFind++)
        {
            if(FindNum(numToFind, mapToCheck) == false)
            {
                return false;
            }
        }

        return true;
    }

    private bool FindNum(int numToFind, int[,] mapToCheck)
    {
        for (int x = 0; x < mapToCheck.GetLength(0); x++)
        {
            for (int y = 0; y < mapToCheck.GetLength(1); y++)
            {
                if (mapToCheck[x, y] == numToFind)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static void SavePuzzle(string path, string fileName, int[] puzzle)
    {
        try
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string array = "";
            for (int i = 0; i < puzzle.Length; i++)
            {
                array += puzzle[i];
            }

            System.IO.File.WriteAllText(path + fileName, array);
            print("Saved Puzzle");
        }
        catch (System.Exception ex)
        {
            string ErrorMessages = "File Write Error\n" + ex.Message;
            Debug.LogError(ErrorMessages);
        }
    }

    public int[] ConvertMapToPuzzle(int[,] mapToConvert)
    {
        //Calculate Map To Save
        //First 8 Num is column data
        //8-16 num is row data
        //Last 4 Num is first and last data

        int numTracks = 0;
        int[] data = new int[(mapSize * 2) + 6];
        int dataPlace = 0;

        //Fill out Column Data (First 8)
        for (int x = 0; x < mapToConvert.GetLength(0); x++)
        {
            int collData = 0;
            for (int y = 0; y < mapToConvert.GetLength(1); y++)
            {
                if (mapToConvert[x, y] > 0)
                {
                    collData++;
                }
            }
            numTracks += collData;
            data[dataPlace] = collData;
            dataPlace++;
        }

        //Fill Out Row Data
        for (int y = 0; y < mapToConvert.GetLength(1); y++)
        {
            int rData = 0;
            for (int x = 0; x < mapToConvert.GetLength(0); x++)
            {
                if (mapToConvert[x, y] > 0)
                {
                    rData++;
                }
            }
            data[dataPlace] = rData;
            dataPlace++;
        }

        //Find And Fill Out First track and mid track as first track
        for (int y = 0; y < mapToConvert.GetLength(1); y++)
        {
            for (int x = 0; x < mapToConvert.GetLength(0); x++)
            {
                if (mapToConvert[x, y] == 1)
                {
                    data[dataPlace] = x;
                    dataPlace++;
                    data[dataPlace] = y;
                    dataPlace++;
                    data[dataPlace] = x;
                    dataPlace++;
                    data[dataPlace] = y;
                    dataPlace++;
                }
            }
        }

        //Find and fill out last track
        for(int x = 0; x < mapToConvert.GetLength(0); x++)
        {
            for(int y = 0; y < mapToConvert.GetLength(1); y++)
            {
                if(mapToConvert[x, y] == numTracks)
                {
                    data[dataPlace] = x;
                    dataPlace++;
                    data[dataPlace] = y;
                }
            }
        }

        return data;
    }

    private void PrintPuzzle(int[] data)
    {
        string array = "";

        for (int i = 0; i < data.Length; i++)
        {
            array += data[i];
            array += " ";
        }

        Debug.Log(array);
    }

    public struct PossiblePos
    {
        //1 = right, 2 = down, 3 = left, 4 = up
        public int oreintation, xPos, yPos;
    }
}
 