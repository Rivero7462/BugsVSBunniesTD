using System.Collections.Generic;
using DefaultNamespace.OnDeathEffects;
using UnityEngine;

public class bulletBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;
    public Vector3 target;
    public float damage;
    public GameObject enemy;
    public float despawnTime = 20;
    public bool DestroyIfEnemyDies;
    public bool SeenEnemy;

    //Spawns + Entangles with enemies when killed (Bubbles foreach enemy etc.)
    public GameObject EntangleWhenKillEnemy;
    
    public int MaxHits = 1;
    private int checkHealth;

    private float destroyAfter;
    private int hits;
    private Vector3 moveDir;
    public List<Debuff> Debuffs;

    private void Update()
    {
        return;
        if (enemy != null && enemy.activeInHierarchy && enemy.GetComponent<DefaultEnemy>() != null)
        {
            SeenEnemy = true;
            target = enemy.transform.position;
            moveDir = (target - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }
        else
        {
            if (DestroyIfEnemyDies || !SeenEnemy)
                Destroy(gameObject);
            else
                transform.position = Vector3.MoveTowards(transform.position, transform.position + moveDir * 10,
                    speed * Time.deltaTime);
        }

        if (Time.timeSinceLevelLoad > destroyAfter)
            //If the bullet has missed and its been destroyAfter seconds we should kill the bullet so we dont accumulate hundreds of missed bullets.
            Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        return;
        if (other.CompareTag("enemies") && other.GetComponent<DefaultEnemy>() != null)
        {
            if (other.TryGetComponent(out IEnemyUnit unit))
                foreach (var debuff in Debuffs)
                    unit.DebuffHandler.ApplyDebuff(debuff);

            if (other.GetComponent<Enemy>().TakeDamage(damage) && EntangleWhenKillEnemy != null)
            {
                var g = Instantiate(EntangleWhenKillEnemy, transform.position, transform.rotation, transform.parent);
                g.transform.localScale *= other.transform.localScale.x;
                other.transform.parent = g.transform;
                other.transform.localPosition = new Vector3(0, -0.25f, 0);
                other.gameObject.layer = g.gameObject.layer;
                other.transform.tag = g.gameObject.tag;
                Destroy(other.GetComponent<DefaultEnemy>());
            }

            hits++;
            if (hits >= MaxHits)
                Destroy(gameObject);
            else if (hits <= 1) Destroy(gameObject, 0.1f);
        }
    }


    // public void Initialize(List<Debuff> debuffs, GameObject target)
    // {
    //     destroyAfter = Time.timeSinceLevelLoad + despawnTime;
    //     Debuffs = debuffs;
    //     enemy = target;
    // }
}