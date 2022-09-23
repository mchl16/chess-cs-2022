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
                if(this.y+(int)color==y) return (_my_board[x,this.y+(int)color]==null); //the basic move
                else if(move_count==0 && this.y+2*(int)color==y){ //advance 2 squares during the first move
                    return (_my_board[x,this.y+(int)color]==null && _my_board[x,this.y+2*(int)color]==null);
                } 
                else return false; //nothing else is legal, I guess

                 
            case -1:
            case 1:
                if(this.y+(int)color==y){
                    if((int)_my_board.GetPieceType(x,y)*(int)color<0) return true;
                    else if((int)_my_board.GetPieceType(x,this.y)==-(int)PieceType.Pawn*(int)color){ //en passant
                        
                        return _my_board[x,this.y].move_count==1; 
                    }
                    else return false;

                }
                else return false;
            default:
                return false;
        }        
    }

    protected override void PostMove(){
        if(y==7) _my_board.MovePiece(Promote(x,7),x,7); //replace pawn with a new piece
        else if(y==0) _my_board.MovePiece(Promote(x,0),x,0);
    }

    public override bool CheckForChecksOrPins(){
        if(x>0 && (int)_my_board.GetPieceType(x-1,y+(int)color)*(int)color==(int)PieceType.King) return true;
        if(x<7 && (int)_my_board.GetPieceType(x+1,y+(int)color)*(int)color==(int)PieceType.King) return true;
        return false;
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