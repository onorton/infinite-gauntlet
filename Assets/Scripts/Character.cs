using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Actions;
using Targets;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int MaxHealth;

    public int _currentHealth;

    public string Description;

    public CharacterType CharacterType;

    public int Initiative;

    private Animation _animation;
    private AnimationPlaying _animationPlaying;

    private IAction _action;
    private ITargetSelector _targetSelector;
    public PositionHandler PositionHandler { get; private set; }

    // Won't attack characters who have helped them
    private List<Character> _friendlyCharacters;

    private GameObject _rangeMarker;

    private AudioSource[] _audioSources;

    void Awake()
    {
        var token = transform.Find("Token");
        _animationPlaying = token.GetComponent<AnimationPlaying>();
        _animation = token.GetComponent<Animation>();
        _action = GetComponent<IAction>();
        _targetSelector = GetComponent<ITargetSelector>();
        _currentHealth = MaxHealth;
        PositionHandler = GetComponent<PositionHandler>();
        _audioSources = GameObject.Find("SoundSource").GetComponents<AudioSource>();


        _rangeMarker = transform.Find("RangeMarker").gameObject;
        _rangeMarker.SetActive(false);

        _friendlyCharacters = new List<Character>();
    }

    public bool IsDead()
    {
        return _currentHealth <= 0;
    }

    public bool ExecuteTurn(List<Character> characters)
    {
        var charactersNotBeingHealedby = characters.Where(c => !_friendlyCharacters.Contains(c)).ToList();
        var targets = _targetSelector.SelectTargets(charactersNotBeingHealedby);
        foreach (var t in targets)
        {
            _action.Execute(t);
        }

        bool targetsHit = targets.Count() > 0;
        if (targetsHit)
        {
            _animationPlaying.QueueAnimation();
            _animation.PlayQueued("Attack");
        }
        return targetsHit && _action is AttackAction;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        _audioSources[0].Play();
        _animationPlaying.QueueAnimation();
        _animation.PlayQueued("Shake");
    }

    public void Heal(int amount, Character friend)
    {
        _friendlyCharacters.Add(friend);
        _currentHealth = Mathf.Min(_currentHealth + amount, MaxHealth);
        _audioSources[1].Play();
        _animationPlaying.QueueAnimation();
        _animation.PlayQueued("Heal");
    }

    public bool PlayingAnimation()
    {
        return _animationPlaying.IsAnimationPlaying;
    }

    public CharacterState CharacterState()
    {
        return new CharacterState { CharacterType = CharacterType, Health = _currentHealth };
    }

    void OnMouseEnter()
    {
        _rangeMarker.SetActive(true);
        var stats = $"Health: {_currentHealth}\n Range: {_targetSelector.Range()} Initiative: {Initiative} \n";
        GameObject.Find("UI/Main").GetComponentInChildren<Tooltip>().Select(CharacterType.ToString(), Description, stats);
    }

    void OnMouseExit()
    {
        _rangeMarker.SetActive(false);
        GameObject.Find("UI/Main").GetComponentInChildren<Tooltip>().Deselect();
    }
}
