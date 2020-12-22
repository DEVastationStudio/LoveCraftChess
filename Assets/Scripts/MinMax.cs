using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Instantanea 
{
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
    public List<Vector2> moves;
    public Piece curPiece;
    public int curPlayer;

    public int cellPos = 1;
    public bool initialTurn = true;
    public bool gameOver = false;

    public Instantanea(
        Cell[,] cells, List<Cell> p1Start, List<Cell> p2Start, List<Cell> p1Jail, 
        List<Cell> p2Jail, List<Cell> p1Keys, List<Cell> p2keys, Cell[] p1Revives, 
        Cell[] p2Revives, List<Piece> p1Pieces, List<Piece> p2Pieces, List<Vector2> moves, 
        Piece curPiece, int curPlayer, int cellPos, bool initialTurn, bool gameOver) 
    {

        this.cells = cells;
        this.p1Start = p1Start;
        this.p2Start = p2Start;
        this.p1Jail = p1Jail;
        this.p2Jail = p2Jail;
        this.p1Keys = p1Keys;
        this.p2Keys = p2keys;
        this.p1Revives = p1Revives;
        this.p2Revives = p2Revives;
        this.p1Pieces = p1Pieces;
        this.p2Pieces = p2Pieces;
        this.moves = moves;
        this.curPiece = curPiece;
        this.curPlayer = curPlayer;
        this.cellPos = cellPos;
        this.initialTurn = initialTurn;
        this.gameOver = gameOver;
    }
}
public class MinMax : MonoBehaviour
{

    [SerializeField] private TableGenerator TB;


    private Instantanea CopyTB() 
    {
        return new Instantanea(
            TB.cells, TB.p1Start, TB.p2Start, TB.p1Jail, TB.p2Jail, TB.p1Keys, TB.p2Keys, 
            TB.p1Revives, TB.p2Revives, TB.p1Pieces, TB.p2Pieces, TB.moves, TB.curPiece,
            TB.curPlayer, TB.cellPos, TB.initialTurn, TB.gameOver);
    }

    private void PasteTB(Instantanea ins)
    {
        TB.cells = ins.cells;
        TB.p1Start = ins.p1Start;
        TB.p2Start = ins.p2Start;
        TB.p1Jail = ins.p1Jail;
        TB.p2Jail = ins.p2Jail;
        TB.p1Keys = ins.p1Keys;
        TB.p2Keys = ins.p2Keys;
        TB.p1Revives = ins.p1Revives;
        TB.p2Revives = ins.p2Revives;
        TB.p1Pieces = ins.p1Pieces;
        TB.p2Pieces = ins.p2Pieces;
        TB.moves = ins.moves;
        TB.curPiece = ins.curPiece;
        TB.curPlayer = ins.curPlayer;
        TB.cellPos = ins.cellPos;
        TB.initialTurn = ins.initialTurn;
        TB.gameOver = ins.gameOver;
    }

    public int alphaBeta(int profundidad, int player, int alpha, int beta) 
    {
        Instantanea ins = CopyTB();
        TB.curPlayer = player;
        if (profundidad == 0) return heuristica(player);
        if (player == 1) 
        {
            for(int i = 0; i<ins.p1Pieces.Count;i++)
            {
                Piece p = ins.p1Pieces[i];
                if (!p.inJail) 
                {
                    Debug.Log(profundidad+" del jugador "+player+" de la pieza "+p.type);
                    TB.curPiece = p;
                    TB.GetMoves(p.type, p.r, p.c);
                    ins.moves = TB.moves.ToList();
                    foreach (Vector2 v in ins.moves)
                    {
                        TB.curPiece = p;
                        if (TB.curPiece.inJail) continue;
                        Debug.Log("Movimiento "+v);
                        TB.MovePiece((int)v.x, (int)v.y, true);
                        PasteTB(ins);
                        alpha = Math.Max(alpha, alphaBeta(profundidad - 1, 2, alpha, beta));
                        if (beta <= alpha) return alpha;
                    }
                    PasteTB(ins);
                }
            }
            return alpha;
        }
        else
        {
            for (int i = 0; i < ins.p2Pieces.Count; i++)
            {
                Piece p = ins.p2Pieces[i];
                if (!p.inJail)
                {
                    Debug.Log(profundidad + " del jugador " + player + " de la pieza " + p.type);
                    TB.curPiece = p;
                    TB.GetMoves(p.type, p.r, p.c);
                    ins.moves = TB.moves.ToList();
                    for (int j = 0; j < ins.moves.Count; j++)
                    {
                        Vector2 v = ins.moves[j];
                        TB.curPiece = p;
                        if (TB.curPiece.inJail) continue;
                        Debug.Log("Movimiento " + v);
                        TB.MovePiece((int)v.x, (int)v.y, true);
                        PasteTB(ins);
                        beta = Math.Min(alpha, alphaBeta(profundidad - 1, 1, alpha, beta));
                        if (beta <= alpha) return beta;
                    }
                    PasteTB(ins);
                }
            }
            return beta;
        }
    }

    public void startMinMax() 
    {
        Instantanea ins = CopyTB();
        Debug.Log(alphaBeta(3,1,-999999, +999999));
        PasteTB(ins);
        colocarPiezas();
    }

    private void colocarPiezas()
    {
        foreach (Piece p in TB.p1Pieces) 
        {
            if (!p.inJail)
                p.SetPosition(p.r, p.c, false);
            else
                p.SetJailPosition(TB.p1Jail[p.jailPos], p.jailPos, false);
        }

        foreach (Piece p in TB.p2Pieces)
        {
            if (!p.inJail)
                p.SetPosition(p.r, p.c, false);
            else
                p.SetJailPosition(TB.p2Jail[p.jailPos], p.jailPos, false);
        }
    }

    private int heuristica(int player)
    {
        if (player == 1) 
        {
            Debug.Log("Heuristica => "+ (TB.p1Pieces.Count - TB.p2Pieces.Count));
            return TB.p1Pieces.Count - TB.p2Pieces.Count;
        }
        else
        {
            Debug.Log("Heuristica => " + (TB.p2Pieces.Count - TB.p1Pieces.Count));
            return TB.p2Pieces.Count - TB.p1Pieces.Count;
        }
    }
}
