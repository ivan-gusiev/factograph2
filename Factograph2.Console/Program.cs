using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Factograph2.Core;

namespace Factograph2.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine(UnificationResult.Unify(Term.NewAtom("42"), Term.NewAtom("42")));
        }
    }
}
