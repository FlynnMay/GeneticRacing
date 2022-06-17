using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace Evo.Tools
{
    public class FilePathWizard : ScriptableWizard
    {
        public static string agentsKey = "EvoFilePath.Agent";
        public static string DNATypeKey = "EvoFilePath.DNAObject";
        public static string DNAValueGeneratorKey = "EvoFilePath.DNAValueGenerator";
        public static string agentFitnessKey = "EvoFilePath.AgentFitness";
        public static string templateKey = "EvoFilePath.template";

        public static string defaultAgentsFilePath = $"Assets/Evo/Agents/";
        public static string defaultTemplateScriptsFilePath = $"Assets/Evo/TemplateScripts/";
        public static string defaultFilePath = $"Assets/";

        static Dictionary<string, string> keyDefaultPairs = new Dictionary<string, string>()
        {
            {agentsKey, defaultAgentsFilePath },
            {templateKey, defaultTemplateScriptsFilePath},
            {DNATypeKey, defaultFilePath},
            {DNAValueGeneratorKey, defaultFilePath},
            {agentFitnessKey, defaultFilePath},
        };

        [Header("Save Paths")]
        [Tooltip("File path for saves agents")]
        public string agentsPath = "";
        [Tooltip("File path for Custom DNA Type Scripts")]
        public string DNATypePath = "";
        [Tooltip("File path for Custom Value Generator Scripts")]
        public string DNAValueGeneratorPath = "";
        [Tooltip("File path for Custom Fitness Scripts")]
        public string customAgentFitnessPath = "";

        [Header("Potentially Destructive")]
        [Tooltip("File path for evo templates. WARNING: Don't move unless you move the templates to the new folder")]
        public string templatePath = "";

        [Header("Options")]
        [Tooltip("Creates path if he doesn't exist")]
        public bool createPaths = false;

        [MenuItem("Evo/File Path Wizard")]
        static void CreateWindow()
        {
            ScriptableWizard.DisplayWizard("File Path Editor", typeof(FilePathWizard), "Confirm");
        }

        void OnEnable()
        {
            SetDefaultPathIfNew(agentsKey, defaultAgentsFilePath);
            SetDefaultPathIfNew(templateKey, defaultTemplateScriptsFilePath);
            SetDefaultPathIfNew(DNATypeKey, defaultFilePath);
            SetDefaultPathIfNew(DNAValueGeneratorKey, defaultFilePath);
            SetDefaultPathIfNew(agentFitnessKey, defaultFilePath);

            agentsPath = PlayerPrefs.GetString(agentsKey);
            templatePath = PlayerPrefs.GetString(templateKey);
            customAgentFitnessPath = PlayerPrefs.GetString(agentFitnessKey);
            DNATypePath = PlayerPrefs.GetString(DNATypeKey);
            DNAValueGeneratorPath = PlayerPrefs.GetString(DNAValueGeneratorKey);
        }

        private static void SetDefaultPathIfNew(string key, string value)
        {
            if (!PlayerPrefs.HasKey(key))
                PlayerPrefs.SetString(key, value);
        }

        void OnWizardUpdate()
        {
            helpString = "Customise file paths";
            isValid = IsValidPath(agentsPath) && IsValidPath(templatePath) && IsValidPath(DNATypePath) && IsValidPath(DNAValueGeneratorPath) && IsValidPath(customAgentFitnessPath);
            errorString = isValid ? "" : "A given file path is not valid!";
        }

        private bool IsValidPath(string path)
        {
            return path.IndexOfAny(Path.GetInvalidPathChars()) == -1;
        }

        void OnWizardCreate()
        {
            PlayerPrefs.SetString(agentsKey, agentsPath);
            PlayerPrefs.SetString(templateKey, templatePath);
            PlayerPrefs.SetString(DNATypeKey, DNATypePath);
            PlayerPrefs.SetString(DNAValueGeneratorKey, DNAValueGeneratorPath);
            PlayerPrefs.SetString(agentFitnessKey, customAgentFitnessPath);

            if (!createPaths)
                return;

            if(!Directory.Exists(agentsPath))
                Directory.CreateDirectory(agentsPath);

            if(!Directory.Exists(DNATypePath))
                Directory.CreateDirectory(DNATypePath);
            
            if(!Directory.Exists(agentsPath))
                Directory.CreateDirectory(agentsPath);

            if(!Directory.Exists(DNAValueGeneratorPath))
                Directory.CreateDirectory(DNAValueGeneratorPath);

            if(!Directory.Exists(customAgentFitnessPath))
                Directory.CreateDirectory(customAgentFitnessPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
        }

        public static string GetPath(string key)
        {
            if (!PlayerPrefs.HasKey(key))
                PlayerPrefs.SetString(key, keyDefaultPairs[key]);

            string path = PlayerPrefs.GetString(key);

            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
    }
}
