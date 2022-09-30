using System;

public class IO{
    /* methods */

    public virtual void DisplayBoard(Board board){
        for(int i=7;i>=0;--i){
            for(int j=0;j<8;++j){
                Console.Write(board[j,i].piece_type switch{
                    1 => "P",
                    2 => "K",
                    3 => "R",
                    4 => "N",
                    5 => "B",
                    6 => "Q",
                    -1 => "p",
                    -2 => "k",
                    -3 => "r",
                    -4 => "n",
                    -5 => "b",
                    -6 => "q",
                    _ => "."
                });
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