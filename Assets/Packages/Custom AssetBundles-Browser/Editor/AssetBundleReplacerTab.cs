﻿using System;
using System.Collections.Generic;
using System.IO;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace AssetBundleBrowser.Custom
{
    [Serializable]
    public class AssetBundleReplacerTab
    {
        [SerializeField] private DictionaryData data;
        private long _key;
        private long _value;
        private bool _logging;
        private Rect _position;
        private Vector2 _scrollPosition;
        AssetBundleCabReplacerTab _tab;
        public AssetBundleReplacerTab(AssetBundleCabReplacerTab tab) 
        {
            _tab = tab;
        }

        internal void OnEnable(Rect pos)
        {
            data = GetDataFromFile();
            _position = pos;
        }

        internal void OnGUI(Rect pos)
        {
            _position = new Rect(pos.position, pos.size);

            OnGUIEditor();
        }

        private void OnGUIEditor()
        {
            var titleStyle = new GUIStyle(EditorStyles.boldLabel) {alignment = TextAnchor.MiddleCenter, fontSize = 16};
            _logging = GUI.Toggle(new Rect(17, 5, 15, 15), _logging, new GUIContent("", "Enable logging"));
            GUILayout.BeginArea(_position, titleStyle);

            EditorGUILayout.BeginHorizontal();
            DrawGUIField("SDK PathID", ref _key, titleStyle);
            GUILayout.Label("Made by SamSWAT", new GUIStyle(titleStyle){margin = new RectOffset(0,0,15,0)});
            DrawGUIField("EFT PathID", ref _value, titleStyle);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5f);
            if (GUILayout.Button("ADD ENTRY"))
            {
                if (data.sdk.Contains(_key))
                {
                    Debug.LogError("This SDK PathID is already defined");
                    return;
                }

                data.Add(_key, _value);
            }

            if (GUILayout.Button("CLEAR EVERYTHING"))
            {
                data.Clear();
            }

            if (GUILayout.Button("SAVE DATA TO FILE"))
            {
                WriteDataToFile();
            }

            if (GUILayout.Button("GET DATA FROM FILE"))
            {
                data = GetDataFromFile();
            }

            if (GUILayout.Button("SORT AND SAVE"))
            {
                data.SortAndSaveData();
            }

            //==\\ VISUAL DICTIONARY REPRESENTATION //==\\
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            GUILayout.Space(5f);
            GUILayout.BeginHorizontal();
            DrawKeys();
            DrawButtons();
            DrawValues();
            DrawDescriptions();
            GUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawGUIField(string label, ref long field, GUIStyle titleStyle)
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Label(label, titleStyle);
            GUILayout.Space(5f);
            field = EditorGUILayout.LongField(field);
            EditorGUILayout.EndVertical();
        }

        private void DrawKeys()
        {
            GUILayout.BeginVertical(GUILayout.Width(_position.width * 0.25f));
            for (var i = 0; i < data.sdk.Count; i++)
            {
                GUILayout.Space(1f);
                EditorGUI.BeginChangeCheck();
                var key = EditorGUILayout.LongField(data.sdk[i]);
                if (!EditorGUI.EndChangeCheck()) continue;

                if (data.sdk.Contains(key))
                {
                    Debug.LogError($"{key} is already defined in SDK PathIDs");
                    break;
                }

                data.sdk[i] = key;
            }

            GUILayout.EndVertical();
        }

        private void DrawValues()
        {
            GUILayout.BeginVertical(GUILayout.Width(_position.width * 0.25f));
            for (var i = 0; i < data.eft.Count; i++)
            {
                GUILayout.Space(1f);
                EditorGUI.BeginChangeCheck();
                var value = EditorGUILayout.LongField(data.eft[i]);
                if (!EditorGUI.EndChangeCheck()) continue;
                data.eft[i] = value;
            }

            GUILayout.EndVertical();
        }

        private void DrawButtons()
        {
            GUILayout.BeginVertical(GUILayout.MaxWidth(25f));
            for (var i = 0; i < data.sdk.Count; i++)
            {
                if (!GUILayout.Button(new GUIContent("∧", "Move item up"))) continue;
                var newIndex = i - 1;
                if (newIndex < 0) return;
                data.Move(i, newIndex);
            }
            GUILayout.EndVertical();
            
            GUILayout.BeginVertical(GUILayout.MaxWidth(25f));
            for (var i = 0; i < data.sdk.Count; i++)
            {
                if (!GUILayout.Button(new GUIContent("x", "Remove item"))) continue;
                data.RemoveAt(i);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.MaxWidth(25f));
            for (var i = 0; i < data.sdk.Count; i++)
            {
                if (!GUILayout.Button(new GUIContent("∨", "Move item down"))) continue;
                var newIndex = i + 1;
                if (newIndex >= data.sdk.Count) return;
                data.Move(i, newIndex);
            }
            GUILayout.EndVertical();
        }

        private void DrawDescriptions()
        {
            GUILayout.BeginVertical();
            for (var i = 0; i < data.sdk.Count; i++)
            {
                GUILayout.Space(1f);
                EditorGUI.BeginChangeCheck();
                var value = EditorGUILayout.TextField(data.descriptionList[i]);
                if (!EditorGUI.EndChangeCheck()) continue;
                data.descriptionList[i] = value;
            }

            GUILayout.EndVertical();
        }

        private void WriteDataToFile()
        {
            var path = $"{Directory.GetCurrentDirectory()}/Assets/Packages/Custom AssetBundles-Browser/path_data.json";
            using var streamWriter = new StreamWriter(path);
            string json = JsonUtility.ToJson(data, true);
            streamWriter.Write(json);
        }

        private DictionaryData GetDataFromFile()
        {
            var path = $"{Directory.GetCurrentDirectory()}/Assets/Packages/Custom AssetBundles-Browser/path_data.json";
            try
            {
	            using var streamReader = new StreamReader(path);
	            string json = streamReader.ReadToEnd();
	            var dictionaryData = JsonUtility.FromJson<DictionaryData>(json);

	            if (dictionaryData == null || !dictionaryData.SuitableForDict)
	            {
		            throw new Exception("Json is faulty");
	            }

	            return dictionaryData;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Some error occured while reading data at path: {path}, temporary empty file will be created. Exception: {ex}");
                /*using (var streamWriter = new StreamWriter(path))
                {
                    streamWriter.Write("{}");
                }*/

                return new DictionaryData();
            }
        }

        public void ReplacePathIDs(AssetsManager assetsManager, string bundleName, string outputDirectory,
	        BuildAssetBundleOptions options)
        {
            try
            {
                var path = $"{Directory.GetCurrentDirectory()}/{outputDirectory}/{bundleName}";

                BundleFileInstance bundle = assetsManager.LoadBundleFile(path);
                AssetsFileInstance assetsFile = assetsManager.LoadAssetsFileFromBundle(bundle, 0, true);
                IList<AssetFileInfo> assetList = assetsFile.file.AssetInfos;

                bool replaced = TryReplaceFields(assetList, assetsManager, assetsFile);
                if (!replaced)
                {
                    Debug.Log($"skipping {bundle.name} as no IDs were found to replace");
                    assetsManager.UnloadAll();
                    return;
                }

                SaveChangesToBundle(assetsManager, assetsFile, path, bundle);

                AssetBundleBrowserMain.instance.m_CabReplacerTab.ReplaceCabIDs(path);

                assetsManager.UnloadAll();
                CompressBundle(assetsManager, options, path);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private bool TryReplaceFields(IList<AssetFileInfo> assetList, AssetsManager am, AssetsFileInstance assetsFile)
        {
            var counter = 0;
            foreach (AssetFileInfo assetInfo in assetList)
            {
                AssetTypeValueField baseField = am.GetBaseField(assetsFile, assetInfo);
	            ReplacePathId(assetInfo);
                RecursiveReplaceChildPathIds(baseField);
                byte[] newBytes = baseField.WriteToByteArray();
                assetInfo.SetNewData(newBytes);
                counter++;
            }

            return counter > 0;
        }

        private void ReplacePathId(AssetFileInfo assetInfo)
        {
	        if (assetInfo.PathId == 0) return;

	        if (data.Lookup.TryGetValue(assetInfo.PathId, out long eftPathId))
	        {
		        assetInfo.PathId = eftPathId;
	        }
        }

        private void ReplacePathId(AssetTypeValueField field)
        {
	        AssetTypeValue fieldValue = field.Get("m_PathID").Value;
	        if (fieldValue == null) return;
	        
	        long pathId = fieldValue.AsLong;
	        if (pathId == 0) return;
	        
	        if (data.Lookup.TryGetValue(pathId, out long eftPathId))
	        {
		        fieldValue.AsLong = eftPathId;
	        }
	        
	        if (_logging)
		        Debug.Log($"Found matching pathID: {pathId.ToString()} asset {field.TypeName}{field.FieldName}");
        }

        private void RecursiveReplaceChildPathIds(AssetTypeValueField field)
        {
            foreach (AssetTypeValueField child in field.Children)
            {
                if (child.TemplateField.HasValue && !child.TemplateField.IsArray) 
                    continue;
                if (child.TemplateField.IsArray && child.TemplateField.Children[1].ValueType != AssetValueType.None)
                    continue;

                string typeName = child.TypeName;
                if (typeName.StartsWith("PPtr<") && child.Children.Count == 2)
                {
	                ReplacePathId(child);
                }
                else
                {
                    RecursiveReplaceChildPathIds(child);
                }
            }
        }

        private static void SaveChangesToBundle(AssetsManager assetsManager, AssetsFileInstance assetsFile, string path, BundleFileInstance bundle)
        {
	        using (var memoryStream = new MemoryStream())
	        using (var assetWriter = new AssetsFileWriter(memoryStream))
	        {
		        assetsFile.file.Write(assetWriter);
	        }
	        
	        List<AssetBundleDirectoryInfo> directoryInfos = bundle.file.BlockAndDirInfo.DirectoryInfos;
	        directoryInfos[0].SetNewData(assetsFile.file);

            string modPath = path + "_mod";
            using (var bundleWriter = new AssetsFileWriter(modPath))
            {
	            bundle.file.Write(bundleWriter);
            }
            
            // We need to unload this bundle file as the AM holds an open handle to it.
            assetsManager.UnloadBundleFile(path);
            File.Delete(path);
            File.Move(modPath, path);

            // !!!if sharing violation exception will come back, uncomment things above!!!
        }

        private void CompressBundle(AssetsManager am, BuildAssetBundleOptions options, string path)
        {
            BundleFileInstance bundle = am.LoadBundleFile(path);
            switch (options)
            {
                case BuildAssetBundleOptions.None:
                {
                    string modPath = path + "_c";
                    using (var writer = new AssetsFileWriter(modPath))
                    {
                        bundle.file.Pack(writer, AssetBundleCompressionType.LZMA);
                    }
                    am.UnloadAll();
                    File.Delete(path);
                    File.Move(modPath, path);
                    break;
                }
                case BuildAssetBundleOptions.ChunkBasedCompression:
                {
                    string modPath = path + "_c";
                    using (var writer = new AssetsFileWriter(modPath))
                    {
                        bundle.file.Pack(writer, AssetBundleCompressionType.LZ4);
                    }
                    am.UnloadAll();
                    File.Delete(path);
                    File.Move(modPath, path);
                    break;
                }
            }
            am.UnloadAll();
        }
    }

    [Serializable]
    internal class DictionaryData : ISerializationCallbackReceiver 
    {
        public DictionaryData()
        {
            sdk = new List<long>();
            eft = new List<long>();
            descriptionList = new List<string>();
            Lookup = new Dictionary<long, long>();
        }
        
        public List<long> sdk;
        public List<long> eft;
        public List<string> descriptionList;
        public Dictionary<long, long> Lookup;
        
        public bool SuitableForDict => sdk.Count == eft.Count;
        

        public void Add(long key, long value, string description = "")
        {
            sdk.Add(key);
            eft.Add(value);
            descriptionList.Add(description);
            Lookup.Add(key, value);
        }

        public void Clear()
        {
            sdk.Clear();
            eft.Clear();
            descriptionList.Clear();
            Lookup.Clear();
        }

        public void RemoveAt(int i)
        {
            var keyToDelete = sdk[i];
            sdk.RemoveAt(i);
            eft.RemoveAt(i);
            descriptionList.RemoveAt(i);
            Lookup.Remove(keyToDelete);
        }
        
        public void Move(int oldIndex, int newIndex)
        {
            var key = sdk[oldIndex];
            var value = eft[oldIndex];
            var desc = descriptionList[oldIndex];
            
            RemoveAt(oldIndex);

            //if (newIndex > oldIndex) newIndex--; 

            sdk.Insert(newIndex, key);
            eft.Insert(newIndex, value);
            descriptionList.Insert(newIndex, desc);
        }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            Lookup = new Dictionary<long, long>();
            for (int i = 0; i != Math.Min(sdk.Count, eft.Count); i++)
            {
                Lookup.Add(sdk[i], eft[i]);
            }
            FixMissingDescriptions();
        }

        private void FixMissingDescriptions()
        {
            if (sdk.Count == descriptionList.Count) return;
            
            foreach (var _ in sdk)
            {
                descriptionList.Add("");
            }
        }

        public void SortAndSaveData()
        {
            var sortedIndices = GetSortedIndices(descriptionList, true); // true for ascending (alphabetical order)

            SortListsByIndices(sortedIndices);
            WriteDataToFile();
        }

        private List<int> GetSortedIndices(List<string> list, bool ascending)
        {
            var sortedIndices = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                sortedIndices.Add(i);
            }

            sortedIndices.Sort((a, b) => ascending ? string.Compare(list[a], list[b]) : string.Compare(list[b], list[a]));
            return sortedIndices;
        }

        private void SortListsByIndices(List<int> sortedIndices)
        {
            var sortedSdk = new List<long>();
            var sortedEft = new List<long>();
            var sortedDescriptions = new List<string>();

            foreach (var index in sortedIndices)
            {
                sortedSdk.Add(sdk[index]);
                sortedEft.Add(eft[index]);
                sortedDescriptions.Add(descriptionList[index]);
            }

            sdk = sortedSdk;
            eft = sortedEft;
            descriptionList = sortedDescriptions;

            Lookup.Clear();
            for (int i = 0; i < sdk.Count; i++)
            {
                Lookup[sdk[i]] = eft[i];
            }
        }

        private void WriteDataToFile()
        {
            var path = $"{Directory.GetCurrentDirectory()}/Assets/Packages/Custom AssetBundles-Browser/path_data.json";
            using (var streamWriter = new StreamWriter(path))
            {
                var json = JsonUtility.ToJson(this, true);
                streamWriter.Write(json);
            }
        }

    }
}
