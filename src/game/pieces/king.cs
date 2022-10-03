using System;

class King : Piece{
    public override PieceType type{get=>PieceType.King;}

    public King(Board board,Color color,int x,int y) : base(board,color,x,y){}

    public override bool CheckMove(int x,int y){
        if(x-this.x<=1 && this.x-x<=1 && y-this.y<=1 && this.y-y<=1) return !CheckAttack(x,y);
        else if(!moved && y==this.y){ //castling 
            
            switch(x-this.x){
                case 2: //king's side castling
                    return CheckCastle(CastleDirection.King);
                case -2: //queen's side castling
                    return CheckCastle(CastleDirection.Queen);
                default:
                    Console.WriteLine(x-this.x);
                    return false;
            }
        } 
        else return false;
    }

    protected bool CheckAttack(int x,int y){
        return (_my_board[x,y].attacked&(color==Color.White ? Board.AttackType.Black : Board.AttackType.White))!=0;
    }

    protected enum CastleDirection{
        Queen=-1,
        King=1
    };

    protected bool CheckCastle(CastleDirection dir){
        int k=(int)dir;
        if(_my_board[(k==-1 ? 0 : 7),y].piece_type*(int)color!=(int)PieceType.Rook) return false;
        if(_my_board[(k==-1 ? 0 : 7),y].piece.moved) return false;

        if(_my_board[k+4,y].piece!=null) return false; //nothing can obstruct our darlings
        if(_my_board[2*k+4,y].piece!=null) return false; //king cheating on his wife with a rook smh
        if(k==-1 && _my_board[1,y].piece!=null) return false;

        if(CheckAttack(k+4,y) || CheckAttack(2*k+4,y)) return false; //fields on king's way cannot be attacked
        
        return true;
    }

    public override bool MoveTo(int x,int y){
        if(x-this.x==2) _my_board[7,y].piece.MoveTo(5,y);  //move rook (unlike in IRL chess, king goes second,
        if(x-this.x==-2) _my_board[0,y].piece.MoveTo(3,y); //it's easier to code and fuck him)
        base.MoveTo(x,y);
        return false;
    }

    public override Board.AttackType CheckForChecksOrPins(){
        int x=0,y=0;
        for (int i=1;i<=8;++i){
            SetDirection((Direction)i,out x,out y);
            if(x>=0 && x<8 && y>=0 && y<8) _my_board[x,y].attacked|=attack_type;
        }
        return Board.AttackType.None; //kings are incapable of checking or pinning, they're pussies
    } 
}