using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class SoundManager
{
    private FMOD.Studio.EventInstance _mainMusicInstance;
    private FMOD.Studio.EventInstance _ambienceInstance;

    public SoundManager()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
        _mainMusicInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Music/music_main");
        _ambienceInstance = FMODUnity.RuntimeManager.CreateInstance("event:/Ambience/ambience");
    }

    private void OnGameStateChanged(GameState newstate)
    {
        switch (newstate)
        {
            case GameState.GameInit:
                break;
            case GameState.GameStart:
                _mainMusicInstance.start();
                _ambienceInstance.start();
                break;
            case GameState.PlayerWin:
                _mainMusicInstance.stop(STOP_MODE.IMMEDIATE);
                _ambienceInstance.stop(STOP_MODE.ALLOWFADEOUT);
                RuntimeManager.PlayOneShot("event:/Music/music_win");
                break;
            case GameState.PlayerLose:
                _mainMusicInstance.stop(STOP_MODE.IMMEDIATE);
                _ambienceInstance.stop(STOP_MODE.ALLOWFADEOUT);
                RuntimeManager.PlayOneShot("event:/Music/music_lose");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newstate), newstate, null);
        }
    }
    
    public static string GetWinnerSfxEventToPlay(UnitType winner)
    {
        var eventToPlay = winner switch
        {
            UnitType.Swordman => "event:/SFX/characters/death_infantry",
            UnitType.Spearman => "event:/SFX/characters/death_infantry",
            UnitType.Horseman => "event:/SFX/characters/death_horseman",
            _ => throw new ArgumentOutOfRangeException()
        };

        return eventToPlay;
    }

    public static string GetMovementSfxEventToPlay(UnitType unitType)
    {
        var eventToPlay = unitType switch
        {
            UnitType.Swordman => "event:/SFX/characters/move_infantry",
            UnitType.Spearman => "event:/SFX/characters/move_infantry",
            UnitType.Horseman => "event:/SFX/characters/move_horseman",
            _ => throw new ArgumentOutOfRangeException()
        };

        return eventToPlay;
    }
}
