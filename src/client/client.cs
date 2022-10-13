using System;

public class Client{
    /* fields */

    protected GameClient game_client=null!;

    protected IInputOutput display;

    /* constructors and destructors*/

    public Client(IInputOutput display){
        this.display=display;
    }    

    /* methods */

    public void Run(){
        display.Initialize();

        CommandParser.ParseResult command;
        Board.BoardInitMode mode;
        bool not_launch=true;

        do{
            try{
                command=CommandParser.Parse(display.GetInput());
            }
            catch(Exception e){
                display.PrintMessage(e.Message);
                continue;
            }

            if(command.result==CommandParser.ParseResult.ParseType.NewGame){
                
                try{
                    mode=display.HandleNewGameEvent();
                }
                catch(Exception e){
                    display.PrintMessage(e.Message);
                    continue;
                }

                game_client=new GameClient(mode,new PositionReader(display),display);
                not_launch=false;
            }
            else display.PrintMessage("Not a valid option");

        } while(not_launch);

        game_client.Play();
    }
}