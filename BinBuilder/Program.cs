using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backtest;

namespace BinBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            //BarDay.BuildAllBins();
            //BarDay.validateBinFile(@"c:\src\data\1998.bin", @"c:\src\data\1998.sym",@"c:\src\data\1998.dates");
            BarDay.BuildConsolidatedBars(@"c:\src\data\1998.bin", @"c:\src\data\1998.sym", @"c:\src\data\1998.dates",5);
            BarDay.validateBinConsolidatedFile(@"c:\src\data\1998.bin5", @"c:\src\data\1998.sym", @"c:\src\data\1998.dates", 5);
            BarDay.BuildConsolidatedBars(@"c:\src\data\1998.bin", @"c:\src\data\1998.sym", @"c:\src\data\1998.dates", 15);
            BarDay.validateBinConsolidatedFile(@"c:\src\data\1998.bin15", @"c:\src\data\1998.sym", @"c:\src\data\1998.dates", 15);
            BarDay.BuildConsolidatedBars(@"c:\src\data\1998.bin", @"c:\src\data\1998.sym", @"c:\src\data\1998.dates", 390);
            BarDay.validateBinConsolidatedFile(@"c:\src\data\1998.bin390", @"c:\src\data\1998.sym", @"c:\src\data\1998.dates", 390);
        }
    }
}
