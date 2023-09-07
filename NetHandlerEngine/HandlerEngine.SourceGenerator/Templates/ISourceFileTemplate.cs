namespace HandlerEngine.SourceGenerator.Templates;

public interface ISourceFileTemplate
{
	string FileName { get; }

	string TransformText();
}