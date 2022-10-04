using System;
using System.Collections.Generic;

public partial class Board{
    protected class BoardCreator{
        public static void InitializeFromFEN(Board board,string FEN){
            string[] tokens=FEN.Split();
            if(tokens.Length!=6) throw new ArgumentException("Error loading from FEN: malformed input file");   
            FENSetPieces(board,tokens[0],FENParseCastling(tokens[2]));
            FENSetMoves(board,tokens[1][0],tokens[4],tokens[5]);
            FENSetEnPassant(board,tokens[3]);
        }

        [Flags]
        private enum CastlingData{
            None=0,
            WhiteKing=1,
            WhiteQueen=2,
            BlackKing=4,
            BlackQueen=8
        };

        private static CastlingData FENParseCastling(string data){
            CastlingData res=CastlingData.None;

            foreach(var i in data){
                switch(i){
                    case 'K':
                        res|=CastlingData.WhiteKing;
                        break;
                    case 'Q':
                        res|=CastlingData.WhiteQueen;
                        break;
                    case 'k':
                        res|=CastlingData.BlackKing;
                        break;
                    case 'q':
                        res|=CastlingData.BlackQueen;
                        break;
                    default:
                        throw new ArgumentException("Error loading from FEN: malformed input file");   
                }
            }
            return res;
        }

        private static readonly char[] PieceLetters={'P','p','K','k','R','r','N','n','B','b','Q','q'};

        public static Piece NewPiece(Board board,Piece.Color color,string name,int x,int y,int move_count=0){
            return (Piece)Activator.CreateInstance(Type.GetType(name)!,new object[]{board,color,x,y,move_count})!;
            //such disgusting reflection sorcery just to make the code look more elegant
        }

        private static void FENSetPieces(Board board,string pos,CastlingData castle_data){
            int x=0,y=7;

            foreach(var i in pos){
                if(i=='/'){
                    --y;
                    x=0;
                    continue;
                }
                else if(i>='1' && i<='8'){
                    x+=(i-'0');
                    continue;
                }    
                else if(!PieceLetters.Contains(i)){
                    throw new ArgumentException("Error loading from FEN: malformed input file");  
                }

                Piece.Color col=i switch{
                    >='a' and <='z' => Piece.Color.Black,
                    _ => Piece.Color.White
                };

                string name=(i switch{
                    'P' or 'p' => "Pawn",
                    'K' or 'k' => "King",
                    'R' or 'r' => "Rook",
                    'N' or 'n' => "Knight",
                    'B' or 'b' => "Bishop",
                    'Q' or 'q' => "Queen",
                    _ => "Piece" //placeholder, throw an exception if it somehow gets here (can't create an abstract class)
                })!;

                bool moved=(name,x,y) switch{
                    ("King",4,0) => (castle_data&(CastlingData.WhiteKing|CastlingData.WhiteQueen))!=0,
                    ("King",4,7) => (castle_data&(CastlingData.BlackKing|CastlingData.BlackQueen))!=0,
                    ("King",_,_) => true,
                    ("Rook",0,0) => (castle_data&CastlingData.WhiteQueen)!=0,
                    ("Rook",7,0) => (castle_data&CastlingData.WhiteKing)!=0,
                    ("Rook",0,7) => (castle_data&CastlingData.BlackQueen)!=0,
                    ("Rook",7,7) => (castle_data&CastlingData.BlackKing)!=0,
                    ("Rook",_,_) => true,
                    (_,_,_) => false
                };

                Piece piece=NewPiece(board,col,name,x,y,moved ? 1 : 0);

                



                
                
                board[x,y].piece=piece;

                ++x;
            }
        }
        
        private static void FENSetMoves(Board board,char turn_data,string halfmove_data,string move_data){
            switch(turn_data){
                case 'w':
                    break;
                case 'b':
                    ++board.move_count;
                    break;
                default: 
                    throw new ArgumentException("Error loading from FEN: malformed input file"); 
            }
            
            int mc=0;
            if(Int32.TryParse(move_data,out mc)) board.move_count+=2*mc;
        }

        private static void FENSetEnPassant(Board board,string data){
            if(data=="-") return;
            if(data.Length==2){

            }
            else throw new ArgumentException("Error loading from FEN: malformed input file");
        }

        
    }
}