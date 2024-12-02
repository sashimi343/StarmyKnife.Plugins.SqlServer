using StarmyKnife.Core.Plugins;

using PoorMansTSqlFormatterRedux.Tokenizers;
using PoorMansTSqlFormatterRedux.Formatters;
using PoorMansTSqlFormatterRedux.Parsers;
using Microsoft.SqlServer.Management.SqlParser.Parser;

namespace StarmyKnife.Plugins.SqlServer
{
    [StarmyKnifePlugin("TSQL (SQL Server)")]
    public class TsqlPrettyValidator : PluginBase, IPrettyValidator
    {
        public bool CanPrettify => true;

        public bool CanMinify => false;

        public PluginInvocationResult Minify(string input, PluginParameterCollection parameters)
        {
            throw new NotImplementedException();
        }

        public PluginInvocationResult Prettify(string input, PluginParameterCollection parameters)
        {
            var tokenizer = new TSqlStandardTokenizer();
            var parser = new TSqlStandardParser();
            var formatter = new TSqlStandardFormatter();

            var tokenizedSql = tokenizer.TokenizeSQL(input);
            var parsedSql = parser.ParseSQL(tokenizedSql);
            var formattedSql = formatter.FormatSQLTree(parsedSql);

            return PluginInvocationResult.OfSuccess(formattedSql);
        }

        public ValidationResult Validate(string input, PluginParameterCollection parameters)
        {
            var parseResult = Parser.Parse(input);

            if (parseResult.Errors.Any())
            {
                var errors = new List<string>();
                foreach (var error in parseResult.Errors)
                {
                    var message = error.Message;
                    var position = error.Start;

                    errors.Add($"Error at position {position}: {message}");
                }

                return ValidationResult.OfFailure(errors);
            }

            return ValidationResult.OfSuccess();
        }

        protected override void ConfigureParameters(PluginParametersConfiguration configuration)
        {
            // No parameters
        }
    }
}
