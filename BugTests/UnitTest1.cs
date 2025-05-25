using BugPro;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace BugTests
{
    [TestClass]
    public sealed class UnitTest1
    {
        [TestMethod]
        public void InitialState_ShouldBeOpen()
        {
            var bug = new Bug(Bug.State.Open);
            Assert.AreEqual(Bug.State.Open, bug.GetState());
        }

        [TestMethod]
        public void Assign_FromOpen_ShouldTransitionToAssigned()
        {
            var bug = new Bug(Bug.State.Open);
            bug.Assign();
            Assert.AreEqual(Bug.State.Assigned, bug.GetState());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Close_FromOpen_ShouldThrowException()
        {
            var bug = new Bug(Bug.State.Open);
            bug.Close();
        }

        [TestMethod]
        public void Test_FromAssigned_ShouldTransitionToTesting()
        {
            var bug = new Bug(Bug.State.Open);
            bug.Assign();
            bug.Test();
            Assert.AreEqual(Bug.State.Testing, bug.GetState());
        }

        [TestMethod]
        public void Resolve_FromTesting_ShouldTransitionToResolved()
        {
            var bug = new Bug(Bug.State.Open);
            bug.Assign();
            bug.Test();
            bug.Resolve();
            Assert.AreEqual(Bug.State.Resolved, bug.GetState());
        }

        [TestMethod]
        public void Verify_FromResolved_ShouldTransitionToVerified()
        {
            var bug = new Bug(Bug.State.Open);
            ExecuteFlow(bug, Bug.State.Resolved);
            bug.Verify();
            Assert.AreEqual(Bug.State.Verified, bug.GetState());
        }

        [TestMethod]
        public void Reopen_FromClosed_ShouldTransitionToReopened()
        {
            var bug = new Bug(Bug.State.Open);
            ExecuteFlow(bug, Bug.State.Closed);
            bug.Reopen();
            Assert.AreEqual(Bug.State.Reopened, bug.GetState());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Resolve_FromAssigned_ShouldThrowException()
        {
            var bug = new Bug(Bug.State.Open);
            bug.Assign();
            bug.Resolve();
        }

        [TestMethod]
        public void DoubleAssign_FromAssigned_ShouldKeepState()
        {
            var bug = new Bug(Bug.State.Open);
            bug.Assign();
            var initialState = bug.GetState();
            bug.Assign();
            Assert.AreEqual(initialState, bug.GetState());
        }

        [TestMethod]
        public void Reject_FromAssigned_ShouldTransitionToRejected()
        {
            var bug = new Bug(Bug.State.Open);
            bug.Assign();
            bug.Reject();
            Assert.AreEqual(Bug.State.Rejected, bug.GetState());
        }

        [TestMethod]
        public void Assign_FromRejected_ShouldTransitionToAssigned()
        {
            var bug = new Bug(Bug.State.Open);
            bug.Assign();
            bug.Reject();
            bug.Assign();
            Assert.AreEqual(Bug.State.Assigned, bug.GetState());
        }

        [TestMethod]
        public void FullLifecycle_ShouldReachClosedState()
        {
            var bug = new Bug(Bug.State.Open);
            bug.Assign();
            bug.Test();
            bug.Resolve();
            bug.Verify();
            bug.Close();
            Assert.AreEqual(Bug.State.Closed, bug.GetState());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Verify_FromClosed_ShouldThrowException()
        {
            var bug = new Bug(Bug.State.Closed);
            bug.Verify();
        }

        [TestMethod]
        public void Reopen_FromVerified_ShouldTransitionToReopened()
        {
            var bug = new Bug(Bug.State.Open);
            ExecuteFlow(bug, Bug.State.Verified);
            bug.Reopen();
            Assert.AreEqual(Bug.State.Reopened, bug.GetState());
        }

        [TestMethod]
        public void Defer_FromRejected_ShouldTransitionToDefered()
        {
            var bug = new Bug(Bug.State.Open);
            bug.Assign();
            bug.Reject();
            bug.Defer();
            Assert.AreEqual(Bug.State.Defered, bug.GetState());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_FromDefered_ShouldThrowException()
        {
            var bug = new Bug(Bug.State.Defered);
            bug.Test();
        }

        [TestMethod]
        public void Close_FromVerified_ShouldTransitionToClosed()
        {
            var bug = new Bug(Bug.State.Open);
            ExecuteFlow(bug, Bug.State.Verified);
            bug.Close();
            Assert.AreEqual(Bug.State.Closed, bug.GetState());
        }

        [TestMethod]
        public void ComplexFlow_WithReopens_ShouldReachExpectedState()
        {
            var bug = new Bug(Bug.State.Open);
            bug.Assign();
            bug.Test();
            bug.Resolve();
            bug.Verify();
            bug.Close();
            bug.Reopen();
            bug.Assign();
            bug.Defer();
            bug.Assign();
            Assert.AreEqual(Bug.State.Assigned, bug.GetState());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void InvalidTransition_FromResolvedToTesting_ShouldThrow()
        {
            var bug = new Bug(Bug.State.Resolved);
            bug.Test();
        }

        [TestMethod]
        public void MultipleRejects_ShouldFollowCorrectPath()
        {
            var bug = new Bug(Bug.State.Open);
            bug.Assign();
            bug.Reject();
            bug.Assign();
            bug.Reject();
            bug.Defer();
            bug.Assign();
            Assert.AreEqual(Bug.State.Assigned, bug.GetState());
        }

        private void ExecuteFlow(Bug bug, Bug.State targetState)
        {
            switch (targetState)
            {
                case Bug.State.Closed:
                    bug.Assign();
                    bug.Test();
                    bug.Resolve();
                    bug.Close();
                    break;
                case Bug.State.Verified:
                    bug.Assign();
                    bug.Test();
                    bug.Resolve();
                    bug.Verify();
                    break;
            }
        }
    }
}
