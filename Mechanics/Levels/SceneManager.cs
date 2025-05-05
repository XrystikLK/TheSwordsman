using System.Collections.Generic;

namespace SomeTest.Maps;

public class SceneManager
{
    private Stack<IScene> scenesStack;

    public SceneManager()
    {
        scenesStack = new();
    }

    public void AddScene(IScene scene)
    {
        scene.Load();
        scenesStack.Push(scene);
    }

    public void RemoveScene()
    {
        scenesStack.Pop();
    }

    public IScene GetCurrentScene()
    {
        return scenesStack.Peek();
    }
    
}