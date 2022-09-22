using System;

public class Rook : Piece{
    public override PieceType type{get=>PieceType.Rook;}

    public Rook(Board board,Color color,int x,int y) : base(board,color,x,y){}

    public override bool CheckMove(int x,int y){
        int f;
        if(this.x==x){
            f=x.CompareTo(this.x);
            for (int i=this.x+f;i!=x-f;i+=f) if(_my_board[i,y]!=null) return false;
            return true;
        }
        else if(this.y==y){
            f=y.CompareTo(this.y);
            for (int i=this.y+f;i!=y-f;i+=f) if(_my_board[x,i]!=null) return false;
            return true;
        }
        else return false;
    }

    public override bool CheckForChecksOrPins(){
        for (int i=1;i<=7;i+=2) if(CheckDirectional((Direction)i)) return true;
        return false;
    }
}