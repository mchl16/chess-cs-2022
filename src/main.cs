using System;

class kaku{
    public static void Main(string[] args){
        if(args.Length>0) Console.WriteLine(args[0]);
        var io=new IO();
        var gc=new GameClient(Board.BoardInitMode.DefaultPosition,new PositionReader(io),io);
        gc.Play();
    }
}