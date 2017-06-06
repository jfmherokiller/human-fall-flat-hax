/**
 * Author: Sander Homan
 * Copyright 2012
 **/

using System;
using UnityEngine;

namespace plgm_DebugConsole.EasyConsole.FrontEnd.UnityGUI
{
    internal class ConsoleGUI : MonoBehaviour
    {
        private string command = "";
        private int commandIndex;

        private int commandLastPos;
        private int commandLastSelectPos;
        private Vector2 componentScrollValue;
        private string[] displayComponents;

        private string[] displayMethods;

        private string[] displayObjects;
        private GUIStyle gamelistlabel;
        private Vector2 hierarchyScrollValue;

        private readonly int hierarchyWidth = 150;
        private int historyScrollValue;

        private bool isOpen;
        public int linesVisible = 17;
        private Vector2 methodScrollValue;

        private bool moveCursorToEnd;
        private string partialCommand = "";
        private bool returnPressed;

        public bool showHierarchy = true;

        public GUISkin skin = new DevConsoleSkin().myGUISkin;
        private bool wasCursorVisible;

        private void Start()
        {
            gamelistlabel = new GUIStyle(GUIStyle.none)
            {
                name = "GameObjectListLabel",
                normal = {textColor = new Color(1, 1, 1, 1)}
            };

            displayObjects = Console.Instance.GetGameobjectsAtPath("/");
            displayComponents = Console.Instance.GetComponentsOfGameobject("/");
            displayMethods = Console.Instance.GetMethodsOfComponent("/");

            float height = Screen.height / 2;
            height -= skin.box.padding.top + skin.box.padding.bottom;
            height -= skin.box.margin.top + skin.box.margin.bottom;
            height -= skin.textField.CalcHeight(new GUIContent(""), 10);
            linesVisible = (int) (height / skin.label.CalcHeight(new GUIContent(""), 10)) - 2;

            // set max line width
            float width = Screen.width - 10;
            width -= hierarchyWidth;
            width -= skin.verticalScrollbar.CalcSize(new GUIContent("")).x;
            Console.Instance.maxLineWidth = (int) (width / skin.label.CalcSize(new GUIContent("A")).x);
        }

        private void OnGUI()
        {
            GUI.skin = skin;

            if (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.Return)
                returnPressed = true;
            else
                returnPressed = false;

            var upPressed = false;
            if (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.UpArrow)
            {
                upPressed = true;
                Event.current.Use();
            }

            var downPressed = false;
            if (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.DownArrow)
            {
                downPressed = true;
                Event.current.Use();
            }

            var escPressed = false;
            if (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.Escape)
            {
                escPressed = true;
                Event.current.Use();
            }

            if (isOpen)
            {
                GUI.depth = -100;
                GUILayout.BeginArea(new Rect(5, 5, Screen.width - 10, Screen.height / 2), "box");
                GUILayout.BeginHorizontal();

                GUILayout.BeginVertical();
                var lines = Console.Instance.Lines;
                // display last 10 lines
                for (var i = lines.Count() - Mathf.Min(linesVisible, lines.Count()) - historyScrollValue;
                    i < lines.Count() - historyScrollValue;
                    i++)
                    GUILayout.Label(lines.GetItemAt(i));
                GUILayout.EndVertical();
                if (lines.Count() > linesVisible)
                    historyScrollValue = (int) GUILayout.VerticalScrollbar(historyScrollValue, linesVisible,
                        lines.Count(), 0, GUILayout.ExpandHeight(true));

                if (showHierarchy)
                {
                    GUILayout.BeginVertical(GUILayout.Width(hierarchyWidth), GUILayout.ExpandHeight(true));
                    var firstDot = command.IndexOf('.');
                    if (firstDot == -1 || command.IndexOf('.', firstDot + 1) == -1)
                    {
                        hierarchyScrollValue = GUILayout.BeginScrollView(hierarchyScrollValue, "box");
                        foreach (var go in displayObjects)
                            if (GUILayout.Button(go, gamelistlabel))
                            {
                                if (command.LastIndexOf('/') >= 0)
                                    command = command.Substring(0, command.LastIndexOf('/'));
                                command += "/" + go.Replace(" ", "\\ ") + "/";
                                displayObjects = Console.Instance.GetGameobjectsAtPath(command);
                                displayComponents = Console.Instance.GetComponentsOfGameobject(command);
                                moveCursorToEnd = true;
                            }
                        GUILayout.EndScrollView();

                        componentScrollValue = GUILayout.BeginScrollView(componentScrollValue, "box");
                        foreach (var comp in displayComponents)
                            if (GUILayout.Button(comp, gamelistlabel))
                            {
                                if (firstDot > 0)
                                    command = command.Substring(0, firstDot);
                                if (command.EndsWith("/"))
                                    command = command.Substring(0, command.Length - 1);
                                command += "." + comp + ".";
                                displayObjects = Console.Instance.GetGameobjectsAtPath(command);
                                displayComponents = Console.Instance.GetComponentsOfGameobject(command);
                                displayMethods = Console.Instance.GetMethodsOfComponent(command);
                                moveCursorToEnd = true;
                            }
                        GUILayout.EndScrollView();
                    }
                    else
                    {
                        methodScrollValue = GUILayout.BeginScrollView(methodScrollValue, "box");
                        foreach (var method in displayMethods)
                            if (GUILayout.Button(method, gamelistlabel))
                            {
                                command = command.Substring(0, command.IndexOf('.', firstDot + 1));
                                command += "." + method;
                                moveCursorToEnd = true;
                            }
                        GUILayout.EndScrollView();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                
                var oldCommand = command;
                GUI.SetNextControlName("CommandTextField");
                command = GUILayout.TextField(command);
                if (!string.Equals(command, oldCommand, StringComparison.InvariantCulture))
                {
                    Debug.Log("String Compare Area");
                    displayObjects = Console.Instance.GetGameobjectsAtPath(command);
                    displayComponents = Console.Instance.GetComponentsOfGameobject(command);
                    displayMethods = Console.Instance.GetMethodsOfComponent(command);
                }
                GUILayout.EndHorizontal();
                GUILayout.EndArea();

                if (Event.current.type == EventType.repaint && moveCursorToEnd)
                {
                    Debug.Log("MoveCursorArea");
                    var te = (TextEditor) GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                    te?.MoveTextEnd();
                    moveCursorToEnd = false;
                }

                if (GUI.GetNameOfFocusedControl().Equals("CommandTextField") && returnPressed)
                {
                    Console.Instance.Print("> " + command);
                    Console.Instance.Eval(command);
                    command = "";
                    commandIndex = 0;
                    displayObjects = Console.Instance.GetGameobjectsAtPath(command);
                    displayComponents = Console.Instance.GetComponentsOfGameobject(command);
                }

                if (GUI.GetNameOfFocusedControl().Equals("CommandTextField") && upPressed)
                {
                    if (commandIndex == 0)
                        partialCommand = command;

                    commandIndex++;
                    var commandsCount = Console.Instance.Commands.Count();
                    if (commandsCount > 0)
                    {
                        if (commandIndex > commandsCount) commandIndex--;

                        command = Console.Instance.Commands.GetItemAt(commandsCount - 1 - (commandIndex - 1));

                        moveCursorToEnd = true;
                    }
                }

                if (GUI.GetNameOfFocusedControl().Equals("CommandTextField") && downPressed)
                {
                    commandIndex--;
                    var commandsCount = Console.Instance.Commands.Count();
                    if (commandIndex < 0) commandIndex = 0;

                    if (commandsCount > 0)
                    {
                        if (commandIndex > 0)
                            command = Console.Instance.Commands.GetItemAt(commandsCount - 1 - (commandIndex - 1));
                        else
                            command = partialCommand;

                        moveCursorToEnd = true;
                    }
                }
            }

            if (!isOpen && Event.current.type == EventType.keyUp && Event.current.keyCode == KeyCode.BackQuote)
            {
                isOpen = true;
                Event.current.Use();
                Event.current.type = EventType.used;
                wasCursorVisible = Cursor.visible;
            }

            if (isOpen)
                Cursor.visible = true;

            if (isOpen && escPressed)
            {
                isOpen = false;
                Cursor.visible = wasCursorVisible;
            }

            // refocus the textfield if focus is lost
            if (isOpen && Event.current.type == EventType.layout && GUI.GetNameOfFocusedControl() != "CommandTextField")
                GUI.FocusControl("CommandTextField");
        }
    }
}