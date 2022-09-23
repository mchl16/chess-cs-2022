using System;

public abstract class Piece{
    /* fields and properties */
    
    public enum PieceType{
        Pawn=1,
        King=2,
        Rook=3,
        Knight=4,
        Bishop=5,
        Queen=6
    };

    public abstract PieceType type{get;}

    public enum Color{White=1,Black=-1};
    public Color color{get;init;}

    public int move_count{get;protected set;}

    protected Board _my_board; //there will probably be a single board

    //to make the Board class responsible for storing this (or not? it's actually convenient this way)
    public int x{get;protected set;}
    public int y{get;protected set;}    

    public enum Direction{
        None, Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft
    };

    public Direction pinned_from;

    /* constructors and destructors */

    public Piece(Board board,Color color,int x,int y){
        this._my_board=board;
        this.color=color;
        this.x=x;
        this.y=y;
        this.move_count=0;
        this.pinned_from=Direction.None;
    }

    /* methods */

    public abstract bool CheckMove(int x,int y);

    public virtual void MoveTo(int x,int y){ 
        _my_board.MovePiece(this,x,y);
        this.x=x;
        this.y=y;
        ++this.move_count;
        PostMove();
    }

    protected virtual void PostMove(){} //to be overridden for pawns

    public abstract bool CheckForChecksOrPins();

    protected bool CheckForChecksOrPinsDirectional(Direction dir){
        Piece? ptr=null;
        int x2=0,y2=0;
        switch(dir){
            case Direction.Up:
                ++y2;
                break;
            case Direction.UpRight:
                ++x2;
                ++y2;
                break;
            case Direction.Right:
                ++x2;
                break;
            case Direction.DownRight:
                ++x2;
                --y2;
                break;
            case Direction.Down:
                --y2;
                break;
            case Direction.DownLeft:
                --x2;
                --y2;
                break;
            case Direction.Left:
                --x2;
                break;
            case Direction.UpLeft:
                --x2;
                ++y2;
                break;
            default:
                throw new ArgumentException("How did you manage to get here?");
        }

        bool res=false;
        for (int x=this.x+x2,y=this.y+y2;x>=0&&x<8&&y>=0&&y<8;x+=x2,y+=y2){
            if(ptr!=null){
                _my_board.attacked[x,y]=(Board.AttackType)((int)_my_board.attacked[x,y]|(color==Color.White ? 0x1 : 0x2));
            }
            
            switch((int)_my_board.GetPieceType(x,y)*(int)color){
                case <0:
                    if(_my_board[x,y].type==PieceType.King){
                        if(ptr!=null) ptr.pinned_from=dir;
                        res=true;
                    }
                    else ptr=_my_board[x,y];
                    break;
                case >0:
                    return res;
            }
            
        }
        return res;
    }

    protected bool CheckMoveHorizontalVertical(int x,int y){
        int f;
        if(this.x==x){
            f=y.CompareTo(this.y);
            for (int i=this.y+f;i!=y;i+=f) if(_my_board[x,i]!=null) return false;
            return true;
        }
        else if(this.y==y){
            f=x.CompareTo(this.x);
            
            for (int i=this.x+f;i!=x;i+=f) if(_my_board[i,y]!=null) return false;
            return true;
        }
        else return false;
    }

    protected bool CheckMoveDiagonal(int x,int y){
        int f;
        if(this.x+y==this.y+x){ //right diagonal
            f=x.CompareTo(this.x);
            for (int i=this.x+f,j=this.y+f;i!=x+f;i+=f,j+=f) if(_my_board[i,j]!=null) return false;
            return true;
        }
        else if(x+y==this.x+this.y){ //left diagonal
            f=x.CompareTo(this.x);
            for (int i=this.x+f,j=this.y-f;i!=x+f;i+=f,j-=f) if(_my_board[i,j]!=null) return false;
            return true;
        }
        else return false;
    }
}