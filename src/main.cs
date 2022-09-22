using System;

class kaku{
    public static void Main(){
        var gc=new GameClient(Board.BoardInitMode.DefaultPosition,new Display());
        gc.Play();
    }
}