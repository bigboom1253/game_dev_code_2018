using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public static PlayerStat instance;

    public int character_Lv;
    public int[] needExp;
    public int currentExp;

    public int hp;
    public int currentHP;
    public int mp;
    public int currentMP;

    public int atk;
    public int def;

    public string dmgSound;

    public GameObject prefabs_Floating_text;
    public GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void Hit(int _enemyAtk)
    {
        int dmg;

        if (def >= _enemyAtk)
            dmg = 1;
        else
            dmg = _enemyAtk - def;

        currentHP -= dmg;

        if (currentHP <= 0)
            Debug.Log("체력 0 미만, 게임오버");

        AudioManager.instance.Play(dmgSound);

        Vector3 vector = transform.position;
        vector.y += 1;
        //TODO:fix

        GameObject clone = Instantiate(prefabs_Floating_text, vector, Quaternion.identity, parent.transform);
        clone.GetComponent<FloatingText>().text.text = dmg.ToString();
        clone.GetComponent<FloatingText>().text.color = Color.yellow;
        clone.GetComponent<FloatingText>().text.fontSize = 15;
       

        //StopAllCoroutines();
        StartCoroutine(HitCoroutine());

    }

    IEnumerator HitCoroutine()
    {
        Color color = GetComponent<SpriteRenderer>().color;
        color.a = 0;
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.1f);
        color.a = 1;
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.1f);

        color.a = 0;
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.1f);
        color.a = 1;
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.1f);

        color.a = 0;

    }



    // Update is called once per frame
    void Update()
    {
        if(currentExp >= needExp[character_Lv])
        {
            //임시 렙업스탯
            character_Lv++;
            hp += character_Lv * 2;
            mp += character_Lv + 10;

            currentHP = hp;
            currentMP = mp;
            atk++;
            def++;
        }
    }
}
