using System;

public class Bishop : Piece{
    public override PieceType type{get=>PieceType.Bishop;}

    public Bishop(Board board,Color color,int x,int y) : base(board,color,x,y){}

    public override bool CheckMove(int x,int y){
        int f;
        if(this.x+y==this.y+x){
            f=x.CompareTo(this.x);
            for (int i=this.x+f,j=this.y+f;i!=x-f;i+=f,j+=f) if(_my_board[i,j]!=null) return false;
            return true;
        }
        else if(this.x+x==this.y+y){
            f=x.CompareTo(this.x);
            for (int i=this.x+f,j=this.y-f;i!=x-f;i+=f,j-=f) if(_my_board[i,j]!=null) return false;
            return true;
        }
        else return false;
    }

    public override bool CheckForChecksOrPins(){
        for (int i=2;i<=8;i+=2) if(CheckDirectional((Direction)i)) return true;
        return false;
    }
}