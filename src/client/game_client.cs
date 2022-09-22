using System;

public abstract class GameClient{
    /* fields and properties */

    protected Board board;

    public Display display{get;init;}

    /* constructors and destructors */

    public GameClient(Board.BoardInitMode mode,Display display){
        this.board=new Board(mode);
        this.display=display;
    }

    /* methods */
}