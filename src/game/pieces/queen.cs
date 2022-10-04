using System;

class Queen : Piece{
    public override PieceType type{get=>PieceType.Queen;}

    public Queen(Board board,Color color,int x,int y,int move_count=0) : base(board,color,x,y,move_count){}

    public override bool CheckMove(int x,int y) => CheckMoveHorizontalVertical(x,y) || CheckMoveDiagonal(x,y);

    public override Board.AttackType CheckForChecksOrPins(){
        for (int i=1;i<=8;++i) if(CheckForChecksOrPinsDirectional((Direction)i)) return attack_type;
        return Board.AttackType.None;
    }
}