using System;

public class Rook : Piece{
    public override PieceType type{get=>PieceType.Rook;}

    public Rook(Board board,Color color,int x,int y) : base(board,color,x,y){}

    public override bool CheckMove(int x,int y) => CheckMoveHorizontalVertical(x,y);

    public override Board.AttackType CheckForChecksOrPins(){
        for (int i=1;i<=7;i+=2) if(CheckForChecksOrPinsDirectional((Direction)i)) return attack_type;
        return Board.AttackType.None;
    }
}