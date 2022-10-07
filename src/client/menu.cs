using System;

public class Menu : IMenu{
    public virtual void Show(){
        Console.WriteLine("Shitty OOP(s) Chess\n");
        Console.WriteLine("Type \"help\" for help");
    }

    public virtual void Hide(){} //may be useful for a GUI of some kind, but serves no purpose for the default CLI
}