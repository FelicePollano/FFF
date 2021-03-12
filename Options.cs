using CommandLine;

namespace fff
{
    class Options
    {
        [Option( 'd',HelpText ="Directory to search in, if not provided default to current dir")]
        public string[] Folders { get; set; }
        [Option('s', HelpText = "Search string")]
        public string[] Search { get; set; }
        [Option('w', HelpText = "Bill of lading.")]
        public string BillOfLading { get; set; }
       
        [Option('s', HelpText = "Sequence Number.")]
        public string SequenceNumber { get; set; }
        [Option('r', HelpText = "returns raw data")]
        public string Raw { get; set; }
        [Option('f', HelpText = @"provide a file name with many rows containing id's.
If after a non digit separator a file name is provided, this will be used to write the content on disk,
otherwise stdout will be used.
Additional parameters will be applied on each single extractions.")]
        public string HaveFile { get; set; }
    }
}