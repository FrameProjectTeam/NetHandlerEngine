﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace HandlerEngine.SourceGenerator.Templates
{
    using HandlerEngine.SourceGenerator.CollectableData;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "E:\Projects\Unity\FrameProject\mmog\mmog_backend\HandlerEngine.SourceGenerator\Templates\ServiceRpcClientTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class ServiceRpcClientTemplate : ServiceRpcClientTemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            this.Write("using System;\r\nusing System.Buffers.Binary;\r\nusing System.Collections.Generic;\r\nusing System.Threading.Tasks;\r\n\r\nusing HandlerEngine;\r\nusing HandlerEngine.Interfaces;\r\nusing HandlerEngine.Serialization;\r\n\r\n");
            
            #line 12 "E:\Projects\Unity\FrameProject\mmog\mmog_backend\HandlerEngine.SourceGenerator\Templates\ServiceRpcClientTemplate.tt"

    if(Info.HasNamespace)
    {

            
            #line default
            #line hidden
            this.Write("namespace ");
            
            #line 16 "E:\Projects\Unity\FrameProject\mmog\mmog_backend\HandlerEngine.SourceGenerator\Templates\ServiceRpcClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Info.Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n");
            
            #line 18 "E:\Projects\Unity\FrameProject\mmog\mmog_backend\HandlerEngine.SourceGenerator\Templates\ServiceRpcClientTemplate.tt"

        PushIndent("\t");
    }

            
            #line default
            #line hidden
            this.Write("public abstract class ");
            
            #line 22 "E:\Projects\Unity\FrameProject\mmog\mmog_backend\HandlerEngine.SourceGenerator\Templates\ServiceRpcClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Info.ServiceName));
            
            #line default
            #line hidden
            this.Write("RpcClient : I");
            
            #line 22 "E:\Projects\Unity\FrameProject\mmog\mmog_backend\HandlerEngine.SourceGenerator\Templates\ServiceRpcClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Info.ServiceName));
            
            #line default
            #line hidden
            this.Write("RpcClient\r\n{\r\n    public string ServiceName => \"");
            
            #line 24 "E:\Projects\Unity\FrameProject\mmog\mmog_backend\HandlerEngine.SourceGenerator\Templates\ServiceRpcClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Info.ServiceName));
            
            #line default
            #line hidden
            this.Write("\";\r\n    public byte ServiceId { get; private set; }\r\n    public INetRecipient Recipient { get; private set; }\r\n\r\n    void IServiceUnit.Bind(INetRecipient recipient, byte serviceId)\r\n    {\r\n        Recipient = recipient;\r\n        ServiceId = serviceId;\r\n    }\r\n\r\n");
            
            #line 34 "E:\Projects\Unity\FrameProject\mmog\mmog_backend\HandlerEngine.SourceGenerator\Templates\ServiceRpcClientTemplate.tt"

    var count = 0;
    foreach(MethodInfo mi in Info.MethodInfos)
    {
        if(!mi.IsRpc())
        {
            continue;
        }

        if(count++ > 0)
        {
            WriteLine(string.Empty);
        }

            
            #line default
            #line hidden
            this.Write("    public ");
            
            #line 48 "E:\Projects\Unity\FrameProject\mmog\mmog_backend\HandlerEngine.SourceGenerator\Templates\ServiceRpcClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(mi.ReturnType));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 48 "E:\Projects\Unity\FrameProject\mmog\mmog_backend\HandlerEngine.SourceGenerator\Templates\ServiceRpcClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(mi.Name));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 48 "E:\Projects\Unity\FrameProject\mmog\mmog_backend\HandlerEngine.SourceGenerator\Templates\ServiceRpcClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(mi.Parameters.Inline(true)));
            
            #line default
            #line hidden
            this.Write(")\r\n    {\r\n        var __opCode = new OperationCode(ServiceId, 0x");
            
            #line 50 "E:\Projects\Unity\FrameProject\mmog\mmog_backend\HandlerEngine.SourceGenerator\Templates\ServiceRpcClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(mi.CallAttributeInfo.CallId.ToString("X2")));
            
            #line default
            #line hidden
            this.Write(");\r\n        NetworkBufferWriter __buffer = WriterBufferPool.Buffer;\r\n        \r\n        BinaryPrimitives.WriteUInt16LittleEndian(__buffer.GetSpan(sizeof(ushort)), __opCode.OpCode);\r\n\t\t__buffer.Advance(sizeof(ushort));\r\n\r\n");
            
            #line 56 "E:\Projects\Unity\FrameProject\mmog\mmog_backend\HandlerEngine.SourceGenerator\Templates\ServiceRpcClientTemplate.tt"

        foreach(MethodArgumentInfo pi in mi.Parameters)
        {

            
            #line default
            #line hidden
            this.Write("        HandlerEngineSerializer.Serializer.Write(ref __buffer, ");
            
            #line 60 "E:\Projects\Unity\FrameProject\mmog\mmog_backend\HandlerEngine.SourceGenerator\Templates\ServiceRpcClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(pi.Name));
            
            #line default
            #line hidden
            this.Write(");\r\n");
            
            #line 61 "E:\Projects\Unity\FrameProject\mmog\mmog_backend\HandlerEngine.SourceGenerator\Templates\ServiceRpcClientTemplate.tt"

        }

            
            #line default
            #line hidden
            this.Write("        var __writtenSpan = __buffer.WrittenSpan;\r\n        Recipient!.Send(ref __writtenSpan, ");
            
            #line 65 "E:\Projects\Unity\FrameProject\mmog\mmog_backend\HandlerEngine.SourceGenerator\Templates\ServiceRpcClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(mi.CallAttributeInfo.ChannelType.InlineChanelType()));
            
            #line default
            #line hidden
            this.Write(",  ");
            
            #line 65 "E:\Projects\Unity\FrameProject\mmog\mmog_backend\HandlerEngine.SourceGenerator\Templates\ServiceRpcClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(mi.CallAttributeInfo.ChannelId.ToString()));
            
            #line default
            #line hidden
            this.Write(");\r\n    }\r\n");
            
            #line 67 "E:\Projects\Unity\FrameProject\mmog\mmog_backend\HandlerEngine.SourceGenerator\Templates\ServiceRpcClientTemplate.tt"

    }

            
            #line default
            #line hidden
            this.Write("}\r\n\r\npublic sealed class ");
            
            #line 72 "E:\Projects\Unity\FrameProject\mmog\mmog_backend\HandlerEngine.SourceGenerator\Templates\ServiceRpcClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Info.ServiceName));
            
            #line default
            #line hidden
            this.Write("BroadcastClient : ");
            
            #line 72 "E:\Projects\Unity\FrameProject\mmog\mmog_backend\HandlerEngine.SourceGenerator\Templates\ServiceRpcClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Info.ServiceName));
            
            #line default
            #line hidden
            this.Write("RpcClient, IBroadcastServiceClient\r\n{ }\r\n");
            
            #line 74 "E:\Projects\Unity\FrameProject\mmog\mmog_backend\HandlerEngine.SourceGenerator\Templates\ServiceRpcClientTemplate.tt"

    if(Info.HasNamespace)
    {
        PopIndent();

            
            #line default
            #line hidden
            this.Write("}\r\n");
            
            #line 80 "E:\Projects\Unity\FrameProject\mmog\mmog_backend\HandlerEngine.SourceGenerator\Templates\ServiceRpcClientTemplate.tt"

    }

            
            #line default
            #line hidden
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public class ServiceRpcClientTemplateBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
