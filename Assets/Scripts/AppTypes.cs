using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace FirstResponse
{
    public enum ScreenId
    {
        Start,
        HowItWorks,
        ScenarioSelect,
        SafetyNotice,
        Setup,
        Simulation,
        Paused,
        Completion,
        History
    }

    public enum ScenarioId
    {
        Forearm,
        Thigh
    }

    public enum PlacementState
    {
        Scanning,
        Aiming,
        Previewing,
        Confirmed
    }

    public enum PlacementAction
    {
        Confirm,
        Move
    }

    public readonly struct PlacementChanged
    {
        public readonly PlacementState State;

        public PlacementChanged(PlacementState state)
        {
            State = state;
        }
    }

    public readonly struct PlacementRequested
    {
        public readonly PlacementAction Action;

        public PlacementRequested(PlacementAction action)
        {
            Action = action;
        }
    }

    public readonly struct ScreenRequested
    {
        public readonly ScreenId Screen;

        public ScreenRequested(ScreenId screen)
        {
            Screen = screen;
        }
    }

    public readonly struct ScenarioChosen
    {
        public readonly ScenarioId Scenario;

        public ScenarioChosen(ScenarioId scenario)
        {
            Scenario = scenario;
        }
    }

    public class PlacementFlow
    {
        public PlacementState State { get; private set; } = PlacementState.Scanning;

        public bool PlanesFound(int count)
        {
            return State == PlacementState.Scanning && count > 0 && Change(PlacementState.Aiming);
        }

        public bool Tapped(bool hitFloor)
        {
            bool allowed = hitFloor && (State == PlacementState.Aiming || State == PlacementState.Previewing);
            return allowed && Change(PlacementState.Previewing);
        }

        public bool Confirm()
        {
            return State == PlacementState.Previewing && Change(PlacementState.Confirmed);
        }

        public bool Move()
        {
            return State == PlacementState.Confirmed && Change(PlacementState.Previewing);
        }

        private bool Change(PlacementState next)
        {
            State = next;
            return true;
        }
    }

    public static class ScreenTap
    {
        public static bool TryGet(out Vector2 point)
        {
            point = default;
            Pointer pointer = Pointer.current;
            if (pointer == null || !pointer.press.wasPressedThisFrame)
            {
                return false;
            }

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return false;
            }

            point = pointer.position.ReadValue();
            return true;
        }

        public static bool IsHeld()
        {
            Pointer pointer = Pointer.current;
            if (pointer == null || !pointer.press.isPressed)
            {
                return false;
            }

            return EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject();
        }
    }
}
