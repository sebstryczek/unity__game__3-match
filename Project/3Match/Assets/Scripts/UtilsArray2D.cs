using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilsArray2D
{
    public struct Element
    {
        public int Row { get; private set; }
        public int Column { get; private set; }

        public Element(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }

    public static Element FindElement<T>(T[,] array, T elem)
    {
        int rowCount = array.GetLength(0);
        int colCount = array.GetLength(1);

        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            for (int colIndex = 0; colIndex < colCount; colIndex++)
            {
                if (array[rowIndex, colIndex].Equals(elem))
                {
                    return new Element(rowIndex, colIndex);
                }
            }
        }
        return new Element(-1, -1);
    }
    
}
