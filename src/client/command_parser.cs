using System;

public static class CommandParser{
    

    public struct ParseResult{
        public enum ParseType{
            Empty,
            Move,
            Promote,
            DrawRequest,
            GiveUpRequest,
            UndoRequest,
            NewGame,
            Load,
            Save,
            ForceEnd,
        };

        public ParseType result;
        public int[] data;

        public ParseResult(ParseType res,int[] data=null!){
            this.result=res;
            this.data=data;
        }
    }

    public static ParseResult Parse(string s){
        string[] tokens=s.Split();
        if(tokens.Length==0) return new ParseResult(ParseResult.ParseType.Empty); //do nothing if no command is 

        ParseResult.ParseType res;
        int[] data=null!;

        switch(tokens[0]){
            case "#": //ignore comments/annotations etc. - useful for unit testing
                return new ParseResult(ParseResult.ParseType.Empty);

            case "move":
                if(tokens.Length!=3) throw new ArgumentException("Provided an incorrect number of arguments");
                int[] t1,t2;
                try{
                    t1=ParseField(tokens[1]);
                    t2=ParseField(tokens[2]);
                }
                catch{
                    throw;
                }

                res=ParseResult.ParseType.Move;
                data=new int[4];
                data[0]=t1[0];
                data[1]=t1[1];
                data[2]=t2[0];
                data[3]=t2[1];
                break;

            case "request":
                if(tokens.Length!=2) throw new ArgumentException("Provided an incorrect number of arguments");
                res=(tokens[1] switch{
                    "giveup" => ParseResult.ParseType.GiveUpRequest,
                    "draw" => ParseResult.ParseType.DrawRequest,
                    "undo" => ParseResult.ParseType.UndoRequest,
                    _ => throw new ArgumentException("Provided an incorrect argument")
                });
                break;

            case "end!" or "exit!":
                res=ParseResult.ParseType.ForceEnd;
                break;

            default:
                throw new ArgumentException("Bad command or file name");
        }
        
        return new ParseResult(res,data);

    }

    private static int[] ParseField(string s){
        if(s.Length!=2) throw new ArgumentException("Incorrect parameter");
        if(s[0]<'a'||s[0]>'h') throw new ArgumentException("Incorrect parameter");
        if(s[1]<'1'||s[1]>'8') throw new ArgumentException("Incorrect parameter");
        return new int[]{s[0]-'a',s[1]-'1'};
    }
}