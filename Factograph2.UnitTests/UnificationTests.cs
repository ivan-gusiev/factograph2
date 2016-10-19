using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

using Factograph2.Core;

namespace Factograph2.UnitTests
{
    [TestFixture]
    public class UnificationTests
    {
        #region Helpers
        private static TestCaseData TryUnify(Term lhs, Term rhs, string caseName)
        {
            return new TestCaseData(lhs, rhs) { TestName = "Unify: " + caseName };
        }

        private static TestCaseData TryUnify(Term lhs, Term rhs)
        {
            return TryUnify(lhs, rhs, $"{lhs} vs {rhs}");
        }

        private static Term C(params Term[] terms)
        {
            return Term.New(terms);
        }
        #endregion

        public static IEnumerable<TestCaseData> PositiveUnificationTestCases()
        {
            var bob = Term.NewAtom("bob");
            var sue = Term.NewAtom("sue");
            var f = Term.NewAtom("f");
            var g = Term.NewAtom("g");
            var x = Term.NewVariable("X");
            var y = Term.NewVariable("Y");

            // atoms
            yield return TryUnify(bob, bob, "same atom reference");
            yield return TryUnify(bob, Term.NewAtom("bob"), "same atom ids");

            // consts
            yield return TryUnify(Term.New(1), Term.New(1));
            yield return TryUnify(Term.New("a"), Term.New("a"));
            yield return TryUnify(Term.New((string) null), Term.New((string) null), "null strings");
            yield return TryUnify(Term.New(false), Term.New(false));
            yield return TryUnify(Term.New(true), Term.New(true));
            yield return TryUnify(Term.New(Math.PI), Term.New(Math.PI), "PI vs PI");
            yield return TryUnify(Term.New(Variant.New(new Version("1.2.3.4"))), Term.New(Variant.New(new Version("1.2.3.4"))));

            // variables
            yield return TryUnify(x, sue);
            yield return TryUnify(Term.New(1), x);
            yield return TryUnify(Term.New(1.1), x);
            yield return TryUnify(x, Term.New(true));
            yield return TryUnify(x, Term.New("string"));
            yield return TryUnify(x, Term.New(Variant.New(new Version("1.2.3.4"))));
            yield return TryUnify(x, y);

            // complex terms
            yield return TryUnify(C(f, bob, sue), C(f, bob, sue));
            yield return TryUnify(C(f, bob, sue), C(f, x, sue));
            yield return TryUnify(C(f, x), C(f, y));
            yield return TryUnify(C(f, C(g, x)), C(f, y));
            yield return TryUnify(C(f, C(g, x), x), C(f, y, Term.NewAtom("a")));
        }

        public static IEnumerable<TestCaseData> NegativeUnificationTestCases()
        {
            var bob = Term.NewAtom("bob");
            var sue = Term.NewAtom("sue");
            var f = Term.NewAtom("f");
            var g = Term.NewAtom("g");
            var x = Term.NewVariable("X");
            var y = Term.NewVariable("Y");

            // atoms
            yield return TryUnify(bob, sue, "different atom ids");

            // constants
            yield return TryUnify(Term.New(1), Term.New(2));
            yield return TryUnify(Term.New("a"), Term.New(""));
            yield return TryUnify(Term.New(string.Empty), Term.New((string)null), "empty vs null strings");
            yield return TryUnify(Term.New(false), Term.New(true));
            yield return TryUnify(Term.New(Math.PI), Term.New(Math.E), "PI vs E");
            yield return TryUnify(Term.New(Variant.New(new Version("1.2.3.4"))), Term.New(Variant.New(new Version("1.2.3.5"))));

            yield return TryUnify(bob, Term.New("bob"), "atom vs string");

            // complex terms
            yield return TryUnify(C( f, bob, sue ), C( f, bob ));
            yield return TryUnify(C( f, x ), C( g, y ));
            yield return TryUnify(C( f, x ), C( f, x, y ));

        }

        [Test, TestCaseSource(nameof(PositiveUnificationTestCases))]
        public void TermsUnify(Term lhs, Term rhs)
        {
            var unificationResult = UnificationResult.Unify(lhs, rhs);
            foreach (var item in (unificationResult.Variables))
            {
                Console.WriteLine($"{item.Item1} = {item.Item2}");
            }

            Assert.IsTrue(unificationResult.IsSuccess);
        }

        [Test, TestCaseSource(nameof(NegativeUnificationTestCases))]
        public void TermsDontUnify(Term lhs, Term rhs)
        {
            var unificationResult = UnificationResult.Unify(lhs, rhs);
            Assert.IsTrue(unificationResult.IsFailure);
        }

        [Test, TestCase(TestName = "Unify: free variable")]
        public void UnifyFreeVariable()
        {
            const string VariableName = "X";
            var toCompare = Term.New(1);
            var unificationResult = UnificationResult.Unify(Term.NewVariable(VariableName), toCompare);
            Assert.IsTrue(unificationResult.IsSuccess);
            Assert.AreEqual(toCompare, unificationResult.Variables.FirstOrDefault(v => v.Item1 == VariableName).Item2);

            unificationResult = UnificationResult.Unify(toCompare, Term.NewVariable(VariableName));
            Assert.IsTrue(unificationResult.IsSuccess);
            Assert.AreEqual(toCompare, unificationResult.Variables.FirstOrDefault(v => v.Item1 == VariableName).Item2);
        }

        [Test, TestCase(TestName = "Unify: no self-alias")]
        public void UnifySameVariable()
        {
            var unificationResult = UnificationResult.Unify(Term.NewVariable("X"), Term.NewVariable("X"));
            Assert.IsTrue(unificationResult.IsSuccess);
            Assert.IsTrue(unificationResult.Variables.Count() == 0);
        }

        [Test, TestCase(TestName = "Unify: check Occurs")]
        public void UnifyOccursCheck()
        {
            var x = Term.NewVariable("X");
            var term = Term.New(new Term[] { Term.NewAtom("f"), x });

            var currentStatus = Config.OccursCheck;

            Config.OccursCheck = true;
            var unificationResult = UnificationResult.Unify(x, term);
            Assert.IsTrue(unificationResult.IsFailure);

            Config.OccursCheck = false;
            unificationResult = UnificationResult.Unify(x, term);
            Assert.IsTrue(unificationResult.IsSuccess);

            Config.OccursCheck = currentStatus;
        }
    }
}
