using System;

class Queen : Piece{
    public override PieceType type{get=>PieceType.Queen;}

    public Queen(Board board,Color color,int x,int y) : base(board,color,x,y){}

    public override bool CheckMove(int x,int y) => CheckMoveHorizontalVertical(x,y) || CheckMoveDiagonal(x,y);

    public override bool CheckForChecksOrPins(){
        for (int i=1;i<=8;++i) if(CheckForChecksOrPinsDirectional((Direction)i)) return true;
        return false;
    }
}