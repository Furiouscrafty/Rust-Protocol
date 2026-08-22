using UnityEngine;
using TMPro;
using System.Collections;

[CreateAssetMenu(fileName = "RoundSettings", menuName = "Game/Round Settings")]
public class RoundSettings : ScriptableObject
{
    public enum RoundState { InRound, BetweenRounds }
    public RoundState state = RoundState.BetweenRounds;

    [Header("Score")]
    public float score = 0f;
    public float highscore;

    [Header("Money")]
    public int money = 0;

    [Header("Round Durations")]
    public float roundDuration = 60f;
    public float betweenRoundDuration = 10f;
    public int CurrentRound;
    public int MostRoundsLasted;

    [Header("Score Settings")]
    public float scoreAddedPerRound = 100f;

    [Header("Money Settings")]
    public int moneyAddedPerRound;
}

