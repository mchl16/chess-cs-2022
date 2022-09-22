using System;

class King : Piece{
    public override PieceType type{get=>PieceType.King;}

    public King(Board board,Color color,int x,int y) : base(board,color,x,y){}

    public override bool CheckMove(int x,int y){
        if(x-this.x<=1 && this.x-x<=1 && y-this.y<=1 && this.y-y<=1){
            return (_my_board.attacked[x,y]&(color==Color.White ? Board.AttackType.White : Board.AttackType.Black))!=0;
        }
        else if(move_count==0 && y==this.y){ //castling 
            
            switch(x-this.x){
                case 2: //king's side castling
                    Console.WriteLine("?");
                    return true;
                case -2: //queen's side castling
                    Console.WriteLine("??");
                    return true;
                default:
                    Console.WriteLine(x-this.x);
                    return false;
            }
        } 
        else return false;
    }

    public override void MoveTo(int x,int y){
        if(x-this.x==2) _my_board[7,y].MoveTo(5,y);  //move rook (unlike in IRL chess, king goes second,
        if(x-this.x==-2) _my_board[0,y].MoveTo(3,y); //it's easier to code and fuck him)
        base.MoveTo(x,y);
    }

    public override bool CheckForChecksOrPins() => false; //kings are incapable of checking or pinning, they're pussies
}