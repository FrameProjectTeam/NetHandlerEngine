using System.Collections.Immutable;

using JetBrains.Annotations;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace HandlerEngine.SourceGenerator;

[UsedImplicitly]
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DuplicateCallIdAnalyzer : DiagnosticAnalyzer
{
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DuplicateMethodIdRule);

	private static DiagnosticDescriptor DuplicateMethodIdRule =>
		new(
			"SCG001",
			"Not unique package id service scope",
			"Unique package identifiers must be used, otherwise the code generation will not work correctly. The duplicated code is {0}",
			"Usage",
			DiagnosticSeverity.Error,
			true,
			"Duplicate method id"
		);

	public override void Initialize(AnalysisContext ctx)
	{
		ctx.EnableConcurrentExecution();
		ctx.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
		ctx.RegisterCompilationStartAction(
			compilationContext =>
			{
				INamedTypeSymbol? attributeSymbol = compilationContext.Compilation.GetTypeByMetadataName(AnalyzerConst.NetCallAttributeFullName);

				if(attributeSymbol != null)
				{
					compilationContext.RegisterSymbolAction(c => AnalyzeMethod(c, attributeSymbol), SymbolKind.Method);
				}
				//TODO: Log error
			}
		);
	}

	private static void AnalyzeMethod(SymbolAnalysisContext context, INamedTypeSymbol attributeSymbol)
	{
		var methodSymbol = (IMethodSymbol)context.Symbol;
		AttributeData? attributeData =
			methodSymbol.GetAttributes()
						.FirstOrDefault(
							a =>
								SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol)
						);

		if(attributeData is not { ConstructorArguments.Length: > 0 })
		{
			return;
		}

		{
			object? byteValue = attributeData.ConstructorArguments[0].Value;

			// Check other methods for this attribute value
			IEnumerable<IMethodSymbol> otherMethods = methodSymbol.ContainingType
																  .GetMembers()
																  .OfType<IMethodSymbol>()
																  .Where(m => !SymbolEqualityComparer.Default.Equals(m, methodSymbol));

			foreach(IMethodSymbol? otherMethod in otherMethods)
			{
				AttributeData? otherAttributeData = otherMethod.GetAttributes()
															   .FirstOrDefault(
																   a =>
																	   SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol)
															   );

				object? value = otherAttributeData?.ConstructorArguments[0].Value;
				if(value == null ||
				   otherAttributeData is not { ConstructorArguments.Length: > 0 } ||
				   !value.Equals(byteValue))
				{
					continue;
				}

				if(attributeData.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken) is AttributeSyntax
				   {
					   ArgumentList.Arguments.Count: > 0
				   } attributeSyntax)
				{
					var diagnostic = Diagnostic.Create(
						DuplicateMethodIdRule, attributeSyntax.ArgumentList.Arguments[0].GetLocation(), $"{byteValue} [0x{(byte)byteValue:X2}]"
					);
					context.ReportDiagnostic(diagnostic);
				}
				else
				{
					var diagnostic = Diagnostic.Create(DuplicateMethodIdRule, methodSymbol.Locations[0], $"{byteValue} [0x{(byte)byteValue:X2}]");
					context.ReportDiagnostic(diagnostic);
				}

				break;
			}
		}
	}
}