using Godot;
using System.Collections.Generic;

public class Weapon : Area {
    public bool attackBool {set; get;}
    public List<Node> hit = new List<Node>();
    public override void _Process(float delta) {
        if(!attackBool && hit.Count > 0) {
            hit = new List<Node>();
        }
    }

    public void body_entered(Node node) {
        if(attackBool) {
            for(int i = 0; i < hit.Count; i++) {
                if(hit[i] == node) return;
            }
            hit.Add(node);
            if(node is Spirit spirit && spirit != GetParent()) {
                spirit.hurt();
            }
        }
    }
}
