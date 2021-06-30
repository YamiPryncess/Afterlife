using Godot;
using System.Linq;
using System.Collections.Generic;

public class Meeting : Spatial {
    Vector3 target = new Vector3();
    float distance = 1;
    List<Player> players = new List<Player>();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        foreach(var child in GetChildren().OfType<Player>()){
            child.AddToGroup("Players");
            players.Add(child);
        }
    }

    public override void _PhysicsProcess(float delta) {
        target = playerCenter();
        distance = playerDist();
    }

    public Vector3 getTarget() {
        return target;
    }
    public float getDistance() {
        return distance;
    }

    public Vector3 playerCenter() {
        Vector3 accumilator = new Vector3();
        for(int i = 0; i < players.Count; i++) {
            accumilator += players[i]
                .GetNode<Spatial>("Target").GlobalTransform.origin;
        }
        return accumilator / players.Count;
    }
    public float playerDist() {
        float largestDist = 0;
        for(int i = 0; i < players.Count; i++) {
            for(int j = 0; j < players.Count; j++){
                float distance = players[i].GetNode<Spatial>("Target")
                .GlobalTransform.origin.DistanceTo(players[j]
                .GetNode<Spatial>("Target").GlobalTransform.origin);
                if(distance > largestDist) {
                    largestDist = distance;
                }
            }
        }
        return largestDist > 1 ? largestDist : 1;
    }
}