## Modify loop scroll rect from: https://github.com/qiankanglai/LoopScrollRect

## Information
- Modify Easy Object Pool use prefab instead of resource name. Now instead of write down your prefab name, just drag and drop then done.
- Remove SendMessage method, use Action C# instead, for easy modify and handle data. Simple register this listener on from **LoopScrollRect**:

> OnInstantiateNextItem(GameObject obj, int index) 

Then do what ever you want on data. This method trigger every time new item of scroll rect spawned. Open *Samples/Scripts/LSR_GridManager* for more information, and how to use

- Also check folder prefab, i create 4 prefabs horizontal, vertical list, grid, so you want to create a scrollview, simple drag to your canvas.

## How to?
- Open folder Samples, then every things you need on this.
