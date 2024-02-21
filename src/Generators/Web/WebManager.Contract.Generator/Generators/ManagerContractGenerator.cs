using System.Diagnostics;
using CodeGenHelpers;
using Generators.Base.Generators.Base;
using Microsoft.CodeAnalysis;
using WebManager.Contract.Generator.CodeBuilders;

namespace WebManager.Contract.Generator.Generators
{
    [Generator]
    public class ManagerContractGenerator : BaseGenerator
    {
        public override void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var result = context.CompilationProvider.Select(static (compilation, _) =>
            {
                var repositoryContractCodeBuilder = new ManagerContractCodeBuilder(compilation.AssemblyName).Get(compilation);

                var result = new List<(List<CodeBuilder> codeBuilder, string? folderName, (string, string)? replace)>
                {
                    (repositoryContractCodeBuilder, "ManagerContracts", null),
                };

                return result;
            });

            AddSourceFileName(context, result);
        }
    }
}
