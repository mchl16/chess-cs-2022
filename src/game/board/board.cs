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

    public AttackType EnemyAttackType(Piece.Color col) => col==Piece.Color.White ? AttackType.Black : AttackType.White;

    public int move_count{get;protected set;}

    public Piece.Color WhoseTurn{get => move_count%2==1 ? Piece.Color.Black : Piece.Color.White;}

    public Piece.Color NotWhoseTurn{get => move_count%2==1 ? Piece.Color.White : Piece.Color.Black;}

    public int last_x{get;protected set;}=-1;
    public int last_y{get;protected set;}=-1;

    public bool en_passant;

    public bool promote;

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

    List<List<PiecePrevious>> previous_moves;

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

    public Board(BoardInitMode mode,out InputCallback status,string data) : this(){
        switch(mode){
            case BoardInitMode.DefaultPosition:
            case BoardInitMode.FEN:
                try{
                    BoardCreator.InitializeFromFEN(this,data);
                }
                catch{
                    throw;
                }
                break;

            default: //to be rewritten later
                throw new NotImplementedException(); 
        }
        status=new InputCallback(CheckForChecksOrPins() switch{
            AttackType.None => InputCallback.Type.NothingSpecial,
            AttackType.White => InputCallback.Type.CheckWhite,
            AttackType.Black => InputCallback.Type.CheckBlack,
            _ => throw new ArgumentException("Attempted to create a board from an invalid position") 
        },""); //both sides can't check each other at once
    }    

    public Board(BoardInitMode mode,out InputCallback status)
     : this(mode,out status,"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"){}

    /* methods */

    protected InputCallback.Type CheckCallback(Piece.Color col){
        return col==Piece.Color.White ? InputCallback.Type.CheckWhite : InputCallback.Type.CheckBlack;
    }

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

    public InputCallback MakeMove(Piece.Color color,int x0,int y0,int x,int y,bool test=false){
        Console.WriteLine(move_count+" "+x0+" "+y0+" "+x+" "+y);
        var check1=CheckBasicConditions(color,x0,y0,x,y);
        if(check1.result==InputCallback.Type.Error) return check1;

        if(!fields[x0,y0].piece.CheckMove(x,y)) return new InputCallback(InputCallback.Type.Error,"Illegal move");

        Piece? taken_piece=null; //save information of a piece to be taken if it needs to be restored
        if(fields[x,y].piece!=null) taken_piece=fields[x,y].piece;
        else if(en_passant && fields[x,y0].piece!=null){
            if(fields[x0,y0].piece.type==Piece.PieceType.Pawn && x!=x0) taken_piece=fields[x,y0].piece;
            else taken_piece=null;
        }
        else taken_piece=null;

        previous_moves.Add(new List<PiecePrevious>());

        if(taken_piece!=null){
            taken_piece.MoveTo(taken_piece.x,taken_piece.y); //a dumb way to increment its move count while doing nothing (useful when restoring a piece)
            previous_moves[^1].Add(new PiecePrevious(taken_piece,taken_piece.x,taken_piece.y,en_passant));
        }

        if(fields[x0,y0].piece.type==Piece.PieceType.King){ //save information about castling if necessary
            switch(x-x0){
                case 2: //king castling
                    previous_moves[^1].Add(new PiecePrevious(fields[7,y0].piece,7,y0,en_passant));
                    break;

                case -2: //queen castling
                    previous_moves[^1].Add(new PiecePrevious(fields[0,y0].piece,0,y0,en_passant));
                    break;
            }
        }
        previous_moves[^1].Add(new PiecePrevious(fields[x0,y0].piece,x0,y0,en_passant));

        if(en_passant && last_x==x && last_y==y0) fields[x,y0].piece=null!; //en passant capture, what an awful rule
        en_passant=false; //it gets way more attention in this code than actual use in chess

        bool f=fields[x0,y0].piece.MoveTo(x,y); //true when a pawn reaches the last row

        AttackType check=CheckForChecksOrPins();
        
        ++move_count;
        if((check&EnemyAttackType(color))!=AttackType.None){
            UndoLastMove();
            return new InputCallback(InputCallback.Type.Error,"Cannot move a piece so king is attacked afterwards");
        }

        if(f){
            promote=true;
            return new InputCallback(InputCallback.Type.Promote,$"{x} {y}");
        }

        last_x=x;
        last_y=y;

        if(check!=Board.AttackType.None){
            Piece.Color color2=(color==Piece.Color.White ? Piece.Color.Black : Piece.Color.White);
            if(test){ //disables FindCheckSolutions
                return new InputCallback(CheckCallback(color),""); 
            }
            return new InputCallback(FindCheckSolutions(color2) ? CheckCallback(color) : InputCallback.Type.Checkmate,"");
        }
        else return new InputCallback(InputCallback.Type.NothingSpecial,"");
    }

    protected AttackType CheckForChecksOrPins(){
        foreach(var i in fields) i.attacked=AttackType.None;
        AttackType check=AttackType.None;
        foreach(var i in fields) if(i.piece!=null) check|=i.piece.CheckForChecksOrPins();
        return check;
    }

    protected bool FindCheckSolutions(Piece.Color color){
        for (int x0=0;x0<8;++x0) for (int y0=0;y0<8;++y0){
            if(fields[x0,y0].piece_type*(int)color<=0) continue;

            for (int x=0;x<8;++x) for (int y=0;y<8;++y){
            //    Piece.Color col=(color==Piece.Color.White ? Piece.Color.Black : Piece.Color.White);
                var res=MakeMove(color,x0,y0,x,y,true).result;

                if(res!=InputCallback.Type.Error) UndoLastMove();
                
                if(res==CheckCallback(color) || res==InputCallback.Type.NothingSpecial) return true;
            }
        }
        return false;
    }

    public InputCallback UndoLastMove(){
        RestorePieces();
        var result=(CheckForChecksOrPins()==AttackType.None ? InputCallback.Type.NothingSpecial : CheckCallback(WhoseTurn));
        return new InputCallback(result,"");
    }

    protected void RestorePieces(){
        foreach(var i in ((IEnumerable<PiecePrevious>)previous_moves[^1]).Reverse()){
            Console.WriteLine("?");
            MovePiece(i.piece,i.x,i.y);
            i.piece.UndoMove(i.x,i.y);
            en_passant=i.en_passant;
            last_x=i.x;
            last_y=i.y;
        }
        --move_count;
        previous_moves.RemoveAt(previous_moves.Count-1);
    }

    public InputCallback AddPiece(string name,int x,int y){
        Piece.Color col=NotWhoseTurn; //relies on the fact that move count is increased before the callback
        try{
            MovePiece(BoardCreator.NewPiece(this,col,name,x,y),x,y);
        }
        catch(Exception e){
            return new InputCallback(InputCallback.Type.Error,e.Message);
        }

        AttackType check=CheckForChecksOrPins();
        if(check!=AttackType.None) return new InputCallback(CheckCallback(col),""); 
        else return new InputCallback(InputCallback.Type.NothingSpecial,"");
    }

    public void MovePiece(Piece piece,int x,int y){
        fields[piece.x,piece.y].piece=null!; //the most annoying C# 8.0 "addition" (CS8625)
        fields[x,y].piece=piece;
    }
}