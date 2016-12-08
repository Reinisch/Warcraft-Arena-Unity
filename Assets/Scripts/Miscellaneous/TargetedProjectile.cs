using UnityEngine;
using System.Collections;

public class TargetedProjectile : MonoBehaviour
{
    public ArenaManager world;
    public Unit caster;
    public Unit target;
    public Spell spell;
    public float speed;

    public GameObject visibleProjectile;
    public ParticleSystem spellParticles;

    public void Initialize(ArenaManager newWorld, Unit newCaster, Unit newTarget, Spell newSpell, float newSpeed)
    {
        world = newWorld;
        caster = newCaster;
        target = newTarget;
        spell = newSpell;
        speed = newSpeed;
    }

    public void Initialize(ArenaManager newWorld, Unit newCaster, Unit newTarget, Spell newSpell)
    {
        world = newWorld;
        caster = newCaster;
        target = newTarget;
        spell = newSpell;
    }

    void FixedUpdate()
    {
        if (target == null || target.IsDead())
        {
            Dispose();
            Destroy(gameObject, 2f);
            if (visibleProjectile != null)
                visibleProjectile.SetActive(false);
            if (spellParticles != null)
                spellParticles.Stop();
        }
        else
        {
            Vector3 vectorToTarget = target.GetComponent<Collider2D>().bounds.center - transform.position;
            float vectorAngle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(vectorAngle, Vector3.forward), 360);
            transform.position = Vector3.MoveTowards(transform.position, target.GetComponent<Collider2D>().bounds.center, speed * Time.deltaTime);

            if (Vector3.Distance(target.GetComponent<Collider2D>().bounds.center, transform.position) < target.GetComponent<Collider2D>().bounds.size.x / 2)
            {
                world.SpellProjectileHit(caster, target, spell);
                Dispose();
                Destroy(gameObject, 2f);
                if (visibleProjectile != null)
                    visibleProjectile.SetActive(false);
                if (spellParticles != null)
                    spellParticles.Stop();
            }
        }      
    }

    public void Dispose()
    {
        world = null;
        caster = null;
        target = null;
        spell = null;
    }
}