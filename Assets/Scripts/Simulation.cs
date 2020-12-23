using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState 
{
    public TableObj map;
    public int[,] table;
    public int[,] pieces;
    public Vector2Int p1Key;
    public Vector2Int p2Key;
    public int[] p1Jail;
    public int[] p2Jail;
    public List<Vector2Int> p1PosibleInitialPositions = new List<Vector2Int>();
    public List<Vector2Int> p2PosibleInitialPositions = new List<Vector2Int>();
    public List<int> p1AvailablePieces = new List<int> { 111, 112, 113, 121, 122, 131, 141, 142, 151 };
    public List<int> p2AvailablePieces = new List<int> { 211, 212, 213, 221, 222, 231, 241, 242, 251 };
    public List<Vector2Int> moves = new List<Vector2Int> { };
    public int curPlayer;
    public int curPiece;
    public int numRows;
    public int numCols;
    public List<Vector3Int> p1AlivePieces = new List<Vector3Int>();
    public List<Vector3Int> p2AlivePieces = new List<Vector3Int>();

    #region Contructors
    public GameState(TableObj map) 
    {
        this.map = map;
        table = new int[map.numRows, map.numCols];
        pieces = new int[map.numRows, map.numCols];
        numRows = map.numRows; numCols = map.numCols;
        p1Jail = new int[3];
        p2Jail = new int[3];
        for (int i = 0; i < map.numRows; i++)
        {
            for (int j = 0; j < map.numCols; j++)
            {
                TableObj.pieceType type = map.rows[i].cols[j];
                if (type == TableObj.pieceType.P1ZONE || type == TableObj.pieceType.P1KEY)
                {
                    table[i, j] = 10;
                    if (type == TableObj.pieceType.P1KEY) p1Key = new Vector2Int(i, j);
                    p1PosibleInitialPositions.Add(new Vector2Int(i, j));
                }
                if (type == TableObj.pieceType.P2ZONE || type == TableObj.pieceType.P2KEY)
                {
                    table[i, j] = 20;
                    if (type == TableObj.pieceType.P2KEY) p1Key = new Vector2Int(i, j);
                    p2PosibleInitialPositions.Add(new Vector2Int(i, j));
                }

                if (type == TableObj.pieceType.P1REVIVE1) { table[i, j] = 501; p1PosibleInitialPositions.Add(new Vector2Int(i, j)); }
                if (type == TableObj.pieceType.P1REVIVE2) { table[i, j] = 502; p1PosibleInitialPositions.Add(new Vector2Int(i, j)); }
                if (type == TableObj.pieceType.P1REVIVE3) { table[i, j] = 503; p1PosibleInitialPositions.Add(new Vector2Int(i, j)); }

                if (type == TableObj.pieceType.P2REVIVE1) { table[i, j] = 501; p2PosibleInitialPositions.Add(new Vector2Int(i, j)); }
                if (type == TableObj.pieceType.P2REVIVE2) { table[i, j] = 502; p2PosibleInitialPositions.Add(new Vector2Int(i, j)); }
                if (type == TableObj.pieceType.P2REVIVE3) { table[i, j] = 503; p2PosibleInitialPositions.Add(new Vector2Int(i, j)); }

                if (type == TableObj.pieceType.BASIC1) table[i, j] = 1;
                if (type == TableObj.pieceType.BASIC2) table[i, j] = -1;
                if (type == TableObj.pieceType.BASIC) table[i, j] = 0;

                if (type == TableObj.pieceType.OBSTACLE) table[i, j] = 99;
            }
        }
        for (int i = 0; i < map.numRows; i++)
        {
            for (int j = 0; j < map.numCols; j++)
            {
                pieces[i, j] = 0;
            }
        }
        int r;
        while (p1AvailablePieces.Count > 0)
        {
            r = (int)UnityEngine.Random.Range(0, p1PosibleInitialPositions.Count);
            pieces[p1PosibleInitialPositions[r].x, p1PosibleInitialPositions[r].y] = p1AvailablePieces[0];
            p1AlivePieces.Add(new Vector3Int(p1PosibleInitialPositions[r].x, p1PosibleInitialPositions[r].y, p1AvailablePieces[0]));
            p1PosibleInitialPositions.RemoveAt(r);
            p1AvailablePieces.RemoveAt(0);
        }
        r = 0;
        while (p2AvailablePieces.Count > 0)
        {
            r = (int)UnityEngine.Random.Range(0, p2PosibleInitialPositions.Count);
            pieces[p2PosibleInitialPositions[r].x, p2PosibleInitialPositions[r].y] = p2AvailablePieces[0];
            p2AlivePieces.Add(new Vector3Int(p2PosibleInitialPositions[r].x, p2PosibleInitialPositions[r].y, p2AvailablePieces[0]));
            p2PosibleInitialPositions.RemoveAt(r);
            p2AvailablePieces.RemoveAt(0);
        }
    }

    public GameState(GameState oldState)
    {
        Debug.Log("State Created");
        numRows = oldState.numRows;
        numCols = oldState.numCols;
        map = oldState.map;
        p1Key = oldState.p1Key;
        p2Key = oldState.p2Key;
        curPlayer = oldState.curPlayer;
        curPiece = oldState.curPiece;

        table = new int[numRows, numCols];
        for (int i = 0; i < numRows; i++) 
        {
            for (int j = 0; j < numCols; j++) 
            {
                table[i, j] = oldState.table[i, j];
            }
        }

        pieces = new int[numRows, numCols];
        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numCols; j++)
            {
                pieces[i, j] = oldState.pieces[i, j];
            }
        }

        p1Jail = new int[3];
        for (int i = 0; i < 3; i++)
        {
            p1Jail[i] = oldState.p1Jail[i];
        }

        p2Jail = new int[3];
        for (int i = 0; i < 3; i++)
        {
            p1Jail[i] = oldState.p1Jail[i];
        }

        for (int i = 0; i < oldState.p1AlivePieces.Count; i++)
        {
            p1AlivePieces.Add(new Vector3Int(oldState.p1AlivePieces[i].x, oldState.p1AlivePieces[i].y, oldState.p1AlivePieces[i].z));
        }

        for (int i = 0; i < oldState.p2AlivePieces.Count; i++)
        {
            p2AlivePieces.Add(new Vector3Int(oldState.p2AlivePieces[i].x, oldState.p2AlivePieces[i].y, oldState.p2AlivePieces[i].z));
        }
    }
    #endregion

    #region Piece Movements
    public void GetMoves(int r, int c)
    {
        Debug.Log("Num moves prev: " + moves.Count);
        int cellPos = 0;
        moves.Clear();
        switch (curPiece)
        {
            //PAWN
            case 111:
            case 112:
            case 113:
            case 211:
            case 212:
            case 213:
                Debug.Log("PAWN moves prev: " + moves.Count);
                if (ValidatePosition(r, c + 1, false, curPlayer)) moves.Add(new Vector2Int(r, c + 1));
                if (ValidatePosition(r + 1, c + 1, true, curPlayer)) moves.Add(new Vector2Int(r + 1, c + 1));
                if (ValidatePosition(r + 1, c, false, curPlayer)) moves.Add(new Vector2Int(r + 1, c));
                if (ValidatePosition(r + 1, c - 1, true, curPlayer)) moves.Add(new Vector2Int(r + 1, c - 1));
                if (ValidatePosition(r, c - 1, false, curPlayer)) moves.Add(new Vector2Int(r, c - 1));
                if (ValidatePosition(r - 1, c - 1, true, curPlayer)) moves.Add(new Vector2Int(r - 1, c - 1));
                if (ValidatePosition(r - 1, c, false, curPlayer)) moves.Add(new Vector2Int(r - 1, c));
                if (ValidatePosition(r - 1, c + 1, true, curPlayer)) moves.Add(new Vector2Int(r - 1, c + 1));
                Debug.Log("PAWN moves post: " + moves.Count);
                break;
            //ROOK
            case 121:
            case 122:
            case 221:
            case 222:
                cellPos = 1;
                while (r + cellPos < numRows)
                {
                    if (ValidatePosition(r + cellPos, c, true, curPlayer))
                    {
                        moves.Add(new Vector2Int(r + cellPos, c));
                        if (pieces[r + cellPos, c] != -1 && pieces[r + cellPos, c] != 0 && pieces[r + cellPos, c] != 1) break;
                        cellPos++;
                    }
                    else break;
                }

                cellPos = 1;

                while (c + cellPos < numCols)
                {
                    if (ValidatePosition(r, c + cellPos, true, curPlayer))
                    {
                        moves.Add(new Vector2Int(r, c + cellPos));
                        if (pieces[r, c + cellPos] != -1 && pieces[r, c + cellPos] != 0 && pieces[r, c + cellPos] != 1) break;
                        cellPos++;
                    }
                    else break;
                }

                cellPos = -1;

                while (r + cellPos >= 0)
                {
                    if (ValidatePosition(r + cellPos, c, true, curPlayer))
                    {
                        moves.Add(new Vector2Int(r + cellPos, c));
                        if (pieces[r + cellPos, c] != -1 && pieces[r + cellPos, c] != 0 && pieces[r + cellPos, c] != 1) break;
                        cellPos--;
                    }
                    else break;
                }

                cellPos = -1;

                while (c + cellPos >= 0)
                {
                    if (ValidatePosition(r, c + cellPos, true, curPlayer))
                    {
                        moves.Add(new Vector2Int(r, c + cellPos));
                        if (pieces[r, c + cellPos] != -1 && pieces[r, c + cellPos] != 0 && pieces[r, c + cellPos] != 1) break;
                        cellPos--;
                    }
                    else break;
                }
                break;
            //KHIGHT
            case 131:
            case 231:
                if (ValidatePosition(r + 2, c + 1, true, curPlayer)) moves.Add(new Vector2Int(r + 2, c + 1));
                if (ValidatePosition(r + 2, c - 1, true, curPlayer)) moves.Add(new Vector2Int(r + 2, c - 1));
                if (ValidatePosition(r - 2, c + 1, true, curPlayer)) moves.Add(new Vector2Int(r - 2, c + 1));
                if (ValidatePosition(r - 2, c - 1, true, curPlayer)) moves.Add(new Vector2Int(r - 2, c - 1));
                if (ValidatePosition(r + 1, c - 2, true, curPlayer)) moves.Add(new Vector2Int(r + 1, c - 2));
                if (ValidatePosition(r - 1, c - 2, true, curPlayer)) moves.Add(new Vector2Int(r - 1, c - 2));
                if (ValidatePosition(r + 1, c + 2, true, curPlayer)) moves.Add(new Vector2Int(r + 1, c + 2));
                if (ValidatePosition(r - 1, c + 2, true, curPlayer)) moves.Add(new Vector2Int(r - 1, c + 2));
                break;
            //BISHOP
            case 141:
            case 142:
            case 241:
            case 242:
                if (ValidatePosition(r, c + 1, false, curPlayer)) moves.Add(new Vector2Int(r, c + 1));
                if (ValidatePosition(r + 1, c, false, curPlayer)) moves.Add(new Vector2Int(r + 1, c));
                if (ValidatePosition(r, c - 1, false, curPlayer)) moves.Add(new Vector2Int(r, c - 1));
                if (ValidatePosition(r - 1, c, false, curPlayer)) moves.Add(new Vector2Int(r - 1, c));
                cellPos = 1;
                while (r + cellPos < numRows && c + cellPos < numCols)
                {
                    if (ValidatePosition(r + cellPos, c + cellPos, true, curPlayer))
                    {
                        moves.Add(new Vector2Int(r + cellPos, c + cellPos));
                        if (pieces[r + cellPos, c + cellPos] != -1 && pieces[r + cellPos, c + cellPos] != 0 && pieces[r + cellPos, c + cellPos] != 1) break;
                        cellPos++;
                    }
                    else break;
                }

                cellPos = 1;

                while (r + cellPos < numRows && c - cellPos >= 0)
                {
                    if (ValidatePosition(r + cellPos, c - cellPos, true, curPlayer))
                    {
                        moves.Add(new Vector2Int(r + cellPos, c - cellPos));
                        if (pieces[r + cellPos, c - cellPos] != -1 && pieces[r + cellPos, c - cellPos] != 0 && pieces[r + cellPos, c - cellPos] != 1) break;
                        cellPos++;
                    }
                    else break;
                }

                cellPos = 1;

                while (r - cellPos >= 0 && c + cellPos < numCols)
                {
                    if (ValidatePosition(r - cellPos, c + cellPos, true, curPlayer))
                    {
                        moves.Add(new Vector2Int(r - cellPos, c + cellPos));
                        if (pieces[r - cellPos, c + cellPos] != -1 && pieces[r - cellPos, c + cellPos] != 0 && pieces[r - cellPos, c + cellPos] != 1) break;
                        cellPos++;
                    }
                    else break;
                }

                cellPos = -1;

                while (r + cellPos >= 0 && c + cellPos >= 0)
                {
                    if (ValidatePosition(r + cellPos, c + cellPos, true, curPlayer))
                    {
                        moves.Add(new Vector2Int(r + cellPos, c + cellPos));
                        if (pieces[r + cellPos, c + cellPos] != -1 && pieces[r + cellPos, c + cellPos] != 0 && pieces[r + cellPos, c + cellPos] != 1) break;
                        cellPos--;
                    }
                    else break;
                }
                break;
            //QUEEN
            case 151:
            case 251:
                //Rectas
                cellPos = 1;
                while (r + cellPos < numRows)
                {
                    if (ValidatePosition(r + cellPos, c, true, curPlayer))
                    {
                        moves.Add(new Vector2Int(r + cellPos, c));
                        if (pieces[r + cellPos, c] != -1 && pieces[r + cellPos, c] != 0 && pieces[r + cellPos, c] != 1) break;
                        cellPos++;
                    }
                    else break;
                }

                cellPos = 1;

                while (c + cellPos < numCols)
                {
                    if (ValidatePosition(r, c + cellPos, true, curPlayer))
                    {
                        moves.Add(new Vector2Int(r, c + cellPos));
                        if (pieces[r, c + cellPos] != -1 && pieces[r, c + cellPos] != 0 && pieces[r, c + cellPos] != 1) break;
                        cellPos++;
                    }
                    else break;
                }

                cellPos = -1;

                while (r + cellPos >= 0)
                {
                    if (ValidatePosition(r + cellPos, c, true, curPlayer))
                    {
                        moves.Add(new Vector2Int(r + cellPos, c));
                        if (pieces[r + cellPos, c] != -1 && pieces[r + cellPos, c] != 0 && pieces[r + cellPos, c] != 1) break;
                        cellPos--;
                    }
                    else break;
                }

                cellPos = -1;

                while (c + cellPos >= 0)
                {
                    if (ValidatePosition(r, c + cellPos, true, curPlayer))
                    {
                        moves.Add(new Vector2Int(r, c + cellPos));
                        if (pieces[r, c + cellPos] != -1 && pieces[r, c + cellPos] != 0 && pieces[r, c + cellPos] != 1) break;
                        cellPos--;
                    }
                    else break;
                }

                //Diagonales
                cellPos = 1;
                while (r + cellPos < numRows && c + cellPos < numCols)
                {
                    if (ValidatePosition(r + cellPos, c + cellPos, true, curPlayer))
                    {
                        moves.Add(new Vector2Int(r + cellPos, c + cellPos));
                        if (pieces[r + cellPos, c + cellPos] != -1 && pieces[r + cellPos, c + cellPos] != 0 && pieces[r + cellPos, c + cellPos] != 1) break;
                        cellPos++;
                    }
                    else break;
                }

                cellPos = 1;

                while (r + cellPos < numRows && c - cellPos >= 0)
                {
                    if (ValidatePosition(r + cellPos, c - cellPos, true, curPlayer))
                    {
                        moves.Add(new Vector2Int(r + cellPos, c - cellPos));
                        if (pieces[r + cellPos, c - cellPos] != -1 && pieces[r + cellPos, c - cellPos] != 0 && pieces[r + cellPos, c - cellPos] != 1) break;
                        cellPos++;
                    }
                    else break;
                }

                cellPos = 1;

                while (r - cellPos >= 0 && c + cellPos < numCols)
                {
                    if (ValidatePosition(r - cellPos, c + cellPos, true, curPlayer))
                    {
                        moves.Add(new Vector2Int(r - cellPos, c + cellPos));
                        if (pieces[r - cellPos, c + cellPos] != -1 && pieces[r - cellPos, c + cellPos] != 0 && pieces[r - cellPos, c + cellPos] != 1) break;
                        cellPos++;
                    }
                    else break;
                }

                cellPos = -1;

                while (r + cellPos >= 0 && c + cellPos >= 0)
                {
                    if (ValidatePosition(r + cellPos, c + cellPos, true, curPlayer))
                    {
                        moves.Add(new Vector2Int(r + cellPos, c + cellPos));
                        if (pieces[r + cellPos, c + cellPos] != -1 && pieces[r + cellPos, c + cellPos] != 0 && pieces[r + cellPos, c + cellPos] != 1) break;
                        cellPos--;
                    }
                    else break;
                }
                break;
        }
        Debug.Log("Num moves post: " + moves.Count);
    }
    private bool ValidatePosition(int r, int c, bool canKill, int player)
    {
        if (r < 0 || c < 0 || r >= numRows || c >= numCols) return false;
        if (table[r, c] == 99) return false;
        if (pieces[r, c] == 0) return true;
        if (pieces[r, c] >= 111 && pieces[r, c] <= 151 && player == 1) return false;
        if (pieces[r, c] >= 111 && pieces[r, c] <= 151 && player == 2) return canKill;
        if (pieces[r, c] >= 211 && pieces[r, c] <= 251 && player == 2) return false;
        if (pieces[r, c] >= 211 && pieces[r, c] <= 251 && player == 1) return canKill;
        return false;
    }
    public void MovePiece(int player, int r2, int c2) 
    {
        int r1 = -1;
        int c1 = -1;
        for (int i = 0; i < numRows; i++) 
        {
            for (int j = 0; j < numCols; j++) 
            {
                if (curPiece == pieces[i, j])
                {
                    r1 = i; c1 = j;
                    break;
                }
            }
        }
        if (pieces[r2, c2] != 0) 
        {
            int eatenPiece = pieces[r2,c2];
            int[] chosenJail;
            if (player == 1)
            {
                chosenJail = p2Jail;
            }
            else 
            {
                chosenJail = p1Jail;
            }

                if (chosenJail[2] != 0)
                {
                    if (player == 1) 
                    {
                        for (int i = 0; i < p1AlivePieces.Count; i++) 
                        {
                            if (p1AlivePieces[i].z == chosenJail[2]) 
                            {
                                p1AlivePieces.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    chosenJail[2] = 0;
                }

                if (chosenJail[1] != 0)
                {
                    chosenJail[2] = chosenJail[1];
                    chosenJail[1] = 0;
                }

                if (chosenJail[0] != 0)
                {
                    chosenJail[1] = chosenJail[0];
                    chosenJail[0] = 0;
                }

                chosenJail[0] = (eatenPiece);
        }
        pieces[r2, c2] = curPiece;
    }
    #endregion
    public bool inJail(int player, int piece) 
    {
        if (player == 1) 
        {
            if (piece == p2Jail[0] || piece == p2Jail[1] || piece == p2Jail[2]) return true;
        }
        else
        {
            if (piece == p1Jail[0] || piece == p1Jail[1] || piece == p1Jail[2]) return true;
        }
        return false;
    }
}
public class Simulation : MonoBehaviour
{
    [SerializeField] private TableObj map;

    private Stack<GameState> gameStates = new Stack<GameState>();
    private GameState initialGameState;

    /*
     0 BASIC
     1 BASIC1
     -1 BASIC2
     10 P1Zone
     20 P2Zone
     99 Obstacle
     REVIVES
        PLAYER1
            501 REVIVE1
            502 REVIVE2
            503 REVIVE3
        PLAYER2
            504 REVIVE1
            505 REVIVE2
            506 REVIVE3
     PLAYER 1:
        111 PAWN 1
        112 PAWN 2
        113 PAWN 3
        121 ROOK 1
        122 ROOK 2
        131 KHIGHT 
        141 BISHOP 1
        142 BISHOP 2
        151 QUEEN
    PLAYER 2:
        211 PAWN 1
        212 PAWN 2
        213 PAWN 3
        221 ROOK 1
        222 ROOK 2
        231 KHIGHT 
        241 BISHOP 1
        242 BISHOP 2
        251 QUEEN
     */

    #region Map Visualization
    public void Start()
    {
        initialGameState = new GameState(map);
        gameStates.Push(initialGameState);
        Debug.Log(alphaBeta(3, 1, -999999, 999999));
    }
    private void PrintMap(int[,] t) 
    {
        string a = "";
        for (int i = map.numCols - 1; i >= 0; i--)
        {
            for (int j = 0; j < map.numRows; j++)
            {
                a += "[" + t[i,j];
                if (t[i, j] >= 9 && t[i, j] < 100)
                    a += "_";
                if (t[i, j] >= 0 && t[i, j] < 10)
                    a += "__";
                if (t[i, j] >= -9 && t[i, j] < 0)
                    a += "_";
                a += "] ";
            }
            a += "\n";
        }
        Debug.Log(a);
    }
    #endregion

    #region MinMax Algorithm
    public float alphaBeta(int profundidad, int player, float alpha, float beta)
    {
        Debug.Log("Aqui entro");
        GameState gs = gameStates.Peek();
        gameStates.Peek().curPlayer = player;
        if (profundidad == 0) return heuristica(player);
        if (player == 1)
        {
            Debug.Log("p1 Alive pieces: "+gameStates.Peek().p1AlivePieces.Count);
            for (int i = 0; i < gameStates.Peek().p1AlivePieces.Count; i++)
            {
                Vector3Int p = gameStates.Peek().p1AlivePieces[i];
                if (!gameStates.Peek().inJail(1,p.z))
                {
                    gameStates.Peek().curPiece = p.z;
                    gameStates.Peek().GetMoves(p.x, p.y);
                    foreach (Vector2Int v in gameStates.Peek().moves)
                    {
                        gameStates.Push(new GameState(gs));
                        gameStates.Peek().MovePiece(1, v.x, v.y);
                        alpha = Math.Max(alpha, alphaBeta(profundidad - 1, 2, alpha, beta));
                        if (beta <= alpha) return alpha;
                    }
                }
            }
            return alpha;
        }
        else
        {
            for (int i = 0; i < gameStates.Peek().p2AlivePieces.Count; i++)
            {
                Vector3Int p = gameStates.Peek().p2AlivePieces[i];
                if (!gameStates.Peek().inJail(1, p.z))
                {
                    gameStates.Peek().curPiece = p.z;
                    gameStates.Peek().GetMoves(p.x, p.y);
                    foreach (Vector2Int v in gameStates.Peek().moves)
                    {
                        gameStates.Push(new GameState(gs));
                        gameStates.Peek().MovePiece(1, v.x, v.y);
                        beta = Math.Min(beta, alphaBeta(profundidad - 1, 1, alpha, beta));
                        if (beta <= alpha) return beta;
                    }
                }
            }
            return beta;
        }
    }
    private int heuristica(int player)
    {
        GameState gs = gameStates.Pop();
        if (player == 1)
        {
            return gs.p1AlivePieces.Count - gs.p2AlivePieces.Count;
        }
        else
        {
            return gs.p2AlivePieces.Count - gs.p1AlivePieces.Count;
        }
    }
    #endregion
}
