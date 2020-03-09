using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class WaterCreature : Creature {

    public static WaterCreature Create(Vector2 position, int birthDay) {
        var creature = Creature.Create<WaterCreature>(position, birthDay);

        creature.InitProperties();

        return creature;
    }

    protected void InitProperties() {
        this.movement = new WaterMovement(this);

        this.gameObject.GetComponent<Renderer>().materials[0].color = new Color(
            0f,
            this.speed.Map(this.speedLimit.x, this.speedLimit.y, 0.0f, 0.15f),
            this.speed.Map(this.speedLimit.x, this.speedLimit.y, 0.1f, 0.4f));
    }

}
