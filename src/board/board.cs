using System;


/* */
public class Board{
    /* fields */

    public Piece[,] pieces{get;private set;}

    public Piece this[int a,int b]{
        get => pieces[a,b];
        private set => pieces[a,b]=value;
    }

    [Flags]
    public enum AttackType{
        None=0x0,
        White=0x1,
        Black=0x2,
        Both=0x3
    };

    public AttackType[,] attacked{get;private set;}
    
    /* constructors and destructors */

    public enum BoardInitMode{
        DefaultPosition,
        FEN,
        PGN
    };

    protected Board(){
        this.pieces=new Piece[8,8];
        this.attacked=new AttackType[8,8];
    }

    public Board(BoardInitMode mode,string data="") : this(){
        switch(mode){
            case BoardInitMode.DefaultPosition: 
                for (int i=0;i<8;++i){
                    pieces[i,1]=new Pawn(this,Piece.Color.White,i,1);
                    pieces[i,7]=new Pawn(this,Piece.Color.Black,i,7);
                }
            
                pieces[0,0]=new Rook(this,Piece.Color.White,0,0); //ugly code, I know
                pieces[7,0]=new Rook(this,Piece.Color.White,7,0);
                pieces[0,7]=new Rook(this,Piece.Color.Black,0,7);
                pieces[7,7]=new Rook(this,Piece.Color.Black,7,7);

                pieces[1,0]=new Knight(this,Piece.Color.White,1,0); 
                pieces[6,0]=new Knight(this,Piece.Color.White,6,0);
                pieces[1,7]=new Knight(this,Piece.Color.Black,1,7);
                pieces[6,7]=new Knight(this,Piece.Color.Black,6,7);

                pieces[2,0]=new Bishop(this,Piece.Color.White,2,0); 
                pieces[5,0]=new Bishop(this,Piece.Color.White,5,0);
                pieces[2,7]=new Bishop(this,Piece.Color.Black,2,7);
                pieces[5,7]=new Bishop(this,Piece.Color.Black,5,7);

                pieces[3,0]=new Queen(this,Piece.Color.White,3,0);
                pieces[3,7]=new Queen(this,Piece.Color.Black,3,7);

                pieces[4,0]=new King(this,Piece.Color.White,3,0); //kings go last, they're shit
                pieces[4,7]=new King(this,Piece.Color.Black,3,7);

                break;

            default: //to be rewritten later
                goto case BoardInitMode.DefaultPosition;    
        }
    }

    /* methods */

    public int GetPieceType(int x,int y){
        if(pieces[x,y]==null) return 0;
        return (int)pieces[x,y].type*(int)pieces[x,y].color;
    }

    public void MakeMove(int x0,int y0,int x,int y){
        if(x==x0&&y==y0) throw new ArgumentException("A piece must be moved to another field");
        if(pieces[x0,y0]==null) throw new ArgumentException("No piece at the selected field");
        if(GetPieceType(x,y).CompareTo(0)*GetPieceType(x0,y0).CompareTo(0)==1){
            throw new ArgumentException("Cannot move a piece to a field occupied by another piece of the same color");
        }

        if(pieces[x0,y0].CheckMove(x,y)) pieces[x0,y0].MoveTo(x,y); //actually move a piece to its new place
        else throw new ArgumentException("Illegal move");

        foreach(var i in pieces) if(i!=null) i.CheckForChecksOrPins();
    }

    public void MovePiece(Piece piece,int x,int y){
        pieces[piece.x,piece.y]=null!; //the most annoying C# 8.0 "addition" (CS8625)
        pieces[x,y]=piece;
    }
}