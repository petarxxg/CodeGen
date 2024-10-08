﻿using Codelisk.GeneratorAttributes.GeneralAttributes.Registration;
using CodeGenHelpers;
using Generator.Foundation.Generators.Base;
using Generators.Base.Helpers;
using Microsoft.CodeAnalysis;
using WebManager.Generator.CodeBuilders;
using Generators.Base;

namespace WebManager.Contract.Generator.CodeBuilders
{
    public class ManagerContractCodeBuilder : BaseCodeBuilder
    {
        public ManagerContractCodeBuilder(string codeBuilderNamespace) : base(codeBuilderNamespace)
        {
        }

        public override List<CodeBuilder> Get(Compilation context, List<CodeBuilder> codeBuilders = null)
        {
            var repos = new ManagerCodeBuilder(context.AssemblyName).Get(context);

            List<CodeBuilder> result = new List<CodeBuilder>();
            EquatableArray<CodeBuilder> equatableRepos = new(repos.ToArray());

            foreach (var repo in equatableRepos)
            {
                var interfaceBuilder = ClassInterface(repo, context);
                result.Add(interfaceBuilder);
            }

            return result;
        }
        private CodeBuilder ClassInterface(CodeBuilder originalBuilder, Compilation context)
        {
            return originalBuilder.GenerateSeperateInterfaceCodeBuilder<RegisterTransient>(context);
        }
    }
}
