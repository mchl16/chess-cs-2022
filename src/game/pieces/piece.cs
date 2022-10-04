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

    public enum Color{
        White=1,
        Black=-1
    };

    public Color color{get;init;}

    public int move_count{get;protected set;}

    public bool moved{get => move_count>0;}

    protected Board _my_board;

    public int x{get;protected set;}
    public int y{get;protected set;}    

    public enum Direction{
        None, Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft
    };

    protected Board.AttackType attack_type{get=>color==Color.White ? Board.AttackType.White : Board.AttackType.Black;}

    /* constructors and destructors */

    public Piece(Board board,Color color,int x,int y,int move_count=0){
        this._my_board=board;
        this.color=color;
        this.x=x;
        this.y=y;
        this.move_count=move_count;
    }

    /* methods */

    public abstract bool CheckMove(int x,int y);

    public virtual bool MoveTo(int x,int y){ 
        _my_board.MovePiece(this,x,y);
        this.x=x;
        this.y=y;
        ++this.move_count;
        return PostMove();
    }

    protected virtual bool PostMove() => false; //to be overridden by pawns when they promote

    public abstract Board.AttackType CheckForChecksOrPins();

    protected void SetDirection(Direction dir,out int x,out int y){
        x=y=0;
        switch(dir){
            case Direction.Up:
                ++y;
                break;
            case Direction.UpRight:
                ++x;
                ++y;
                break;
            case Direction.Right:
                ++x;
                break;
            case Direction.DownRight:
                ++x;
                --y;
                break;
            case Direction.Down:
                --y;
                break;
            case Direction.DownLeft:
                --x;
                --y;
                break;
            case Direction.Left:
                --x;
                break;
            case Direction.UpLeft:
                --x;
                ++y;
                break;
            default:
                throw new ArgumentException("How did you manage to get here?");
        }
    }

    protected bool CheckForChecksOrPinsDirectional(Direction dir){
        int x2,y2;
        SetDirection(dir,out x2,out y2);

        for (int x=this.x+x2,y=this.y+y2;x>=0 && x<8 && y>=0 && y<8;x+=x2,y+=y2){     
            _my_board[x,y].attacked|=(color==Color.White ? Board.AttackType.White : Board.AttackType.Black);

            switch((int)_my_board[x,y].piece_type*(int)color){
                case -(int)PieceType.King:
                    return true;
                case 0:
                    break;
                default:
                    return false;
            }
        }
        return false;
    }

    protected bool CheckMoveHorizontalVertical(int x,int y){
        int f;
        if(this.x==x){
            f=y.CompareTo(this.y);
            for (int i=this.y+f;i!=y;i+=f) if(_my_board[x,i].piece!=null) return false;
            return true;
        }
        else if(this.y==y){
            f=x.CompareTo(this.x);
            
            for (int i=this.x+f;i!=x;i+=f) if(_my_board[i,y].piece!=null) return false;
            return true;
        }
        else return false;
    }

    protected bool CheckMoveDiagonal(int x,int y){
        int f;
        if(this.x+y==this.y+x){ //right diagonal
            f=x.CompareTo(this.x);
            for (int i=this.x+f,j=this.y+f;i!=x+f;i+=f,j+=f) if(_my_board[i,j].piece!=null) return false;
            return true;
        }
        else if(x+y==this.x+this.y){ //left diagonal
            f=x.CompareTo(this.x);
            for (int i=this.x+f,j=this.y-f;i!=x+f;i+=f,j-=f) if(_my_board[i,j].piece!=null) return false;
            return true;
        }
        else return false;
    }

    public void RestoreMoveCount(Board caller){
        if(caller==_my_board) --move_count;
    }
}