using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reseda.Parser;

namespace Reseda.Tests
{
    [TestClass]
    public class LivenessTests
    {
        [TestMethod]
        public void BoundedTest()
        {
            var p = new ResedaParser();


            string input = "A[?]," +
                           "A[?]," +
                           "A[?]," +
                           "B[]" +
                           "; A[@.:v == 5] *--> B";
            var term = p.Generate(input);
            Assert.IsTrue(term.Bounded());


            input = "B[]" +
                    "; B -->> {B[];}";
            term = p.Generate(input);
            Assert.IsFalse(term.Bounded());


            input = "B[]" +
                    "; * -->> {C[];}";
            term = p.Generate(input);
            Assert.IsFalse(term.Bounded());

            input = "B[]" +
                    "; B -->> {C[];C -->> {C[];}}";
            term = p.Generate(input);
            Assert.IsFalse(term.Bounded());


            input = "B[]{A[]; A-->> {A[];}}" +
                    ";";
            term = p.Generate(input);
            Assert.IsFalse(term.Bounded());

            // Cycles
            input = "B[]" +
                    "; B -->> {C[];}, C -->> {B[];}";
            term = p.Generate(input);
            Assert.IsFalse(term.Bounded());

        }



        [TestMethod]
        public void ProcessIsLiveTest()
        {
            var p = new ResedaParser();
            

            string input = "A[?]," +
                           "A[?]," +
                           "A[?]," +
                           "B[]" +
                           "; A[@.:v == 5] *--> B";
            var term = p.Generate(input);
            Assert.IsTrue(term.ProcessIsLive());


            input = "B[]" +
                    "; B -->> {B[];}";
            term = p.Generate(input);
            //System.Diagnostics.Debug.WriteLine(term.subProcess.ToSource());
            Assert.IsFalse(term.Bounded());
            Assert.IsFalse(term.ProcessIsLive());
            


            input = "A[?]," +
                    "B[]" +
                    "; A *--> B";
            term = p.Generate(input);
            Assert.IsTrue(term.ProcessIsLive());

            input = "A[?]," +
                    "B[]" +
                    "; A *--> A";
            term = p.Generate(input);
            Assert.IsTrue(term.ProcessIsLive());


            input = "A[?]," +
                    "B[]" +
                    "; A *--> B, B *--> A";
            term = p.Generate(input);
            Assert.IsTrue(term.ProcessIsLive());


            input = "A[?]," +
                    "B[]" +
                    "; A -->* B, B -->* A";
            term = p.Generate(input);
            Assert.IsFalse(term.ProcessIsLive());

            input = "A[?]," +
                    "B[]" +
                    "; A *--> B, B -->* A";
            term = p.Generate(input);
            Assert.IsFalse(term.ProcessIsLive());

            input = "A[?]," +
                    "B[]," +
                    "C[]" + 
                    "; A -->* B, B -->* C, C -->* A";
            term = p.Generate(input);
            Assert.IsFalse(term.ProcessIsLive());
            

            input = "A[?]," +
                    "B[]," +
                    "C[]" +
                    "; A -->* B, B -->* C, C -->* A";
            term = p.Generate(input);
            Assert.IsFalse(term.ProcessIsLive());


            input = "A[?]{D[];}," +
                    "B[]," +
                    "C[]" +
                    "; D -->* C, C -->* B, B -->* A";
            term = p.Generate(input);
            Assert.IsTrue(term.ProcessIsLive());

            input = "A[?]{D[];}," +
                    "B[]," +
                    "C[]" +
                    "; A/D -->* C, C -->* B, B -->* A";
            term = p.Generate(input);
            Assert.IsFalse(term.ProcessIsLive());


            // type: would be nice to type check this somehow as well, since it doesn't make sense to have this path expression.
            input = "A[?]{D[];}," +
                    "B[]," +
                    "C[]" +
                    "; D -->* C, C -->* B, B -->* A";
            term = p.Generate(input);
            Assert.IsTrue(term.ProcessIsLive());


            input = "A[?]," +
                    "B[?]," +
                    "C[@A:v + @B:v]," +
                    "N[]{A[];A-->>{D[],E[@../C:v];}};" +
                    "A-->*C," +
                    "B-->*C";
            term = p.Generate(input);
            Assert.IsTrue(term.ProcessIsLive());
        }


    }
}
