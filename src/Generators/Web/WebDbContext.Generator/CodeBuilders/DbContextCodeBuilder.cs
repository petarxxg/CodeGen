using Codelisk.GeneratorAttributes;
using Codelisk.GeneratorAttributes.GeneratorAttributes;
using CodeGenHelpers;
using Foundation.Crawler.Crawlers;
using Generator.Foundation.Generators.Base;
using Generators.Base.Extensions;
using Microsoft.CodeAnalysis;
using Codelisk.GeneratorAttributes.WebAttributes.Repository;

namespace WebDbContext.Generator.CodeBuilders
{
    public class DbContextCodeBuilder : BaseCodeBuilder
    {
        public DbContextCodeBuilder(string codeBuilderNamespace) : base(codeBuilderNamespace)
        {
        }

        public override List<CodeBuilder> Get(Compilation compilation, List<CodeBuilder> codeBuilders = null)
        {
            var attributeCompilationCrawler = new AttributeCompilationCrawler(compilation);
            var dtos = attributeCompilationCrawler.Entities().ToList();
            return Build(attributeCompilationCrawler, dtos);
        }

        private List<CodeBuilder?> Build(AttributeCompilationCrawler context, IEnumerable<INamedTypeSymbol> entities)
        {
            var result = new List<CodeBuilder?>();
            var baseContext = context.BaseContext();
            var builder = CreateBuilder(baseContext.ContainingNamespace.ToString());
            Class(builder, entities, baseContext);
            result.Add(builder);

            return result;
        }
        private IReadOnlyList<ClassBuilder> Class(CodeBuilder builder, IEnumerable<INamedTypeSymbol> entities, INamedTypeSymbol baseContext)
        {
            var result = builder.AddClass(baseContext.Name).WithAccessModifier(Accessibility.Public).AddNamespaceImport("Microsoft.EntityFrameworkCore")
                .AddAttribute(typeof(GeneratedDbContextAttribute).FullName);

            foreach (var entity in entities)
            {
                result.AddProperty(entity.Name.GetParameterName(), Accessibility.Public).SetType($"DbSet<{entity.Name}>").UseAutoProps();
            }

            return builder.Classes;
        }

    }
}
