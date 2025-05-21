using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class RPSGame : MonoBehaviour
{
    public TMP_Text resultText;
    public TMP_Text player1HPText;
    public TMP_Text player2HPText;
    public TMP_Text enemy1HPText;
    public TMP_Text enemy2HPText;

    public Button rockButton, paperButton, scissorsButton;
    public Button nextTurnButton;
    public Button enemy1SelectButton, enemy2SelectButton;

    private enum Choice { Rock, Paper, Scissors }
    private enum CharacterType { Player, Enemy }

    private class Fighter
    {
        public string name;
        public int hp;
        public CharacterType type;
        public Choice lastChoice;
        public TMP_Text hpText;

        public Fighter(string name, int hp, CharacterType type, TMP_Text hpText)
        {
            this.name = name;
            this.hp = hp;
            this.type = type;
            this.hpText = hpText;
        }

        public bool IsAlive => hp > 0;
    }

    private List<Fighter> turnOrder = new List<Fighter>();
    private int currentTurnIndex = 0;
    private Fighter selectedEnemyTarget = null;

    void Start()
    {
        var player1 = new Fighter("Player1", 30, CharacterType.Player, player1HPText);
        var player2 = new Fighter("Player2", 30, CharacterType.Player, player2HPText);
        var enemy1 = new Fighter("Enemy1", 30, CharacterType.Enemy, enemy1HPText);
        var enemy2 = new Fighter("Enemy2", 30, CharacterType.Enemy, enemy2HPText);

        turnOrder.Add(player1);
        turnOrder.Add(player2);
        turnOrder.Add(enemy1);
        turnOrder.Add(enemy2);

        UpdateAllUI();
        EnableButtonsForCurrentTurn();
    }

    public void OnPlayerChoose(int choice)
    {
        Fighter current = turnOrder[currentTurnIndex];
        if (current.type != CharacterType.Player || !current.IsAlive)
            return;

        if (selectedEnemyTarget == null)
        {
            Debug.LogWarning("selectedEnemyTarget is null");
            resultText.text = "Please select a valid enemy target FIRST!";
            return;
        }

        if (!selectedEnemyTarget.IsAlive)
        {
            Debug.LogWarning($"selectedEnemyTarget {selectedEnemyTarget.name} is not alive. HP: {selectedEnemyTarget.hp}");
            resultText.text = "Please select a valid enemy target FIRST!";
            return;
        }

        Debug.Log($"{current.name} attacking {selectedEnemyTarget.name} with {(Choice)choice}");
        current.lastChoice = (Choice)choice;
        DoAttack(current, selectedEnemyTarget);

        selectedEnemyTarget = null;
        UpdateEnemyTargetButtonsVisual();
        EndTurn();
    }

    public void OnNextTurn()
    {
        Fighter current = turnOrder[currentTurnIndex];
        if (current.type != CharacterType.Enemy || !current.IsAlive)
            return;

        resultText.text = "";

        current.lastChoice = (Choice)Random.Range(0, 3);
        DoAttack(current, GetRandomAliveOpponent(CharacterType.Player));
        EndTurn();
    }

    private void DoAttack(Fighter attacker, Fighter target)
    {
        if (target == null)
        {
            resultText.text = $"{attacker.name} has no one to attack!";
            return;
        }

        Choice defenderChoice = (attacker.type == CharacterType.Enemy)
            ? (Choice)Random.Range(0, 3)
            : target.lastChoice;

        string battleLog = $"{attacker.name} ({attacker.lastChoice}) vs {target.name} ({defenderChoice})\n";

        if (attacker.lastChoice == defenderChoice)
        {
            target.hp -= 5;
            battleLog += $"{target.name} took light damage!";
        }
        else if (WinsAgainst(attacker.lastChoice, defenderChoice))
        {
            target.hp -= 10;
            battleLog += $"{target.name} took heavy damage!";
        }
        else
        {
            battleLog += $"{attacker.name}'s attack failed!";
        }

        UpdateAllUI();
        resultText.text = battleLog;
        CheckGameOver();
    }

    private Fighter GetRandomAliveOpponent(CharacterType targetType)
    {
        List<Fighter> options = turnOrder.FindAll(f => f.type == targetType && f.IsAlive);
        if (options.Count == 0) return null;
        return options[Random.Range(0, options.Count)];
    }

    private void EndTurn()
    {
        currentTurnIndex = (currentTurnIndex + 1) % turnOrder.Count;
        EnableButtonsForCurrentTurn();
    }

    private void EnableButtonsForCurrentTurn()
    {
        Fighter current = turnOrder[currentTurnIndex];

        bool isPlayerTurn = current.type == CharacterType.Player && current.IsAlive;
        bool isEnemyTurn = current.type == CharacterType.Enemy && current.IsAlive;

        rockButton.interactable = isPlayerTurn;
        paperButton.interactable = isPlayerTurn;
        scissorsButton.interactable = isPlayerTurn;
        nextTurnButton.interactable = isEnemyTurn;

        enemy1SelectButton.interactable = isPlayerTurn;
        enemy2SelectButton.interactable = isPlayerTurn;
    }

    private void UpdateAllUI()
    {
        foreach (var fighter in turnOrder)
        {
            fighter.hp = Mathf.Max(0, fighter.hp);
            fighter.hpText.text = $"{fighter.name} HP: {fighter.hp}";
        }
    }

    private bool WinsAgainst(Choice a, Choice b)
    {
        return (a == Choice.Rock && b == Choice.Scissors) ||
               (a == Choice.Paper && b == Choice.Rock) ||
               (a == Choice.Scissors && b == Choice.Paper);
    }

    private void CheckGameOver()
    {
        bool allPlayersDead = !turnOrder.Exists(f => f.type == CharacterType.Player && f.IsAlive);
        bool allEnemiesDead = !turnOrder.Exists(f => f.type == CharacterType.Enemy && f.IsAlive);

        if (allPlayersDead)
        {
            resultText.text = "Game Over! All players defeated.";
            DisableAllButtons();
        }
        else if (allEnemiesDead)
        {
            resultText.text = "Victory! All enemies defeated.";
            DisableAllButtons();
        }
    }

    private void DisableAllButtons()
    {
        rockButton.interactable = false;
        paperButton.interactable = false;
        scissorsButton.interactable = false;
        nextTurnButton.interactable = false;
        enemy1SelectButton.interactable = false;
        enemy2SelectButton.interactable = false;
    }

    public void SelectEnemy1()
    {
        selectedEnemyTarget = turnOrder.Find(f => f.name == "Enemy1" && f.IsAlive);
        if (selectedEnemyTarget != null)
        {
            Debug.Log("Enemy1 selected. HP: " + selectedEnemyTarget.hp);
            resultText.text = "Targeting Enemy1";
        }
        else
        {
            Debug.LogWarning("Enemy1 is dead or not found.");
            resultText.text = "Enemy1 is dead!";
        }
        UpdateEnemyTargetButtonsVisual();
    }

    public void SelectEnemy2()
    {
        selectedEnemyTarget = turnOrder.Find(f => f.name == "Enemy2" && f.IsAlive);
        if (selectedEnemyTarget != null)
        {
            Debug.Log("Enemy2 selected. HP: " + selectedEnemyTarget.hp);
            resultText.text = "Targeting Enemy2";
        }
        else
        {
            Debug.LogWarning("Enemy2 is dead or not found.");
            resultText.text = "Enemy2 is dead!";
        }
        UpdateEnemyTargetButtonsVisual();
    }

    private void UpdateEnemyTargetButtonsVisual()
    {
        ColorBlock selectedColor = ColorBlock.defaultColorBlock;
        selectedColor.normalColor = Color.yellow;

        ColorBlock normalColor = ColorBlock.defaultColorBlock;

        enemy1SelectButton.colors = (selectedEnemyTarget != null && selectedEnemyTarget.name == "Enemy1") ? selectedColor : normalColor;
        enemy2SelectButton.colors = (selectedEnemyTarget != null && selectedEnemyTarget.name == "Enemy2") ? selectedColor : normalColor;
    }
}
