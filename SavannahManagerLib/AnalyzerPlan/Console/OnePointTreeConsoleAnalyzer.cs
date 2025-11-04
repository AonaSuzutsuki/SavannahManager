using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvManagerLibrary.AnalyzerPlan.Console
{
    public interface IConsoleAnalyzer
    {
        public string GetChatExpression();
        public (string first, string second) GetPlayerExpression();
        public string GetTimeExpression();
    }

    public class OnePointTreeConsoleAnalyzer : IConsoleAnalyzer
    {
        public string GetChatExpression()
        {
            return "^(?<date>[0-9a-zA-Z:-]+) ([0-9.]+?) INF Chat \\(from '(?<steamId>[a-zA-Z0-9_-]+)', entity id '(?<id>[0-9-]+)'.*\\): ('(?<name>.*)': )*(?<chat>.*)$"; ;
        }

        public (string first, string second) GetPlayerExpression()
        {
            const string firstExpression = "(?<number>[0-9]+\\.) id=(?<identity>.*?), (?<name>.*?), ";
            const string secondExpression = "(, )*(?<name>[a-zA-Z]+)=(?<value>(\\([0-9., -]+\\))|([0-9a-zA-Z_\\.:]+))";

            return (firstExpression, secondExpression);
        }

        public string GetTimeExpression()
        {
            return "^Day (?<day>.*?), (?<hour>.*?):(?<minute>.*?)$";
        }
    }
}
