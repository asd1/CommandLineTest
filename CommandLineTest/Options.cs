using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace CommandLineTest
{
    public class Options
    {
        [Value(index: 0, Required = true, HelpText = "File to process")]
        public string FileName { get; set; }
        [Value(index: 1, Required = true, HelpText = "A number")]
        public int number { get; set; }
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages")]
        public bool Verbose { get; set; }
        [Option('d', "debug", Required = false, HelpText = "Turn on debugging")]
        public bool debug { get; set; }
    }
}
