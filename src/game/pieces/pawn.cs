using System;

public class Pawn : Piece{
    /* fields and properties */

    public override PieceType type{get=>PieceType.Pawn;}

    /* constructors and destructors */

    public Pawn(Board board,Color color,int x,int y) : base(board,color,x,y){}

    /* methods */

    public override bool CheckMove(int x,int y){
        switch(x-this.x){
            case 0:
                if(this.y+(int)color==y) return (_my_board[x,this.y+(int)color].piece==null); //the basic move
                else if(!moved && this.y+2*(int)color==y){ //advance 2 squares during the first move
                    
                    return (_my_board[x,this.y+(int)color].piece==null && _my_board[x,this.y+2*(int)color].piece==null);
                } 
                else return false; //nothing else is legal, I guess

                 
            case -1:
            case 1:
                if(this.y+(int)color==y){
                    if((int)_my_board[x,y].piece_type*(int)color<0) return true;
                    else if((int)_my_board[x,this.y].piece_type==-(int)PieceType.Pawn*(int)color){ //en passant
                        if(_my_board.en_passant){
                            return (_my_board.last_x==x && _my_board.last_y==this.y);
                        }
                    }
                }
                return false;
            default:
                return false;
        }        
    }

    protected override Board.InputCallback PostMove(){
        if(move_count==1 && (y==3 || y==4)) _my_board.en_passant=true;
        if(y==7 || y==0) return new Board.InputCallback(Board.InputCallback.Type.Promote,""); //replace pawn with a new piece
        else return new Board.InputCallback(Board.InputCallback.Type.NothingSpecial,"");
    }

    public override Board.AttackType CheckForChecksOrPins(){
        if(x>0){
            _my_board[x-1,y+(int)color].attacked|=attack_type;
            if((int)_my_board[x-1,y+(int)color].piece_type*(int)color==(int)PieceType.King) return attack_type;
        }
        if(x<7){
            _my_board[x+1,y+(int)color].attacked|=attack_type;
            if((int)_my_board[x+1,y+(int)color].piece_type*(int)color==(int)PieceType.King) return attack_type;
        }
        return Board.AttackType.None;
    }

    protected Piece Promote(int x,int y){
        int n=Convert.ToInt32(Console.ReadLine()); //placeholder
        switch(n){
            case 3:
                return new Rook(_my_board,color,x,y);
            case 4:
                return new Knight(_my_board,color,x,y); 
            case 5:
                return new Bishop(_my_board,color,x,y);       
            case 6:
                return new Queen(_my_board,color,x,y);
            default:
                throw new ArgumentException("What the fuck have you just done?"); //ofc this is to be replaced 
        }
    }
}