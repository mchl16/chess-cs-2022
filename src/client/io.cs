using System;

public class IO{
    /* methods */

    public virtual void DisplayBoard(Board board){
        for(int i=7;i>=0;--i){
            for(int j=0;j<8;++j){
                switch(board[j,i].piece_type){
                    case 0:
                        Console.Write(".");
                        break;
                    case 1:
                        Console.Write("P");
                        break;
                    case 2:
                        Console.Write("K");
                        break;
                    case 3:
                        Console.Write("R");
                        break;
                    case 4:
                        Console.Write("N");
                        break;
                    case 5:
                        Console.Write("B");
                        break;
                    case 6:
                        Console.Write("Q");
                        break;
                    case -1:
                        Console.Write("p");
                        break;
                    case -2:
                        Console.Write("k");
                        break;
                    case -3:
                        Console.Write("r");
                        break;
                    case -4:
                        Console.Write("n");
                        break;
                    case -5:
                        Console.Write("b");
                        break;
                    case -6:
                        Console.Write("q");
                        break;
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    public virtual string GetInput() => Console.ReadLine() ?? "";

    public virtual void PrintMessage(string s) => Console.WriteLine(s);

    public virtual string HandlePromoteEvent(){
        PrintMessage("Promote a pawn at to:");
        return GetInput();
    }

    public virtual string HandleYesNoEvent(string message){
        PrintMessage(message);
        return GetInput();
    }
}