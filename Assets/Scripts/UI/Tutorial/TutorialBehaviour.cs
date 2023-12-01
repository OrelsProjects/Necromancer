using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TutorialState
{
    Idle,
    WaitForResponse,
    Done,
}


// For saving tutorial progress
public enum TutorialType
{
    Onboarding,
}

public struct TutorialData : ISaveableObject
{
    public bool IsTutorialComplete;
    public TutorialType Type;
    public int Part; // Each tutorial can have multiple parts, e.g. onboarding has 2 parts, 1 for raiding 2 for the raid itself.

    public readonly string GetName() =>
           GetObjectType() + "." + nameof(Type) + "." + Part;

    public readonly string GetObjectType() => GetType().FullName;

}

abstract public class TutorialBehaviour : MonoBehaviour, ISaveable
{
    internal List<TutorialStep> _steps;
    internal List<TutorialStep> _stepsNotDone;
    internal TutorialStep _currentStep;
    internal TutorialStep CurrentStep;
    internal TutorialState _state = TutorialState.Idle;

    [SerializeField]
    internal TutorialType _type;
    [SerializeField]
    internal int Part;

    public ISaveableObject GetData()
    {
        return new TutorialData()
        {
            IsTutorialComplete = _stepsNotDone != null && _stepsNotDone.Count == 0,
            Type = _type,
            Part = Part,
        };
    }

    public void LoadData(ISaveableObject item)
    {
        if (item is not TutorialData data)
        {
            return;
        }
        if (data.Type == _type && data.Part == Part)
        {
            if (data.IsTutorialComplete)
            {
                TutorialManager.Instance.ClearAllObjects();
                Destroy(this);
            }
            else
            {
                _state = TutorialState.Idle;
            }
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
        SaveManager.Instance.LoadItem(this);
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
                FinishTutorial();
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
                FinishTutorial();
            }
        }
    }

    private void FinishTutorial()
    {
        SaveManager.Instance.SaveItem(GetData());
        TutorialManager.Instance.ClearAllObjects();
        Destroy(this);
    }

    public string GetObjectName() => new TutorialData()
    {
        Type = _type,
        Part = Part,
    }.GetName();
}