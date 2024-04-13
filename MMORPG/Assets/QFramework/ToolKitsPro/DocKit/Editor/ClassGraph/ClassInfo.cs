/****************************************************************************
 * Copyright (c) 2022 liangxiegame UNDER Paid MIT LICENSE  see licenses.txt
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityEngine;

namespace QFramework.Pro
{
    [System.Serializable]
    public enum AccessType
    {
        Public,
        Protected,
        Private,
        Internal,
    }

    public class AccessTypeHelper
    {
        public static AccessType GetAccessType(SyntaxTokenList syntaxTokenList)
        {
            if (syntaxTokenList.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
            {
                return AccessType.Public;
            }

            if (syntaxTokenList.Any(m => m.IsKind(SyntaxKind.ProtectedKeyword)))
            {
                return AccessType.Protected;
            }

            if (syntaxTokenList.Any(m => m.IsKind(SyntaxKind.PrivateKeyword)))
            {
                return AccessType.Private;
            }

            if (syntaxTokenList.Any(m => m.IsKind(SyntaxKind.InternalKeyword)))
            {
                return AccessType.Internal;
            }

            return AccessType.Public;
        }
    }

    [System.Serializable]
    public class CommentInfo
    {
        [SerializeField] public string Content;

        public CommentInfo Parse(SyntaxTrivia syntaxTrivia)
        {
            var content = syntaxTrivia.ToString().RemoveString("/// ").RemoveString("///");

            var startIndex = content.IndexOf("<summary>", StringComparison.CurrentCulture) + "<summary>".Length;
            var endIndex = content.IndexOf("</summary>", StringComparison.CurrentCulture);

            if (endIndex == -1) // 容错 有的地方不写  </summary>
            {
                endIndex = content.Length;
            }

            var commentContent = content.Substring(startIndex, endIndex - startIndex);

            var stringBuilder = new StringBuilder();
            foreach (var s in commentContent.Split('\n').Select(c => c.TrimStart())
                         .Where(c => c.IsTrimNotNullAndEmpty()))
            {
                stringBuilder.Append("// " + s);
                stringBuilder.AppendLine();
            }

            // remove last
            if (stringBuilder.Length != 0) // 容错 
            {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }

            Content = stringBuilder.ToString();

            return this;
        }
    }

    [System.Serializable]
    public class FieldInfo
    {
        [SerializeField] public AccessType AccessType = AccessType.Public;

        [SerializeField] public int StartLineNumber = 0;
        [SerializeField] public string Type = string.Empty;
        [SerializeField] public string Name = string.Empty;

        [SerializeField] public CommentInfo Comment = new CommentInfo();
        
        public FieldInfo Parse(FieldDeclarationSyntax fieldDeclarationSyntax)
        {
            StartLineNumber = fieldDeclarationSyntax.SyntaxTree.GetLineSpan(fieldDeclarationSyntax.Span)
                .StartLinePosition.Line;
            Type = fieldDeclarationSyntax.Declaration.Type.ToString();

            Name = fieldDeclarationSyntax.Declaration.Variables.ToString();

            AccessType = AccessTypeHelper.GetAccessType(fieldDeclarationSyntax.Modifiers);

            if (fieldDeclarationSyntax.HasLeadingTrivia)
            {
                var trivia = fieldDeclarationSyntax.GetLeadingTrivia();

                foreach (var syntaxTrivia in trivia)
                {
                    if (syntaxTrivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    {
                        if (syntaxTrivia.ToString().IsNullOrEmpty())
                        {
                            continue;
                        }
                        else
                        {
                            Comment.Parse(syntaxTrivia);
                        }
                    }
                }
            }

            return this;
        }

        public override string ToString()
        {
            return new StringBuilder()
                .Append(Name)
                .Append(":")
                .Append(Type)
                .ToString();
        }
    }

    [System.Serializable]
    public class PropertyInfo
    {
        [SerializeField] public AccessType AccessType = AccessType.Public;

        [SerializeField] public int StartLineNumber = 0;
        [SerializeField] public string Type = string.Empty;
        [SerializeField] public string Name = string.Empty;

        [SerializeField] public CommentInfo Comment = new CommentInfo();

        public PropertyInfo Parse(PropertyDeclarationSyntax propertyDeclarationSyntax)
        {
            StartLineNumber = propertyDeclarationSyntax.SyntaxTree.GetLineSpan(propertyDeclarationSyntax.Span)
                .StartLinePosition.Line;
            Type = propertyDeclarationSyntax.Type.ToString();

            Name = propertyDeclarationSyntax.Identifier.ToString();

            AccessType = AccessTypeHelper.GetAccessType(propertyDeclarationSyntax.Modifiers);

            if (propertyDeclarationSyntax.HasLeadingTrivia)
            {
                var trivia = propertyDeclarationSyntax.GetLeadingTrivia();

                foreach (var syntaxTrivia in trivia)
                {
                    if (syntaxTrivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    {
                        if (syntaxTrivia.ToString().IsNullOrEmpty())
                        {
                            continue;
                        }
                        else
                        {
                            Comment.Parse(syntaxTrivia);
                        }
                    }
                }
            }

            return this;
        }

        public override string ToString()
        {
            return new StringBuilder()
                .Append(Name)
                .Append(":")
                .Append(Type)
                .ToString();
        }
    }

    [System.Serializable]
    public class ParameterInfo
    {
        public string Type;
        public string Name;
    }

    [System.Serializable]
    public class MethodInfo
    {
        [SerializeField] public AccessType AccessType = AccessType.Public;

        [SerializeField] public int StartLineNumber = 0;
        [SerializeField] public string ReturnType = string.Empty;
        [SerializeField] public string Name = string.Empty;
        [SerializeField] public List<ParameterInfo> Parameters = new List<ParameterInfo>();
        [SerializeField] public CommentInfo Comment = new CommentInfo();

        public MethodInfo Parse(MethodDeclarationSyntax methodDeclarationSyntax)
        {
            
            StartLineNumber = methodDeclarationSyntax.SyntaxTree.GetLineSpan(methodDeclarationSyntax.Span)
                .StartLinePosition.Line;

            
            Name = methodDeclarationSyntax.Identifier.ToString();
            ReturnType = methodDeclarationSyntax.ReturnType.ToString();

            AccessType = AccessTypeHelper.GetAccessType(methodDeclarationSyntax.Modifiers);

            if (methodDeclarationSyntax.HasLeadingTrivia)
            {
                var trivia = methodDeclarationSyntax.GetLeadingTrivia();

                foreach (var syntaxTrivia in trivia)
                {
                    if (syntaxTrivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    {
                        if (syntaxTrivia.ToString().IsNullOrEmpty())
                        {
                            continue;
                        }
                        else
                        {
                            Comment.Parse(syntaxTrivia);
                        }
                    }
                }
            }

            foreach (var parameter in methodDeclarationSyntax.ParameterList.Parameters)
            {
                Parameters.Add(new ParameterInfo()
                {
                    Type = parameter.Type.ToString(),
                    Name = parameter.Identifier.ToString()
                });
            }

            return this;
        }

        public override string ToString()
        {
            return new StringBuilder()
                // .Append(AccessType.ToString().ToLower())
                // .Append(" ")
                .Append(ReturnType)
                .Append(" ")
                .Append(Name)
                .Append("(")
                .Append(string.Join(",", Parameters.Select(p => p.Type + " " + p.Name).ToArray()))
                .Append(")")
                .ToString();
        }

        private GUIContent mGUIContent;

        public GUIContent GetGUIContent()
        {
            if (mGUIContent == null)
            {
                mGUIContent = new GUIContent("  " + ToString());
            }

            return mGUIContent;
        }
    }

    [System.Serializable]
    public class ClassInfo
    {
        [SerializeField] public int StartLineNumber;

        public ClassInfo(string filePath)
        {
            FilePath = filePath;
        }

        [SerializeField] public string FilePath = string.Empty;

        [SerializeField] public string Inherit;

        [SerializeField] public string Name;

        [SerializeField] public CommentInfo Comment = new CommentInfo();

        [SerializeField] public List<PropertyInfo> Properties = new List<PropertyInfo>();

        [SerializeField] public List<FieldInfo> Fields = new List<FieldInfo>();
        
        [SerializeField] public List<MethodInfo> Methods = new List<MethodInfo>();

        private HashSet<string> MethodNameCache = new HashSet<string>();

        public bool IsGeneric = false;
        
        public ClassInfo Parse(ClassDeclarationSyntax classDeclarationSyntax)
        {
            Name = classDeclarationSyntax.Identifier.ToFullString();
            
            if (classDeclarationSyntax.BaseList?.Types.Count > 0)
            {
                
                Inherit = string.Join(",", classDeclarationSyntax.BaseList.Types.Select(t => t.Type.ToFullString()));
            }
            else
            {
                Inherit = "System.Object";
            }

            StartLineNumber = classDeclarationSyntax.SyntaxTree.GetLineSpan(classDeclarationSyntax.Span)
                .StartLinePosition.Line;

            if (classDeclarationSyntax.HasLeadingTrivia)
            {
                var trivia = classDeclarationSyntax.GetLeadingTrivia();

                foreach (var syntaxTrivia in trivia)
                {
                    if (syntaxTrivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia))
                    {
                        if (syntaxTrivia.ToString().IsNullOrEmpty())
                        {
                            continue;
                        }
                        else
                        {
                            Comment.Parse(syntaxTrivia);
                        }
                    }
                }
            }

            foreach (var memberDeclarationSyntax in classDeclarationSyntax.Members)
            {
                if (memberDeclarationSyntax.IsKind(SyntaxKind.PropertyDeclaration))
                {
                    Properties.Add(new PropertyInfo().Parse(memberDeclarationSyntax.As<PropertyDeclarationSyntax>()));
                }
                else if (memberDeclarationSyntax.IsKind(SyntaxKind.MethodDeclaration))
                {
                    var method = new MethodInfo().Parse(memberDeclarationSyntax.As<MethodDeclarationSyntax>());
                    if (MethodNameCache.Contains(method.Name)) continue;
                    Methods.Add(method);
                    MethodNameCache.Add(method.Name);
                }
                else if (memberDeclarationSyntax.IsKind(SyntaxKind.FieldDeclaration))
                {
                    var method = new FieldInfo().Parse(memberDeclarationSyntax.As<FieldDeclarationSyntax>());
                    Fields.Add(method);
                }
                
            }

            return this;
        }
    }

    [System.Serializable]
    public class FileInfo
    {
        [SerializeField] public string FilePath = string.Empty;
        [SerializeField] public List<ClassInfo> Classes = new List<ClassInfo>();
    }
}