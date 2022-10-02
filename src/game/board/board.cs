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

    public int last_x{get;protected set;}=-1;
    public int last_y{get;protected set;}=-1;

    public bool en_passant;

    List<List<PiecePrevious>> previous_moves;

    /* constructors and destructors */

    public enum BoardInitMode{
        DefaultPosition,
        FEN,
        PGN
    };

    protected Board(){
        this.fields=new Field[8,8];
        for(int i=0;i<8;++i) for (int j=0;j<8;++j) fields[i,j]=new Field();
        previous_moves=new();
    }

    public Board(BoardInitMode mode,string data="rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1") : this(){
        switch(mode){
            case BoardInitMode.DefaultPosition:
            case BoardInitMode.FEN:
                BoardCreator.InitializeFromFEN(this,data);
                break;

            default: //to be rewritten later
                throw new NotImplementedException(); 
        }
    }

    public struct InputCallback{
        public enum Type{
            NothingSpecial=0,
            Promote=1,
            CheckWhite=2,
            CheckBlack=3,
            Checkmate=4,
            Error=2137
        };

        public Type result{get;init;}
        public string? message{get;init;}

        public InputCallback(Type clb,string msg){
            this.result=clb;
            this.message=msg;
        }
    }

    protected InputCallback.Type CheckCallback(Piece.Color col) => (InputCallback.Type)(2+(col==Piece.Color.White ? 0 : 1));

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
        public bool en_passant;

        public PiecePrevious(Piece piece,int x,int y,bool en_passant){
            this.piece=piece;
            this.x=x;
            this.y=y;
            this.en_passant=en_passant;
        }
    }

    public InputCallback MakeMove(Piece.Color color,int x0,int y0,int x,int y,bool test=false){
        Piece? taken_piece;
        if(fields[x,y].piece!=null) taken_piece=fields[x,y].piece;
        else if(en_passant && fields[x,y0].piece!=null) taken_piece=fields[x,y0].piece;

        var check1=CheckBasicConditions(color,x0,y0,x,y);
        if(check1.result!=InputCallback.Type.NothingSpecial) return check1;

        if(!fields[x0,y0].piece.CheckMove(x,y)) return new InputCallback(InputCallback.Type.Error,"Illegal move");
        if(fields[x0,y0].piece.type==Piece.PieceType.King){
            switch(x-x0){
                case 2: //king castling
                    previous_moves.Add(new List<PiecePrevious>{new PiecePrevious(fields[7,y0].piece,7,y0,en_passant)});
                    break;

                case -2: //queen castling
                    previous_moves.Add(new List<PiecePrevious>{new PiecePrevious(fields[0,y0].piece,0,y0,en_passant)});
                    break;

                default:
                    previous_moves.Add(new List<PiecePrevious>());
                    break;
            }
        }
        else previous_moves.Add(new List<PiecePrevious>());
        previous_moves[^1].Add(new PiecePrevious(fields[x0,y0].piece,x0,y0,en_passant));

        if(en_passant && last_x==x && last_y==y0) fields[x,y0].piece=null!; //en passant capture, what an awful rule
        en_passant=false; //it gets way more attention in this code than actual use in chess
        fields[x0,y0].piece.MoveTo(x,y); //actually move a piece to its new place

        foreach(var i in fields) i.attacked=AttackType.None;
        AttackType check=AttackType.None;
        foreach(var i in fields) if(i.piece!=null) check|=i.piece.CheckForChecksOrPins();
        if((check&(color==Piece.Color.White ? AttackType.Black : AttackType.White))!=AttackType.None){
            return new InputCallback(InputCallback.Type.Error,"Cannot move a piece so king is attacked afterwards");
        }

        ++move_count;
        last_x=x;
        last_y=y;

        if(check!=Board.AttackType.None){
            if(test){
                return new InputCallback(CheckCallback(color==Piece.Color.White ? Piece.Color.White : Piece.Color.Black),"");
            }
            return new InputCallback(FindCheckSolutions(color) ? CheckCallback(color) : InputCallback.Type.Checkmate,"");
        }
        else return new InputCallback(InputCallback.Type.NothingSpecial,"");
    }

    protected bool FindCheckSolutions(Piece.Color color){
        for (int x0=0;x0<8;++x0) for (int y0=0;y0<8;++y0){
            if(fields[x0,y0].piece_type*(int)color<=0) continue;
            for (int x=0;x<8;++x) for (int y=0;y<8;++y){
                Piece.Color col=(color==Piece.Color.White ? Piece.Color.Black : Piece.Color.White);
                var res=MakeMove(col,x0,y0,x,y,true).result;
                if(res==InputCallback.Type.NothingSpecial) return true; 
            }
        }
        return false;
    }

    protected void RestorePieces(List<IEnumerable<PiecePrevious>> prevs){
        foreach(var i in prevs.Last().Reverse()){
            MovePiece(i.piece,i.x,i.y);
            i.piece.RestoreMoveCount(this);
            en_passant=i.en_passant;
        }
        --move_count;
    }

    public void MovePiece(Piece piece,int x,int y){
        fields[piece.x,piece.y].piece=null!; //the most annoying C# 8.0 "addition" (CS8625)
        fields[x,y].piece=piece;
    }
}