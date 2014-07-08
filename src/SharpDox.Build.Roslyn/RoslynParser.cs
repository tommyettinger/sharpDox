using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.VisualBasic;
using SharpDox.Model.Repository;
using SharpDox.Sdk.Config;

namespace SharpDox.Build.Roslyn
{
    public class RoslynParser : ICodeParser
    {
        public event Action<string> OnDocLanguageFound;
        public event Action<string> OnStepMessage;
        public event Action<int> OnStepProgress;

        private readonly ParserStrings _parserStrings;

        public RoslynParser(ParserStrings parserStrings)
        {
            _parserStrings = parserStrings;
        }

        public SDRepository GetStructureParsedSolution(string solutionFile)
        {
            var sdRepository = new SDRepository();
            /*var solution = LoadSolution(solutionFile, 3);

            StructureParseNamespaces(solution, sdRepository);
            StructureParseTypes(solution, sdRepository);*/

            return sdRepository;
        }

        public SDRepository GetFullParsedSolution(string solutionFile, ICoreConfigSection sharpDoxConfig)
        {
            var sdRepository = new SDRepository();
            /*var solution = LoadSolution(solutionFile, 5);

            ParseNamespaces(solution, sdRepository, sharpDoxConfig);
            ParseTypes(solution, sdRepository, sharpDoxConfig);
            ParseMethodCalls(solution, sdRepository);
            ResolveUses(sdRepository);*/

            return sdRepository;
        }

        /*private CSharpSolution LoadSolution(string solutionFile, int steps)
        {
            var solution = MSBuildWorkspace.Create().OpenSolutionAsync("");
            solution.RunSynchronously();

            Compilation comp;
            solution.Result.Projects.First().TryGetCompilation(out comp);
            

            comp.RootNamespace().
            solution.OnLoadingProject += (m) => ExecuteOnStepMessage(string.Format(_parserStrings.ReadingProject, m));
            solution.OnLoadedProject += (t, i) => ExecuteOnStepProgress((int)(((double)i/(double)t) * 100 / steps));
            solution.LoadSolution(solutionFile);

            return solution;
        }*/

        private void ExecuteOnDocLanguageFound(string lang)
        {
            var handlers = OnDocLanguageFound;
            if (handlers != null)
            {
                handlers(lang);
            }
        }

        private void ExecuteOnStepMessage(string message)
        {
            var handlers = OnStepMessage;
            if (handlers != null)
            {
                handlers(message);
            }
        }

        private void ExecuteOnStepProgress(int progress)
        {
            var handlers = OnStepProgress;
            if (handlers != null)
            {
                handlers(progress);
            }
        }
    }
}
