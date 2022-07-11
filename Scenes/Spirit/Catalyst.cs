using Godot;
using System.Collections.Generic;

public class Catalyst : Area {
    public CATALYST state {set; get;} //Only works 1 animation to hit x amount of characters 1 time.
    public List<Affectee> hit = new List<Affectee>();
    public override void _Process(float delta) {
        if(state == CATALYST.ENTER) {
            state = CATALYST.UPDATE;
        }
        if(state == CATALYST.UPDATE && hit.Count > 0) {
            for(int i = 0; i < hit.Count; i++) {
                if(hit[i].node is Spirit spirit && spirit != GetParent()) {
                    if(!hit[i].hurting) {
                        spirit.hurt();
                        hit[i].hurting = true;
                    }
                }
            }
        }
        if(state == CATALYST.EXIT) {
            for(int i = 0; i < hit.Count; i++) {
                if(hit[i].node is Spirit spirit && spirit != GetParent()) {
                    hit[i].hurting = false;
                }
            }
            state = CATALYST.RESERVED;
        }
        if(state == CATALYST.RESERVED && hit.Count > 0) {
            List<Affectee> copy = hit;
            for(int i = 0; i < copy.Count; i++) {
                if(copy[i].node is Spirit spirit && spirit != GetParent()) {
                    if(!IsInstanceValid(spirit) || !OverlapsBody(spirit)) {
                        hit.Remove(copy[i]);
                    }
                }
            }
        }
    }
    public void body_entered(Node node) {
        for(int i = 0; i < hit.Count; i++) {
            if(hit[i].node == node) return;
        }
        // for(int i = 0; i < hit.Count; i++) {
        //     if(hit[i] == node) return;
        // }
        hit.Add(new Affectee(node, false));
    }
}

public class Affectee {
    public Node node;
    public bool hurting;
    public Affectee(Node _node, bool _hurting) {
        node = _node;
        hurting = _hurting;
    }
}

public enum CATALYST {
    RESERVED, UPDATE, ENTER, EXIT
}