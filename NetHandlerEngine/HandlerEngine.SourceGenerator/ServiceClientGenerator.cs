using System.Collections.Immutable;
using System.Text;

using HandlerEngine.SourceGenerator.CollectableData;
using HandlerEngine.SourceGenerator.Templates;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace HandlerEngine.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class ServiceClientGenerator : IIncrementalGenerator
{
#region IIncrementalGenerator Implementation

	public void Initialize(IncrementalGeneratorInitializationContext ctx)
	{
		IncrementalValuesProvider<InterfaceDeclarationSyntax> interfaceDeclaration =
			ctx.SyntaxProvider
			   .CreateSyntaxProvider(
				   (node, _) => node is InterfaceDeclarationSyntax { AttributeLists.Count: > 0 },
				   (context, _) => GetSemanticTargetForGeneration(context)
			   )
			   .Where(static m => m is not null)!;

		IncrementalValueProvider<(Compilation, ImmutableArray<InterfaceDeclarationSyntax>)> compilationAndInterfaces
			= ctx.CompilationProvider.Combine(interfaceDeclaration.Collect());

		ctx.RegisterSourceOutput(
			compilationAndInterfaces,
			static (spc, source) => Execute(source.Item1, source.Item2, spc)
		);
	}

#endregion

	private static void Execute(Compilation compilation, ImmutableArray<InterfaceDeclarationSyntax> interfaces, SourceProductionContext context)
	{
		if(interfaces.IsDefaultOrEmpty)
		{
			// nothing to do yet
			return;
		}

		IEnumerable<InterfaceDeclarationSyntax> distinctEnums = interfaces.Distinct();
		List<ServiceInfo> interfacesToGenerate = GetTypesToGenerate(compilation, distinctEnums, context);

		if(interfacesToGenerate.Count <= 0)
		{
			return;
		}

		foreach(ServiceInfo itg in interfacesToGenerate)
		{
			bool hasRpcInterface = itg.HasRpc();
			ISourceFileTemplate sourceTemplate;

			if(hasRpcInterface)
			{
				sourceTemplate = new ServiceRpcClientInterfaceTemplate { Info = itg };
				context.AddSource(sourceTemplate.FileName, SourceText.From(sourceTemplate.TransformText(), Encoding.UTF8));
			}

			sourceTemplate = new ServiceClientInterfaceTemplate { Info = itg };
			context.AddSource(sourceTemplate.FileName, SourceText.From(sourceTemplate.TransformText(), Encoding.UTF8));

			if(hasRpcInterface)
			{
				sourceTemplate = new ServiceRpcClientTemplate { Info = itg };
				context.AddSource(sourceTemplate.FileName, SourceText.From(sourceTemplate.TransformText(), Encoding.UTF8));
			}

			sourceTemplate = new ServiceClientTemplate { Info = itg };
			context.AddSource(sourceTemplate.FileName, SourceText.From(sourceTemplate.TransformText(), Encoding.UTF8));

			sourceTemplate = new ServiceMediatorTemplate { Info = itg };
			context.AddSource(sourceTemplate.FileName, SourceText.From(sourceTemplate.TransformText(), Encoding.UTF8));
		}
	}

	private static List<ServiceInfo> GetTypesToGenerate(
		Compilation compilation,
		IEnumerable<InterfaceDeclarationSyntax> interfaces,
		SourceProductionContext ctx)
	{
		var interfacesToGenerate = new List<ServiceInfo>();
		INamedTypeSymbol? serviceAttribute = compilation.GetTypeByMetadataName(AnalyzerConst.NetServiceAttributeFullName);

		if(serviceAttribute == null)
		{
			return interfacesToGenerate;
		}

		INamedTypeSymbol? netCallAttributeSymbol = compilation.GetTypeByMetadataName(AnalyzerConst.NetCallAttributeFullName);

		if(netCallAttributeSymbol == null)
		{
			return interfacesToGenerate;
		}

		INamedTypeSymbol? taskSymbol = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task");
		INamedTypeSymbol? valueTaskSymbol = compilation.GetTypeByMetadataName("System.Threading.Tasks.ValueTask");
		INamedTypeSymbol? taskGenericSymbol = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1");
		INamedTypeSymbol? valueTaskGenericSymbol = compilation.GetTypeByMetadataName("System.Threading.Tasks.ValueTask`1");

		foreach(InterfaceDeclarationSyntax interfaceDeclarationSyntax in interfaces)
		{
			ctx.CancellationToken.ThrowIfCancellationRequested();

			SemanticModel semanticModel = compilation.GetSemanticModel(interfaceDeclarationSyntax.SyntaxTree);
			if(semanticModel.GetDeclaredSymbol(interfaceDeclarationSyntax) is not INamedTypeSymbol interfaceSymbol)
			{
				continue;
			}

			string interfaceName = interfaceDeclarationSyntax.Identifier.Text.TrimStart('I');
			string nameSpace = interfaceSymbol.ContainingNamespace.IsGlobalNamespace ? string.Empty : interfaceSymbol.ContainingNamespace.ToDisplayString();

			ImmutableArray<(IMethodSymbol methodSymbol, AttributeData attributeData)> methods =
				interfaceSymbol
					.GetMembers()
					.OfType<IMethodSymbol>()
					.SelectMany(
						m => m.GetAttributes()
							  .Where(attributeData => SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, netCallAttributeSymbol))
							  .Select(attributeData => (m, attributeData))
					)
					.ToImmutableArray();

			var methodsToGenerate = new List<MethodInfo>();

			HashSet<int> _usedMethodIds = new();

			// Get all the fields from the enum, and add their name to the list
			foreach((IMethodSymbol method, AttributeData attributeData) in methods)
			{
				var methodId = (int)(byte)(attributeData.ConstructorArguments[0].Value ?? -1);

				if(!_usedMethodIds.Add(methodId))
				{
					continue;
				}

				//Default reliable ordered
				byte channelType = 0;
				byte channelId = 0;

				foreach(KeyValuePair<string, TypedConstant> namedArgument in attributeData.NamedArguments)
				{
					switch(namedArgument.Key)
					{
						case "ChannelType":
							channelType = (byte)(namedArgument.Value.Value ?? 0);
							break;
						case "ChannelId":
							channelId = (byte)(namedArgument.Value.Value ?? 0);
							break;
					}
				}

				var callAttributeInfo = new CallAttributeInfo(methodId, channelType, channelId);

				MethodArgumentInfo[] parameters =
				(
					from parameter in method.Parameters
					let parameterName = parameter.Name
					let parameterTypeFullName = parameter.Type.ToDisplayString()
					select new MethodArgumentInfo(parameter.RefKind, parameterTypeFullName, parameterName)).ToArray();

				bool isTask;
				string returnType;

				if(method.ReturnType is INamedTypeSymbol { IsGenericType: true } returnTypeSymbol &&
				   (SymbolEqualityComparer.Default.Equals(returnTypeSymbol.ConstructedFrom, taskGenericSymbol) ||
					SymbolEqualityComparer.Default.Equals(returnTypeSymbol.ConstructedFrom, valueTaskGenericSymbol)))
				{
					isTask = true;
					ITypeSymbol genericType = returnTypeSymbol.TypeArguments[0];
					returnType = genericType.ToDisplayString();
				}
				else if(SymbolEqualityComparer.Default.Equals(method.ReturnType.OriginalDefinition, taskSymbol) ||
						SymbolEqualityComparer.Default.Equals(method.ReturnType.OriginalDefinition, valueTaskSymbol))
				{
					isTask = true;
					returnType = "void";
				}
				else
				{
					isTask = false;
					returnType = method.ReturnType.ToDisplayString();
				}

				var methodToGenerate = new MethodInfo(
					method.Name,
					returnType,
					callAttributeInfo,
					parameters,
					isTask
				);
				methodsToGenerate.Add(methodToGenerate);
			}

			interfacesToGenerate.Add(
				new ServiceInfo(interfaceName.TrimStart('I'), methodsToGenerate.ToArray(), nameSpace)
			);
		}

		return interfacesToGenerate;
	}

	private static InterfaceDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
	{
		if(context.Node is InterfaceDeclarationSyntax interfaceDeclarationSyntax)
		{
			SemanticModel semanticModel = context.SemanticModel;

			if(semanticModel.GetDeclaredSymbol(interfaceDeclarationSyntax) is INamedTypeSymbol interfaceSymbol)
			{
				INamedTypeSymbol? serviceAttribute = semanticModel.Compilation.GetTypeByMetadataName(AnalyzerConst.NetServiceAttributeFullName);

				if(serviceAttribute == null)
				{
					return null;
				}

				foreach(AttributeData? attributeData in interfaceSymbol.GetAttributes())
				{
					if(SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, serviceAttribute))
					{
						return interfaceDeclarationSyntax;
					}
				}
			}
		}

		return null;
	}
}