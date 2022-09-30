using System;
using System.Collections.Generic;

public partial class Board{
    /* fields */

    public class Field{
        public Piece piece;
        
        public int piece_type{
            get => (piece==null ? 0 : (int)piece.type*(int)piece.color);
        }

        public AttackType attacked;

        public Field(){
            this.piece=null!;
        }

        public Field(Piece p) : this(){
            this.piece=p;
        }
    }

    public Field[,] fields{get;protected set;}

    public Field this[int a,int b]{ //a bit of syntactic sugar, ma'am
        get => fields[a,b];
        set => fields[a,b]=value;
    }

    [Flags]
    public enum AttackType{
        None=0x0,
        White=0x1,
        Black=0x2,
        Both=0x3
    };

    public AttackType check{get;protected set;}

    public int move_count{get;protected set;}

    public int last_x{get;protected set;}=-1; //coordinates of the field the last move was FROM
    public int last_y{get;protected set;}=-1;
    

    public bool en_passant;

    /* constructors and destructors */

    public enum BoardInitMode{
        DefaultPosition,
        FEN,
        PGN
    };

    protected Board(){
        this.fields=new Field[8,8];
        for(int i=0;i<8;++i) for (int j=0;j<8;++j) fields[i,j]=new Field();
    }

    public Board(BoardInitMode mode,string data) : this(){
        switch(mode){
            case BoardInitMode.DefaultPosition: 
                BoardCreator.InitializeFromFEN(this,"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
                break;
            case BoardInitMode.FEN:
                BoardCreator.InitializeFromFEN(this,data);
                break;

            default: //to be rewritten later
                goto case BoardInitMode.DefaultPosition;    
        }
    }

    public struct InputCallback{
        public enum Type{
            NothingSpecial=0,
            Promote=1,
            Check=2,
            Checkmate=3,
            Error=2137
        };

        public Type result{get;init;}
        public string? message{get;init;}

        public InputCallback(Type clb,string msg){
            this.result=clb;
            this.message=msg;
        }
    }

    /* methods */

    protected InputCallback CheckBasicConditions(Piece.Color color,int x0,int y0,int x,int y){
        if(x==x0 && y==y0){
            return new InputCallback(InputCallback.Type.Error,"A piece must be moved to another field");
        }
        if(fields[x0,y0].piece==null){
            return new InputCallback(InputCallback.Type.Error,"No piece at the selected field");
        }
        if(fields[x0,y0].piece.color!=color){
            return new InputCallback(InputCallback.Type.Error,"Cannot move pieces of opposite color");
        }
        if(fields[x,y].piece_type.CompareTo(0)*fields[x0,y0].piece_type.CompareTo(0)==1){
            return new InputCallback(InputCallback.Type.Error,
                                    "Cannot move a piece to a field occupied by another piece of the same color");
        }

        if(x<0 || x>7 || y<0 || y>7){
            return new InputCallback(InputCallback.Type.Error,"Cannot move a piece outside the board");
        }

        return new InputCallback(InputCallback.Type.NothingSpecial,"");
    }

    protected struct PiecePrevious{
        public Piece piece;
        public int x;
        public int y;

        public PiecePrevious(Piece piece,int x,int y){
            this.piece=piece;
            this.x=x;
            this.y=y;
        }
    }

    public InputCallback MakeMove(Piece.Color color,int x0,int y0,int x,int y,bool test=false){
        last_x=x0;
        last_y=y0;

        Piece? taken_piece;
        if(fields[x,y].piece!=null) taken_piece=fields[x,y].piece;
        else if(en_passant && fields[x,y0].piece!=null) taken_piece=fields[x,y0].piece;

        var check1=CheckBasicConditions(color,x0,y0,x,y);
        if(check1.result!=InputCallback.Type.NothingSpecial) return check1;

        List<PiecePrevious> prevs=new();

        if(fields[x0,y0].piece.CheckMove(x,y)){
            prevs.Add(new PiecePrevious(fields[x0,y0].piece,x0,y0));
            fields[x0,y0].piece.MoveTo(x,y); //actually move a piece to its new place
            if(en_passant && last_x==x && last_y==y0){
                fields[x,y0].piece=null!; //en passant capture, what an awful rule
                en_passant=false; //it gets way more attention in this code than actual use in chess
            }
        }
        else return new InputCallback(InputCallback.Type.Error,"Illegal move");

        foreach(var i in fields) i.attacked=AttackType.None;
        AttackType check=AttackType.None;
        foreach(var i in fields) if(i.piece!=null) check|=i.piece.CheckForChecksOrPins();
        if((check&(color==Piece.Color.White ? AttackType.Black : AttackType.White))!=AttackType.None){
            return new InputCallback(InputCallback.Type.Error,"Cannot move a piece so king is attacked afterwards");
        }

        ++move_count;
        en_passant=false;

        if(check!=Board.AttackType.None){
            if(test) return new InputCallback(InputCallback.Type.Check,"");
            return new InputCallback(FindCheckSolutions(color) ? InputCallback.Type.Check : InputCallback.Type.Checkmate,"");
        }
        else return new InputCallback(InputCallback.Type.NothingSpecial,"");

    }

    protected bool FindCheckSolutions(Piece.Color color){
        for (int x0=0;x0<8;++x0) for (int y0=0;y0<8;++y0){
            if(fields[x0,y0].piece_type*(int)color<=0) continue;
            for (int x=0;x<8;++x) for (int y=0;y<8;++y){
                Piece.Color col=color==Piece.Color.White ? Piece.Color.Black : Piece.Color.White;
                if(MakeMove(col,x0,y0,x,y,true).result!=InputCallback.Type.Check) return true; 
            }
        }
        return false;
    }

    protected void RestorePieces(IEnumerable<PiecePrevious> prevs){
        foreach(var i in prevs.Reverse()){
            MovePiece(i.piece,i.x,i.y);
            i.piece.RestoreMoveCount(this);
        }
    }

    public void MovePiece(Piece piece,int x,int y){
        fields[piece.x,piece.y].piece=null!; //the most annoying C# 8.0 "addition" (CS8625)
        fields[x,y].piece=piece;
    }
}