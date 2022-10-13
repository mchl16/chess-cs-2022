using System;

public class Pawn : Piece{
    /* fields and properties */

    public override PieceType type{get=>PieceType.Pawn;}

    protected int Advance(int n) => this.y+n*(int)color;

    /* constructors and destructors */

    public Pawn(Board board,Color color,int x,int y,int move_count=0) : base(board,color,x,y,move_count){}

    /* methods */

    public override bool CheckMove(int x,int y){
        switch(x-this.x){
            case 0:
                if(Advance(1)==y) return (_my_board[x,Advance(1)].piece==null); //the basic move
                else if(!moved && Advance(2)==y){ //advance 2 squares during the first move
                    
                    return (_my_board[x,Advance(1)].piece==null && _my_board[x,Advance(2)].piece==null);
                } 
                else return false; //nothing else is legal, I guess

                 
            case -1:
            case 1:
                if(Advance(1)==y){
                    if((int)_my_board[x,y].piece_type*(int)color<0) return true;
                    else if((int)_my_board[x,this.y].piece_type==-(int)PieceType.Pawn*(int)color){ //en passant
                        if(_my_board.en_passant){
                            Console.WriteLine(_my_board.last_x);
                            Console.WriteLine(_my_board.last_y);
                            return (_my_board.last_x==x && _my_board.last_y==this.y);
                        }
                    }
                }
                return false;
            default:
                return false;
        }        
    }

    protected override bool PostMove(){
        if(move_count==1 && (y==3 || y==4)) _my_board.en_passant=true;
        return (y==7 || y==0); //true if the pawn shall be promoted
    }

    public override Board.AttackType CheckForChecksOrPins(){
        if(y==0 || y==7) return Board.AttackType.None;
        if(x>0){
            _my_board[x-1,Advance(1)].attacked|=attack_type;
            if((int)_my_board[x-1,Advance(1)].piece_type*(int)color==-(int)PieceType.King) return attack_type;
        }
        if(x<7){
            _my_board[x+1,Advance(1)].attacked|=attack_type;
            if((int)_my_board[x+1,Advance(1)].piece_type*(int)color==-(int)PieceType.King) return attack_type;
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