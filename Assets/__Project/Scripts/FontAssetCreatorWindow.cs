//using System.Collections;
//using UnityEngine;
//using UnityEditor;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Threading;
//using System.IO;
//using System.Linq;
//using System.Text.RegularExpressions;
//using UnityEngine.TextCore;
//using UnityEngine.TextCore.LowLevel;
//using Object = UnityEngine.Object;
//
//namespace TMPro.EditorUtilities
//{
//    public class FontAssetCreatorWindow : TMPro_FontAssetCreatorWindow
//    {
////        private string _selectedFontPath;
////        private readonly List<Object> _selectedFontSiblings = new List<Object>();
////        private Object _lastFontFile;
////        private bool _multiGenerate;
//        
//        public override void Update()
//        {
//            if (m_IsRepaintNeeded)
//            {
//                //Debug.Log("Repainting...");
//                m_IsRepaintNeeded = false;
//                Repaint();
//            }
//
//            // Update Progress bar is we are Rendering a Font.
//            if (m_IsProcessing)
//            {
//                m_AtlasGenerationProgress = GetFontEngineGenerationProgress();
//
//                m_IsRepaintNeeded = true;
//            }
//
//            // Update Feedback Window & Create Font Texture once Rendering is done.
//            if (m_IsRenderingDone)
//            {
//                m_IsProcessing = false;
//                m_IsRenderingDone = false;
//
//                if (m_IsGenerationCancelled == false)
//                {
//                    m_AtlasGenerationProgressLabel = "Generation completed in: " +
//                                                     (m_GlyphPackingGenerationTime + m_GlyphRenderingGenerationTime)
//                                                     .ToString("0.00 ms.");
//
//                    UpdateRenderFeedbackWindow();
//                    CreateFontAtlasTexture();
//
//                    // If dynamic make readable ...
//                    m_FontAtlasTexture.Apply(false, false);
//                }
//
//                Repaint();
//            }
//        }
//
//
//        protected override void DrawControls()
//        {
//            GUILayout.Space(5f);
//
//            if (position.width > position.height && position.width > k_TwoColumnControlsWidth)
//            {
//                m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition, GUILayout.Width(315));
//            }
//            else
//            {
//                m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
//            }
//
//            GUILayout.Space(5f);
//
//            GUILayout.Label(
//                m_SelectedFontAsset != null
//                    ? $"Font Settings [{m_SelectedFontAsset.name}]"
//                    : "Font Settings", EditorStyles.boldLabel);
//
//            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
//
//            EditorGUIUtility.labelWidth = 125f;
//            EditorGUIUtility.fieldWidth = 5f;
//
//            // Disable Options if already generating a font atlas texture.
//            EditorGUI.BeginDisabledGroup(m_IsProcessing);
//            {
//                // FONT TTF SELECTION
//                EditorGUI.BeginChangeCheck();
//                m_SourceFontFile =
//                    EditorGUILayout.ObjectField("Source Font File", m_SourceFontFile, typeof(Font), false) as Font;
//                if (EditorGUI.EndChangeCheck())
//                {
//                    m_SelectedFontAsset = null;
//                    m_IsFontAtlasInvalid = true;
//
//                    var assetPath = AssetDatabase.GetAssetPath(m_SourceFontFile);
//                    var parent = Directory.GetParent(assetPath).FullName.Replace("\\", "/");
//                    var last = "Assets" + parent.Replace(Application.dataPath, "") + "/";
//
//
////                  tring[] allFonts = Directory.GetFiles(last, "*.otf|*.ttf", SearchOption.TopDirectoryOnly);
//                    var allFonts = Directory.GetFiles(last, "*.*", SearchOption.TopDirectoryOnly)
//                        .Where(x => x.EndsWith("otf") || x.EndsWith("ttf")).ToArray();
//                    _selectedFontSiblings.Clear();
//                    foreach (string fontPath in allFonts)
//                    {
//                        Object font = (Object) AssetDatabase.LoadAssetAtPath(fontPath, typeof(Object));
//                        _selectedFontSiblings.Add(font);
//                    }
//                }
//
//                // FONT SIZING
//                EditorGUI.BeginChangeCheck();
//                if (m_PointSizeSamplingMode == 0)
//                {
//                    m_PointSizeSamplingMode = EditorGUILayout.Popup("Sampling Point Size", m_PointSizeSamplingMode,
//                        m_FontSizingOptions);
//                }
//                else
//                {
//                    GUILayout.BeginHorizontal();
//                    m_PointSizeSamplingMode = EditorGUILayout.Popup("Sampling Point Size", m_PointSizeSamplingMode,
//                        m_FontSizingOptions, GUILayout.Width(225));
//                    m_PointSize = EditorGUILayout.IntField(m_PointSize);
//                    GUILayout.EndHorizontal();
//                }
//
//                if (EditorGUI.EndChangeCheck())
//                {
//                    m_IsFontAtlasInvalid = true;
//                }
//
//                // FONT PADDING
//                EditorGUI.BeginChangeCheck();
//                m_Padding = EditorGUILayout.IntField("Padding", m_Padding);
//                m_Padding = (int) Mathf.Clamp(m_Padding, 0f, 64f);
//                if (EditorGUI.EndChangeCheck())
//                {
//                    m_IsFontAtlasInvalid = true;
//                }
//
//                // FONT PACKING METHOD SELECTION
//                EditorGUI.BeginChangeCheck();
//                m_PackingMode = (FontPackingModes) EditorGUILayout.EnumPopup("Packing Method", m_PackingMode);
//                if (EditorGUI.EndChangeCheck())
//                {
//                    m_IsFontAtlasInvalid = true;
//                }
//
//                // FONT ATLAS RESOLUTION SELECTION
//                GUILayout.BeginHorizontal();
//                GUI.changed = false;
//
//                EditorGUI.BeginChangeCheck();
//                EditorGUILayout.PrefixLabel("Atlas Resolution");
//                m_AtlasWidth = EditorGUILayout.IntPopup(m_AtlasWidth, m_FontResolutionLabels, m_FontAtlasResolutions);
//                m_AtlasHeight = EditorGUILayout.IntPopup(m_AtlasHeight, m_FontResolutionLabels, m_FontAtlasResolutions);
//                if (EditorGUI.EndChangeCheck())
//                {
//                    m_IsFontAtlasInvalid = true;
//                }
//
//                GUILayout.EndHorizontal();
//
//
//                // FONT CHARACTER SET SELECTION
//                EditorGUI.BeginChangeCheck();
//                bool hasSelectionChanged = false;
//                m_CharacterSetSelectionMode =
//                    EditorGUILayout.Popup("Character Set", m_CharacterSetSelectionMode, m_FontCharacterSets);
//                if (EditorGUI.EndChangeCheck())
//                {
//                    m_CharacterSequence = "";
//                    hasSelectionChanged = true;
//                    m_IsFontAtlasInvalid = true;
//                }
//
//                switch (m_CharacterSetSelectionMode)
//                {
//                    case 0: // ASCII
//                        //characterSequence = "32 - 126, 130, 132 - 135, 139, 145 - 151, 153, 155, 161, 166 - 167, 169 - 174, 176, 181 - 183, 186 - 187, 191, 8210 - 8226, 8230, 8240, 8242 - 8244, 8249 - 8250, 8252 - 8254, 8260, 8286";
//                        m_CharacterSequence = "32 - 126, 160, 8203, 8230, 9633";
//                        break;
//
//                    case 1: // EXTENDED ASCII
//                        m_CharacterSequence = "32 - 126, 160 - 255, 8192 - 8303, 8364, 8482, 9633";
//                        // Could add 9632 for missing glyph
//                        break;
//
//                    case 2: // Lowercase
//                        m_CharacterSequence = "32 - 64, 91 - 126, 160";
//                        break;
//
//                    case 3: // Uppercase
//                        m_CharacterSequence = "32 - 96, 123 - 126, 160";
//                        break;
//
//                    case 4: // Numbers & Symbols
//                        m_CharacterSequence = "32 - 64, 91 - 96, 123 - 126, 160";
//                        break;
//
//                    case 5: // Custom Range
//                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
//                        GUILayout.Label(
//                            "Enter a sequence of decimal values to define the characters to be included in the font asset or retrieve one from another font asset.",
//                            TMP_UIStyleManager.label);
//                        GUILayout.Space(10f);
//
//                        EditorGUI.BeginChangeCheck();
//                        m_ReferencedFontAsset = EditorGUILayout.ObjectField("Select Font Asset", m_ReferencedFontAsset,
//                            typeof(TMP_FontAsset), false) as TMP_FontAsset;
//                        if (EditorGUI.EndChangeCheck() || hasSelectionChanged)
//                        {
//                            if (m_ReferencedFontAsset != null)
//                                m_CharacterSequence =
//                                    TMP_EditorUtility.GetDecimalCharacterSequence(
//                                        TMP_FontAsset.GetCharactersArray(m_ReferencedFontAsset));
//
//                            m_IsFontAtlasInvalid = true;
//                        }
//
//                        // Filter out unwanted characters.
//                        char chr = Event.current.character;
//                        if ((chr < '0' || chr > '9') && (chr < ',' || chr > '-'))
//                        {
//                            Event.current.character = '\0';
//                        }
//
//                        GUILayout.Label("Character Sequence (Decimal)", EditorStyles.boldLabel);
//                        EditorGUI.BeginChangeCheck();
//                        m_CharacterSequence = EditorGUILayout.TextArea(m_CharacterSequence,
//                            TMP_UIStyleManager.textAreaBoxWindow, GUILayout.Height(120), GUILayout.ExpandWidth(true));
//                        if (EditorGUI.EndChangeCheck())
//                        {
//                            m_IsFontAtlasInvalid = true;
//                        }
//
//                        EditorGUILayout.EndVertical();
//                        break;
//
//                    case 6: // Unicode HEX Range
//                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
//                        GUILayout.Label(
//                            "Enter a sequence of Unicode (hex) values to define the characters to be included in the font asset or retrieve one from another font asset.",
//                            TMP_UIStyleManager.label);
//                        GUILayout.Space(10f);
//
//                        EditorGUI.BeginChangeCheck();
//                        m_ReferencedFontAsset = EditorGUILayout.ObjectField("Select Font Asset", m_ReferencedFontAsset,
//                            typeof(TMP_FontAsset), false) as TMP_FontAsset;
//                        if (EditorGUI.EndChangeCheck() || hasSelectionChanged)
//                        {
//                            if (m_ReferencedFontAsset != null)
//                                m_CharacterSequence =
//                                    TMP_EditorUtility.GetUnicodeCharacterSequence(
//                                        TMP_FontAsset.GetCharactersArray(m_ReferencedFontAsset));
//
//                            m_IsFontAtlasInvalid = true;
//                        }
//
//                        // Filter out unwanted characters.
//                        chr = Event.current.character;
//                        if ((chr < '0' || chr > '9') && (chr < 'a' || chr > 'f') && (chr < 'A' || chr > 'F') &&
//                            (chr < ',' || chr > '-'))
//                        {
//                            Event.current.character = '\0';
//                        }
//
//                        GUILayout.Label("Character Sequence (Hex)", EditorStyles.boldLabel);
//                        EditorGUI.BeginChangeCheck();
//                        m_CharacterSequence = EditorGUILayout.TextArea(m_CharacterSequence,
//                            TMP_UIStyleManager.textAreaBoxWindow, GUILayout.Height(120), GUILayout.ExpandWidth(true));
//                        if (EditorGUI.EndChangeCheck())
//                        {
//                            m_IsFontAtlasInvalid = true;
//                        }
//
//                        EditorGUILayout.EndVertical();
//                        break;
//
//                    case 7: // Characters from Font Asset
//                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
//                        GUILayout.Label(
//                            "Type the characters to be included in the font asset or retrieve them from another font asset.",
//                            TMP_UIStyleManager.label);
//                        GUILayout.Space(10f);
//
//                        EditorGUI.BeginChangeCheck();
//                        m_ReferencedFontAsset = EditorGUILayout.ObjectField("Select Font Asset", m_ReferencedFontAsset,
//                            typeof(TMP_FontAsset), false) as TMP_FontAsset;
//                        if (EditorGUI.EndChangeCheck() || hasSelectionChanged)
//                        {
//                            if (m_ReferencedFontAsset != null)
//                                m_CharacterSequence = TMP_FontAsset.GetCharacters(m_ReferencedFontAsset);
//
//                            m_IsFontAtlasInvalid = true;
//                        }
//
//                        EditorGUI.indentLevel = 0;
//
//                        GUILayout.Label("Custom Character List", EditorStyles.boldLabel);
//                        EditorGUI.BeginChangeCheck();
//                        m_CharacterSequence = EditorGUILayout.TextArea(m_CharacterSequence,
//                            TMP_UIStyleManager.textAreaBoxWindow, GUILayout.Height(120), GUILayout.ExpandWidth(true));
//                        if (EditorGUI.EndChangeCheck())
//                        {
//                            m_IsFontAtlasInvalid = true;
//                        }
//
//                        EditorGUILayout.EndVertical();
//                        break;
//
//                    case 8: // Character List from File
//                        EditorGUI.BeginChangeCheck();
//                        m_CharactersFromFile =
//                            EditorGUILayout.ObjectField("Character File", m_CharactersFromFile, typeof(TextAsset),
//                                false) as TextAsset;
//                        if (EditorGUI.EndChangeCheck())
//                        {
//                            m_IsFontAtlasInvalid = true;
//                        }
//
//                        if (m_CharactersFromFile != null)
//                        {
//                            Regex rx = new Regex(@"(?<!\\)(?:\\u[0-9a-fA-F]{4}|\\U[0-9a-fA-F]{8})");
//
//                            m_CharacterSequence = rx.Replace(m_CharactersFromFile.text,
//                                match =>
//                                {
//                                    if (match.Value.StartsWith("\\U"))
//                                        return char.ConvertFromUtf32(int.Parse(match.Value.Replace("\\U", ""),
//                                            NumberStyles.HexNumber));
//
//                                    return char.ConvertFromUtf32(int.Parse(match.Value.Replace("\\u", ""),
//                                        NumberStyles.HexNumber));
//                                });
//                        }
//
//                        break;
//                }
//
//                // FONT STYLE SELECTION
//                //GUILayout.BeginHorizontal();
//                //EditorGUI.BeginChangeCheck();
//                ////m_FontStyle = (FaceStyles)EditorGUILayout.EnumPopup("Font Style", m_FontStyle, GUILayout.Width(225));
//                ////m_FontStyleValue = EditorGUILayout.IntField((int)m_FontStyleValue);
//                //if (EditorGUI.EndChangeCheck())
//                //{
//                //    m_IsFontAtlasInvalid = true;
//                //}
//                //GUILayout.EndHorizontal();
//
//                // Render Mode Selection
//                CheckForLegacyGlyphRenderMode();
//
//                EditorGUI.BeginChangeCheck();
//                m_GlyphRenderMode = (GlyphRenderMode) EditorGUILayout.EnumPopup("Render Mode", m_GlyphRenderMode);
//                if (EditorGUI.EndChangeCheck())
//                {
//                    m_IsFontAtlasInvalid = true;
//                }
//
//                m_IncludeFontFeatures = EditorGUILayout.Toggle("Get Kerning Pairs", m_IncludeFontFeatures);
//                _multiGenerate = EditorGUILayout.Toggle("Multi Generate", _multiGenerate);
//
//                EditorGUILayout.Space();
//            }
//
//            EditorGUI.EndDisabledGroup();
//
//            if (!string.IsNullOrEmpty(m_WarningMessage))
//            {
//                EditorGUILayout.HelpBox(m_WarningMessage, MessageType.Warning);
//            }
//
//            #region Generate Font
//
//            GUI.enabled =
//                m_SourceFontFile != null && !m_IsProcessing &&
//                !m_IsGenerationDisabled; // Enable Preview if we are not already rendering a font.
//            if (GUILayout.Button("Generate Font Atlas") && GUI.enabled)
//            {
//                TMP_EditorCoroutine.StartCoroutine(GenerateFonts());
//            }
//
//            #endregion
//
//
//            // FONT RENDERING PROGRESS BAR
//            GUILayout.Space(1);
//            Rect progressRect = EditorGUILayout.GetControlRect(false, 20);
//
//            GUI.enabled = true;
//            progressRect.width -= 22;
//            EditorGUI.ProgressBar(progressRect, Mathf.Max(0.01f, m_AtlasGenerationProgress),
//                m_AtlasGenerationProgressLabel);
//            progressRect.x = progressRect.x + progressRect.width + 2;
//            progressRect.y -= 1;
//            progressRect.width = 20;
//            progressRect.height = 20;
//
//            GUI.enabled = m_IsProcessing;
//            if (GUI.Button(progressRect, "X"))
//            {
//                FontEngineSendCancellationRequest();
//                m_AtlasGenerationProgress = 0;
//                m_IsProcessing = false;
//                m_IsGenerationCancelled = true;
//            }
//
//            GUILayout.Space(5);
//
//            // FONT STATUS & INFORMATION
//            GUI.enabled = true;
//
//            GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(200));
//            m_OutputScrollPosition = EditorGUILayout.BeginScrollView(m_OutputScrollPosition);
//            EditorGUILayout.LabelField(m_OutputFeedback, TMP_UIStyleManager.label);
//            EditorGUILayout.EndScrollView();
//            GUILayout.EndVertical();
//
//            // SAVE TEXTURE & CREATE and SAVE FONT XML FILE
//            GUI.enabled =
//                m_FontAtlasTexture != null && !m_IsProcessing; // Enable Save Button if font_Atlas is not Null.
//
//            EditorGUILayout.BeginHorizontal();
//
//            if (GUILayout.Button("Save") && GUI.enabled)
//            {
//                if (m_SelectedFontAsset == null)
//                {
//                    if (m_LegacyFontAsset != null)
//                        SaveNewFontAssetWithSameName(m_LegacyFontAsset);
//                    else
//                        SaveNewFontAsset(_lastFontFile);
//                }
//                else
//                {
//                    // Save over exiting Font Asset
//                    string filePath = Path.GetFullPath(AssetDatabase.GetAssetPath(m_SelectedFontAsset))
//                        .Replace('\\', '/');
//
//                    if (CheckGlyphResterMode())
//                        Save_Bitmap_FontAsset(filePath);
//                    else
//                        Save_SDF_FontAsset(filePath);
//                }
//            }
//
//            if (GUILayout.Button("Save as...") && GUI.enabled)
//            {
//                if (m_SelectedFontAsset == null)
//                {
//                    SaveNewFontAsset(_lastFontFile);
//                }
//                else
//                {
//                    SaveNewFontAssetWithSameName(m_SelectedFontAsset);
//                }
//            }
//
//            EditorGUILayout.EndHorizontal();
//
//            EditorGUILayout.Space();
//
//            EditorGUILayout.EndVertical();
//
//            GUI.enabled = true; // Re-enable GUI
//
//            if (position.height > position.width || position.width < k_TwoColumnControlsWidth)
//            {
//                DrawPreview();
//                GUILayout.Space(5);
//            }
//
//            EditorGUILayout.EndScrollView();
//
//            if (m_IsFontAtlasInvalid)
//                ClearGeneratedData();
//        }
//
//        protected override IEnumerator GenerateFonts()
//        {
//            int count = 0;
//
//            while (count < _selectedFontSiblings.Count)
//            {
//                #region while
//
//                if (!m_IsProcessing && m_SourceFontFile != null)
//                {
//                    _lastFontFile = _selectedFontSiblings[count];
//                    DestroyImmediate(m_FontAtlasTexture);
//                    m_FontAtlasTexture = null;
//                    m_SavedFontAtlas = null;
//
//                    // Initialize font engine
//                    FontEngineError errorCode = FontEngine.InitializeFontEngine();
//                    if (errorCode != FontEngineError.Success)
//                    {
//                        Debug.Log("Font Asset Creator - Error [" + errorCode +
//                                  "] has occurred while Initializing the FreeType Library.");
//                    }
//
//                    // Get file path of the source font file.
//                    string fontPath = AssetDatabase.GetAssetPath(_lastFontFile);
//
//                    if (errorCode == FontEngineError.Success)
//                    {
//                        errorCode = FontEngine.LoadFontFace(fontPath);
//
//                        if (errorCode != FontEngineError.Success)
//                        {
//                            Debug.Log("Font Asset Creator - Error Code [" + errorCode +
//                                      "] has occurred trying to load the [" + _lastFontFile.name +
//                                      "] font file. This typically results from the use of an incompatible or corrupted font file.");
//                        }
//                    }
//
//
//                    // Define an array containing the characters we will render.
//                    if (errorCode == FontEngineError.Success)
//                    {
//                        uint[] characterSet = null;
//
//                        // Get list of characters that need to be packed and rendered to the atlas texture.
//                        if (m_CharacterSetSelectionMode == 7 || m_CharacterSetSelectionMode == 8)
//                        {
//                            List<uint> char_List = new List<uint>();
//
//                            for (int i = 0; i < m_CharacterSequence.Length; i++)
//                            {
//                                uint unicode = m_CharacterSequence[i];
//
//                                // Handle surrogate pairs
//                                if (i < m_CharacterSequence.Length - 1 && char.IsHighSurrogate((char) unicode) &&
//                                    char.IsLowSurrogate(m_CharacterSequence[i + 1]))
//                                {
//                                    unicode = (uint) char.ConvertToUtf32(m_CharacterSequence[i],
//                                        m_CharacterSequence[i + 1]);
//                                    i += 1;
//                                }
//
//                                // Check to make sure we don't include duplicates
//                                if (char_List.FindIndex(item => item == unicode) == -1)
//                                    char_List.Add(unicode);
//                            }
//
//                            characterSet = char_List.ToArray();
//                        }
//                        else if (m_CharacterSetSelectionMode == 6)
//                        {
//                            characterSet = ParseHexNumberSequence(m_CharacterSequence);
//                        }
//                        else
//                        {
//                            characterSet = ParseNumberSequence(m_CharacterSequence);
//                        }
//
//                        m_CharacterCount = characterSet.Length;
//
//                        m_AtlasGenerationProgress = 0;
//                        m_IsProcessing = true;
//                        m_IsGenerationCancelled = false;
//
//                        GlyphLoadFlags glyphLoadFlags = GetGlyphLoadFlags();
//
//                        // 
//                        AutoResetEvent autoEvent = new AutoResetEvent(false);
//
//                        // Worker thread to pack glyphs in the given texture space.
////                            ThreadPool.QueueUserWorkItem(PackGlyphs =>
////                            {
//                        // Start Stop Watch
//                        m_StopWatch = System.Diagnostics.Stopwatch.StartNew();
//
//                        // Clear the various lists used in the generation process.
//                        m_AvailableGlyphsToAdd.Clear();
//                        m_MissingCharacters.Clear();
//                        m_ExcludedCharacters.Clear();
//                        m_CharacterLookupMap.Clear();
//                        m_GlyphLookupMap.Clear();
//                        m_GlyphsToPack.Clear();
//                        m_GlyphsPacked.Clear();
//
//                        // Check if requested characters are available in the source font file.
//                        for (int i = 0; i < characterSet.Length; i++)
//                        {
//                            uint unicode = characterSet[i];
//                            uint glyphIndex;
//
//                            if (FontEngine.TryGetGlyphIndex(unicode, out glyphIndex))
//                            {
//                                // Skip over potential duplicate characters.
//                                if (m_CharacterLookupMap.ContainsKey(unicode))
//                                    continue;
//
//                                // Add character to character lookup map.
//                                m_CharacterLookupMap.Add(unicode, glyphIndex);
//
//                                // Skip over potential duplicate glyph references.
//                                if (m_GlyphLookupMap.ContainsKey(glyphIndex))
//                                {
//                                    // Add additional glyph reference for this character.
//                                    m_GlyphLookupMap[glyphIndex].Add(unicode);
//                                    continue;
//                                }
//
//                                // Add glyph reference to glyph lookup map.
//                                m_GlyphLookupMap.Add(glyphIndex, new List<uint>() {unicode});
//
//                                // Add glyph index to list of glyphs to add to texture.
//                                m_AvailableGlyphsToAdd.Add(glyphIndex);
//                            }
//                            else
//                            {
//                                // Add Unicode to list of missing characters.
//                                m_MissingCharacters.Add(unicode);
//                            }
//                        }
//
//                        // Pack available glyphs in the provided texture space.
//                        if (m_AvailableGlyphsToAdd.Count > 0)
//                        {
//                            int packingModifier = PackingModifier();
//
//                            if (m_PointSizeSamplingMode == 0) // Auto-Sizing Point Size Mode
//                            {
//                                // Estimate min / max range for auto sizing of point size.
//                                int minPointSize = 0;
//                                int maxPointSize =
//                                    (int) Mathf.Sqrt((m_AtlasWidth * m_AtlasHeight) / m_AvailableGlyphsToAdd.Count) * 3;
//
//                                m_PointSize = (maxPointSize + minPointSize) / 2;
//
//                                bool optimumPointSizeFound = false;
//                                for (int iteration = 0; iteration < 15 && optimumPointSizeFound == false; iteration++)
//                                {
//                                    m_AtlasGenerationProgressLabel = "Packing glyphs - Pass (" + iteration + ")";
//
//                                    FontEngine.SetFaceSize(m_PointSize);
//
//                                    m_GlyphsToPack.Clear();
//                                    m_GlyphsPacked.Clear();
//
//                                    m_FreeGlyphRects.Clear();
//                                    m_FreeGlyphRects.Add(new GlyphRect(0, 0, m_AtlasWidth - packingModifier,
//                                        m_AtlasHeight - packingModifier));
//                                    m_UsedGlyphRects.Clear();
//
//                                    for (int i = 0; i < m_AvailableGlyphsToAdd.Count; i++)
//                                    {
//                                        uint glyphIndex = m_AvailableGlyphsToAdd[i];
//                                        Glyph glyph;
//
//                                        if (FontEngine.TryGetGlyphWithIndexValue(glyphIndex, glyphLoadFlags, out glyph))
//                                        {
//                                            if (glyph.glyphRect.width > 0 && glyph.glyphRect.height > 0)
//                                            {
//                                                m_GlyphsToPack.Add(glyph);
//                                            }
//                                            else
//                                            {
//                                                m_GlyphsPacked.Add(glyph);
//                                            }
//                                        }
//                                    }
//
//                                    FontEngineTryPackGlyphsInAtlas();
//
//                                    if (m_IsGenerationCancelled)
//                                    {
//                                        DestroyImmediate(m_FontAtlasTexture);
//                                        m_FontAtlasTexture = null;
//                                        yield break;
//                                    }
//
//                                    //Debug.Log("Glyphs remaining to add [" + m_GlyphsToAdd.Count + "]. Glyphs added [" + m_GlyphsAdded.Count + "].");
//
//                                    if (m_GlyphsToPack.Count > 0)
//                                    {
//                                        if (m_PointSize > minPointSize)
//                                        {
//                                            maxPointSize = m_PointSize;
//                                            m_PointSize = (m_PointSize + minPointSize) / 2;
//
//                                            //Debug.Log("Decreasing point size from [" + maxPointSize + "] to [" + m_PointSize + "].");
//                                        }
//                                    }
//                                    else
//                                    {
//                                        if (maxPointSize - minPointSize > 1 && m_PointSize < maxPointSize)
//                                        {
//                                            minPointSize = m_PointSize;
//                                            m_PointSize = (m_PointSize + maxPointSize) / 2;
//
//                                            //Debug.Log("Increasing point size from [" + minPointSize + "] to [" + m_PointSize + "].");
//                                        }
//                                        else
//                                        {
//                                            //Debug.Log("[" + iteration + "] iterations to find the optimum point size of : [" + m_PointSize + "].");
//                                            optimumPointSizeFound = true;
//                                        }
//                                    }
//                                }
//                            }
//                            else // Custom Point Size Mode
//                            {
//                                m_AtlasGenerationProgressLabel = "Packing glyphs...";
//
//                                // Set point size
//                                FontEngine.SetFaceSize(m_PointSize);
//
//                                m_GlyphsToPack.Clear();
//                                m_GlyphsPacked.Clear();
//
//                                m_FreeGlyphRects.Clear();
//                                m_FreeGlyphRects.Add(new GlyphRect(0, 0, m_AtlasWidth - packingModifier,
//                                    m_AtlasHeight - packingModifier));
//                                m_UsedGlyphRects.Clear();
//
//                                for (int i = 0; i < m_AvailableGlyphsToAdd.Count; i++)
//                                {
//                                    uint glyphIndex = m_AvailableGlyphsToAdd[i];
//                                    Glyph glyph;
//
//                                    if (FontEngine.TryGetGlyphWithIndexValue(glyphIndex, glyphLoadFlags, out glyph))
//                                    {
//                                        if (glyph.glyphRect.width > 0 && glyph.glyphRect.height > 0)
//                                        {
//                                            m_GlyphsToPack.Add(glyph);
//                                        }
//                                        else
//                                        {
//                                            m_GlyphsPacked.Add(glyph);
//                                        }
//                                    }
//                                }
//
//                                FontEngineTryPackGlyphsInAtlas();
//
//                                if (m_IsGenerationCancelled)
//                                {
//                                    DestroyImmediate(m_FontAtlasTexture);
//                                    m_FontAtlasTexture = null;
//                                    yield break;
//                                }
//
//                                //Debug.Log("Glyphs remaining to add [" + m_GlyphsToAdd.Count + "]. Glyphs added [" + m_GlyphsAdded.Count + "].");
//                            }
//                        }
//                        else
//                        {
//                            int packingModifier = PackingModifier();
//
//                            FontEngine.SetFaceSize(m_PointSize);
//
//                            m_GlyphsToPack.Clear();
//                            m_GlyphsPacked.Clear();
//
//                            m_FreeGlyphRects.Clear();
//                            m_FreeGlyphRects.Add(new GlyphRect(0, 0, m_AtlasWidth - packingModifier,
//                                m_AtlasHeight - packingModifier));
//                            m_UsedGlyphRects.Clear();
//                        }
//
//                        //Stop StopWatch
//                        m_StopWatch.Stop();
//                        m_GlyphPackingGenerationTime = m_StopWatch.Elapsed.TotalMilliseconds;
////                      Debug.Log("Glyph packing completed in: " + m_GlyphPackingGenerationTime.ToString("0.000 ms."));
//                        m_StopWatch.Reset();
//
//                        m_FontCharacterTable.Clear();
//                        m_FontGlyphTable.Clear();
//                        m_GlyphsToRender.Clear();
//
//                        // Add glyphs and characters successfully added to texture to their respective font tables.
//                        foreach (Glyph glyph in m_GlyphsPacked)
//                        {
//                            uint glyphIndex = glyph.index;
//
//                            m_FontGlyphTable.Add(glyph);
//
//                            // Add glyphs to list of glyphs that need to be rendered.
//                            if (glyph.glyphRect.width > 0 && glyph.glyphRect.height > 0)
//                                m_GlyphsToRender.Add(glyph);
//
//                            foreach (uint unicode in m_GlyphLookupMap[glyphIndex])
//                            {
//                                // Create new Character
//                                m_FontCharacterTable.Add(new TMP_Character(unicode, glyph));
//                            }
//                        }
//
//                        // 
//                        foreach (Glyph glyph in m_GlyphsToPack)
//                        {
//                            foreach (uint unicode in m_GlyphLookupMap[glyph.index])
//                            {
//                                m_ExcludedCharacters.Add(unicode);
//                            }
//                        }
//
//                        // Get the face info for the current sampling point size.
//                        m_FaceInfo = FontEngine.GetFaceInfo();
//
//                        autoEvent.Set();
//                        autoEvent.WaitOne();
//
//                        // Start Stop Watch
//                        m_StopWatch = System.Diagnostics.Stopwatch.StartNew();
//
//                        m_IsRenderingDone = false;
//
//                        // Allocate texture data
//                        m_AtlasTextureBuffer = new byte[m_AtlasWidth * m_AtlasHeight];
//
//                        m_AtlasGenerationProgressLabel = "Rendering glyphs...";
//
//                        // Render and add glyphs to the given atlas texture.
//                        if (m_GlyphsToRender.Count > 0)
//                        {
//                            FontEngineRenderGlyphsToTexture();
//                        }
//
//                        m_IsRenderingDone = true;
//
//                        // Stop StopWatch
//                        m_StopWatch.Stop();
//                        m_GlyphRenderingGenerationTime = m_StopWatch.Elapsed.TotalMilliseconds;
//                        //Debug.Log("Font Atlas generation completed in: " + m_GlyphRenderingGenerationTime.ToString("0.000 ms."));
//
//
//                        m_AtlasGenerationProgressLabel = "Generation completed in: " +
//                                                         (m_GlyphPackingGenerationTime + m_GlyphRenderingGenerationTime)
//                                                         .ToString("0.00 ms.");
//
//                        UpdateRenderFeedbackWindow();
//                        CreateFontAtlasTexture();
//
//                        // If dynamic make readable ...
//                        m_FontAtlasTexture.Apply(false, false);
//                        SaveNewFont(_lastFontFile);
//                        m_StopWatch.Reset();
//                    }
//                    
//                    SaveCreationSettingsToEditorPrefs(SaveFontCreationSettings());
//                    count++;
//                }
//
//                #endregion
//
//                yield return new WaitForEndOfFrame();
//            }
//        }
//
//
//        void SaveNewFont(Object sourceObject)
//        {
//            string filePath = "";
//
//            // Save new Font Asset and open save file requester at Source Font File location.
//            string saveDirectory = new FileInfo(AssetDatabase.GetAssetPath(sourceObject)).DirectoryName;
//
//            var siplittedSaveFilePath = saveDirectory.Split('\\');
//
//            for (int i = 0; i < siplittedSaveFilePath.Length - 1; i++)
//            {
//                filePath += i == 0 ? siplittedSaveFilePath[i] : "\\" + siplittedSaveFilePath[i];
//            }
//
//            filePath += "\\" + sourceObject.name + ".asset";
//            filePath = filePath.Replace("\\", "/");
////                Debug.Log(filePath);
////                var aaa = EditorUtility.SaveFilePanel("Save TextMesh Pro! Font Asset File", saveDirectory, sourceObject.name, "asset");
////                Debug.Log(aaa);
//
////                return;
//            if (filePath.Length == 0)
//                return;
//
//            if (CheckGlyphResterMode())
//            {
//                Save_Bitmap_FontAsset(filePath);
//            }
//            else
//            {
//                Save_SDF_FontAsset(filePath);
//            }
//        }
//    }
//}