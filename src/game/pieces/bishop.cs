using System;

public class Bishop : Piece{
    public override PieceType type{get=>PieceType.Bishop;}

    public Bishop(Board board,Color color,int x,int y,int move_count=0) : base(board,color,x,y,move_count){}

    public override bool CheckMove(int x,int y) => CheckMoveDiagonal(x,y);

    public override Board.AttackType CheckForChecksOrPins(){
        for (int i=2;i<=8;i+=2) if(CheckForChecksOrPinsDirectional((Direction)i)) return attack_type;
        return Board.AttackType.None;
    }
}