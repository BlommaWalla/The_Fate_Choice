using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour {

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    Unit playerUnit;
    Unit enemyUnit;

    public Text dialogueText;

    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public BattleState state;


    public bool block = false;
    public bool isDead = false;
    public int damageMult = 0;
    private int hintInt = 0;
    // Start is called before the first frame update
    void Start() {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle() {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();

        GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGO.GetComponent<Unit>();

        dialogueText.text = "A wild " + enemyUnit.unitName + " approaches...";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack() {
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage += damageMult);

        enemyHUD.SetHP(enemyUnit.currentHP);

        dialogueText.text = "The attack is successful!";

        if (damageMult >= 5) {
            dialogueText.text = "Critical hit!";

        }
        damageMult = 0;

        yield return new WaitForSeconds(2f);

        if (isDead) {
            state = BattleState.WON;
            EndBattle();
        } else {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn() {

        if (!block) {
            dialogueText.text = enemyUnit.unitName + " attacks!";
            yield return new WaitForSeconds(1f);
            isDead = playerUnit.TakeDamage(enemyUnit.damage);
            playerHUD.SetHP(playerUnit.currentHP);
            yield return new WaitForSeconds(1f);

        } else {
            dialogueText.text = "You blocked the attack!";
            block = false;
            yield return new WaitForSeconds(1f);

        }

        if (isDead) {
            state = BattleState.LOST;
            EndBattle();

        } else {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }

    }

    void EndBattle() {
        if (state == BattleState.WON) {
            dialogueText.text = "You won the battle!";

            SceneManager.LoadScene("City");

        } else if (state == BattleState.LOST) {

            dialogueText.text = "You were defeated.";

            SceneManager.LoadScene("City");
        }
    }

    IEnumerator PlayerTurn() {
        dialogueText.text = "Choose an action:";

        return null;
    }

    IEnumerator PlayerHeal() {
        block = true;
        damageMult += 1;
        //playerUnit.Heal(5);

        //playerHUD.SetHP(playerUnit.currentHP);

        yield return new WaitForSeconds(1f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    IEnumerator PlayerHint() {
        hintInt++;

        System.Random random = new System.Random();
        int gandon = random.Next(3);

        bool wallah = random.Next(15) == 1;
        if (wallah) {
            hintInt = 8;
        }

        switch (hintInt) {
            case 1:
                dialogueText.text = "The attack button makes the enemy take damage";

                break;


            case 2:
                dialogueText.text = "The block button blocks an attack";

                break;

            case 3:
                dialogueText.text = "Blocking an attack makes your next attack stronger";

                break;

            case 4:
                dialogueText.text = "Try blocking more than one attack in a row";

                break;

            case 5:
                dialogueText.text = "The hint button shows hints :O wow";

                break;

            case 6:
                dialogueText.text = "Try blocking more than one attack in a row";

                break;

            case 7:
                dialogueText.text = "¯\\_(ツ)_ /¯";
                hintInt = 0;
                break;

            case 8:
                dialogueText.text = "大猿";
                hintInt = 0;
                break;
        }

        yield return new WaitForSeconds(5);
        dialogueText.text = "Choose an action:";

    }




    public void OnAttackButton() {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }

    public void OnHealButton() {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerHeal());
    }

    public void OnHintButton() {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerHint());
    }

}