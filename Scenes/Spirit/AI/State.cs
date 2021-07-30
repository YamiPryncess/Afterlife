using Godot;
using System.Collections.Generic;
public class State {
    public Spirit target;
    public List<Spirit> concerns;
    public Dictionary<BELIEF, bool> beliefs;
}