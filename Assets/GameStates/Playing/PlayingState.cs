using UnityEngine;
using TMPro;
using System.Collections;


public class PlayingState : MonoBehaviour
{
    [Header("Settings")]
    public RoundSettings roundSettingsAsset;
    private RoundSettings roundSettings;

    [Header("Round Management")]
    public TMP_Text RoundText;
    private int currentRound = 0;
    private float betweenRoundTimer = 0f;

    [Header("Round Music")]
    public AudioSource musicSource;
    public AudioClip roundSong1;
    public AudioClip roundSong2;

    private AudioClip currentRoundSong;

    [Header("Testing")]
    public bool inRoun;
    public float roundTimer = 0f;

    private void Awake()
    {
        roundSettings = Instantiate(roundSettingsAsset);
        ResetGame();
    }

    private void Start()
    {
        StartNextRound();
    }

    private void Update()
    {
        Testing();
        roundSettings.CurrentRound = currentRound;
        roundSettingsAsset.CurrentRound = currentRound;

        RoundText.text = $"Round: {currentRound}";

        if (roundSettingsAsset.highscore < roundSettingsAsset.score)
            roundSettingsAsset.highscore = roundSettingsAsset.score;

        if (roundSettingsAsset.MostRoundsLasted < roundSettingsAsset.CurrentRound)
            roundSettingsAsset.MostRoundsLasted = roundSettingsAsset.CurrentRound;

        if (roundSettings.state == RoundSettings.RoundState.InRound)
        {
            HandleRoundLogic();
            UpdateRoundMusic();
        }
        else if (roundSettings.state == RoundSettings.RoundState.BetweenRounds)
        {
            HandleBetweenRoundLogic();

            if (musicSource != null && musicSource.isPlaying)
                musicSource.Stop();
        }
    }

    private void HandleRoundLogic()
    {
        roundTimer += Time.deltaTime;

        if (roundTimer >= roundSettings.roundDuration)
            EndRound();
    }

    private void HandleBetweenRoundLogic()
    {
        betweenRoundTimer += Time.deltaTime;

        if (betweenRoundTimer >= roundSettings.betweenRoundDuration)
            StartNextRound();
    }

    public void StartNextRound()
    {
        currentRound++;

        roundSettings.state = RoundSettings.RoundState.InRound;
        roundSettingsAsset.state = RoundSettings.RoundState.InRound;

        roundTimer = 0f;
        betweenRoundTimer = 0f;

        PlayRandomRoundSong();
    }

    public void EndRound()
    {
        roundSettings.state = RoundSettings.RoundState.BetweenRounds;
        roundSettingsAsset.state = RoundSettings.RoundState.BetweenRounds;

        betweenRoundTimer = 0f;

        roundSettingsAsset.score += roundSettings.scoreAddedPerRound;
        roundSettingsAsset.money += roundSettings.moneyAddedPerRound;

        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();
    }

    private void PlayRandomRoundSong()
    {
        if (musicSource == null || roundSong1 == null || roundSong2 == null)
            return;

        currentRoundSong = Random.value > 0.5f ? roundSong1 : roundSong2;
        musicSource.clip = currentRoundSong;
        musicSource.Play();
    }

    private void UpdateRoundMusic()
    {
        if (musicSource == null)
            return;

        if (!musicSource.isPlaying)
        {
            currentRoundSong = currentRoundSong == roundSong1 ? roundSong2 : roundSong1;
            musicSource.clip = currentRoundSong;
            musicSource.Play();
        }
    }

    public void AddScore(float points)
    {
        roundSettingsAsset.score += points;
    }

    public void AddMoney(int amount)
    {
        roundSettingsAsset.money += amount;
    }

    public void ResetGame()
    {
        roundSettings.state = RoundSettings.RoundState.BetweenRounds;
        roundSettingsAsset.state = RoundSettings.RoundState.BetweenRounds;

        roundSettingsAsset.score = 0f;
        roundSettingsAsset.money = 0;

        currentRound = 0;
        roundTimer = 0f;
        betweenRoundTimer = 0f;

        if (musicSource != null)
            musicSource.Stop();
    }

    public void Testing()
    {
        if (roundSettings.state == RoundSettings.RoundState.InRound)
        {
            inRoun = true;
        }
        else if (roundSettings.state == RoundSettings.RoundState.BetweenRounds)
        {
            inRoun = false;
        }
    }
}
