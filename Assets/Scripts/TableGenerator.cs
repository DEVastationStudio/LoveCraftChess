using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableGenerator : MonoBehaviour
{
    [SerializeField] private TableObj tableObj;
    [SerializeField] private Cell cellUnit;
    [SerializeField] private Piece pieceRef;

    public Cell[,] cells;
    public List<Cell> p1Start;
    public List<Cell> p2Start;

    private int curPlayer;
    private Piece curPiece;
    private int cellPos = 1;

    void Start()
    {
        curPlayer = 1;
        GenerateTable();
    }

    private void GenerateTable()
    {
        int numRows = tableObj.numRows;
        int numCols = tableObj.numCols;
        cells = new Cell[numRows,numCols];
        p1Start = new List<Cell>();
        p2Start = new List<Cell>();
        TableObj.pieceType type;

        for (int i = 0; i<numRows; i++) 
        {
            for (int j = 0; j < numCols; j++) 
            {
                type = tableObj.rows[i].cols[j];
                cells[i, j] = Instantiate(cellUnit, new Vector3(j,0,i), Quaternion.identity);
                cells[i, j].Init(type, i, j, this);
                if (type == TableObj.pieceType.P1ZONE || type == TableObj.pieceType.P1KEY) 
                {
                    p1Start.Add(cells[i,j]);
                }
                if (type == TableObj.pieceType.P2ZONE || type == TableObj.pieceType.P2KEY)
                {
                    p2Start.Add(cells[i, j]);
                }
            }
        }

        foreach (Cell c in p1Start) 
        {
            c.ChangePiece(Instantiate(pieceRef, new Vector3(c.c, 1f, c.r), Quaternion.identity));
            c.getPiece().Init(Piece.pieces.KNIGHT, c.r, c.c, this, 1);
        }
        foreach (Cell c in p2Start)
        {
            c.ChangePiece(Instantiate(pieceRef, new Vector3(c.c, 1f, c.r), Quaternion.identity));
            c.getPiece().Init(Piece.pieces.KNIGHT, c.r, c.c, this, 2);
        }
    }
    
    public void SelectPiece(int r, int c) 
    {
        curPiece?.SetChosen(false);
        Piece piece = cells[r, c].getPiece();
        if (piece.player != curPlayer) return;
        curPiece = piece;
        curPiece?.SetChosen(true);
        GetMoves(curPiece.type, r, c);
    }
    public void GetMoves(Piece.pieces t, int r, int c) 
    {
        ResetClickables();
        switch (t) 
        {
            case Piece.pieces.PAWN:
                if (ValidatePosition(r, c+1, false, curPlayer)) cells[r, c+1].SetClickable(true);
                if (ValidatePosition(r+1, c+1, true, curPlayer)) cells[r+1, c+1].SetClickable(true);
                if (ValidatePosition(r+1, c, false, curPlayer)) cells[r+1, c].SetClickable(true);
                if (ValidatePosition(r+1, c-1, true, curPlayer)) cells[r+1, c-1].SetClickable(true);
                if (ValidatePosition(r, c-1, false, curPlayer)) cells[r, c-1].SetClickable(true);
                if (ValidatePosition(r-1, c-1, true, curPlayer)) cells[r-1, c-1].SetClickable(true);
                if (ValidatePosition(r-1, c, false, curPlayer)) cells[r-1, c].SetClickable(true);
                if (ValidatePosition(r-1, c+1, true, curPlayer)) cells[r-1, c+1].SetClickable(true);
                break;
            case Piece.pieces.ROOK:
                cellPos = 1;
                while (r+cellPos<tableObj.numRows) 
                {
                    if (ValidatePosition(r + cellPos, c, true, curPlayer))
                    {
                        cells[r + cellPos, c].SetClickable(true);
                        if (cells[r + cellPos, c].getPiece() != null) break;
                        cellPos++;
                    }
                    else break;
                }

                cellPos = 1;

                while (c+cellPos < tableObj.numCols)
                {
                    if (ValidatePosition(r, c + cellPos, true, curPlayer))
                    {
                        cells[r, c + cellPos].SetClickable(true);
                        if (cells[r, c + cellPos].getPiece() != null) break;
                        cellPos++;
                    }
                    else break;
                }

                cellPos = -1;
                
                while (r+cellPos >= 0)
                {
                    if (ValidatePosition(r + cellPos, c, true, curPlayer))
                    {
                        cells[r + cellPos, c].SetClickable(true);
                        if (cells[r + cellPos, c].getPiece() != null) break;
                        cellPos--;
                    }
                    else break;
                }

                cellPos = -1;

                while (c+cellPos >= 0)
                {
                    if (ValidatePosition(r, c + cellPos, true, curPlayer))
                    {
                        cells[r, c + cellPos].SetClickable(true);
                        if (cells[r, c + cellPos].getPiece() != null) break;
                        cellPos--;
                    }
                    else break;
                }
                break;
            case Piece.pieces.KNIGHT:
                if (ValidatePosition(r + 2, c + 1, true, curPlayer)) cells[r + 2, c + 1].SetClickable(true);
                if (ValidatePosition(r + 2, c - 1, true, curPlayer)) cells[r + 2, c - 1].SetClickable(true);
                if (ValidatePosition(r - 2, c + 1, true, curPlayer)) cells[r - 2, c + 1].SetClickable(true);
                if (ValidatePosition(r - 2, c - 1, true, curPlayer)) cells[r - 2, c - 1].SetClickable(true);
                if (ValidatePosition(r + 1, c - 2, true, curPlayer)) cells[r + 1, c - 2].SetClickable(true);
                if (ValidatePosition(r - 1, c - 2, true, curPlayer)) cells[r - 1, c - 2].SetClickable(true);
                if (ValidatePosition(r + 1, c + 2, true, curPlayer)) cells[r + 1, c + 2].SetClickable(true);
                if (ValidatePosition(r - 1, c + 2, true, curPlayer)) cells[r - 1, c + 2].SetClickable(true);
                break;
            case Piece.pieces.BISHOP:
                if (ValidatePosition(r, c + 1, true, curPlayer)) cells[r, c + 1].SetClickable(true);
                if (ValidatePosition(r + 1, c, true, curPlayer)) cells[r + 1, c].SetClickable(true);
                if (ValidatePosition(r, c - 1, true, curPlayer)) cells[r, c - 1].SetClickable(true);
                if (ValidatePosition(r - 1, c, true, curPlayer)) cells[r - 1, c].SetClickable(true);
                cellPos = 1;
                while (r + cellPos < tableObj.numRows && c + cellPos < tableObj.numCols)
                {
                    if (ValidatePosition(r + cellPos, c + cellPos, true, curPlayer))
                    {
                        cells[r + cellPos, c + cellPos].SetClickable(true);
                        if (cells[r + cellPos, c + cellPos].getPiece() != null) break;
                        cellPos++;
                    }
                    else break;
                }

                cellPos = 1;

                while (r + cellPos < tableObj.numRows && c - cellPos >= 0)
                {
                    if (ValidatePosition(r + cellPos, c - cellPos, true, curPlayer))
                    {
                        cells[r + cellPos, c - cellPos].SetClickable(true);
                        if (cells[r + cellPos, c - cellPos].getPiece() != null) break;
                        cellPos++;
                    }
                    else break;
                }

                cellPos = 1;

                while (r - cellPos >= 0 && c + cellPos < tableObj.numCols)
                {
                    if (ValidatePosition(r - cellPos, c + cellPos, true, curPlayer))
                    {
                        cells[r - cellPos, c + cellPos].SetClickable(true);
                        if (cells[r - cellPos, c + cellPos].getPiece() != null) break;
                        cellPos++;
                    }
                    else break;
                }

                cellPos = -1;

                while (r + cellPos >= 0 && c + cellPos >= 0)
                {
                    if (ValidatePosition(r + cellPos, c + cellPos, true, curPlayer))
                    {
                        cells[r + cellPos, c + cellPos].SetClickable(true);
                        if (cells[r + cellPos, c + cellPos].getPiece() != null) break;
                        cellPos--;
                    }
                    else break;
                }
                break;
            case Piece.pieces.QUEEN:
                //Rectas
                cellPos = 1;
                while (r + cellPos < tableObj.numRows)
                {
                    if (ValidatePosition(r + cellPos, c, true, curPlayer))
                    {
                        cells[r + cellPos, c].SetClickable(true);
                        if (cells[r + cellPos, c].getPiece() != null) break;
                        cellPos++;
                    }
                    else break;
                }

                cellPos = 1;

                while (c + cellPos < tableObj.numCols)
                {
                    if (ValidatePosition(r, c + cellPos, true, curPlayer))
                    {
                        cells[r, c + cellPos].SetClickable(true);
                        if (cells[r, c + cellPos].getPiece() != null) break;
                        cellPos++;
                    }
                    else break;
                }

                cellPos = -1;

                while (r + cellPos >= 0)
                {
                    if (ValidatePosition(r + cellPos, c, true, curPlayer))
                    {
                        cells[r + cellPos, c].SetClickable(true);
                        if (cells[r + cellPos, c].getPiece() != null) break;
                        cellPos--;
                    }
                    else break;
                }

                cellPos = -1;

                while (c + cellPos >= 0)
                {
                    if (ValidatePosition(r, c + cellPos, true, curPlayer))
                    {
                        cells[r, c + cellPos].SetClickable(true);
                        if (cells[r, c + cellPos].getPiece() != null) break;
                        cellPos--;
                    }
                    else break;
                }

                //Diagonales
                cellPos = 1;
                while (r + cellPos < tableObj.numRows && c + cellPos < tableObj.numCols)
                {
                    if (ValidatePosition(r + cellPos, c + cellPos, true, curPlayer))
                    {
                        cells[r + cellPos, c + cellPos].SetClickable(true);
                        if (cells[r + cellPos, c + cellPos].getPiece() != null) break;
                        cellPos++;
                    }
                    else break;
                }

                cellPos = 1;

                while (r + cellPos < tableObj.numRows && c - cellPos >= 0)
                {
                    if (ValidatePosition(r + cellPos, c - cellPos, true, curPlayer))
                    {
                        cells[r + cellPos, c - cellPos].SetClickable(true);
                        if (cells[r + cellPos, c - cellPos].getPiece() != null) break;
                        cellPos++;
                    }
                    else break;
                }

                cellPos = 1;

                while (r - cellPos >= 0 && c + cellPos < tableObj.numCols)
                {
                    if (ValidatePosition(r - cellPos, c + cellPos, true, curPlayer))
                    {
                        cells[r - cellPos, c + cellPos].SetClickable(true);
                        if (cells[r - cellPos, c + cellPos].getPiece() != null) break;
                        cellPos++;
                    }
                    else break;
                }

                cellPos = -1;

                while (r + cellPos >= 0 && c + cellPos >= 0)
                {
                    if (ValidatePosition(r + cellPos, c + cellPos, true, curPlayer))
                    {
                        cells[r + cellPos, c + cellPos].SetClickable(true);
                        if (cells[r + cellPos, c + cellPos].getPiece() != null) break;
                        cellPos--;
                    }
                    else break;
                }
                break;
        }
    }
    public bool ValidatePosition(int r, int c, bool canKill, int player)
    {
        if (r < 0 || c < 0 || r >= tableObj.numRows || c >= tableObj.numCols) return false;
        if (cells[r, c].isObstacle) return false;
        if (cells[r, c].getPiece() == null) return true;
        if (cells[r, c].getPiece().player == player) return false;
        if (cells[r, c].getPiece().player != player) return canKill;
        return false;
    }

    public void MovePiece(int r, int c) 
    {
        cells[curPiece.r, curPiece.c].ChangePiece(null);
        if (cells[r, c].getPiece() != null) 
        {
            cells[r, c].getPiece().inJail = true;
            cells[r, c].getPiece().transform.Translate(new Vector3(0,1,0));
            //mas cosas pa Luego
        }
        cells[r, c].ChangePiece(curPiece);
        cells[r, c].getPiece().SetPosition(r, c);
        curPiece.SetChosen(false);
        curPiece = null;
        ResetClickables();
        if (curPlayer == 1) curPlayer = 2;
        else curPlayer = 1;
    }

    private void ResetClickables() 
    {
        for (int i = 0; i < tableObj.numRows; i++)
        {
            for (int j = 0; j < tableObj.numCols; j++)
            {
                cells[i, j].SetClickable(false);
            }
        }
    }
}
