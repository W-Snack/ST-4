using Stateless;
using System;
namespace BugPro
{
    public class Bug
    {
        public enum State { Open, Assigned, Defered, Closed, Testing, Resolved, Reopened, Verified, Rejected }
        private enum Trigger { Assign, Defer, Close, Test, Resolve, Reopen, Verify, Reject }

        private StateMachine<State, Trigger> sm;

        public Bug(State state)
        {
            sm = new StateMachine<State, Trigger>(state);

            ConfigureTransitions();
        }

        private void ConfigureTransitions()
        {
            sm.Configure(State.Open)
                .Permit(Trigger.Assign, State.Assigned)
                .PermitReentry(Trigger.Defer);

            sm.Configure(State.Assigned)
                .Permit(Trigger.Close, State.Closed)
                .Permit(Trigger.Defer, State.Defered)
                .Permit(Trigger.Test, State.Testing)
                .Permit(Trigger.Reject, State.Rejected)
                .Ignore(Trigger.Assign);

            sm.Configure(State.Defered)
                .Permit(Trigger.Assign, State.Assigned);

            sm.Configure(State.Closed)
                .Permit(Trigger.Reopen, State.Reopened);

            sm.Configure(State.Testing)
                .Permit(Trigger.Resolve, State.Resolved)
                .Permit(Trigger.Assign, State.Assigned)
                .Permit(Trigger.Reject, State.Rejected);

            sm.Configure(State.Resolved)
                .Permit(Trigger.Close, State.Closed)
                .Permit(Trigger.Reopen, State.Reopened)
                .Permit(Trigger.Verify, State.Verified);

            sm.Configure(State.Reopened)
                .Permit(Trigger.Assign, State.Assigned);

            sm.Configure(State.Verified)
                .Permit(Trigger.Close, State.Closed)
                .Permit(Trigger.Reopen, State.Reopened);

            sm.Configure(State.Rejected)
                .Permit(Trigger.Assign, State.Assigned)
                .Permit(Trigger.Defer, State.Defered);
        }

        public void Close() => FireWithLog(Trigger.Close, "Close");
        public void Assign() => FireWithLog(Trigger.Assign, "Assign");
        public void Defer() => FireWithLog(Trigger.Defer, "Defer");
        public void Test() => FireWithLog(Trigger.Test, "Test");
        public void Resolve() => FireWithLog(Trigger.Resolve, "Resolve");
        public void Reopen() => FireWithLog(Trigger.Reopen, "Reopen");
        public void Verify() => FireWithLog(Trigger.Verify, "Verify");
        public void Reject() => FireWithLog(Trigger.Reject, "Reject");

        private void FireWithLog(Trigger trigger, string action)
        {
            sm.Fire(trigger);
            Console.WriteLine(action);
        }

        public State GetState() => sm.State;
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var bug = new Bug(Bug.State.Open);
            bug.Assign();
            bug.Test();
            bug.Resolve();
            bug.Close();
            bug.Reopen();
            bug.Assign();
            bug.Defer();
            bug.Assign();
            Console.WriteLine(bug.GetState());
        }
    }
}