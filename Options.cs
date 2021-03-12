using CommandLine;

namespace fff
{
    class Options
    {
        [Option( 'd',HelpText ="Directory to search in, if not provided default to current dir, allow multiple")]
        public string[] Folders { get; set; }
        [Option('s', HelpText = "Search string",Required=true )]
        public string[] Search { get; set; }
        [Option('w', HelpText = "File pattern to search into. allow multiple")]
        public string[] Wildcards { get; set; }
       
       
    }
}