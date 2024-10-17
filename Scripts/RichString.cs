using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;


namespace AuraDev
{
    [Serializable]
    public class RichString
    {
        public delegate string RichTextDelegate(string text);
        private static RichStringSettings _sharedSettings;
        public static RichStringSettings sharedSettings
        {
            get
            {
                if (_sharedSettings == null)
                {
                    _sharedSettings = AssetDatabase.FindAssets("t:RichStringSettings").
                        Select(guid => AssetDatabase.LoadAssetAtPath<RichStringSettings>(AssetDatabase.GUIDToAssetPath(guid))).
                        FirstOrDefault();

                    if (_sharedSettings == null) Debug.LogWarning("No RichStringSettings asset could be found. Make sure to create one from create asset menu or RichString won't work.");
                }

                return _sharedSettings;
            }
        }
        [RuntimeInitializeOnLoadMethod]
        public static void SaveSharedSettings()
        {
            sharedSettings.Save();
        }
        [field: SerializeField, TextArea(3, 10)] public string Expression { get; set; }

        private MemberReference[] _memberReferences;
        private Type _targetType;
        private object _targetObject;
        private bool _isInitialized;

        /// <summary>
        /// Initializes the parser with the current expression. Invoke again if the expression is updated.
        /// </summary>
        /// <param name="obj">The target object whose properties will be accessed for referencing.</param>
        public void Initialize(object obj)
        {
            _targetObject = obj;
            _targetType = obj.GetType();

            // Initializing and finding property references
            SetupPropertyReferences();
        }
        /// <summary>
        /// Parses the expression to produce the final output string with evaluated references and rich text formatting.
        /// Replaces all property references with their corresponding values and applies any specified rich text styles.
        /// </summary>
        /// <param name="alternate">Determines whether to use the Normal or Alternate format, 
        /// as specified by classes implementing the IRichStringCustomFormat interface.</param>
        /// <returns>The fully parsed string, with all references evaluated and rich text formatting applied.</returns>
        public string GetParsedString(bool alternate = false)
        {
            string result = Expression;

            // Replacing property references
            foreach (var memberRef in _memberReferences)
            {
                result = result.Replace(memberRef.OriginalExpression, memberRef.GetStringValue(alternate));
            }

            result = GetParsedRichText(result);

            return result;
        }
        public static void HandleError(string message)
        {
            if (sharedSettings.throwExepction)
            {
                throw new Exception(message);
            }
            else
            {
                Debug.LogWarning(message);
            }
        }
        #region Private Methods
        private void SetupPropertyReferences()
        {
            string[] tokens = RegexHelper.GetMatchesByPropertyPattern(Expression);
            _memberReferences = new MemberReference[tokens.Length];

            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];

                MemberReference memberRef = GetLastReference(token);
                memberRef.OriginalExpression = GetOriginalPropertyExpression(token);

                _memberReferences[i] = memberRef;
            }
        }

        // Checks if a token is a chained reference.
        private bool IsReferenceChained(string token)
        {
            return token.Contains(sharedSettings.propertyRefernce);
        }
        // Checks if a reference is an enumerating referenec.
        private bool IsReferenceEnumerating(string reference)
        {
            return reference.Contains(sharedSettings.enumerableIndex);
        }
        private (int Index, string MemberName) GetMemberEnumerationResult(string reference)
        {
            (int Index, string MemberName) result;

            if (!IsReferenceEnumerating(reference))
            {
                result.Index = -1;
                result.MemberName = reference;
                return result;
            }

            string[] parts = reference.Split(sharedSettings.enumerableIndex);
            if (!int.TryParse(parts[1], out result.Index))
            {
                HandleError($"Index is not an integer in {reference} or Enumerable format (EnumerableName->Index) is not correct."); 
                result.Index = -1;
            }
            result.MemberName = parts[0];

            return result;
        }
        public MemberReference GetLastReference(string chainedToken)
        {
            MemberReference result = new();
            string[] refs = chainedToken.Split(sharedSettings.propertyRefernce, StringSplitOptions.None);

            MemberInfo info = GetMemberInfo(GetMemberEnumerationResult(refs[0]).MemberName, _targetType);
            result.DeclaringObject = refs.Length > 1 ? MemberReference.GetValue(info, _targetObject, IsReferenceEnumerating(refs[0]), GetMemberEnumerationResult(refs[0]).Index) : _targetObject;
            result.MemberInfo = info;

            for (int i = 1; i < refs.Length; i++)
            {
                string reference = refs[i];

                info = GetMemberInfo(GetMemberEnumerationResult(reference).MemberName, MemberReference.GetMemberType(result.MemberInfo));
                result.MemberInfo = info;
                result.IsEnumerable = IsReferenceEnumerating(reference);
                result.Index = GetMemberEnumerationResult(reference).Index;

                if (i != refs.Length - 1)
                {
                    object tempDeclaringObject = MemberReference.GetValue(info, result.DeclaringObject, result.IsEnumerable, result.Index);
                    result.DeclaringObject = tempDeclaringObject;
                }
            }

            return result;
        }

        private string[] GetRichTextActionKeys(string text, out string textToBeModified)
        {
            if (!text.Contains(sharedSettings.richText))
            {
                HandleError($"There is no rich text modification specifier in {text}");
                textToBeModified = string.Empty;
                return null;
            }

            string[] splitedText = text.Split(sharedSettings.richText);
            string[] actions = new string[splitedText.Length - 1];

            for (int i = 0; i < actions.Length; i++)
            {
                actions[i] = splitedText[i + 1];
            }

            textToBeModified = splitedText[0];
            return actions;
        }

        // Gets the memeber info from the target type
        private MemberInfo GetMemberInfo(string reference, Type targetType)
        {
            MemberInfo info = targetType.GetMember(reference).FirstOrDefault();

            if (info == null) HandleError($"There is no member named {reference} in {targetType}");

            if (IsReferenceEnumerating(reference))
            {
                return MemberReference.GetMemberType(info).GetGenericArguments()[0];
            }

            return info;
        }
        // Gets the original expression form for replacing in the main expression.
        private string GetOriginalPropertyExpression(string token)
        {
            return $"{RegexHelper.PropertySpec.Start}{token}{RegexHelper.PropertySpec.End}";
        }
        private string GetOriginalRichTextExpression(string text)
        {
            return $"{RegexHelper.RichTextSpec.Start}{text}{RegexHelper.RichTextSpec.End}";
        }

        private string[] GetAllReferencesStringValues()
        {
            string[] result = new string[_memberReferences.Length];

            for (int i = 0; i < _memberReferences.Length; i++)
            {
                result[i] = _memberReferences[i].GetValue().ToString();
            }

            return result;
        }


        private string GetParsedRichText(string expression)
        {
            string[] texts = RegexHelper.GetMatchesByTextModPattern(expression);
            string richText = expression;

            foreach (string text in texts)
            {
                string originalForm = GetOriginalRichTextExpression(text);

                string[] actionKeys = GetRichTextActionKeys(text, out string toBeRichText);

                foreach (string actionKey in actionKeys)
                {
                    toBeRichText = sharedSettings.actionTable.Find(x => x.Key == actionKey).Value(toBeRichText);
                }

                richText = richText.Replace(originalForm, toBeRichText);
            }

            return richText;
        }
        #endregion

        #region RegexHelper
        public static class RegexHelper
        {
            public static Specifier PropertySpec { get; set; } = new Specifier() { Start = '{', End = '}' };
            public static Specifier RichTextSpec { get; set; } = new Specifier() { Start = '[', End = ']' };
            public static readonly string PropertyPattern = $@"\{PropertySpec.Start}(.*?)\{PropertySpec.End}";
            public static readonly string RichTextPattern = $@"\{RichTextSpec.Start}(.*?)\{RichTextSpec.End}";

            public static string[] GetMatchesByPattern(string expression, string pattern)
            {
                Regex regex = new Regex(pattern);
                MatchCollection matches = regex.Matches(expression);

                string[] result = new string[matches.Count];

                for (int i = 0; i < matches.Count; i++)
                {
                    result[i] = matches[i].Groups[1].Value;
                }

                return result;
            }

            public static string[] GetMatchesByPropertyPattern(string expression) => GetMatchesByPattern(expression, PropertyPattern);
            public static string[] GetMatchesByTextModPattern(string expression) => GetMatchesByPattern(expression, RichTextPattern);
        } 
        #endregion
    }

    public struct Specifier
    {
        public char Start { get; set; }
        public char End { get; set; }
    }
}
