using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Evo.EditorStyle
{
#if UNITY_EDITOR
    [CustomEditor(typeof(EvolutionGroup))]
    public class EvolutionGroupEditor : Editor
    {
        bool foldout = false;
        bool dangerFoldout = false;
        int addAgentCount = 0;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "agents", "m_Script");

            EvolutionGroup evolutionGroup = target as EvolutionGroup;

            GUILayout.Space(5);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("agents"));

            GUILayout.Space(5);
            ButtonAddAgents(evolutionGroup);
            AgentAssignmentButtons(evolutionGroup);
            MutationRateButton(evolutionGroup);

            GUILayout.Space(5);
            DestructiveTools(evolutionGroup);

            GUILayout.Space(5);
            DebugInfo(evolutionGroup);

            GUILayout.Space(5);
            FitnessProgressBar(evolutionGroup);

            serializedObject.ApplyModifiedProperties();
        }

        private void FitnessProgressBar(EvolutionGroup evolutionGroup)
        {
            Rect r = EditorGUILayout.BeginVertical();
            EditorGUI.ProgressBar(r, evolutionGroup.GetBestFitness(), "Fitness");
            GUILayout.Space(18);
            EditorGUILayout.EndVertical();
        }

        private void MutationRateButton(EvolutionGroup evolutionGroup)
        {
            GUIContent calculateMutationRateContent = new GUIContent("Calculate Mutation Rate", "Calculates the mutation rate based on the number of assigned agents.\n(1/AgentCount)");
            if (GUILayout.Button(calculateMutationRateContent))
                evolutionGroup.AssignMutationRateToCalculatedRate();
        }

        private void DebugInfo(EvolutionGroup evolutionGroup)
        {
            foldout = EditorGUILayout.Foldout(foldout, new GUIContent("Debug", "Useful Info"), EditorStyles.foldoutHeader);
            if (foldout)
            {
                EditorGUILayout.HelpBox($"Generation: {evolutionGroup.GetGeneration()}\n" +
                    $"Best Fitness {evolutionGroup.GetBestFitness()}", MessageType.None);
            }
        }

        private void DestructiveTools(EvolutionGroup evolutionGroup)
        {
            dangerFoldout = EditorGUILayout.Foldout(dangerFoldout, new GUIContent("Destructive Tools", "WARNING: the following tools may be destructive to the project"), EditorStyles.foldoutHeader);
            if (dangerFoldout)
            {
                EditorGUILayout.HelpBox("WARNING: These tools can break the project, use at own risk", MessageType.Warning);
                if (GUILayout.Button(new GUIContent("Destroy Agents", "WARNING: This will destroy all child agents of this object!")))
                {
                    EvolutionAgent[] agents = evolutionGroup.GetComponentsInChildren<EvolutionAgent>();
                    foreach (EvolutionAgent agent in agents)
                        DestroyImmediate(agent.gameObject);
                }
            }
        }

        private void ButtonAddAgents(EvolutionGroup evolutionGroup)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent("Add Agents", "Add agents based on the given prefab")))
            {
                evolutionGroup.InstantiateNewAgents(addAgentCount);
            }

            addAgentCount = EditorGUILayout.IntField(addAgentCount, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(50));

            GUILayout.EndHorizontal();
        }

        private void AgentAssignmentButtons(EvolutionGroup evolutionGroup)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent("Assign Agents", "Loads all agents from the children")))
                evolutionGroup.LoadAgents();
            if (GUILayout.Button(new GUIContent("Clear Agents", "Clears the agents array")))
                evolutionGroup.ClearAgents();

            GUILayout.EndHorizontal();
        }
    }
#endif
}