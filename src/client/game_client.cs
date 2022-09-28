using System;

public class GameClient{
    /* fields and properties */

    protected Board board{get;init;}

    public IO display{get;init;}

    public int move_count{get=>board.move_count;}

    /* constructors and destructors */

    public GameClient(Board.BoardInitMode mode,PositionReader pos,IO display){
        this.board=new Board(mode,pos.GetPosition());
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
            display.PrintMessage($"{WhoseTurn} to move");
            display.DisplayBoard(board);
            int[] t;
            try{
                t=CommandParser.Parse(display.GetInput());
            }
            catch(Exception e){
                display.PrintMessage(e.Message);
                continue;
            }
            switch(t[0]){
                case 0:
                    string s;
                    var clb=Move(t[1],t[2],t[3],t[4]);
                    switch(clb.result){
                        case Board.InputCallback.Type.Error:
                            display.PrintMessage("Error: "+clb.message+"\n");
                            break;
                        case Board.InputCallback.Type.Promote:
                            
                            display.DisplayBoard(board);
                            try{
                                CommandParser.Parse(s=display.HandlePromoteEvent());
                            }
                            catch(Exception e){
                                display.PrintMessage(e.Message);
                            }
                            break;

                    }
                    break;
                case 1:
                    s=display.HandleYesNoEvent($"{WhoseTurn} requests draw. Do you accept?");
                    break;
                case 2:
                    display.HandleYesNoEvent($"{WhoseTurn} requests draw. Do you accept?");
                    break;
                    
                default:
                    break;
            }
        }
    }
}