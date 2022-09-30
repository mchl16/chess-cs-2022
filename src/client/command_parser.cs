using System;

public static class CommandParser{
    

    public struct ParseResult{
        public enum ParseType{
            Move,
            Promote,
            Draw,
            GiveUp,

        };

        ParseType result;
        int[] data;
    }

    public static int[] Parse(string s){
        string[] tokens=s.Split();
        if(tokens.Length==0) return new int[]{-1}; //do nothing if no command is 

        int[] res;
        switch(tokens[0]){
            case "#": //ignore comments/annotations etc. - useful for unit testing
                return new int[]{-1};

            case "move":
                if(tokens.Length!=3) throw new ArgumentException("Provided an incorrect number of arguments");
                int[] t1,t2;
                try{
                    t1=ParseField(tokens[1]);
                    t2=ParseField(tokens[2]);
                }
                catch (ArgumentException e){
                    throw e;
                }
                res=new int[5];
                res[1]=t1[0];
                res[2]=t1[1];
                res[3]=t2[0];
                res[4]=t2[1];

                break;

            case "request":
                if(tokens.Length!=2) throw new ArgumentException("Provided an incorrect number of arguments");
                switch(tokens[1]){
                    case "giveup":
                    case "draw":
                    default:
                        throw new ArgumentException("Provided an incorrect argument");
                }
                

            case "end!" or "exit!":
                Environment.Exit(0);    
                return new int[1];

            default:
                throw new ArgumentException("Bad command or file name");
        }
        
        return res;

    }

    private static int[] ParseField(string s){
        if(s.Length!=2) throw new ArgumentException("Incorrect parameter");
        if(s[0]<'a'||s[0]>'h') throw new ArgumentException("Incorrect parameter");
        if(s[1]<'1'||s[1]>'8') throw new ArgumentException("Incorrect parameter");
        return new int[]{s[0]-'a',s[1]-'1'};
    }
}