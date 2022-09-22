using System;

class Knight : Piece{
    /* fields and properties */

    public override PieceType type{get=>PieceType.Knight;}

    /* constructors and destructors */

    public Knight(Board board,Color color,int x,int y) : base(board,color,x,y){}

    /* methods */

    public override bool CheckMove(int x,int y){
        switch((this.x-x)*(this.y-y)){
            case 2:
            case -2:
                return true;
            default:
                return false;
        }
    }

    public override bool CheckForChecksOrPins(){
        int a=2,b=1;
        for (int i=0;i<8;++i){
            int x=this.x+a;
            int y=this.y+b;
            if(x>=0 && x<8 && y>=0 && y<8){
                if((int)_my_board.GetPieceType(x,y)*(int)color==(int)PieceType.King) return true;
            }
            
            int tmp=a; //this shit generates all 8 knight positions (maybe I'll come up with sth more elegant)
            a=b;
            b=tmp;
            if(i!=3) b=-b;
        }
        return false;
    }
}