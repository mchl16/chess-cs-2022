using System;

class Queen : Piece{
    public override PieceType type{get=>PieceType.Queen;}

    public Queen(Board board,Color color,int x,int y) : base(board,color,x,y){}

    public override bool CheckMove(int x,int y){
        int f;
        if(this.x==x){ //rook-like moves
            f=x.CompareTo(this.x);
            for (int i=this.x+f;i!=x-f;i+=f) if(_my_board[i,y]!=null) return false;
            return true;
        }
        else if(this.y==y){
            f=y.CompareTo(this.y);
            for (int i=this.y+f;i!=y-f;i+=f) if(_my_board[x,i]!=null) return false;
            return true;
        }

        if(this.x+y==this.y+x){ //bishop-like moves
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
        for (int i=1;i<=8;++i) if(CheckDirectional((Direction)i)) return true;
        return false;
    }
}