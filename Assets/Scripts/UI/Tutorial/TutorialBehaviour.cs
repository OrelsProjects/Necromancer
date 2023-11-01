using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TutorialState
{
    Idle,
    WaitForResponse,
    Done,
}

public enum TutorialType
{
    Onboarding,
}

public struct TutorialData : ISaveableObject
{
    public bool IsTutorialComplete;
    public TutorialType Type;

    public readonly string GetObjectType() =>
        GetType().FullName + "." + nameof(Type);

}

abstract public class TutorialBehaviour : MonoBehaviour, ISaveable
{
    internal List<TutorialStep> _steps;
    internal List<TutorialStep> _stepsNotDone;
    internal TutorialStep _currentStep;
    internal TutorialStep CurrentStep;
    internal TutorialType _type;
    internal TutorialState _state = TutorialState.Idle;

    public ISaveableObject GetData()
    {
        return new TutorialData()
        {
            IsTutorialComplete = _stepsNotDone != null && _stepsNotDone.Count == 0,
        };
    }

    public void LoadData(ISaveableObject item)
    {
        if (item is not TutorialData data)
        {
            return;
        }
        if (data.IsTutorialComplete)
        {
            _state = TutorialState.Done;
        }
    }

    internal virtual void Awake()
    {
        _steps = new List<TutorialStep>(GetComponentsInChildren<TutorialStep>());
        _steps = _steps.OrderBy(step => step.Index).ToList();
        _steps.ForEach(step => step.gameObject.SetActive(false));
    }

    public void Start()
    {
        _stepsNotDone = _steps.FindAll(step => !step.IsComplete);

        if (_stepsNotDone != null && _stepsNotDone.Count > 0)
        {
            CurrentStep = _stepsNotDone[0];
            CurrentStep.gameObject.SetActive(true);
        }

        SetType();
    }

    internal virtual void Update()
    {
        switch (_state)
        {
            case TutorialState.Idle:
                if (CurrentStep != null)
                {
                    CurrentStep.InitiateStep();
                    _state = TutorialState.WaitForResponse;
                }
                else
                {
                    _state = TutorialState.Done;
                }
                break;
            case TutorialState.WaitForResponse:
                if (CurrentStep.IsComplete)
                {
                    UpdateCurrentStep();
                    _state = TutorialState.Idle;
                }
                break;
            case TutorialState.Done:
                // SaveManager.Instance.InitiateSave();
                TutorialManager.Instance.ClearAllObjects();
                Destroy(this);
                break;
            default:
                break;
        }
    }

    private void UpdateCurrentStep()
    {
        if (CurrentStep.IsComplete)
        {
            TutorialManager.Instance.ClearAllObjects();
            CurrentStep.gameObject.SetActive(false);
            _stepsNotDone.Remove(CurrentStep);
            if (_stepsNotDone.Count > 0)
            {
                CurrentStep = _stepsNotDone[0];
                CurrentStep.gameObject.SetActive(true);
            }
            else
            {
                CurrentStep = null;
            }
        }
    }

    abstract internal void SetType();
}