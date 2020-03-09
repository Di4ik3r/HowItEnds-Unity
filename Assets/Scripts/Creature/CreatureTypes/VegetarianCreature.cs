using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class VegetarianCreature : GroundCreature {

    public static VegetarianCreature Create(Vector2 position, int birthDay) {
        var creature = Creature.Create<VegetarianCreature>(position, birthDay);

        creature.InitProperties();

        return creature;
    }

    protected void InitProperties() {
        this.type = CreatureType.Vegetarian;

        this.gameObject.GetComponent<Renderer>().materials[0].color = new Color(
            this.speed.Map(this.speedLimit.x, this.speedLimit.y, 0f, 0.15f),
            this.speed.Map(this.speedLimit.x, this.speedLimit.y, 0.1f, 0.4f),
            0f);
    }

}
