﻿using Codelisk.GeneratorAttributes.GeneralAttributes.Registration;
using CodeGenHelpers;
using Generators.Base.Extensions;
using Microsoft.CodeAnalysis;
using Generator.Foundation.Generators.Base;

namespace Generators.Base.CodeBuilders
{
    public abstract class ClassServicesModuleInitializerBuilder : BaseCodeBuilder
    {
        public ClassServicesModuleInitializerBuilder(string codeBuilderNamespace) : base(codeBuilderNamespace)
        {
        }
        public virtual List<(string serviceUsage, string serviceType, string serviceImplementation)> Services { get; set; } = new List<(string serviceUsage, string serviceType, string serviceImplementation)>();

        public void InitServices(Compilation context, List<CodeBuilder> codeBuilders = null)
        {
            if (Services is null)
            {
                Services = new List<(string, string, string)>();
            }

            List<string> serviceTypes = new List<string>();
            if (codeBuilders is not null)
            {
                foreach (var codeBuilder in codeBuilders)
                {
                    foreach (var c in codeBuilder.GetClasses(context))
                    {
                        var registerAttribute = c.GetAttributes().FirstOrDefault(x => nameof(RegisterTransient).Equals(x.AttributeClass.Name));
                        var registerSingletonAttribute = c.GetAttributes().FirstOrDefault(x => nameof(RegisterSingleton).Equals(x.AttributeClass.Name));

                        if (registerAttribute is not null)
                        {
                            var type = registerAttribute.GetFirstConstructorArgumentAsTypedConstant().Value as Type;
                            if (type is not null)
                            {
                                Services.Add(("AddTransient", type.Name, c.Name));
                            }
                            else
                            {
                                Services.Add(("AddTransient", c.Interfaces.FirstOrDefault()?.Name, c.Name));
                            }
                        }
                        else if(registerSingletonAttribute is not null)
                        {
                            var type = registerSingletonAttribute.GetFirstConstructorArgumentAsTypedConstant().Value as Type;
                            if (type is not null)
                            {
                                Services.Add(("AddSingleton", type.Name, c.Name));
                            }
                            else
                            {
                                Services.Add(("AddSingleton", c.Interfaces.FirstOrDefault()?.Name, c.Name));
                            }
                        }
                    }
                }
            }
        }

        public override List<CodeBuilder> Get(Compilation compilation, List<CodeBuilder> codeBuilders = null)
        {
            InitServices(compilation, codeBuilders);
            var builder = CreateBuilder();
            builder.AddClass("ModuleInitializer")
                .AddNamespaceImport("Microsoft.Extensions.DependencyInjection").WithAccessModifier(Accessibility.Public)
                .AddMethod("partial AddServices", Accessibility.NotApplicable)
                .AddParameter("IServiceCollection", "services")
                .WithBody(x =>
                {
                    foreach (var service in Services)
                    {
                        var serviceUsage = string.IsNullOrEmpty(service.serviceUsage) ? "AddTransient" : service.serviceUsage;
                        var serviceImplementation = service.serviceImplementation is null ? string.Empty : service.serviceImplementation;

                        if (!string.IsNullOrEmpty(serviceImplementation))
                        {
                            x.AppendLine($"services.{serviceUsage}<{service.serviceType},{serviceImplementation}>();");
                        }
                        else
                        {
                            x.AppendLine($"services.{serviceUsage}<{service.serviceType}>();");
                        }
                    }
                });

            return new List<CodeBuilder> { builder };
        }
    }
}
