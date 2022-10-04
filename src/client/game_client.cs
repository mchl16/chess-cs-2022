using System;

public class GameClient{
    /* fields and properties */

    protected Board board{get;init;}

    public IO display{get;init;}

    public int move_count{get => board.move_count;}

    public Board.InputCallback.Type status{get;protected set;}

    /* constructors and destructors */

    public GameClient(Board.BoardInitMode mode,PositionReader pos,IO display){
        if(mode==Board.BoardInitMode.DefaultPosition) this.board=new Board(mode);
        else this.board=new Board(mode,pos.Read());
        this.display=display;
    }

    /* methods */

    protected Board.InputCallback Move(int x1,int y1,int x2,int y2){
        return board.MakeMove(move_count%2==1 ? Piece.Color.Black : Piece.Color.White,x1,y1,x2,y2); 
    }

    //protected 

    protected string WhoseTurn{get => move_count%2==1 ? "Black" : "White";}

    public void Play(){
        while(true){
            if(status!=Board.InputCallback.Type.Checkmate) display.PrintMessage($"{WhoseTurn} to move");
            display.DisplayBoard(board);

            CommandParser.ParseResult command;
            try{
                command=CommandParser.Parse(display.GetInput());
            }
            catch(Exception e){
                display.PrintMessage(e.Message);
                continue;
            }
            
            switch(command.result){
                case CommandParser.ParseResult.ParseType.Move:
                    string s;
                    if(status!=Board.InputCallback.Type.Checkmate){
                        HandleMoveCallback(Move(command.data[0],command.data[1],command.data[2],command.data[3]));
                    }
                    break;

                case CommandParser.ParseResult.ParseType.DrawRequest:
                    s=display.HandleYesNoEvent($"{WhoseTurn} requests draw. Do you accept?");
                    break;

                case CommandParser.ParseResult.ParseType.GiveUpRequest:
                    display.HandleYesNoEvent($"{WhoseTurn} wants to give up. Do you accept?");
                    break;                

                case CommandParser.ParseResult.ParseType.UndoRequest:
                    display.HandleYesNoEvent($"{WhoseTurn} wants to undo the last move. Do you accept?");
                    break;

                case CommandParser.ParseResult.ParseType.ForceEnd:
                    Environment.Exit(0);
                    break;

                default:
                    break;
            }
        }
    }

    protected void HandleMoveCallback(Board.InputCallback clb){
        switch(clb.result){
            case Board.InputCallback.Type.Error:
                display.PrintMessage("Error: "+clb.message+"\n");
                break;

            case Board.InputCallback.Type.Promote:
                display.DisplayBoard(board);
                HandlePromoteEvent(clb.message!);
                break;

            case Board.InputCallback.Type.CheckWhite:
            case Board.InputCallback.Type.CheckBlack:
                display.PrintMessage("Check!");
                break;
            
            case Board.InputCallback.Type.Checkmate:
                status=Board.InputCallback.Type.Checkmate;
                display.PrintMessage($"Checkmate! {WhoseTurn} loses.");
                break;
        }
    }

    protected void HandlePromoteEvent(string data){
        string? piece_type=null;
        while(piece_type==null){
            display.PrintMessage("Type the name or a single letter symbol of a piece you want to promote "+
                                 $"a pawn at {(char)(data[0]+('a'-'0'))}{(char)(data[2]+1)} to");

            try{
                piece_type=display.HandlePromoteEvent();
            }
            catch(Exception e){
                display.PrintMessage(e.Message);
            }
        }
        
        board.AddPiecePromote(piece_type,data[0]-'0',data[2]-'0');
    }

    protected void HandleYesNoEvent(string message){
        display.PrintMessage(message);

    }
}