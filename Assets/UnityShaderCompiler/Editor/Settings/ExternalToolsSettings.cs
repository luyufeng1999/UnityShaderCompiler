using System;
using UnityEditor;

public class ExternalToolsSettings : IUnityShaderCompilerSettings
    {

        public enum OfflineCompiler
        {
            Adreno,
            Mali
        }
        
        public const string SELECTED_OFFLINE_COMPILER_KEY = "Tools_SelectedOfflineCompiler";
        public const string AOC_PATH_KEYS = "ToolPaths_AOC";
        public const string MALIOC_PATH_KEYS = "ToolPaths_MaliOC";
        public const string REPORT_PARSER_PATH_KEYS = "ToolPaths_ReportParser";
        
        public const string AOC_ARCHITECTURE_KEYS = "Tools_AOC_Architecture";
        public const string AOC_API_KEYS = "Tools_AOC_API";
        public const string MALIOC_ARCHITECTURE_KEYS = "Tools_MaliOC_Architecture";


        public OfflineCompiler _selectedTool;

        public OfflineCompiler selectedTool
        {
            get
            {
                return _selectedTool;
            }
            set
            {
                if (_selectedTool != value)
                {
                    _selectedTool = value;
                    EditorPrefs.SetInt(SELECTED_OFFLINE_COMPILER_KEY, (int)_selectedTool);
                }
            }
        }
        
        public Action<string> OnAOCPathChanged;
        public Action<AOC.Architecture> OnAOCArchitectureChanged;
        public Action<AOC.API> OnAOCAPIChanged;
        
        public Action<string> OnMaliOCPathChanged;
        public Action<MaliOC.Architecture> OnMaliOCArchitectureChanged;
        
        public Action<string> OnReportParserPathChanged;

        private string _aocPath;
        public string aocPath
        {
            get
            {
                return _aocPath;
            }
            set
            {
                if (_aocPath != value)
                {
                    _aocPath = value;
                    EditorPrefs.SetString(AOC_PATH_KEYS, _aocPath);
                    OnAOCPathChanged(_aocPath);
                }
            }
        }
        
        private string _maliocPath;
        public string maliocPath
        {
            get
            {
                return _maliocPath;
            }
            set
            {
                if (_maliocPath != value)
                {
                    _maliocPath = value;
                    EditorPrefs.SetString(MALIOC_PATH_KEYS, _maliocPath);
                    OnMaliOCPathChanged(_maliocPath);
                }
            }
        }
        
        private string _reportParserPath;
        public string reportParserPath
        {
            get
            {
                return _reportParserPath;
            }
            set
            {
                if (_reportParserPath != value)
                {
                    _reportParserPath = value;
                    EditorPrefs.SetString(REPORT_PARSER_PATH_KEYS, _reportParserPath);
                    OnReportParserPathChanged(_reportParserPath);
                }
            }
        }
        

        private AOC.Architecture _aocArch;

        public AOC.Architecture aocArch
        {
            get
            {
                return _aocArch;    
            }
            set
            {
                if (_aocArch != value)
                {
                    _aocArch = value;
                    EditorPrefs.SetInt(AOC_ARCHITECTURE_KEYS, (int)_aocArch);
                    OnAOCArchitectureChanged(_aocArch);
                }
            }
            
        }
        
        private AOC.API _aocAPI;
        public AOC.API aocAPI
        {
            get
            {
                return _aocAPI;    
            }
            set
            {
                if (_aocAPI != value)
                {
                    _aocAPI = value;
                    EditorPrefs.SetInt(AOC_API_KEYS, (int)_aocAPI);
                    OnAOCAPIChanged(_aocAPI);
                }
            }
            
        }
        
        private MaliOC.Architecture _maliocArch;

        public MaliOC.Architecture maliocArch
        {
            get
            {
                return _maliocArch;    
            }
            set
            {
                if (_maliocArch != value)
                {
                    _maliocArch = value;
                    EditorPrefs.SetInt(MALIOC_ARCHITECTURE_KEYS, (int)_maliocArch);
                    OnMaliOCArchitectureChanged(_maliocArch);
                }
            }
        }
        
        public void LoadData()
        {
            _selectedTool = (OfflineCompiler)EditorPrefs.GetInt(SELECTED_OFFLINE_COMPILER_KEY, (int)_selectedTool);
            _aocPath = EditorPrefs.GetString(AOC_PATH_KEYS, "");
            _aocArch = (AOC.Architecture)EditorPrefs.GetInt(AOC_ARCHITECTURE_KEYS, (int)AOC.Architecture.None);
            _aocAPI = (AOC.API)EditorPrefs.GetInt(AOC_API_KEYS, (int)AOC.API.None);
            
            _maliocPath = EditorPrefs.GetString(MALIOC_PATH_KEYS, "");
            _maliocArch = (MaliOC.Architecture)EditorPrefs.GetInt(MALIOC_ARCHITECTURE_KEYS, (int)MaliOC.Architecture.None);
            
            _reportParserPath = EditorPrefs.GetString(REPORT_PARSER_PATH_KEYS, "");
        }
    }