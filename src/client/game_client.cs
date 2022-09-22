using System;

public class GameClient{
    /* fields and properties */

    protected Board board{get;init;}

    public Display display{get;init;}

    public int move_count{get;protected set;}

    public bool check{get;protected set;}

    /* constructors and destructors */

    public GameClient(Board.BoardInitMode mode,Display display){
        this.board=new Board(mode);
        this.display=display;
        this.move_count=0;
        this.check=false;
    }

    /* methods */

    protected void Move(int x1,int y1,int x2,int y2){
        bool f;
        try{
            board.MakeMove(move_count%2==1 ? Piece.Color.Black : Piece.Color.White,x1,y1,x2,y2);
            ++move_count;
        }
        catch(Exception e){
            Console.WriteLine(e);
            return;
        }

        
    }

    public void Play(){
        while(true){
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
                    Move(t[1],t[2],t[3],t[4]);
                    break;
                case 1:
                case 2:
                default:
                    break;
            }
        }
    }
}