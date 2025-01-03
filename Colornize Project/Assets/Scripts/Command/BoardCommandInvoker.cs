using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BoardCommandInvoker {

    Stack<ICommand> commandStack;
    Stack<ICommand> redoStack;

    public BoardCommandInvoker() {
        commandStack = new Stack<ICommand>();
        redoStack = new Stack<ICommand>();
    }

    public void AddCommand(ICommand newCommand) {
        newCommand.Execute();
        commandStack.Push(newCommand);
        redoStack.Clear();
    }

    public void Undo() {
        if (commandStack.Count > 0) {
            ICommand lastCommand = commandStack.Pop();
            lastCommand.Undo();
            redoStack.Push(lastCommand);
        } else {
            Debug.Log("Unable to undo, stack has no command");
        }
    }

    public void Redo() {
        if (redoStack.Count > 0) {
            ICommand redoCommand = redoStack.Pop();
            redoCommand.Execute();

            // Push the redone command back onto the main stack
            commandStack.Push(redoCommand);
        } else {
            Debug.Log("Unable to redo, stack has no command");
        }
    }

    public void ClaerRedoStack() {
        redoStack.Clear();
    }

}