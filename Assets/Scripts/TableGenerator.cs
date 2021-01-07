using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class TableGenerator : MonoBehaviourPunCallbacks
{
    [SerializeField] private TableObj tableObj;
    [SerializeField] private Cell cellUnit;
    [SerializeField] private Piece pieceRef;
    [SerializeField] private Text turnText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Camera _camera;

    #region End Screen
    [SerializeField] private Text endText;
    [SerializeField] private GameObject endScreen;
    #endregion
    [SerializeField] private InGameInterfaceController UIController;
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
    private bool initialTurn = true;
    private bool gameOver = false;

    private bool p1Ready, p2Ready;

    public bool isOnline = false;

    public int barrierControl; //No one: -1 // Player1: 1 // Player2: 2//
    public List<Cell> barriers;
    public Cell barrierBtn;

    private Piece.pieces[] pieceOrder = { Piece.pieces.PAWN, Piece.pieces.PAWN, Piece.pieces.PAWN, Piece.pieces.ROOK, Piece.pieces.ROOK, Piece.pieces.KNIGHT, Piece.pieces.BISHOP, Piece.pieces.BISHOP, Piece.pieces.QUEEN };

    public static TableGenerator instance;

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
            }
        }

        int pI = 0;
        int maxI = pieceOrder.Length;
        foreach (Cell c in p1Start) 
        {
            c.ChangePiece(Instantiate(pieceRef, new Vector3(c.c, 1f, c.r), Quaternion.identity));
            c.getPiece().Init(pieceOrder[maxI-pI-1], c.r, c.c, this, 1);
            pI = (pI+1)%maxI;
            p1Pieces.Add(c.getPiece());
        }
        pI = 0;
        foreach (Cell c in p2Start)
        {
            c.ChangePiece(Instantiate(pieceRef, new Vector3(c.c, 1f, c.r), Quaternion.identity));
            c.getPiece().Init(pieceOrder[pI], c.r, c.c, this, 2);
            pI = (pI+1)%maxI;
            p2Pieces.Add(c.getPiece());
        }
        foreach (JailCell jc in tableObj.p1JailCells)
        {
            p1Jail.Add(Instantiate(cellUnit, new Vector3(jc.col,0,jc.row), Quaternion.identity));
            p1Jail[p1Jail.Count-1].Init(TableObj.pieceType.P1JAIL, -1, -1, this);
        }
        foreach (JailCell jc in tableObj.p2JailCells)
        {
            p2Jail.Add(Instantiate(cellUnit, new Vector3(jc.col,0,jc.row), Quaternion.identity));
            p2Jail[p2Jail.Count-1].Init(TableObj.pieceType.P2JAIL, -1, -1, this);
        }

        //Camera position
        _camera.transform.position = new Vector3(numCols/2, Mathf.Max(numCols,numRows)*1.5f, numRows/2);
        if (isOnline && localPlayer == 2) _camera.transform.eulerAngles += new Vector3(0,180,0);
    }
    
    public void SelectPiece(int r, int c) 
    {
        if (initialTurn && !confirmButton.interactable) return;
        Piece piece = cells[r, c].getPiece();
        if (piece.player != curPlayer || (isOnline && (curPlayer != localPlayer)) || gameOver) return;

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
        if (cells[r, c].getPiece() == null) return true;
        if (cells[r, c].getPiece().player == player) return false;
        if (cells[r, c].getPiece().player != player) return canKill;
        return false;
    }

    [PunRPC]
    public void MovePiece(int r, int c, int pr, int pc) 
    {
        Piece piece = cells[pr,pc].getPiece();
        
        cells[piece.r, piece.c].ChangePiece(null);
        if (cells[r, c].getPiece() != null) 
        {
            Piece eatenPiece = cells[r,c].getPiece();
            eatenPiece.inJail = true;
            //cells[r, c].getPiece().transform.Translate(new Vector3(0,1,0));
            //mas cosas pa Luego
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
        if (!initialTurn) NextTurn();
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

    public void NextTurn(int startingPlayer = 2) {
        
        if (initialTurn) 
        {
            ResetClickables();
            if (isOnline)
            {
                curPlayer = startingPlayer;
                initialTurn = false;
                confirmButton.gameObject.SetActive(false);
            }
            else
            {
                if (curPlayer == 2)
                {
                    initialTurn = false;
                    confirmButton.gameObject.SetActive(false);
                }
            }
        }

        if (curPlayer == 1) curPlayer = 2;
        else curPlayer = 1;
        turnText.text = (curPlayer==1?"BLUE":"RED") + " PLAYER TURN";

        if (barrierBtn.getPiece() != null)
            barrierControl = barrierBtn.getPiece().player;
        
        foreach (Cell b in barriers)
        {
            if (barrierControl != -1)
            {

                if (barrierControl == curPlayer)
                    b.barrier.SetActive(false); //Animacion de bajar
                else
                    b.barrier.SetActive(true); //Animacion de subir
            }
            else
                b.barrier.SetActive(true);
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

        if (found)
        {
            if (p2Revives[0].getPiece() == null)
            {
                if (p2Jail[0].getPiece() != null)
                {
                    p2Revives[0].ChangePiece(p2Jail[0].getPiece());
                    p2Revives[0].getPiece().SetPosition(p2Revives[0].r, p2Revives[0].c);
                    p2Revives[0].getPiece().inJail = false;
                    p2Jail[0].ChangePiece(null);
                    if (p2Jail[1].getPiece() != null)
                    {
                        p2Jail[0].ChangePiece(p2Jail[1].getPiece());
                        p2Jail[0].getPiece().SetJailPosition(p2Jail[0]);
                        p2Jail[1].ChangePiece(null);
                    }
                    if (p2Jail[2].getPiece() != null)
                    {
                        p2Jail[1].ChangePiece(p2Jail[2].getPiece());
                        p2Jail[1].getPiece().SetJailPosition(p2Jail[1]);
                        p2Jail[2].ChangePiece(null);
                    }
                }
            }
            if (p2Revives[1].getPiece() == null)
            {
                if (p2Jail[0].getPiece() != null)
                {
                    p2Revives[1].ChangePiece(p2Jail[0].getPiece());
                    p2Revives[1].getPiece().SetPosition(p2Revives[1].r, p2Revives[1].c);
                    p2Revives[1].getPiece().inJail = false;
                    p2Jail[0].ChangePiece(null);
                    if (p2Jail[1].getPiece() != null)
                    {
                        p2Jail[0].ChangePiece(p2Jail[1].getPiece());
                        p2Jail[0].getPiece().SetJailPosition(p2Jail[0]);
                        p2Jail[1].ChangePiece(null);
                    }
                    if (p2Jail[2].getPiece() != null)
                    {
                        p2Jail[1].ChangePiece(p2Jail[2].getPiece());
                        p2Jail[1].getPiece().SetJailPosition(p2Jail[1]);
                        p2Jail[2].ChangePiece(null);
                    }
                }
            }
            if (p2Revives[2].getPiece() == null)
            {
                if (p2Jail[0].getPiece() != null)
                {
                    p2Revives[2].ChangePiece(p2Jail[0].getPiece());
                    p2Revives[2].getPiece().SetPosition(p2Revives[2].r, p2Revives[2].c);
                    p2Revives[2].getPiece().inJail = false;
                    p2Jail[0].ChangePiece(null);
                    if (p2Jail[1].getPiece() != null)
                    {
                        p2Jail[0].ChangePiece(p2Jail[1].getPiece());
                        p2Jail[0].getPiece().SetJailPosition(p2Jail[0]);
                        p2Jail[1].ChangePiece(null);
                    }
                    if (p2Jail[2].getPiece() != null)
                    {
                        p2Jail[1].ChangePiece(p2Jail[2].getPiece());
                        p2Jail[1].getPiece().SetJailPosition(p2Jail[1]);
                        p2Jail[2].ChangePiece(null);
                    }
                }
            }
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
            if (p1Revives[0].getPiece() == null)
            {
                if (p1Jail[0].getPiece() != null)
                {
                    p1Revives[0].ChangePiece(p1Jail[0].getPiece());
                    p1Revives[0].getPiece().SetPosition(p1Revives[0].r, p1Revives[0].c);
                    p1Revives[0].getPiece().inJail = false;
                    p1Jail[0].ChangePiece(null);
                    if (p1Jail[1].getPiece() != null)
                    {
                        p1Jail[0].ChangePiece(p1Jail[1].getPiece());
                        p1Jail[0].getPiece().SetJailPosition(p1Jail[0]);
                        p1Jail[1].ChangePiece(null);
                    }
                    if (p1Jail[2].getPiece() != null)
                    {
                        p1Jail[1].ChangePiece(p1Jail[2].getPiece());
                        p1Jail[1].getPiece().SetJailPosition(p1Jail[1]);
                        p1Jail[2].ChangePiece(null);
                    }
                }
            }
            if (p1Revives[1].getPiece() == null)
            {
                if (p1Jail[0].getPiece() != null)
                {
                    p1Revives[1].ChangePiece(p1Jail[0].getPiece());
                    p1Revives[1].getPiece().SetPosition(p1Revives[1].r, p1Revives[1].c);
                    p1Revives[1].getPiece().inJail = false;
                    p1Jail[0].ChangePiece(null);
                    if (p1Jail[1].getPiece() != null)
                    {
                        p1Jail[0].ChangePiece(p1Jail[1].getPiece());
                        p1Jail[0].getPiece().SetJailPosition(p1Jail[0]);
                        p1Jail[1].ChangePiece(null);
                    }
                    if (p1Jail[2].getPiece() != null)
                    {
                        p1Jail[1].ChangePiece(p1Jail[2].getPiece());
                        p1Jail[1].getPiece().SetJailPosition(p1Jail[1]);
                        p1Jail[2].ChangePiece(null);
                    }
                }
            }
            if (p1Revives[2].getPiece() == null)
            {
                if (p1Jail[0].getPiece() != null)
                {
                    p1Revives[2].ChangePiece(p1Jail[0].getPiece());
                    p1Revives[2].getPiece().SetPosition(p1Revives[2].r, p1Revives[2].c);
                    p1Revives[2].getPiece().inJail = false;
                    p1Jail[0].ChangePiece(null);
                    if (p1Jail[1].getPiece() != null)
                    {
                        p1Jail[0].ChangePiece(p1Jail[1].getPiece());
                        p1Jail[0].getPiece().SetJailPosition(p1Jail[0]);
                        p1Jail[1].ChangePiece(null);
                    }
                    if (p1Jail[2].getPiece() != null)
                    {
                        p1Jail[1].ChangePiece(p1Jail[2].getPiece());
                        p1Jail[1].getPiece().SetJailPosition(p1Jail[1]);
                        p1Jail[2].ChangePiece(null);
                    }
                }
            }
        }

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
                SetWinner(2);
                return;
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
                SetWinner(1);
                return;
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
            if (c.type != TableObj.pieceType.P2ZONE && c.type != TableObj.pieceType.P2KEY) {
                finished = false;
                break;
            }
        }
        if (finished) 
        {
            SetWinner(1);
            return;
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
            if (c.type != TableObj.pieceType.P1ZONE && c.type != TableObj.pieceType.P1KEY) {
                finished = false;
                break;
            }
        }
        if (finished) 
        {
            SetWinner(2);
            return;
        }
    
    }

    public void AbandonVictory()
    {
        SetWinner(localPlayer);
    }

    private void SetWinner(int player)
    {
        PhotonNetwork.LeaveRoom();
        turnText.text = (player==1?"BLUE":"RED") + " PLAYER WINS";
        gameOver = true;

        endScreen.SetActive(true);
        endText.text = ((isOnline)?((localPlayer==player)?("YOU WIN"):("YOU LOSE")):((player==1?"BLUE":"RED") + " PLAYER WINS"));
    }
}
