using CsvProcessing;
using TableExtensions;
using WordProcessingFunctions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab7
{
    class Program
    {
        static void Main(string[] args)
        {
            //загрузка корпуса
            var tableCorpus = "replica.csv".ReadCsv().ToList();

            //выделение реплик
            var replica = tableCorpus
                .Where(row => row.GetBy("replica").Any())
                .Select(row => row.GetBy("replica").First()).ToList();

            //препроцессинг
            replica.ForEach(node => node.Value = node.Value.ToString().PreProcess(".'/,<>@`-=+\"\\()[]{}«»:;–_"));

            //стемминг
            replica.ForEach(node => node.Value = node.Value.ToString().StemIt());

            //tf idf
            var replicaValues = replica.Select(rep => rep.Value.ToString());
            var tfIdfList = replica.Select(node => node.Value.ToString().Split(' ').Select(s => new []{s}).GetAnalysisFrequency().ConvertToTfIdf(replicaValues)).ToList();

            var tfIdfListFormatted = tfIdfList.Select(f => f.AsString());

            using (var sw = new StreamWriter("output.txt"))
                sw.Write(string.Join("\n", tfIdfListFormatted));

            Console.WriteLine(replica.ToPlainTable().ToSingleRowString());
            Console.ReadKey();
        }
    }
}
