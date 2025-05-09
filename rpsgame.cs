using UnityEngine;
using TMPro;

public class RPSBattleSystem : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI choicesText;
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI enemyHPText;

    private enum Choice { Rock, Paper, Scissors }

    private int playerHP = 30;
    private int enemyHP = 30;

    public void PlayerChoose(int playerChoiceIndex)
    {
        if (playerHP <= 0 || enemyHP <= 0) return;

        Choice playerChoice = (Choice)playerChoiceIndex;
        Choice enemyChoice = (Choice)Random.Range(0, 3);

        string result = GetResult(playerChoice, enemyChoice);

        UpdateUI(playerChoice, enemyChoice, result);
        CheckGameOver();
    }

    private string GetResult(Choice player, Choice enemy)
    {
        if (player == enemy)
        {
            enemyHP -= 5;
            playerHP -= 5;
            return "It's a Tie! Both take some damage.";
        }

        bool playerWins = (player == Choice.Rock && enemy == Choice.Scissors) ||
                          (player == Choice.Paper && enemy == Choice.Rock) ||
                          (player == Choice.Scissors && enemy == Choice.Paper);

        if (playerWins)
        {
            enemyHP -= 10;
            return "You Win the round! Enemy takes damage.";
        }
        else
        {
            playerHP -= 10;
            return "You Lose the round! You take damage.";
        }
    }

    private void UpdateUI(Choice player, Choice enemy, string result)
    {
        choicesText.text = $"You chose {player}, Enemy chose {enemy}";
        resultText.text = result;
        playerHPText.text = $"Player HP: {playerHP}";
        enemyHPText.text = $"Enemy HP: {enemyHP}";
    }

    private void CheckGameOver()
    {
        if (playerHP <= 0)
        {
            resultText.text = "Game Over! You Lost.";
        }
        else if (enemyHP <= 0)
        {
            resultText.text = "Victory! You Defeated the Enemy.";
        }
    }
}