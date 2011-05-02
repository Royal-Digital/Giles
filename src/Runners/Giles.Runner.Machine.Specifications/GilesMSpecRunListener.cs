﻿using System.Collections.Generic;
using Giles.Core.Runners;
using Machine.Specifications;
using Machine.Specifications.Runner;

namespace Giles.Runner.Machine.Specifications
{
    public class GilesMSpecRunListener : ISpecificationRunListener
    {
        readonly ITestListener testListener;
        readonly ResultFormatterFactory resultFormatterFactory;
        
        SessionRunState sessionRunState = SessionRunState.NoTests;
        public SessionRunState SessionRunState
        {
            get { return sessionRunState; }
        }

        readonly List<TestResult> testResults = new List<TestResult>();

        public GilesMSpecRunListener(ITestListener testListener)
        {
            this.testListener = testListener;
            resultFormatterFactory = new ResultFormatterFactory();
        }

        public void OnAssemblyStart(AssemblyInfo assembly)
        {
            
        }

        public void OnAssemblyEnd(AssemblyInfo assembly)
        {
            
        }

        public void OnRunStart()
        {
            
        }

        public void OnRunEnd()
        {
            if (testResults.Count == 0) return;

            bool failure = false;

            foreach (var testResult in testResults)
            {
                testListener.AddTestSummary(testResult);
                failure |= testResult.State == TestState.Failed;
            }

            sessionRunState = failure ? SessionRunState.Failure : SessionRunState.Success;
        }

        public void OnContextStart(ContextInfo context)
        {
            testListener.WriteLine(context.FullName, "Output");
        }

        public void OnContextEnd(ContextInfo context)
        {
            testListener.WriteLine("", "Output");
        }

        public void OnSpecificationStart(SpecificationInfo specification)
        {
        }

        public void OnSpecificationEnd(SpecificationInfo specification, Result result)
        {
            var formatter = resultFormatterFactory.GetResultFormatterFor(result);
            testListener.WriteLine(formatter.FormatResult(specification), "Output");

            var testResult = new TestResult {Name = specification.Name, TestRunner = "MSPEC"};

            if (result.Passed)
            {
                testResult.State = TestState.Passed;
            }
            else if (result.Status == Status.Ignored)
            {
                testResult.State = TestState.Ignored;
                testResult.Message = "Ignored";
            }
            else if (result.Status == Status.NotImplemented)
            {
                testResult.State = TestState.Ignored;
                testResult.Message = "Not Implemented";
            }
            else
            {
                testResult.State = TestState.Failed;
                if (result.Exception != null)
                {
                    testResult.StackTrace = result.Exception.ToString();
                }
            }

            testResults.Add(testResult);
        }

        public void OnFatalError(ExceptionResult exception)
        {
            testListener.WriteLine("Fatal error: " + exception, "Output");
        }
    }
}