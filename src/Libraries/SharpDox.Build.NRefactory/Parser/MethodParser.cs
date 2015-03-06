﻿using ICSharpCode.NRefactory.TypeSystem;
using SharpDox.Model;
using SharpDox.Model.Repository;
using SharpDox.Model.Repository.Members;
using System.Collections.Generic;
using System.Linq;
using SharpDox.Sdk.Config;

namespace SharpDox.Build.NRefactory.Parser
{
    internal class MethodParser : BaseParser
    {
        private readonly TypeParser _typeParser;

        internal MethodParser(SDRepository repository, TypeParser typeParser, ICoreConfigSection sharpDoxConfig) : base(repository, sharpDoxConfig)
        {
            _typeParser = typeParser;
        }

        internal void ParseConstructors(SDType sdType, IType type)
        {
            var constructors = type.GetConstructors(null, GetMemberOptions.IgnoreInheritedMembers);
            constructors = constructors.Where(o => !o.DeclaringType.FullName.StartsWith("System.Object"));
            ParseMethodList(sdType.Constructors, constructors, true);
        }

        internal void ParseMethods(SDType sdType, IType type)
        {
            var methods = type.GetMethods(null, GetMemberOptions.IgnoreInheritedMembers);
            methods = methods.Where(o => !o.DeclaringType.FullName.StartsWith("System.Object"));
            ParseMethodList(sdType.Methods, methods, false);
        }

        private void ParseMethodList(SortedList<SDMethod> sdMethodList, IEnumerable<IMethod> methodList, bool isCtor)
        {
            foreach (var method in methodList)
            {
                if (sdMethodList.SingleOrDefault((i => i.Identifier == method.GetIdentifier())) == null
                    && !IsMemberExcluded(method.GetIdentifier(), method.Accessibility.ToString()))
                {
                    var sdMethod = GetParsedMethod(method, isCtor);
                    sdMethodList.Add(sdMethod);
                }
            }
        }

        private SDMethod GetParsedMethod(IMethod method, bool isCtor)
        {
            var sdMethod = _repository.GetMethodByIdentifier(method.GetIdentifier());
            if (sdMethod != null)
            {
                return sdMethod;
            }

            var returnType = _typeParser.GetParsedType(method.ReturnType);

            string acc = method.Accessibility.ToString().ToLower();
            if (acc == "none") acc = "public";

            sdMethod = new SDMethod(method.GetIdentifier(), isCtor ? method.DeclaringType.Name : method.Name)
            {
                Namespace = method.Namespace,
                DeclaringType = _typeParser.GetParsedType(method.DeclaringType),
                ReturnType = returnType,
                IsCtor = isCtor,
                Accessibility = acc,
                IsAbstract = method.IsAbstract,
                IsShadowing = method.IsShadowing,
                IsOverride = method.IsOverride,
                IsPrivate = method.IsPrivate,
                IsProtected = method.IsProtected,
                IsPublic = method.IsPublic,
                IsSealed = method.IsSealed,
                IsVirtual = method.IsVirtual,
                IsStatic = method.IsStatic,
                Documentations = _documentationParser.ParseDocumentation(method),
                Region = new SDRegion
                {
                    BeginColumn = method.Region.BeginColumn,
                    BeginLine = method.Region.BeginLine,
                    EndColumn = method.Region.EndColumn,
                    EndLine = method.Region.EndLine,
                    Filename = method.Region.FileName
                }
            };

            foreach (ITypeParameter typeParameter in method.TypeParameters)
            {
                sdMethod.TypeParameters.Add(_typeParser.GetParsedType(typeParameter));
            }

            foreach (IParameter parameter in method.Parameters)
            {
                sdMethod.Parameters.Add(new SDParameter
                {
                    Name = parameter.Name,
                    ParamType = _typeParser.GetParsedType(parameter.Type),
                    IsOptional = parameter.IsOptional,
                    IsConst = parameter.IsConst,
                    ConstantValue = parameter.ConstantValue,
                    IsRef = parameter.IsRef,
                    IsOut = parameter.IsOut
                });
            }

            _repository.AddMethod(sdMethod);
            return sdMethod;
        }

        internal static void ParseMinimalConstructors(SDType sdType, IType type)
        {
            var constructors = type.GetConstructors(null, GetMemberOptions.IgnoreInheritedMembers);
            constructors = constructors.Where(o => !o.DeclaringType.FullName.StartsWith("System.Object"));
            MinimalParseMethodList(sdType.Constructors, constructors, true);
        }

        internal static void ParseMinimalMethods(SDType sdType, IType type)
        {
            var methods = type.GetMethods(null, GetMemberOptions.IgnoreInheritedMembers);
            methods = methods.Where(o => !o.DeclaringType.FullName.StartsWith("System.Object"));
            MinimalParseMethodList(sdType.Methods, methods, false);
        }

        private static void MinimalParseMethodList(SortedList<SDMethod> sdMethods, IEnumerable<IMethod> methods, bool isCtor)
        {
            foreach (var method in methods)
            {
                var parsedMethod = GetMinimalParsedMethod(method, isCtor);
                if (sdMethods.SingleOrDefault(f => f.Name == parsedMethod.Name) == null)
                {
                    sdMethods.Add(parsedMethod);
                }
            }
        }

        private static SDMethod GetMinimalParsedMethod(IMethod method, bool isCtor)
        {
            string acc = method.Accessibility.ToString().ToLower();
            if(acc == "none") acc = "public";
            return new SDMethod(method.GetIdentifier(), isCtor ? method.DeclaringType.Name : method.Name)
            {
                IsCtor = isCtor,
                Accessibility = acc
            };
        }
    }
}
