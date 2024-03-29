using System;

public class IO : IInputOutput{
    /* methods */

    public void Initialize(){
        Console.WriteLine("Shitty OOP(s) Chess\n");
        Console.WriteLine("Type \"help\" for help");
    }

    public Board.BoardInitMode HandleNewGameEvent(){
        PrintMessage("Type FEN or PGN to load a saved game, leave empty for a new game");
        return GetInput() switch{
            "" => Board.BoardInitMode.DefaultPosition,
            "FEN" => Board.BoardInitMode.FEN,
            "PGN" => Board.BoardInitMode.PGN,
            _ => throw new ArgumentException("Not a valid option")
        };
    }

    public void DisplayBoard(Board board){
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

    public string GetInput(){
        Console.Write("> ");
        return Console.ReadLine() ?? "";
    }

    public void PrintMessage(string s) => Console.WriteLine(s);

    public bool HandleYesNoEvent(string message){
        PrintMessage(message);
        switch(GetInput()){
            case "Y" or "y" or "yes" or "Yes":
                PrintMessage("Request accepted");
                return true;
                
            case "N" or "n" or "no" or "No": 
                PrintMessage("Request not accepted");
                return false;

            default:
                throw new ArgumentException("Not a valid response to a yes/no question");
        }
    }

    public string HandlePromoteEvent(){
        return GetInput() switch{
            "R" or "r" or "rook" => "Rook",
            "N" or "n" or "knight" => "Knight",
            "B" or "b" or "bishop" => "Bishop",
            "Q" or "q" or "queen" => "Queen",
            _ => throw new ArgumentException("Not a valid piece a pawn can be promoted to")
        };
    }
}