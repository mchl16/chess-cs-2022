using System;

class kaku{
    public static void Main(string[] args){
        var io=new IO();
        GameClient gc;
        if(args.Length>0){
            Console.WriteLine(args[0]);
            gc=new GameClient(Board.BoardInitMode.FEN,new PositionReader(io),io);
        }
        else gc=new GameClient(Board.BoardInitMode.DefaultPosition,new PositionReader(io),io);
        gc.Play();
    }
}