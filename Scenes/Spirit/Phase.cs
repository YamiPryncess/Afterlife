using Godot;
public class Phase : Area {
    public bool isPhasing() {
        Godot.Collections.Array bodies = GetOverlappingBodies();
        bool result = false;
        for(int i = 0; i < bodies.Count; i++) {
            if(bodies[i] is PhysicsBody body && body.GetCollisionLayerBit(0) == true) {
                result = true;
            }
        }
        return result;
    }
}