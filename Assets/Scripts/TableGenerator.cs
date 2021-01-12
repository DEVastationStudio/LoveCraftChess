using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class TableGenerator : MonoBehaviourPunCallbacks
{
    [SerializeField] public TableObj tableObj;
    [SerializeField] private Cell cellUnit;
    [SerializeField] private Piece pieceRef;
    [SerializeField] private Text turnText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button moveConfirmButton;
    [SerializeField] private Camera _camera;
    
    [SerializeField] private Image _minimap;
    [SerializeField] private Image _minimapCell;
    private Vector2 _minimapOrigin;
    private Vector3 _minimapScale;

    #region End Screen
    [SerializeField] private Text endText;
    [SerializeField] private GameObject endScreen;
    #endregion
    [SerializeField] private InGameInterfaceController UIController;
    [SerializeField] private GameObject[,] pieceModels;
    public static int localPlayer;

    public Cell[,] cells;
    public List<Cell> p1Start;
    public List<Cell> p2Start;
    public List<Cell> p1Jail;
    public List<Cell> p2Jail;
    public List<Cell> p1Keys;
    public List<Cell> p2Keys;
    public Cell[] p1Revives;
    public Cell[] p2Revives;

    public List<Piece> p1Pieces;
    public List<Piece> p2Pieces;

    public static int curPlayer;
    public Piece curPiece;
    private int cellPos = 1;
    public bool initialTurn = true;
    private bool gameOver = false;
    private bool gameOverP1 = false;
    private bool gameOverP2 = false;

    private bool p1Ready, p2Ready;

    public bool isOnline = false;

    public int barrierControl; //No one: -1 // Player1: 1 // Player2: 2//
    public List<Cell> barriers;
    public Cell barrierBtn;

    public int PitControl; //No one: -1 // Player1: 1 // Player2: 2//
    public List<Cell> Pits;
    public Cell PitBtn;
    
    private int[] _movePieceParams;

    private Piece.pieces[] pieceOrder = { Piece.pieces.PAWN, Piece.pieces.PAWN, Piece.pieces.PAWN, Piece.pieces.ROOK, Piece.pieces.ROOK, Piece.pieces.KNIGHT, Piece.pieces.BISHOP, Piece.pieces.BISHOP, Piece.pieces.QUEEN };

    public static TableGenerator instance;

    public bool performingMove;

    public GameObject world;

    void Start()
    {
        instance = this;
        isOnline = PhotonNetwork.IsConnected;
        if (isOnline) 
        {
            curPlayer = localPlayer;
            turnText.text = "START TURN";
            UIController.OnlineInterface();
        }
        else 
        {
            localPlayer = -1;
            curPlayer = 1;
            UIController.LocalInterface();
        }
    }

    public void GenerateTable(TableObj level)
    {
        barrierControl = -1;
        PitControl = -1;
        tableObj = level;
        initialTurn = true;
        int numRows = tableObj.numRows;
        int numCols = tableObj.numCols;
        cells = new Cell[numRows,numCols];
        p1Start = new List<Cell>();
        p2Start = new List<Cell>();
        p1Jail = new List<Cell>();
        p2Jail = new List<Cell>();
        p1Keys = new List<Cell>();
        p2Keys = new List<Cell>();
        barriers = new List<Cell>();
        p1Revives = new Cell[3];
        p2Revives = new Cell[3];
        p1Pieces = new List<Piece>();
        p2Pieces = new List<Piece>();
        TableObj.pieceType type;
        _minimapScale = new Vector3(10/(float)numRows, 10/(float)numCols, 1);

        _minimapCell.transform.localScale = Vector3.one;
        float mapOffsetX = 40;
        float mapOffsetY = 40;
        _minimapOrigin =  - new Vector3((numRows/2f)*mapOffsetX*_minimapScale.x, (numCols/2f)*mapOffsetY*_minimapScale.y, 0);

        for (int i = 0; i<numRows; i++) 
        {
            for (int j = 0; j < numCols; j++) 
            {
                Image bgImage = Instantiate(_minimapCell, Vector3.zero, Quaternion.identity);
                bgImage.transform.SetParent(_minimap.transform);
                bgImage.transform.localScale = Vector3.one;
                bgImage.transform.localPosition = (_minimapOrigin + new Vector2((j+0.5f)*mapOffsetX*_minimapScale.x, (i+0.5f)*mapOffsetY*_minimapScale.y));
                Image pieceImage = Instantiate(_minimapCell, Vector3.zero, Quaternion.identity);
                pieceImage.transform.SetParent(_minimap.transform);
                pieceImage.transform.localScale = Vector3.one;
                pieceImage.transform.localPosition = (_minimapOrigin + new Vector2((j+0.5f)*mapOffsetX*_minimapScale.x, (i+0.5f)*mapOffsetY*_minimapScale.y));

                type = tableObj.rows[i].cols[j];
                cells[i, j] = Instantiate(cellUnit, new Vector3(j,0,i), Quaternion.identity);
                cells[i,j].minimapCell = bgImage;
                cells[i,j].minimapPiece = pieceImage;
                cells[i, j].Init(type, i, j, this);
                if (type == TableObj.pieceType.P1ZONE || type == TableObj.pieceType.P1KEY) 
                {
                    p1Start.Add(cells[i,j]);
                    if (type == TableObj.pieceType.P1KEY) p1Keys.Add(cells[i,j]);
                }
                if (type == TableObj.pieceType.P2ZONE || type == TableObj.pieceType.P2KEY)
                {
                    p2Start.Add(cells[i, j]);
                    if (type == TableObj.pieceType.P2KEY) p2Keys.Add(cells[i,j]);
                }
                if (type == TableObj.pieceType.P1REVIVE1) p1Revives[0] = cells[i,j];
                if (type == TableObj.pieceType.P1REVIVE2) p1Revives[1] = cells[i,j];
                if (type == TableObj.pieceType.P1REVIVE3) p1Revives[2] = cells[i,j];

                if (type == TableObj.pieceType.P2REVIVE1) p2Revives[0] = cells[i,j];
                if (type == TableObj.pieceType.P2REVIVE2) p2Revives[1] = cells[i,j];
                if (type == TableObj.pieceType.P2REVIVE3) p2Revives[2] = cells[i,j];

                if (type == TableObj.pieceType.BARRIER) barriers.Add(cells[i, j]);
                if (type == TableObj.pieceType.BARRIERBTN) barrierBtn = cells[i, j];

                if (type == TableObj.pieceType.PIT) Pits.Add(cells[i, j]);
                if (type == TableObj.pieceType.PITBTN) PitBtn = cells[i, j];
            }
        }

        int pI = 0;
        int maxI = pieceOrder.Length;
        foreach (Cell c in p1Start) 
        {
            Piece p = Instantiate(pieceRef, new Vector3(c.c, 1f, c.r), Quaternion.identity);
            p.Init(pieceOrder[maxI-pI-1], c.r, c.c, this, 1);
            c.ChangePiece(p);
            pI = (pI+1)%maxI;
            p1Pieces.Add(c.getPiece());
        }
        pI = 0;
        foreach (Cell c in p2Start)
        {
            Piece p = Instantiate(pieceRef, new Vector3(c.c, 1f, c.r), Quaternion.identity);
            p.Init(pieceOrder[pI], c.r, c.c, this, 2);
            c.ChangePiece(p);
            pI = (pI+1)%maxI;
            p2Pieces.Add(c.getPiece());
        }
        for(int i = 0; i < 3; i++)
        {
            Image bgImage = Instantiate(_minimapCell, Vector3.zero, Quaternion.identity);
            bgImage.transform.SetParent(_minimap.transform);
            bgImage.transform.localScale = Vector3.one;
            bgImage.transform.localPosition = _minimapOrigin + new Vector2((tableObj.p1JailCells[i].col+0.5f)*40*_minimapScale.x, (tableObj.p1JailCells[i].row+0.5f)*40*_minimapScale.y);
            Image pieceImage = Instantiate(_minimapCell, Vector3.zero, Quaternion.identity);
            pieceImage.transform.SetParent(_minimap.transform);
            pieceImage.transform.localScale = Vector3.one;
            pieceImage.transform.localPosition = _minimapOrigin + new Vector2((tableObj.p1JailCells[i].col+0.5f)*40*_minimapScale.x, (tableObj.p1JailCells[i].row+0.5f)*40*_minimapScale.y);


            p1Jail.Add(Instantiate(cellUnit, new Vector3(tableObj.p1JailCells[i].col,0, tableObj.p1JailCells[i].row), Quaternion.identity));

            p1Jail[p1Jail.Count-1].minimapCell = bgImage;
            p1Jail[p1Jail.Count-1].minimapPiece = pieceImage;

            if(i == 0)
                p1Jail[p1Jail.Count-1].Init(TableObj.pieceType.P11JAIL, -1, -1, this); 
            if (i == 1)
                p1Jail[p1Jail.Count - 1].Init(TableObj.pieceType.P12JAIL, -1, -1, this); 
            if (i == 2)
                p1Jail[p1Jail.Count - 1].Init(TableObj.pieceType.P13JAIL, -1, -1, this);
        }
        Instantiate(cellUnit, new Vector3(tableObj.p1GodCell.col, 0, tableObj.p1GodCell.row), Quaternion.identity).Init(TableObj.pieceType.P1GOD, -1, -1, this);

        for (int i = 0; i < 3; i++)
        {
            Image bgImage = Instantiate(_minimapCell, Vector3.zero, Quaternion.identity);
            bgImage.transform.SetParent(_minimap.transform);
            bgImage.transform.localScale = Vector3.one;
            bgImage.transform.localPosition = _minimapOrigin + new Vector2((tableObj.p2JailCells[i].col+0.5f)*40*_minimapScale.x, (tableObj.p2JailCells[i].row+0.5f)*40*_minimapScale.y);
            Image pieceImage = Instantiate(_minimapCell, Vector3.zero, Quaternion.identity);
            pieceImage.transform.SetParent(_minimap.transform);
            pieceImage.transform.localScale = Vector3.one;
            pieceImage.transform.localPosition = _minimapOrigin + new Vector2((tableObj.p2JailCells[i].col+0.5f)*40*_minimapScale.x, (tableObj.p2JailCells[i].row+0.5f)*40*_minimapScale.y);

            p2Jail.Add(Instantiate(cellUnit, new Vector3(tableObj.p2JailCells[i].col, 0, tableObj.p2JailCells[i].row), Quaternion.identity));
            
            p2Jail[p2Jail.Count-1].minimapCell = bgImage;
            p2Jail[p2Jail.Count-1].minimapPiece = pieceImage;
            
            if (i == 0)
                p2Jail[p2Jail.Count - 1].Init(TableObj.pieceType.P21JAIL, -1, -1, this);
            if (i == 1)
                p2Jail[p2Jail.Count - 1].Init(TableObj.pieceType.P22JAIL, -1, -1, this);
            if (i == 2)
                p2Jail[p2Jail.Count - 1].Init(TableObj.pieceType.P23JAIL, -1, -1, this);
        }
        Instantiate(cellUnit, new Vector3(tableObj.p2GodCell.col, 0, tableObj.p2GodCell.row), Quaternion.identity).Init(TableObj.pieceType.P2GOD, -1, -1, this);

        //cam position
        /*_camera.transform.position = new Vector3(numCols/2, Mathf.Max(numCols,numRows)*1.5f, numRows/2);
        if (isOnline && localPlayer == 2) _camera.transform.eulerAngles += new Vector3(0,180,0);*/
        _camera.GetComponent<CameraController>().ResetTarget();
        world.transform.position = new Vector3(numCols/2,-28,numRows/2);
    }

    public int GetLocalPlayer() 
    {
        if (isOnline)
            return localPlayer;
        return curPlayer;
    }

    public void SelectPiece(int r, int c) 
    {
        if (initialTurn && !confirmButton.interactable) return;
        Piece piece = cells[r, c].getPiece();
        if (piece.player != curPlayer || (isOnline && (curPlayer != localPlayer)) || gameOver) return;
        ResetConfirmMove();
        curPiece?.SetChosen(false);
        curPiece = piece;
        curPiece?.SetChosen(true);

        if (initialTurn)
        {
            GetMovesStart();
        }
        else
        {
            GetMoves(curPiece.type, r, c);
        }

    }
    public void GetMovesStart() 
    {
        ResetClickables();
        foreach (Cell c in cells) {
            switch (c.type) {
                case TableObj.pieceType.BASIC1:
                case TableObj.pieceType.P1KEY:
                case TableObj.pieceType.P1ZONE:
                case TableObj.pieceType.P1REVIVE1:
                case TableObj.pieceType.P1REVIVE2:
                case TableObj.pieceType.P1REVIVE3:
                    if (curPlayer == 1) {
                        if (ValidatePosition(c.r, c.c, false, curPlayer)) cells[c.r, c.c].SetClickable(true);
                    }
                    break;
                case TableObj.pieceType.BASIC2:
                case TableObj.pieceType.P2KEY:
                case TableObj.pieceType.P2ZONE:
                case TableObj.pieceType.P2REVIVE1:
                case TableObj.pieceType.P2REVIVE2:
                case TableObj.pieceType.P2REVIVE3:
                    if (curPlayer == 2) {
                        if (ValidatePosition(c.r, c.c, false, curPlayer)) cells[c.r, c.c].SetClickable(true);
                    }
                    break;
            }
        }
        //if (ValidatePosition(r, c+1, false, curPlayer)) cells[r, c+1].SetClickable(true);
    }
    public void GetMoves(Piece.pieces t, int r, int c) 
    {
        ResetClickables();
        switch (t) 
        {
            case Piece.pieces.PAWN:
                if (ValidatePosition(r, c+1, false, curPlayer)) if (!cells[r, c + 1].isBarrier) cells[r, c+1].SetClickable(true);
                if (ValidatePosition(r+1, c+1, true, curPlayer)) if (!cells[r + 1, c + 1].isBarrier) cells[r+1, c+1].SetClickable(true);
                if (ValidatePosition(r+1, c, false, curPlayer)) if (!cells[r + 1, c].isBarrier) cells[r+1, c].SetClickable(true);
                if (ValidatePosition(r+1, c-1, true, curPlayer)) if (!cells[r + 1, c - 1].isBarrier) cells[r+1, c-1].SetClickable(true);
                if (ValidatePosition(r, c-1, false, curPlayer)) if (!cells[r, c - 1].isBarrier) cells[r, c-1].SetClickable(true);
                if (ValidatePosition(r-1, c-1, true, curPlayer)) if (!cells[r - 1, c - 1].isBarrier) cells[r-1, c-1].SetClickable(true);
                if (ValidatePosition(r-1, c, false, curPlayer)) if (!cells[r - 1, c].isBarrier) cells[r-1, c].SetClickable(true);
                if (ValidatePosition(r-1, c+1, true, curPlayer)) if (!cells[r - 1, c + 1].isBarrier) cells[r-1, c+1].SetClickable(true);
                break;
            case Piece.pieces.ROOK:
                cellPos = 1;
                while (r+cellPos<tableObj.numRows) 
                {
                    if (ValidatePosition(r + cellPos, c, true, curPlayer))
                    {
                       if (!cells[r + cellPos, c].isBarrier)
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
                        if (!cells[r, c + cellPos].isBarrier)
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
                        if (!cells[r + cellPos, c].isBarrier)
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
                        if (!cells[r, c + cellPos].isBarrier)
                            cells[r, c + cellPos].SetClickable(true);
                        if (cells[r, c + cellPos].getPiece() != null) break;
                        cellPos--;
                    }
                    else break;
                }
                break;
            case Piece.pieces.KNIGHT:
                if (ValidatePosition(r + 2, c + 1, true, curPlayer)) if( !cells[r + 2, c + 1].isBarrier) cells[r + 2, c + 1].SetClickable(true);
                if (ValidatePosition(r + 2, c - 1, true, curPlayer)) if( !cells[r + 2, c - 1].isBarrier) cells[r + 2, c - 1].SetClickable(true);
                if (ValidatePosition(r - 2, c + 1, true, curPlayer)) if( !cells[r - 2, c + 1].isBarrier) cells[r - 2, c + 1].SetClickable(true);
                if (ValidatePosition(r - 2, c - 1, true, curPlayer)) if( !cells[r - 2, c - 1].isBarrier) cells[r - 2, c - 1].SetClickable(true);
                if (ValidatePosition(r + 1, c - 2, true, curPlayer)) if( !cells[r + 1, c - 2].isBarrier) cells[r + 1, c - 2].SetClickable(true);
                if (ValidatePosition(r - 1, c - 2, true, curPlayer)) if( !cells[r - 1, c - 2].isBarrier) cells[r - 1, c - 2].SetClickable(true);
                if (ValidatePosition(r + 1, c + 2, true, curPlayer)) if( !cells[r + 1, c + 2].isBarrier) cells[r + 1, c + 2].SetClickable(true);
                if (ValidatePosition(r - 1, c + 2, true, curPlayer)) if (!cells[r - 1, c + 2].isBarrier) cells[r - 1, c + 2].SetClickable(true);
                break;
            case Piece.pieces.BISHOP:
                if (ValidatePosition(r, c + 1, false, curPlayer)) if (!cells[r, c + 1].isBarrier) cells[r, c + 1].SetClickable(true);
                if (ValidatePosition(r + 1, c, false, curPlayer)) if (!cells[r + 1, c].isBarrier) cells[r + 1, c].SetClickable(true);
                if (ValidatePosition(r, c - 1, false, curPlayer)) if (!cells[r, c - 1].isBarrier) cells[r, c - 1].SetClickable(true);
                if (ValidatePosition(r - 1, c, false, curPlayer)) if (!cells[r - 1, c].isBarrier) cells[r - 1, c].SetClickable(true);
                cellPos = 1;
                while (r + cellPos < tableObj.numRows && c + cellPos < tableObj.numCols)
                {
                    if (ValidatePosition(r + cellPos, c + cellPos, true, curPlayer))
                    {
                        if(!cells[r + cellPos, c + cellPos].isBarrier)
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
                        if (!cells[r + cellPos, c - cellPos].isBarrier)
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
                        if (!cells[r - cellPos, c + cellPos].isBarrier)
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
                        if (!cells[r + cellPos, c + cellPos].isBarrier)
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
                        if (!cells[r + cellPos, c].isBarrier)
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
                        if (!cells[r, c + cellPos].isBarrier)
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
                        if (!cells[r + cellPos, c].isBarrier)
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
                        if (!cells[r, c + cellPos].isBarrier)
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
                        if (!cells[r + cellPos, c + cellPos].isBarrier)
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
                        if (!cells[r + cellPos, c - cellPos].isBarrier)
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
                        if (!cells[r - cellPos, c + cellPos].isBarrier)
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
                        if (!cells[r + cellPos, c + cellPos].isBarrier)
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
        if (cells[r, c].isBarrier && barrierControl != player) return false;
        if (cells[r, c].isPit && PitControl != player && PitControl != -1) return false;
        if (PitControl == -1 && cells[r, c].isPit && cells[r, c].getPiece() == null) return true;
        if (cells[r, c].getPiece()!=null) 
        {
            if (PitControl == -1 && cells[r, c].isPit && cells[r, c].getPiece().player == player) return false;
            if (PitControl == -1 && cells[r, c].isPit && cells[r, c].getPiece().player != player) return true;
        }
        if (cells[r, c].getPiece() == null) return true;
        if (cells[r, c].getPiece().player == player) return false;
        if (cells[r, c].getPiece().player != player) return canKill;
        return false;
    }

    public void ResetConfirmMove()
    {
        _movePieceParams = new int[4];
        moveConfirmButton.gameObject.SetActive(false);
    }
    public void ShowConfirmMove(int[] parameters)
    {
        _movePieceParams = parameters;
        moveConfirmButton.gameObject.SetActive(true);
        ResetClickables();
    }

    public void ConfirmMove()
    {
        if (isOnline)
        {
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("MovePiece", RpcTarget.All, _movePieceParams[0], _movePieceParams[1], _movePieceParams[2], _movePieceParams[3]);
        }
        else
        {
            MovePiece(_movePieceParams[0], _movePieceParams[1], _movePieceParams[2], _movePieceParams[3]);
        }
        cells[_movePieceParams[0], _movePieceParams[1]].SetSelected(false);
        moveConfirmButton.gameObject.SetActive(false);
    }

    [PunRPC]
    public void MovePiece(int r, int c, int pr, int pc) 
    {
        Piece piece = cells[pr,pc].getPiece();
        performingMove = true;
        confirmButton.interactable = false;
        
        cells[piece.r, piece.c].ChangePiece(null);
        
        if (initialTurn)
        {

            if (cells[r, c].getPiece() != null) 
            {
                Piece eatenPiece = cells[r,c].getPiece();
                eatenPiece.inJail = true;

                List<Cell> chosenJail;
                if (eatenPiece.player == 1)
                {
                    chosenJail = p2Jail;
                }
                else 
                {
                    chosenJail = p1Jail;
                }

                if (chosenJail[2].getPiece() != null)
                {
                    p1Pieces.Remove(chosenJail[2].getPiece());
                    p2Pieces.Remove(chosenJail[2].getPiece());
                    Destroy(chosenJail[2].getPiece().gameObject);
                    chosenJail[2].ChangePiece(null);
                }

                if (chosenJail[1].getPiece() != null)
                {
                    chosenJail[2].ChangePiece(chosenJail[1].getPiece());
                    chosenJail[2].getPiece().SetJailPosition(chosenJail[2]);
                    chosenJail[1].ChangePiece(null);
                }

                if (chosenJail[0].getPiece() != null)
                {
                    chosenJail[1].ChangePiece(chosenJail[0].getPiece());
                    chosenJail[1].getPiece().SetJailPosition(chosenJail[1]);
                    chosenJail[0].ChangePiece(null);
                }

                chosenJail[0].ChangePiece(eatenPiece);
                eatenPiece.SetJailPosition(chosenJail[0]);
            }

            cells[r, c].ChangePiece(piece);
            cells[r, c].getPiece().SetPosition(r, c);
            piece.SetChosen(false);
            if (piece.player == localPlayer || !isOnline)
                ResetClickables();
            piece = null;
            performingMove = false;
            confirmButton.interactable = true;
        }
        else
        {
            StartCoroutine(PerformPieceMovement(piece, new Vector3Int(pc, 1, pr), new Vector3Int(c, 1, r)));
        }
    }

    private IEnumerator PerformPieceMovement(Piece piece, Vector3Int startPos, Vector3Int endPos)
    {
        Vector3Int startFloatPos = startPos + new Vector3Int(0,1,0);
        Vector3Int endFloatPos = endPos + new Vector3Int(0,1,0);
        Quaternion startRot = piece.transform.rotation;
        Quaternion endRot1 = Quaternion.LookRotation(endPos-startPos);
        Quaternion endRot2 = Quaternion.LookRotation(startPos-endPos);
        Quaternion endRot = (piece.player == 1)?endRot1:endRot2;
        float elapsedTime = 0.0f;

        GameObject pieceContainer = piece.SetChosenMovement(false);
        if (piece.player == localPlayer || !isOnline)
            ResetClickables();

        Vector3 containerPos = pieceContainer.transform.localPosition;
        Quaternion containerRot = pieceContainer.transform.localRotation;

        //Up movement

        while (elapsedTime < 0.5f)
        {
            pieceContainer.transform.localPosition = Vector3.Lerp(containerPos, Vector3.zero, elapsedTime*2);
            pieceContainer.transform.localRotation = Quaternion.Slerp(containerRot, Quaternion.identity, elapsedTime*2);
            piece.transform.position = Vector3.Lerp(startPos, startFloatPos, elapsedTime*2);
            piece.transform.rotation = Quaternion.Slerp(startRot, endRot, elapsedTime*2);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        piece.transform.position = startFloatPos;

        //Forward movement

        elapsedTime = 0.0f;
        while (elapsedTime < 1)
        {
            piece.transform.position = Vector3.Lerp(startFloatPos, endFloatPos, elapsedTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        piece.transform.position = endFloatPos;

        Piece eatenPiece = cells[endPos.z, endPos.x].getPiece();
        bool eatsPiece = (eatenPiece != null);
        Vector3 eatenPiecePos = (eatsPiece)?(eatenPiece.transform.position):(Vector3.zero);
        Vector3 finalEatenPiecePos = eatenPiecePos + new Vector3(0, -1, 0);


        //Down movement

        elapsedTime = 0.0f;
        while (elapsedTime < 0.5f)
        {
            piece.transform.position = Vector3.Lerp(endFloatPos, endPos, elapsedTime*2);
            if (eatsPiece) eatenPiece.transform.position = Vector3.Lerp(eatenPiecePos, finalEatenPiecePos, elapsedTime*2);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        piece.transform.position = endPos;

        //Eaten Piece management
        yield return eatPiece(eatsPiece, eatenPiece, elapsedTime, eatenPiecePos, finalEatenPiecePos, false);

        //Finishing touches

        cells[endPos.z, endPos.x].ChangePiece(piece);
        cells[endPos.z, endPos.x].getPiece().SetCoords(endPos.z, endPos.x);
        piece = null;
        if (!initialTurn) NextTurn();
        performingMove = false;
        confirmButton.interactable = true;
        yield return null;
    }

    private IEnumerator eatPiece(bool eatsPiece, Piece eatenPiece, float elapsedTime, Vector3 eatenPiecePos, Vector3 finalEatenPiecePos, bool deathByTrap)
    {
        if (eatsPiece)
        {
            if (isOnline && eatenPiece.player != localPlayer) 
            {
                if (deathByTrap) PlayerPrefs.SetInt("TrapKills", PlayerPrefs.GetInt("TrapKills", 0)+1);
                else PlayerPrefs.SetInt("EatenPieces", PlayerPrefs.GetInt("EatenPieces", 0)+1);
            }

            eatenPiece.inJail = true;
            
            List<Cell> chosenJail;
            if (eatenPiece.player == 1)
            {
                chosenJail = p2Jail;
            }
            else 
            {
                chosenJail = p1Jail;
            }

            Vector3 jailOffset = new Vector3(chosenJail[1].transform.position.x-chosenJail[0].transform.position.x, 0, chosenJail[1].transform.position.z-chosenJail[0].transform.position.z);

            List<Piece> moveablePieceList = new List<Piece>();
            List<Vector3> moveablePiecePositions = new List<Vector3>();
            List<Vector3> moveablePieceFinalPositions = new List<Vector3>();
            bool pieceDestroyed = false;
            Piece destroyedPiece = null;

            //Get List of pieces that should be moved

            if (chosenJail[2].getPiece() != null)
            {
                p1Pieces.Remove(chosenJail[2].getPiece());
                p2Pieces.Remove(chosenJail[2].getPiece());

                moveablePieceList.Add(chosenJail[2].getPiece());
                moveablePiecePositions.Add(chosenJail[2].getPiece().transform.position);
                moveablePieceFinalPositions.Add(chosenJail[2].getPiece().transform.position+jailOffset);

                destroyedPiece = chosenJail[2].getPiece();
                pieceDestroyed = (destroyedPiece != null);

                chosenJail[2].ChangePiece(null);
            }

            if (chosenJail[1].getPiece() != null)
            {
                chosenJail[2].ChangePiece(chosenJail[1].getPiece());
                
                moveablePieceList.Add(chosenJail[1].getPiece());
                moveablePiecePositions.Add(chosenJail[1].getPiece().transform.position);
                moveablePieceFinalPositions.Add(chosenJail[1].getPiece().transform.position+jailOffset);

                chosenJail[1].ChangePiece(null);
                chosenJail[2].getPiece().SetCoords(-1,-1);
            }

            if (chosenJail[0].getPiece() != null)
            {
                chosenJail[1].ChangePiece(chosenJail[0].getPiece());
                
                moveablePieceList.Add(chosenJail[0].getPiece());
                moveablePiecePositions.Add(chosenJail[0].getPiece().transform.position);
                moveablePieceFinalPositions.Add(chosenJail[0].getPiece().transform.position+jailOffset);
                
                chosenJail[0].ChangePiece(null);
                chosenJail[1].getPiece().SetCoords(-1,-1);
            }

            if (moveablePieceList.Count > 0)
            {
                elapsedTime = 0.0f;
                int numPieces = moveablePieceList.Count;
                while (elapsedTime < 0.5f)
                {
                    for (int i = 0; i < numPieces; i++)
                    {
                        moveablePieceList[i].transform.position = Vector3.Lerp(moveablePiecePositions[i], moveablePieceFinalPositions[i], elapsedTime*2);
                    }
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                for (int i = 0; i < numPieces; i++)
                {
                    moveablePieceList[i].transform.position = moveablePieceFinalPositions[i];
                }
            }

            if (pieceDestroyed)
            {
                Vector3 destroyedPieceStart = destroyedPiece.transform.position;
                Vector3 destroyedPieceEnd = destroyedPieceStart + new Vector3(0,-1,0);

                elapsedTime = 0.0f;
                while (elapsedTime < 1)
                {
                    destroyedPiece.transform.position = Vector3.Lerp(destroyedPieceStart, destroyedPieceEnd, elapsedTime);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                Destroy(destroyedPiece.gameObject);
            }

            //Elevate new dead piece

            eatenPiece.transform.position = new Vector3(chosenJail[0].transform.position.x, -1, chosenJail[0].transform.position.z);
            finalEatenPiecePos = new Vector3(eatenPiece.transform.position.x, eatenPiecePos.y, eatenPiece.transform.position.z);
            eatenPiecePos = eatenPiece.transform.position;


            elapsedTime = 0.0f;
            while (elapsedTime < 1)
            {
                eatenPiece.transform.position = Vector3.Lerp(eatenPiecePos, finalEatenPiecePos, elapsedTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            eatenPiece.transform.position = finalEatenPiecePos;
            eatenPiece.SetCoords(-1,-1);

            chosenJail[0].ChangePiece(eatenPiece);
            

        }
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


    public void ButtonNextTurn()
    {
        if (initialTurn)
        {
            if (isOnline)
            {
                if (curPlayer == localPlayer)
                {
                    photonView.RPC("ButtonNextTurnRPC", RpcTarget.All, localPlayer, UnityEngine.Random.Range(1,3));
                }
            }
            else
            {
                NextTurn();
            }
        }
    }

    [PunRPC]
    public void ButtonNextTurnRPC(int readyPlayer, int startingPlayer)
    {
        if (initialTurn)
        {
            if (readyPlayer == 1) 
            {
                if (p1Ready) return;
                p1Ready = true;
            }
            else if (readyPlayer == 2) 
            {
                if (p2Ready) return;
                p2Ready = true;
            }
            
            if (readyPlayer == localPlayer)
            {
                curPiece?.SetChosen(false);
                ResetClickables();
                confirmButton.interactable = false;
            }


            if (p1Ready && p2Ready) NextTurn(startingPlayer);
        }
    }

    public void NextTurn(int startingPlayer = 2) 
    {
        if (initialTurn) 
        {
            curPiece?.SetChosen(false);
            ResetClickables();
            if (isOnline)
            {
                curPlayer = startingPlayer;
                initialTurn = false;
                _minimap.gameObject.SetActive(true);
                confirmButton.gameObject.SetActive(false);
            }
            else
            {
                if (curPlayer == 2)
                {
                    initialTurn = false;
                    _minimap.gameObject.SetActive(true);
                    confirmButton.gameObject.SetActive(false);
                }
            }
        }
        else if (isOnline && localPlayer != curPlayer)
        {
            PlayerPrefs.SetInt("TurnsTaken", PlayerPrefs.GetInt("TurnsTaken", 0)+1);
        }


        if (PitBtn != null && PitBtn.getPiece() != null)
        {
            PitControl = PitBtn.getPiece().player;
            if (isOnline && barrierControl == localPlayer) PlayerPrefs.SetInt("TrapsTriggered", PlayerPrefs.GetInt("TrapsTriggered", 0)+1);
        }
        else
            PitControl = -1;

        StartCoroutine(NextTurnAnimations());

    }

    private IEnumerator NextTurnAnimations()
    {
        foreach (Cell p in Pits)
        {
            if (PitControl != -1)
            {
                if (p.getPiece() != null)
                {
                    if (p.getPiece().player != PitControl)
                    {
                        Piece eatenPiece = cells[p.r, p.c].getPiece();
                        bool eatsPiece = (eatenPiece != null);
                        Vector3 eatenPiecePos = (eatsPiece)?(eatenPiece.transform.position):(Vector3.zero);
                        Vector3 finalEatenPiecePos = eatenPiecePos + new Vector3(0, -1, 0);

                        Quaternion closedPitRot = Quaternion.identity;
                        Quaternion openPitLRot = Quaternion.Euler(0,0,89);
                        Quaternion openPitRRot = Quaternion.Euler(0,0,-89);

                        //Open Trapdoors

                        float elapsedTime = 0.0f;
                        while (elapsedTime < 0.25f)
                        {
                            if (eatsPiece) eatenPiece.transform.position = Vector3.Lerp(eatenPiecePos, finalEatenPiecePos, elapsedTime*4);
                            if (!p.pitOpen)
                            {
                                p.pitHingeL.transform.localRotation = Quaternion.Slerp(closedPitRot, openPitLRot, elapsedTime*4);
                                p.pitHingeR.transform.localRotation = Quaternion.Slerp(closedPitRot, openPitRRot, elapsedTime*4);
                            }
                            elapsedTime += Time.deltaTime;
                            yield return null;
                        }
                        p.pitOpen = true;
                        yield return new WaitForSeconds(0.5f);
                        yield return CloseTrapdoors(p);

                        //Eaten Piece management
                        yield return eatPiece(eatsPiece, eatenPiece, elapsedTime, eatenPiecePos, finalEatenPiecePos, true);

                        p.ChangePiece(null);

                    }
                    else
                    {
                        yield return CloseTrapdoors(p);
                    }
                }
                else
                {
                    if (PitControl != curPlayer)
                    {
                        yield return CloseTrapdoors(p);
                    }
                    else
                    {
                        //Open Trapdoors
                        if (!p.pitOpen)
                        {
                            Quaternion closedPitRot = Quaternion.identity;
                            Quaternion openPitLRot = Quaternion.Euler(0,0,89);
                            Quaternion openPitRRot = Quaternion.Euler(0,0,-89);
                            float elapsedTime = 0.0f;
                            while (elapsedTime < 0.25f)
                            {
                                p.pitHingeL.transform.localRotation = Quaternion.Slerp(closedPitRot, openPitLRot, elapsedTime*4);
                                p.pitHingeR.transform.localRotation = Quaternion.Slerp(closedPitRot, openPitRRot, elapsedTime*4);
                                elapsedTime += Time.deltaTime;
                                yield return null;
                            }
                        }
                        p.pitOpen = true;
                    }
                }
            }
            else
            {
                yield return CloseTrapdoors(p);
            }
        }


        if (curPlayer == 1) curPlayer = 2;
        else curPlayer = 1;
        turnText.text = (curPlayer == 1 ? "BLUE" : "RED") + " PLAYER TURN";

        if (barrierBtn != null && barrierBtn.getPiece() != null)
        {
            barrierControl = barrierBtn.getPiece().player;
            if (isOnline && barrierControl == localPlayer) PlayerPrefs.SetInt("TrapsTriggered", PlayerPrefs.GetInt("TrapsTriggered", 0)+1);
        }

        if (barrierBtn != null && barrierBtn.getPiece() == null)
            barrierControl = -1;

        foreach (Cell b in barriers)
        {
            if (barrierControl != -1)
            {
                float elapsedTime = 0.0f;
                Vector3 barrierDownPos = Vector3.zero;
                Vector3 barrierUpPos = Vector3.up;
                if (barrierControl == curPlayer)
                {
                    //Barrier sink animation
                    if (!b.barrierDown)
                    {
                        while (elapsedTime < 0.25f)
                        {
                            b.barrier.transform.localPosition = Vector3.Lerp(barrierUpPos, barrierDownPos, elapsedTime*4);
                            elapsedTime += Time.deltaTime;
                            yield return null;
                        }
                        b.barrier.transform.localPosition = barrierDownPos;
                    }
                    b.barrierDown = true;
                }
                else
                {
                    //Barrier rise animation
                    if (b.barrierDown)
                    {
                        while (elapsedTime < 0.25f)
                        {
                            b.barrier.transform.localPosition = Vector3.Lerp(barrierDownPos, barrierUpPos, elapsedTime*4);
                            elapsedTime += Time.deltaTime;
                            yield return null;
                        }
                        b.barrier.transform.localPosition = barrierUpPos;
                    }
                    b.barrierDown = false;
                }
                    
            }
            else
            {
                //Barrier rise animation
                float elapsedTime = 0.0f;
                Vector3 barrierDownPos = Vector3.zero;
                Vector3 barrierUpPos = Vector3.up;
                if (b.barrierDown)
                {
                    while (elapsedTime < 0.25f)
                    {
                        b.barrier.transform.localPosition = Vector3.Lerp(barrierDownPos, barrierUpPos, elapsedTime*4);
                        elapsedTime += Time.deltaTime;
                        yield return null;
                    }
                    b.barrier.transform.localPosition = barrierUpPos;
                }
                b.barrierDown = false;
            }
        }

        bool found = false;
        foreach (Cell c in p2Keys)
        {
            if (c.getPiece() != null)
            {
                if (c.getPiece().player == 1)
                {
                    found = true;
                }
            }
        }

        float _elapsedTime = 0.0f;
        List<Piece> moveablePieceList = new List<Piece>();
        List<Vector3> moveablePiecePositions = new List<Vector3>();
        List<Vector3> moveablePieceFinalPositions = new List<Vector3>();
        Piece pi = null;
        Vector3 oldPos = Vector3.zero;
        Vector3 newPos = Vector3.zero;

        if (found)
        {
            yield return RevivePieces(_elapsedTime, pi, oldPos, newPos, p2Revives, p2Jail);
        }

        _elapsedTime = 0.0f;
        int numPieces = moveablePieceList.Count;
        while (_elapsedTime < 0.5f)
        {
            for (int i = 0; i < numPieces; i++)
            {
                moveablePieceList[i].transform.position = Vector3.Lerp(moveablePiecePositions[i], moveablePieceFinalPositions[i], _elapsedTime*2);
            }
            _elapsedTime += Time.deltaTime;
            yield return null;
        }
        for (int i = 0; i < numPieces; i++)
        {
            moveablePieceList[i].transform.position = moveablePieceFinalPositions[i];
        }


        found = false;
        foreach (Cell c in p1Keys)
        {
            if (c.getPiece() != null)
            {
                if (c.getPiece().player == 2)
                {
                    found = true;
                }
            }
        }

        if (found)
        {
            yield return RevivePieces(_elapsedTime, pi, oldPos, newPos, p1Revives, p1Jail);
        }

        yield return new WaitForSeconds(0.5f);
        if(!isOnline) _camera.GetComponent<CameraController>().ResetTarget();

        
        bool allDead;

        if (p1Pieces.Count <= 3)
        {
            allDead = true;
            foreach (Piece p in p1Pieces)
            {
                if (!p.inJail) allDead = false;
            }
            if (allDead)
            {
                SetWinner(2,1);
                yield break;
            }
        }
        if (p2Pieces.Count <= 3)
        {
            allDead = true;
            foreach (Piece p in p2Pieces)
            {
                if (!p.inJail) allDead = false;
            }
            if (allDead)
            {
                SetWinner(1,1);
                yield break;
            }
        }

        bool finished = true;
        foreach (Piece p in p1Pieces)
        {
            if (p.inJail)
            {
                finished = false;
                break;
            }
            Cell c = cells[p.r, p.c];
            if (c.type != TableObj.pieceType.P2ZONE && c.type != TableObj.pieceType.P2KEY)
            {
                finished = false;
                break;
            }
        }
        if (finished)
        {
            SetWinner(1,0);
            yield break;
        }

        finished = true;
        foreach (Piece p in p2Pieces)
        {
            if (p.inJail)
            {
                finished = false;
                break;
            }
            Cell c = cells[p.r, p.c];
            if (c.type != TableObj.pieceType.P1ZONE && c.type != TableObj.pieceType.P1KEY)
            {
                finished = false;
                break;
            }
        }
        if (finished)
        {
            SetWinner(2,0);
            yield break;
        }
        yield return null;

    }

    private IEnumerator RevivePieces(float _elapsedTime, Piece pi, Vector3 oldPos, Vector3 newPos, Cell[] p2Revives, List<Cell> p2Jail)
    {
        for (int i = 0; i < 3; i++)
        {
            if (p2Revives[i].getPiece() == null)
            {
                if (p2Jail[0].getPiece() != null)
                {
                    if (isOnline && p2Jail[0].getPiece().player == localPlayer) PlayerPrefs.SetInt("Resurrections", PlayerPrefs.GetInt("Resurrections", 0)+1);
                    p2Revives[i].ChangePiece(p2Jail[0].getPiece());
                    p2Revives[i].getPiece().SetCoords(p2Revives[i].r, p2Revives[i].c);
                    p2Revives[i].getPiece().inJail = false;
                    p2Jail[0].ChangePiece(null);

                    _elapsedTime = 0.0f;
                    pi = p2Revives[i].getPiece();
                    oldPos = pi.transform.position;
                    newPos = new Vector3(p2Revives[i].c, 1, p2Revives[i].r);
                    while (_elapsedTime < 0.5f)
                    {
                        pi.transform.position = Vector3.Lerp(oldPos, newPos, _elapsedTime*2);
                        _elapsedTime += Time.deltaTime;
                        yield return null;
                    }
                    pi.transform.position = newPos;

                    
                    if (p2Jail[1].getPiece() != null)
                    {
                        p2Jail[0].ChangePiece(p2Jail[1].getPiece());
                        p2Jail[0].getPiece().SetCoords(-1,-1);

                        _elapsedTime = 0.0f;
                        pi = p2Jail[0].getPiece();
                        oldPos = pi.transform.position;
                        newPos = new Vector3(p2Jail[0].transform.position.x, 1, p2Jail[0].transform.position.z);
                        while (_elapsedTime < 0.5f)
                        {
                            pi.transform.position = Vector3.Lerp(oldPos, newPos, _elapsedTime*2);
                            _elapsedTime += Time.deltaTime;
                            yield return null;
                        }
                        pi.transform.position = newPos;

                        p2Jail[1].ChangePiece(null);
                    }

                    if (p2Jail[2].getPiece() != null)
                    {
                        p2Jail[1].ChangePiece(p2Jail[2].getPiece());
                        p2Jail[1].getPiece().SetCoords(-1,-1);

                        _elapsedTime = 0.0f;
                        pi = p2Jail[1].getPiece();
                        oldPos = pi.transform.position;
                        newPos = new Vector3(p2Jail[1].transform.position.x, 1, p2Jail[1].transform.position.z);
                        while (_elapsedTime < 0.5f)
                        {
                            pi.transform.position = Vector3.Lerp(oldPos, newPos, _elapsedTime*2);
                            _elapsedTime += Time.deltaTime;
                            yield return null;
                        }
                        pi.transform.position = newPos;

                        p2Jail[2].ChangePiece(null);
                    }
                }
            }
        }
    }

    private IEnumerator CloseTrapdoors(Cell p)
    {
        //Close Trapdoors
        if (p.pitOpen)
        {
            Quaternion closedPitRot = Quaternion.identity;
            Quaternion openPitLRot = Quaternion.Euler(0,0,89);
            Quaternion openPitRRot = Quaternion.Euler(0,0,-89);
            float elapsedTime = 0.0f;
            while (elapsedTime < 0.25f)
            {
                p.pitHingeL.transform.localRotation = Quaternion.Slerp(openPitLRot, closedPitRot, elapsedTime*4);
                p.pitHingeR.transform.localRotation = Quaternion.Slerp(openPitRRot, closedPitRot, elapsedTime*4);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        p.pitOpen = false;
    }

    public void AbandonVictory()
    {
        if (gameOver) return;
        SetWinner(localPlayer,2);
    }

    private void SetWinner(int player, int cause)
    {
        gameOver = true;
        turnText.text = (player==1?"BLUE":"RED") + " PLAYER WINS";

        endText.text = ((isOnline)?((localPlayer==player)?("YOU WIN"):("YOU LOSE")):((player==1?"BLUE":"RED") + " PLAYER WINS"));

        if (isOnline) photonView.RPC("GameOver", RpcTarget.All, localPlayer, cause);
        else GameOver(localPlayer, cause);
    }
    [PunRPC]
    private void GameOver(int p, int cause)
    {
        if (isOnline)
        {
            PlayerPrefs.SetInt("OnlineGames", PlayerPrefs.GetInt("OnlineGames", 0)+1);
            switch (cause)
            {
                case 0: //Conqueror victory
                    if (localPlayer == p) 
                    {
                        PlayerPrefs.SetInt("OnlineWins", PlayerPrefs.GetInt("OnlineWins", 0)+1);
                        PlayerPrefs.SetInt("OnlineWinsConqueror", PlayerPrefs.GetInt("OnlineWinsConqueror", 0)+1);
                    }
                    else
                    {
                        PlayerPrefs.SetInt("OnlineLosses", PlayerPrefs.GetInt("OnlineLosses", 0)+1);
                        PlayerPrefs.SetInt("OnlineLossesConqueror", PlayerPrefs.GetInt("OnlineLossesConqueror", 0)+1);
                    }
                break;
                case 1: //Devastator victory
                    if (localPlayer == p) 
                    {
                        PlayerPrefs.SetInt("OnlineWins", PlayerPrefs.GetInt("OnlineWins", 0)+1);
                        PlayerPrefs.SetInt("OnlineWinsDevastator", PlayerPrefs.GetInt("OnlineWinsDevastator", 0)+1);
                    }
                    else
                    {
                        PlayerPrefs.SetInt("OnlineLosses", PlayerPrefs.GetInt("OnlineLosses", 0)+1);
                        PlayerPrefs.SetInt("OnlineLossesDevastator", PlayerPrefs.GetInt("OnlineLossesDevastator", 0)+1);
                    }
                break;
                case 2: //Abandon victory
                    if (localPlayer == p) 
                    {
                        PlayerPrefs.SetInt("OnlineWins", PlayerPrefs.GetInt("OnlineWins", 0)+1);
                        PlayerPrefs.SetInt("OnlineWinsSurrender", PlayerPrefs.GetInt("OnlineWinsSurrender", 0)+1);
                    }
                    else
                    {
                        PlayerPrefs.SetInt("OnlineLosses", PlayerPrefs.GetInt("OnlineLosses", 0)+1);
                        PlayerPrefs.SetInt("OnlineLossesSurrender", PlayerPrefs.GetInt("OnlineLossesSurrender", 0)+1);
                    }
                break;
            }

            if (p == 1)
            {
                gameOverP1 = true;
            } 
            else if (p == 2)
            {
                gameOverP2 = true;
            }
            if (gameOverP1 && gameOverP2)
            {
                PhotonNetwork.LeaveRoom();
                endScreen.SetActive(true);
            }
        }
        else
        {
            if (p == 1)
            {
                PlayerPrefs.SetInt("P1Victories", PlayerPrefs.GetInt("P1Victories", 0)+1);
            }
            else
            {
                PlayerPrefs.SetInt("P2Victories", PlayerPrefs.GetInt("P2Victories", 0)+1);
            }

            endScreen.SetActive(true);
        }
    }
}
