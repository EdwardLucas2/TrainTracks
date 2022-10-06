using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleInputScript : MonoBehaviour
{
    public InputField columns, rows, startPoint, midPoint, endPoint;
    int[] puzzleArray;
    public PuzzleSolver pSolver;
    public GameObject inputs;

    public void FinishedInput()
    {
        if (!ValidateInput())
        {
            print("Invalid");
            return;
        }
        puzzleArray = new int[(columns.text.Length * 2) + 6];

        //Enter col data
        char[] colCharArray = columns.text.ToCharArray();

        for (int i = 0; i < colCharArray.Length; i++)
        {
            puzzleArray[i] = (int)char.GetNumericValue(colCharArray[i]);
        }

        //Enter row data
        char[] rowCharArray = rows.text.ToCharArray();
        for (int u = colCharArray.Length; u < rowCharArray.Length + colCharArray.Length; u++)
        {
            puzzleArray[u] = (int)char.GetNumericValue(rowCharArray[u - colCharArray.Length]);
        }

        //Start point data
        char[] startPointChar = startPoint.text.ToCharArray();
        puzzleArray[rowCharArray.Length + colCharArray.Length] = (int)char.GetNumericValue(startPointChar[0]);
        puzzleArray[rowCharArray.Length + colCharArray.Length + 1] = (int)char.GetNumericValue(startPointChar[1]);

        //mid and end point data
        char[] midPointArray = midPoint.text.ToCharArray();
        char[] endPointArray = endPoint.text.ToCharArray();
        puzzleArray[puzzleArray.Length - 2] = (int)char.GetNumericValue(endPointArray[0]);
        puzzleArray[puzzleArray.Length - 1] = (int)char.GetNumericValue(endPointArray[1]);

        //All data is inputted and valid, send to solver
        print("Valid");
        inputs.SetActive(false);
        SendDataToSolver(midPointArray);
    }

    private void SendDataToSolver(char[] midPoint)
    {
        if(midPoint.Length > 0)
        {
            //Mid point is distinct from start point, set it
            puzzleArray[puzzleArray.Length - 4] = (int)char.GetNumericValue(midPoint[0]);
            puzzleArray[puzzleArray.Length - 3] = (int)char.GetNumericValue(midPoint[1]);
        }
        else
        {
            //mid point is indisinct from start point, set mid = start
            puzzleArray[puzzleArray.Length - 4] = puzzleArray[puzzleArray.Length - 6];
            puzzleArray[puzzleArray.Length - 3] = puzzleArray[puzzleArray.Length - 5];
        }

        /*string t = "";
        for(int i = 0; i < puzzleArray.Length; i++)
        {
            t += puzzleArray[i];
            t += " ";
        }
        print(t);*/
        pSolver.LoadPuzzle(puzzleArray);
    }

    private bool ValidateInput()
    {
        if(columns.text.Length == rows.text.Length)
        {
            if(columns.text.Length > 4)
            {
                if(startPoint.text.Length == 2 && endPoint.text.Length == 2)
                {
                    if (midPoint.text.Length == 0 || midPoint.text.Length == 2)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
