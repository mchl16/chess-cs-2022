using System;

class Knight : Piece{
    public override PieceType type{get=>PieceType.Knight;}

    public Knight(Board board,Color color,int x,int y) : base(board,color,x,y){}

    public override bool CheckMove(int x,int y) => true; //nothing can obstruct a knight, what a lucky piece

    public override bool CheckForChecksOrPins(){
        int a=2,b=1;
        bool res=false;
        for (int i=0;i<8;++i){
            int x=this.x+a;
            int y=this.y+b;
            if(x>=0 && x<8 && y>=0 && y<8){
                if(_my_board[x,y].type==PieceType.King&&_my_board[x,y].color!=color) res=true;
            }
            int tmp=a; //this shit generates all 8 knight positions (maybe I'll come up with sth more elegant)
            a=b;
            b=tmp;
            if(i!=3) b=-b;
        }
        return res;
    }
}