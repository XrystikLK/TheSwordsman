using System.Collections.Generic;

namespace SomeTest.Maps;

/// <summary>
/// Менеджер сцен для управления стэком игровых сцен
/// </summary>
public class SceneManager
{
    /// <summary>
    /// Стек активных сцен (последняя добавленная - текущая)
    /// </summary>
    public Stack<IScene> scenesStack;

    /// <summary>
    /// Инициализирует новый экземпляр менеджера сцен
    /// </summary>
    public SceneManager()
    {
        scenesStack = new Stack<IScene>();
    }

    /// <summary>
    /// Добавляет новую сцену на вершину стека и загружает ее
    /// </summary>
    /// <param name="scene">Добавляемая сцена</param>
    public void AddScene(IScene scene)
    {
        scene.Load(); 
        scenesStack.Push(scene); 
    }

    /// <summary>
    /// Удаляет текущую активную сцену с вершины стека
    /// </summary>
    public void RemoveScene()
    {
        if (scenesStack.Count > 0)
        {
            scenesStack.Pop();
        }
    }

    /// <summary>
    /// Возвращает текущую активную сцену
    /// </summary>
    /// <returns>Текущая сцена или null если стек пуст</returns>
    public IScene GetCurrentScene()
    {
        return scenesStack.Count > 0 ? scenesStack.Peek() : null;
    }

    /// <summary>
    /// Очищает стек сцен
    /// </summary>
    public void Clear()
    {
        scenesStack.Clear();
    }
}