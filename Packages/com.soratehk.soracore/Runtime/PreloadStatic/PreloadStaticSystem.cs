using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using SoraTehk.Collections;
using SoraTehk.Extensions;
using UnityEngine;

namespace VTBeat.Attributes {
    public static class PreloadStaticSystem {
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.AfterAssembliesLoaded)]
#endif
        // PREF: Move this to loading scene if it took too long, refactor LINQ, source gen
        // TODO: Virtual grouping (siblings)
        private static void HandleAfterAssembliesLoaded() {
#if UNITY_EDITOR
            bool isPlayMode = UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;
#else
            bool isPlayMode = true;
#endif
            PreloadStaticModes currentMode = isPlayMode ? PreloadStaticModes.PLAY_MODE : PreloadStaticModes.EDIT_MODE;
            
            var typesWithAttr = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Select(t => (Type: t, Attr: t.GetCustomAttribute<PreloadStaticAttribute>()))
                .Where(tp => tp.Attr != null && (tp.Attr.Modes & currentMode) != 0)
                .ToList();
            
            // Dependency graph
            var graph = new Dictionary<Type, HashSet<Type>>();
            foreach (var (type, attr) in typesWithAttr) {
                if (!graph.ContainsKey(type)) graph[type] = new HashSet<Type>();
                
                if (attr.After != null) {
                    if (!graph.ContainsKey(attr.After)) graph[attr.After] = new HashSet<Type>();
                    graph[attr.After].Add(type);
                }
                
                if (attr.Before != null) {
                    if (!graph.ContainsKey(attr.Before)) graph[attr.Before] = new HashSet<Type>();
                    graph[type].Add(attr.Before);
                }
            }
            
            // DFS
            var visited = new HashSet<Type>();
            var orderedTypeList = new List<Type>();
            var currentEnumerator = new Dictionary<Type, IEnumerator<Type>>();
            
            foreach (var start in graph.Keys) {
                if (visited.Contains(start)) continue;
                
                var stack = new UniqueStack<Type>();
                stack.Push(start);
                currentEnumerator[start] = graph[start].GetEnumerator();
                
                while (stack.TryPeek(out var node)) {
                    var enumerator = currentEnumerator[node];
                    bool finished = true;
                    
                    // Process neighbor
                    while (enumerator.MoveNext()) {
                        var neighbor = enumerator.Current!;
                        
                        if (stack.Contains(neighbor)) {
                            // Convert stack to list from bottom -> top
                            var stackList = stack.Reverse().ToList();
                            // First occurrence of repeated node
                            var startIndex = stackList.IndexOf(neighbor);
                            // Build the cycle
                            var cycleTypeList = stackList.Skip(startIndex)
                                .Concat(new[] { neighbor })
                                .ToList();
                            
                            throw new Exception($"Cycle detected: {string.Join(" -> ", cycleTypeList.Select(t => t.Name))}");
                        }
                        
                        if (!visited.Contains(neighbor)) {
                            stack.Push(neighbor);
                            currentEnumerator[neighbor] = graph[neighbor].GetEnumerator();
                            finished = false;
                            break;
                        }
                    }
                    
                    if (finished) {
                        stack.Pop();
                        visited.Add(node);
                        orderedTypeList.Add(node);
                        currentEnumerator.Remove(node);
                    }
                }
            }
            
            foreach (var type in orderedTypeList) {
                Debug.Log($"[PreloadStatic] RunClassConstructor: Type={type.GetFriendlyTypeName()}");
                RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }
        }
    }
}