using System;
using System.IO;

public class PositionReader{
    protected IO display;

    public enum Mode{
        File
    };

    public PositionReader(IO display){
        this.display=display;
    }

    public virtual string Read(){
        display.PrintMessage("Type the path to the save file:");
        string s=display.GetInput();
        try{
            return File.ReadAllText(s);
        }
        catch(Exception e){
            throw e;
        }
    }
}