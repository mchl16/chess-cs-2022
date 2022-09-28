using System;

public class Bishop : Piece{
    public override PieceType type{get=>PieceType.Bishop;}

    public Bishop(Board board,Color color,int x,int y) : base(board,color,x,y){}

    public override bool CheckMove(int x,int y) => CheckMoveDiagonal(x,y);

    public override bool CheckForChecksOrPins(){
        for (int i=2;i<=8;i+=2) if(CheckForChecksOrPinsDirectional((Direction)i)) return true;
        return false;
    }
}