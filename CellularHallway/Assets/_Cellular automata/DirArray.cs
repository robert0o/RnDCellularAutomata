using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class DirArray 
{
    public static Vector2Int[] directions = { 
        Vector2Int.left, 
        Vector2Int.right, 
        Vector2Int.up, 
        Vector2Int.down };

    public static Vector2Int[] diagonalDir =
    {
        Vector2Int.left + Vector2Int.up,
        Vector2Int.right + Vector2Int.up,
        Vector2Int.left + Vector2Int.down,
        Vector2Int.right + Vector2Int.down
    };

    public static Vector2Int[] aroundDir = {
        Vector2Int.left,
        Vector2Int.right,
        Vector2Int.up,
        Vector2Int.down,

        Vector2Int.left + Vector2Int.up,
        Vector2Int.right + Vector2Int.up,
        Vector2Int.left + Vector2Int.down,
        Vector2Int.right + Vector2Int.down
    };

}
