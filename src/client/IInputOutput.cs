using System;

public interface IInputOutput{
    public void DisplayBoard(Board board);

    public string GetInput();

    public void PrintMessage(string s);

    public bool HandleYesNoEvent(string message);

    public string HandlePromoteEvent();
}