using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace CtrlClickCancel
{
    [StaticConstructorOnStartup]
    public static class CtrlClickCancel
    {
        // Store the current designator to restore it after cancellation
        public static Designator CurrentDesignator = null;
        private static bool isInCancelMode = false;
        
        static CtrlClickCancel()
        {
            var harmony = new Harmony("Stephen.CtrlClickCancel");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Log.Message("CtrlClickCancel initialized");
        }

        // Helper method to check if Ctrl key is pressed
        public static bool IsCtrlPressed()
        {
            return UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.RightControl);
        }
        
        // Helper method to check if we're in a valid context for the mod to operate
        public static bool IsValidContext()
        {
            // Only operate in map view, not in world view
            if (WorldRendererUtility.WorldRenderedNow || Find.CurrentMap == null)
            {
                return false;
            }
            
            return true;
        }
        
        // Centralized method to cancel designations at a cell
        public static void CancelDesignationsAt(IntVec3 cell)
        {
            Map map = Find.CurrentMap;
            if (map == null) return;

            // Get all designations at the cell
            List<Designation> designationsAtCell = map.designationManager.AllDesignationsAt(cell).ToList();
            
            // Cancel all designations at the cell
            foreach (Designation designation in designationsAtCell)
            {
                map.designationManager.RemoveDesignation(designation);
            }

            // Also check for blueprint/frame at the cell
            List<Thing> thingsAtCell = map.thingGrid.ThingsListAt(cell).ToList();
            foreach (Thing thing in thingsAtCell)
            {
                // Check if it's a blueprint or frame
                if (thing is Blueprint || thing is Frame)
                {
                    thing.Destroy();
                }
            }
        }
        
        // Method to store the current designator
        public static void StoreCurrentDesignator()
        {
            if (!isInCancelMode && Find.DesignatorManager?.SelectedDesignator != null)
            {
                CurrentDesignator = Find.DesignatorManager.SelectedDesignator;
                isInCancelMode = true;
            }
        }
        
        // Method to restore the previously selected designator
        public static void RestoreDesignator()
        {
            if (isInCancelMode && CurrentDesignator != null)
            {
                // Make sure the designator is still valid
                if (CurrentDesignator.GetType().IsSubclassOf(typeof(Designator)))
                {
                    try
                    {
                        Find.DesignatorManager.Select(CurrentDesignator);
                    }
                    catch (Exception)
                    {
                        // If there's an error, just ignore it
                    }
                }
                
                isInCancelMode = false;
            }
        }
    }

    // Patch to monitor Ctrl key state changes
    [HarmonyPatch(typeof(Root), "Update")]
    public static class Root_Update_Patch
    {
        private static bool wasCtrlPressed = false;
        
        public static void Postfix()
        {
            // Only process in valid contexts (map view, not world view)
            if (!CtrlClickCancel.IsValidContext())
            {
                // If we were in cancel mode but now in an invalid context, reset state
                if (wasCtrlPressed)
                {
                    wasCtrlPressed = false;
                }
                return;
            }
            
            bool isCtrlPressed = CtrlClickCancel.IsCtrlPressed();
            
            // If Ctrl was just pressed, store the current designator
            if (isCtrlPressed && !wasCtrlPressed)
            {
                CtrlClickCancel.StoreCurrentDesignator();
            }
            // If Ctrl was just released, restore the designator
            else if (!isCtrlPressed && wasCtrlPressed)
            {
                CtrlClickCancel.RestoreDesignator();
            }
            
            wasCtrlPressed = isCtrlPressed;
        }
    }

    // Patch for Designator.ProcessInput to handle mouse clicks and drags
    [HarmonyPatch(typeof(Designator), "ProcessInput")]
    public static class Designator_ProcessInput_Patch
    {
        public static bool Prefix(Designator __instance, Event ev)
        {
            // Only handle mouse events when Ctrl is pressed and in valid context
            if (CtrlClickCancel.IsCtrlPressed() && CtrlClickCancel.IsValidContext() && 
                (ev.type == EventType.MouseDown || ev.type == EventType.MouseDrag) && ev.button == 0)
            {
                // Get the cell under the mouse
                IntVec3 cell = UI.MouseCell();
                if (cell.InBounds(Find.CurrentMap))
                {
                    CtrlClickCancel.CancelDesignationsAt(cell);
                    
                    // Consume the event
                    Event.current.Use();
                    return false;
                }
            }
            return true;
        }
    }

    // Patch for Designator.DesignateSingleCell
    [HarmonyPatch(typeof(Designator), "DesignateSingleCell")]
    public static class Designator_DesignateSingleCell_Patch
    {
        public static bool Prefix(Designator __instance, IntVec3 c)
        {
            if (CtrlClickCancel.IsCtrlPressed() && CtrlClickCancel.IsValidContext())
            {
                CtrlClickCancel.CancelDesignationsAt(c);
                return false; // Skip original method
            }
            return true; // Continue with original method
        }
    }

    // Patch for Designator.DesignateMultiCell
    [HarmonyPatch(typeof(Designator), "DesignateMultiCell")]
    public static class Designator_DesignateMultiCell_Patch
    {
        public static bool Prefix(Designator __instance, IEnumerable<IntVec3> cells)
        {
            if (CtrlClickCancel.IsCtrlPressed() && CtrlClickCancel.IsValidContext())
            {
                foreach (IntVec3 cell in cells)
                {
                    CtrlClickCancel.CancelDesignationsAt(cell);
                }
                return false; // Skip original method
            }
            return true; // Continue with original method
        }
    }
    
    // Patch for Designator_Build.DesignateSingleCell to ensure it works with building designators
    [HarmonyPatch(typeof(Designator_Build), "DesignateSingleCell")]
    public static class Designator_Build_DesignateSingleCell_Patch
    {
        public static bool Prefix(Designator_Build __instance, IntVec3 c)
        {
            if (CtrlClickCancel.IsCtrlPressed() && CtrlClickCancel.IsValidContext())
            {
                CtrlClickCancel.CancelDesignationsAt(c);
                return false; // Skip original method
            }
            return true; // Continue with original method
        }
    }
    
    // Patch to handle mouse input directly for dragging
    [HarmonyPatch(typeof(MapInterface), "HandleMapClicks")]
    public static class MapInterface_HandleMapClicks_Patch
    {
        private static IntVec3 lastProcessedCell = IntVec3.Invalid;
        private static int dragTick = 0;
        
        public static bool Prefix()
        {
            // If Ctrl is pressed and mouse button is down (for both clicks and drags)
            if (CtrlClickCancel.IsCtrlPressed() && CtrlClickCancel.IsValidContext() && UnityEngine.Input.GetMouseButton(0))
            {
                // Get the cell under the mouse
                IntVec3 cell = UI.MouseCell();
                
                // Only process if it's a valid cell and different from the last one we processed
                // or if enough time has passed (for continuous dragging over the same cell)
                if (cell.InBounds(Find.CurrentMap) && 
                    (cell != lastProcessedCell || Find.TickManager.TicksGame >= dragTick + 5))
                {
                    CtrlClickCancel.CancelDesignationsAt(cell);
                    
                    // Update the last processed cell and tick
                    lastProcessedCell = cell;
                    dragTick = Find.TickManager.TicksGame;
                    
                    return false; // Skip original method
                }
            }
            else
            {
                // Reset the last processed cell when not dragging
                lastProcessedCell = IntVec3.Invalid;
            }
            
            return true; // Continue with original method
        }
    }
    
    // Patch to prevent selection of map objects when in cancel mode
    [HarmonyPatch(typeof(Selector), "HandleMapClicks")]
    public static class Selector_HandleMapClicks_Patch
    {
        public static bool Prefix()
        {
            // If we're in cancel mode, prevent the selector from handling clicks
            if (CtrlClickCancel.IsCtrlPressed() && CtrlClickCancel.IsValidContext())
            {
                return false; // Skip the original method
            }
            return true; // Continue with original method
        }
    }
    
    // Patch to prevent selection of map objects when dragging in cancel mode
    [HarmonyPatch(typeof(Selector), "Select")]
    public static class Selector_Select_Patch
    {
        public static bool Prefix(object obj)
        {
            // If we're in cancel mode, prevent selection of objects
            if (CtrlClickCancel.IsCtrlPressed() && CtrlClickCancel.IsValidContext())
            {
                return false; // Skip the original method
            }
            return true; // Continue with original method
        }
    }
}
