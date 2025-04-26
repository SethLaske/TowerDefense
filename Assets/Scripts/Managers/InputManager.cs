using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static InputManager instance = null;

    public Vector2 lastTappedPosition { get; private set; }
    public float gameTimeOfLastTap { get; private set; }

    public float trueTimeOfLastTap { get; private set; }

    public const float TIME_MARGIN_FOR_TAP = .2f;

    private Dictionary<object, Action<Vector2>> onTapReleasedActions = new Dictionary<object, Action<Vector2>>();
    private List<object> tapObjectsToRemove = new List<object>();
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void DoFirstUpdate()
    {

    }

    public void DoUpdate(float argDelta)
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse click
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return; // Stop execution if clicking on UI
            }

            lastTappedPosition = GetGameMousePosition();
            gameTimeOfLastTap = GameManager.instance.gameLogic != null ? GameManager.instance.gameLogic.timeSinceGameStarted : -1;
            trueTimeOfLastTap = Time.time;
            
            ISelectable selectedObject = GetClickedGameObject();
            if (selectedObject != null)
            {
                selectedObject.Select();

                int heldKeysValue = Util.GetSumHeldNumberKeys();
                if (heldKeysValue > 0 && selectedObject is UnitBaseController unitController)
                {
                    if (Input.GetKey(KeyCode.D))
                    {
                        unitController.TakeDamage((int)Mathf.Pow(10, heldKeysValue));
                    }
                }
            }
            else
            {
                UIManager.instance.HideUpgradeRing();
            }

        }

        if (Input.GetMouseButtonUp(0))
        {
            if (Time.time >= trueTimeOfLastTap && Time.time <= trueTimeOfLastTap + TIME_MARGIN_FOR_TAP)
            {
                NotifyTapSubscribers(GetGameMousePosition());
            }
        }
    }

    private Vector2 GetGameMousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private ISelectable GetClickedGameObject()
    {
        Collider2D hitCollider = Physics2D.OverlapPoint(lastTappedPosition); // Check for colliders at the position

        if (hitCollider != null)
        {
            return hitCollider.gameObject.GetComponent<ISelectable>();
        }

        return null;
    }

    public void NotifyTapSubscribers(Vector2 argWorldPosition)
    {
        RemoveObjectsFromTap();
            
        foreach (var tapSubscriberPair in onTapReleasedActions)
        {
            if (tapSubscriberPair.Key != null)
            {
                tapSubscriberPair.Value.Invoke(argWorldPosition);
            }
        }

        RemoveObjectsFromTap();
    }

    public void SubscribeToTap(object argObject, Action<Vector2> argAction)
    {
        if (onTapReleasedActions.ContainsKey(argObject))
        {
            Debug.Log("Action already assigned");
        }

        onTapReleasedActions[argObject] = argAction;
    }

    public void UnsubscribeFromTap(object argObject)
    {
        tapObjectsToRemove.Add(argObject);
    }

    private void RemoveObjectsFromTap()
    {
        if (tapObjectsToRemove.Count <= 0)
        {
            return;
        }

        foreach (object tapObject in tapObjectsToRemove)
        {
            if (onTapReleasedActions.ContainsKey(tapObject))
            {
                onTapReleasedActions.Remove(tapObject);
            }
        }
        
        tapObjectsToRemove.Clear();
    }
}
