using System.Collections.Generic;
using Godot;

public class Controls {
    
}

//Old Code
// public enum INPUT {
//     JUMP, INTERACT, ATTACK, MAGIC, 
//     DASH, STANCE, FOCUS, EXPRESS, 
//     CROUCH, TARGET, SELECT, START, 
//     HOME, CAPTURE, TOUCH, 
//     MOVEUP, MOVELEFT, MOVEDOWN, MOVERIGHT, 
//     CAMUP, CAMDOWN, CAMLEFT, CAMRIGHT
// }
// public Dictionary<string, INPUT> map {set; get;} = new Dictionary<string, INPUT>();
//public Dictionary<INPUT, float> state {set; get;} = new Dictionary<INPUT, float>();  
/*public void setMap() {
        map.Add("faceA", INPUT.JUMP);
        map.Add("faceB", INPUT.INTERACT);
        map.Add("faceC", INPUT.ATTACK);
        map.Add("faceD", INPUT.MAGIC);

        map.Add("rightA", INPUT.DASH);
        map.Add("rightB", INPUT.FOCUS);
        map.Add("rightC", INPUT.TARGET);
        map.Add("leftA", INPUT.STANCE);
        map.Add("leftB", INPUT.EXPRESS);
        map.Add("leftC", INPUT.CROUCH);
        
        map.Add("menuA", INPUT.START);
        map.Add("menuB", INPUT.SELECT);
        map.Add("menuC", INPUT.HOME);
        map.Add("extraA", INPUT.CAPTURE);
        map.Add("extraB", INPUT.TOUCH);
        
        map.Add("moveUp", INPUT.MOVEUP);
        map.Add("moveDown", INPUT.MOVEDOWN);
        map.Add("moveLeft", INPUT.MOVELEFT);
        map.Add("moveRight", INPUT.MOVERIGHT);
        
        map.Add("camUp", INPUT.CAMUP);
        map.Add("camDown", INPUT.CAMDOWN);
        map.Add("camLeft", INPUT.CAMLEFT);
        map.Add("camRight", INPUT.CAMRIGHT);

        state.Add(INPUT.JUMP, 0);
        state.Add(INPUT.INTERACT, 0);
        state.Add(INPUT.ATTACK, 0);
        state.Add(INPUT.MAGIC, 0);

        state.Add(INPUT.DASH, 0);
        state.Add(INPUT.FOCUS, 0);
        state.Add(INPUT.TARGET, 0);
        state.Add(INPUT.STANCE, 0);
        state.Add(INPUT.EXPRESS, 0);
        state.Add(INPUT.CROUCH, 0);
        
        state.Add(INPUT.START, 0);
        state.Add(INPUT.SELECT, 0);
        state.Add(INPUT.HOME, 0);
        state.Add(INPUT.CAPTURE, 0);
        state.Add(INPUT.TOUCH, 0);
        
        state.Add(INPUT.MOVEUP, 0);
        state.Add(INPUT.MOVEDOWN, 0);
        state.Add(INPUT.MOVELEFT, 0);
        state.Add(INPUT.MOVERIGHT, 0);
        
        state.Add(INPUT.CAMUP, 0);
        state.Add(INPUT.CAMDOWN, 0);
        state.Add(INPUT.CAMLEFT, 0);
        state.Add(INPUT.CAMRIGHT, 0);
    }*/